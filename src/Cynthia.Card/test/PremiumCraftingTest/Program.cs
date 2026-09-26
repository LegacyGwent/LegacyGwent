using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

class Program
{
    static int checks;
    static void Check(bool value, string label) { if (!value) throw new Exception(label); checks++; Console.WriteLine("PASS " + label); }
    static async Task Main()
    {
        // Dedicated local test accounts only. Never targets a deployed server.
        var mongo = new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121");
        // Price/insufficient-balance contracts deliberately run with the launch grant paused.
        var provider = new ServiceCollection().AddSingleton<IMongoClient>(mongo).AddSingleton(new InitialPowderOptions(0)).BuildServiceProvider();
        var service = new GwentDatabaseService(provider);
        var users = mongo.GetDatabase("gwentdiy").GetCollection<UserInfo>("user");
        var accounts = mongo.GetDatabase("gwentdiy").GetCollection<PremiumCollection>("premium_collection");
        var user = new UserInfo { UserName = "premium-contract-" + Guid.NewGuid().ToString("N"), PlayerName = "Premium Contract Test", Decks = new List<DeckModel>() };
        await users.InsertOneAsync(user);
        var first = await service.GetPremiumCollection(user.UserName);
        Check(first.Success && first.Collection.MeteoritePowder == 0 && first.Collection.OwnedCards.Count == 0 && first.Collection.SelectedCards.Count == 0, "old account defaults to zero and locked");
        Check(first.Costs.Count > 500 && first.Costs.Values.Contains(100) && first.Costs.Values.Contains(400) && first.Costs.Values.Contains(800) && first.Costs.Values.Contains(1000), "catalog and approved prices");
        string card = first.Costs.First(x => x.Value == 100).Key;
        Check((await service.CraftPremium(user.UserName, card)).Status == "insufficient_powder", "insufficient funds rejected");
        Check((await service.SelectPremium(user.UserName, card, true)).Status == "not_owned", "unowned equip rejected");
        Check((await service.CraftPremium(user.UserName, "invalid-card")).Status == "unavailable", "invalid card rejected");
        Check((await service.CraftPremium("unknown-account", card)).Status == "unauthenticated", "missing identity rejected");
        await accounts.UpdateOneAsync(x => x.Id == user.Id, Builders<PremiumCollection>.Update.Set(x => x.MeteoritePowder, 150));
        var crafts = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => service.CraftPremium(user.UserName, card)));
        var state = (await service.GetPremiumCollection(user.UserName)).Collection;
        Check(crafts.Count(x => x.Success) == 1 && state.MeteoritePowder == 50 && state.OwnedCards.SequenceEqual(new[] {card}), "20 concurrent requests charge exactly once");
        Check(state.SelectedCards.Contains(card), "successful craft equips premium");
        Check((await service.CraftPremium(user.UserName, card)).Status == "already_owned", "retry does not charge");
        Check((await service.SelectPremium(user.UserName, card, false)).Success, "equip standard");
        var fresh = new GwentDatabaseService(provider);
        var reloaded = (await fresh.GetPremiumCollection(user.UserName)).Collection;
        Check(reloaded.MeteoritePowder == 50 && reloaded.OwnedCards.Contains(card) && reloaded.SelectedCards.Count == 0, "wallet ownership and selection persist across service reload");
        Check((await fresh.SelectPremium(user.UserName, card, true)).Success, "equip owned premium");
        await users.ReplaceOneAsync(x => x.Id == user.Id, user);
        Check((await fresh.GetPremiumCollection(user.UserName)).Collection.MeteoritePowder == 50, "legacy whole-user saves preserve powder wallet");
        var other = new UserInfo { UserName = "premium-contract-" + Guid.NewGuid().ToString("N"), PlayerName = "Other Premium Contract", Decks = new List<DeckModel>() };
        await users.InsertOneAsync(other);
        Check((await fresh.GetPremiumCollection(other.UserName)).Collection.OwnedCards.Count == 0, "second account does not inherit unlocks");
        var hidden = new CardStatus(Faction.Monsters);
        Check(hidden.IsCardBack && hidden.IsPremium != true, "hidden card omits premium ownership");
        var leader = GwentMap.CardMap.First(x => x.Value.Group == Group.Leader).Key;
        var one = new TestPlayer { PlayerName="one", Deck=new DeckModel { Leader=leader, Deck=new List<string> { card } }, PremiumCards=new HashSet<string> { card, leader } };
        var two = new TestPlayer { PlayerName="two", Deck=new DeckModel { Leader=leader, Deck=new List<string> { card } } };
        var game = new GwentServerGame(one, two);
        Check(game.PlayersDeck[0].Single().Status.IsPremium == true && game.PlayersDeck[1].Single().Status.IsPremium == false, "same card differs by server-confirmed owner selection in match");
        Check(game.PlayersLeader[0].Single().Status.IsPremium == true && game.PlayersLeader[1].Single().Status.IsPremium == false, "leader and opening-screen status preserve selected version");
        Check(game.PlayersDeck[0].Single().Status.CreateBackCard().IsPremium != true, "concealing an owned premium does not leak its state");
        var duplicates = new TestPlayer { PlayerName="three copies", Deck=new DeckModel { Leader=leader, Deck=new List<string> {card,card,card} }, PremiumCards=new HashSet<string>(state.SelectedCards) };
        CardInventory.InitializeDeck(duplicates.Deck, (await fresh.GetPremiumCollection(user.UserName)).Collection);
        var duplicateGame = new GwentServerGame(duplicates, two);
        Check(duplicateGame.PlayersDeck[0].Count()==3 && duplicateGame.PlayersDeck[0].Count(x=>x.Status.IsPremium==true)==1,
            "one bronze craft supplies one premium and two standard deck copies");
        Check(CardInventory.PremiumCount((await fresh.GetPremiumCollection(user.UserName)).Collection,card)==1,
            "the account persists an explicit one-copy premium inventory");
        Check((await fresh.GetPremiumCollection(user.UserName)).Collection.MeteoritePowder==50,
            "using a mixed deck does not charge additional crafting fees");

        // Transform (变形) inherits the premium appearance from the card that caused the
        // transform, not from the transformed target's own previous state.
        var transformId = first.Costs.Keys.First(x => x != card);
        TestPlayer Owner(string name, bool premium) => new TestPlayer
        {
            PlayerName = name,
            Deck = new DeckModel { Leader = leader, Deck = new List<string> { card } },
            PremiumCards = premium ? new HashSet<string> { card, leader } : new HashSet<string>()
        };

        var sourceDecides = new GwentServerGame(Owner("premium source", true), Owner("standard target", false));
        var targetOfPremiumSource = sourceDecides.PlayersDeck[1].Single();
        Check(sourceDecides.PlayersDeck[0].Single().Status.IsPremium == true && targetOfPremiumSource.Status.IsPremium == false,
            "transform fixture separates a premium source from a standard target");
        await targetOfPremiumSource.Effect.Transform(transformId, sourceDecides.PlayersDeck[0].Single());
        Check(targetOfPremiumSource.Status.CardId == transformId && targetOfPremiumSource.Status.IsPremium == true,
            "a premium source makes the transformed target premium");

        var sourceOverrides = new GwentServerGame(Owner("standard source", false), Owner("premium target", true));
        var targetOfStandardSource = sourceOverrides.PlayersDeck[1].Single();
        Check(targetOfStandardSource.Status.IsPremium == true, "transform target starts premium");
        await targetOfStandardSource.Effect.Transform(transformId, sourceOverrides.PlayersDeck[0].Single());
        Check(targetOfStandardSource.Status.IsPremium == false,
            "a standard source turns a premium transformed target standard");

        var selfTransform = new GwentServerGame(Owner("self premium", true), Owner("opponent", false));
        var selfCard = selfTransform.PlayersDeck[0].Single();
        await selfCard.Effect.Transform(transformId, selfCard);
        Check(selfCard.Status.CardId == transformId && selfCard.Status.IsPremium == true,
            "a self transform keeps its own premium state");

        // Generated/created derivatives (CreateCard) inherit the premium appearance of the
        // card effect that created them, exactly. The receiving account's own premium
        // ownership is only a fallback for system-owned creation without a source.
        TestPlayer Generator(string name, params string[] premiumCards) => new TestPlayer
        {
            PlayerName = name,
            Deck = new DeckModel { Leader = leader, Deck = new List<string> { card } },
            PremiumCards = new HashSet<string>(premiumCards)
        };
        CardLocation Stay() => new CardLocation(RowPosition.MyStay, 0);

        // Premium source -> premium derivative, even though the creating account does not
        // own the generated card as premium.
        var premiumGen = new GwentServerGame(Generator("premium generator", card), Generator("plain receiver"));
        var premiumSource = premiumGen.PlayersDeck[0].Single();
        Check(premiumSource.Status.IsPremium == true && !premiumGen.Players[0].PremiumCards.Contains(transformId),
            "generation fixture separates a premium source from an unowned derivative");
        var premiumDerivative = await premiumGen.CreateCard(transformId, 0, Stay(), source: premiumSource);
        Check(premiumDerivative != null && premiumDerivative.Status.IsPremium == true,
            "a premium source creates a premium derivative for an account that does not own it");

        // Standard source -> standard derivative, even when the receiving player does own
        // the generated card as premium. This is the opponent's side of the board.
        var standardGen = new GwentServerGame(Generator("standard generator"), Generator("premium owner", transformId));
        var standardSource = standardGen.PlayersDeck[0].Single();
        Check(standardSource.Status.IsPremium == false && standardGen.Players[1].PremiumCards.Contains(transformId),
            "generation fixture separates a standard source from an owned derivative");
        var standardDerivative = await standardGen.CreateCard(transformId, 1, Stay(), source: standardSource);
        Check(standardDerivative != null && standardDerivative.Status.IsPremium == false,
            "a standard source creates a standard derivative even when the receiver owns the premium");

        // Cross-side premium creation: the source still decides on the opponent's side.
        var crossGen = new GwentServerGame(Generator("premium cross source", card), Generator("plain opponent"));
        var crossSource = crossGen.PlayersDeck[0].Single();
        var crossDerivative = await crossGen.CreateCard(transformId, 1, Stay(), source: crossSource);
        Check(crossDerivative != null && crossDerivative.Status.IsPremium == true,
            "opponent-side creation still follows a premium source");

        // Only a source-less system/API creation may fall back to account ownership.
        var fallbackGen = new GwentServerGame(Generator("system owner", transformId), Generator("other"));
        var fallbackDerivative = await fallbackGen.CreateCard(transformId, 0, Stay());
        Check(fallbackDerivative != null && fallbackDerivative.Status.IsPremium == true,
            "no-source system creation keeps the account ownership fallback");
        var explicitStandard = await fallbackGen.CreateCard(transformId, 0, Stay(), setting: x => x.IsPremium = false);
        Check(explicitStandard != null && explicitStandard.Status.IsPremium == false,
            "an explicit source-less setting still wins over account ownership");

        Console.WriteLine("COMPLETE checks=" + checks);
    }
    private sealed class TestPlayer : Player { public TestPlayer() { _downstream.Receive += _ => Task.CompletedTask; } }
}
