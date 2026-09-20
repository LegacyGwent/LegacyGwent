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
        Check(first.Costs.Count > 500 && first.Costs.Values.Contains(100) && first.Costs.Values.Contains(200) && first.Costs.Values.Contains(400), "catalog and approved prices");
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
        Console.WriteLine("COMPLETE checks=" + checks);
    }
    private sealed class TestPlayer : Player { }
}
