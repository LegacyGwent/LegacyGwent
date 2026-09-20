using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using MongoDB.Driver;
using Newtonsoft.Json;

partial class Program
{
    static string CraftId() => Guid.NewGuid().ToString("N");
    static async Task PremiumCopyCases()
    {
        using (var wire = await WireClient.Connect(url))
        using (var unauth = await WireClient.Connect(url))
        {
            var user = await Login(wire, "copy-inventory");
            var wallet = await db.GetPremiumCollection(user.UserName);
            var bronze = wallet.Costs.First(x => x.Value == 100).Key;
            var otherBronze = wallet.Costs.First(x => x.Value == 100 && x.Key != bronze).Key;
            var leader = wallet.Costs.Keys.First(x => GwentMap.CardMap[x].Group == Cynthia.Card.Group.Leader);
            Check(CardInventory.Limit(bronze) == 3 && CardInventory.PremiumCount(wallet.Collection, bronze) == 0,
                "new account has three standard bronze copies and zero premium copies");
            Check((await unauth.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId())).Status == "unauthenticated",
                "copy-craft endpoint authenticates the connection");
            Check((await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, "bad")).Status == "invalid_request",
                "copy crafting requires a valid idempotency key");
            await Grant(user.Id, 990, "copy-funds");
            long before = (await Account(user)).MeteoritePowder;
            string request = CraftId();
            var retries = await Task.WhenAll(Enumerable.Range(0, 24).Select(_ => wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, request)));
            var account = await Account(user);
            Check(retries.All(x => x.Success) && CardInventory.PremiumCount(account, bronze) == 1 && account.MeteoritePowder == before - 100,
                "24 network retries for one copy charge once and create exactly one premium");
            Check(CardInventory.Limit(bronze) == 3, "crafting preserves all three standard copies");
            Check((await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", otherBronze, request)).Status == "request_conflict",
                "reusing a craft request ID for a different card is rejected");
            var deck = new DeckModel { Id = Guid.NewGuid().ToString(), Name = "mixed copies", Leader = leader,
                Deck = new List<string> { bronze, bronze, bronze }, PremiumCards = new Dictionary<string, int> { [bronze] = 1 }, PremiumLeader = false };
            Check(await wire.Invoke<bool>("AddDeck", deck), "server saves a deck with two standard and one premium bronze");
            var stored = (await Users.Find(x => x.Id == user.Id).FirstAsync()).Decks.Single(x => x.Id == deck.Id);
            PremiumDeckSelection storedSelection = null;
            var selectionDeadline = DateTimeOffset.UtcNow.AddSeconds(10);
            while (DateTimeOffset.UtcNow < selectionDeadline)
            {
                var selections = (await Account(user)).DeckSelections;
                if (selections != null && selections.TryGetValue(deck.Id, out storedSelection)) break;
                await Task.Delay(25);
            }
            Check(stored.PremiumCards == null && storedSelection?.PremiumCards[bronze] == 1 && stored.Deck.Count == 3,
                "per-deck copy selection persists outside the rollback-compatible user deck");
            var attacker = JsonConvert.DeserializeObject<DeckModel>(JsonConvert.SerializeObject(deck));
            attacker.PremiumCards[bronze] = 2;
            Check(!await wire.Invoke<bool>("ModifyDeck", deck.Id, attacker), "server rejects more premium deck copies than owned");
            attacker.PremiumCards[bronze] = -1;
            Check(!await wire.Invoke<bool>("ModifyDeck", deck.Id, attacker), "server rejects negative premium counts");
            attacker.PremiumCards[bronze] = 0; attacker.PremiumLeader = true;
            Check(!await wire.Invoke<bool>("ModifyDeck", deck.Id, attacker), "server rejects unowned premium leader");
            attacker.PremiumLeader = false; attacker.PremiumCards[otherBronze] = 1;
            Check(!await wire.Invoke<bool>("ModifyDeck", deck.Id, attacker), "server rejects premium entries absent from the deck");
            var player = new SinkPlayer { Deck = deck, PremiumCards = new HashSet<string> { bronze, leader } };
            var opponent = new SinkPlayer { Deck = new DeckModel { Leader = leader, Deck = new List<string> { bronze, bronze, bronze },
                PremiumCards = new Dictionary<string, int> { [bronze] = 2 }, PremiumLeader = true }, PremiumCards = new HashSet<string> { bronze, leader } };
            var game = new GwentServerGame(player, opponent);
            Check(game.PlayersDeck[0].Count(x => x.Status.IsPremium == true) == 1 && game.PlayersDeck[1].Count(x => x.Status.IsPremium == true) == 2,
                "each player's shuffled deck retains its own mixed copy versions");
            Check(game.PlayersLeader[0].Single().Status.IsPremium == false && game.PlayersLeader[1].Single().Status.IsPremium == true,
                "leader appearance follows explicit deck choice even when both own premium");
            var ai = new CopyTestAI { Deck = new DeckModel { Leader = leader, Deck = new List<string> { bronze, bronze, bronze },
                PremiumCards = new Dictionary<string, int>(), PremiumLeader = false } };
            var aiGame = new GwentServerGame(player, ai);
            Check(aiGame.PlayersDeck[1].All(x => x.Status.IsPremium == true) && aiGame.PlayersLeader[1].Single().Status.IsPremium == true,
                "AI still uses all premium copies independent of player inventory and saved versions");
            Check(game.PlayersDeck[0].All(x => x.Status.CreateBackCard().IsPremium != true), "hidden mixed copies never leak premium state");
            var second = await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId());
            Check(CardInventory.PremiumCount(second.Collection, bronze) == 2 && second.Collection.MeteoritePowder == before - 200,
                "second deliberate craft buys only the second premium copy");
            var last = await Task.WhenAll(Enumerable.Range(0, 24).Select(_ => wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId())));
            account = await Account(user);
            Check(last.Count(x => x.Success) == 1 && CardInventory.PremiumCount(account, bronze) == 3 && account.MeteoritePowder == before - 300,
                "concurrent distinct requests at two copies buy only the final copy");
            Check((await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, request)).Success && (await Account(user)).MeteoritePowder == before - 300,
                "retrying first request after reaching cap remains successful without another debit");
            Check((await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId())).Status == "already_owned",
                "fourth bronze premium is rejected without spending");
            var standardOnly = JsonConvert.DeserializeObject<DeckModel>(JsonConvert.SerializeObject(deck));
            standardOnly.PremiumCards.Clear();
            Check(await wire.Invoke<bool>("ModifyDeck", deck.Id, standardOnly), "explicit standard deck can be saved after owning all premiums");
            player.Deck = standardOnly;
            Check(new GwentServerGame(player, opponent).PlayersDeck[0].All(x => x.Status.IsPremium == false),
                "owning three premiums never overrides explicit standard deck choices");
            using (var reconnect = await WireClient.Connect(url))
            {
                var login = await reconnect.Invoke<UserInfo>("Login", user.UserName, "RewardTest2026");
                var reloaded = await reconnect.Invoke<PremiumCollectionResult>("GetPremiumCollection");
                Check(login.Decks.Single(x => x.Id == deck.Id).PremiumCards == null &&
                    reloaded.Collection.DeckSelections[deck.Id].PremiumCards.Count == 0 && CardInventory.PremiumCount(reloaded.Collection, bronze) == 3,
                    "reconnect preserves both inventory counts and independently saved standard deck");
            }
            var fresh = new GwentDatabaseService(host.Services);
            Check((await fresh.CraftPremiumCopy(user.UserName, bronze, request)).Success && (await Account(user)).MeteoritePowder == before - 300,
                "request receipts survive a database-service restart");
            var legacy = await NewUser("copy-legacy");
            await Accounts.InsertOneAsync(new PremiumCollection { Id = legacy.Id, MeteoritePowder = 321, OwnedCards = new List<string> { bronze, leader },
                SelectedCards = new List<string> { bronze }, Revision = 5 });
            var migrated = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => fresh.GetPremiumCollection(legacy.UserName)));
            var upgraded = await Account(legacy);
            Check(upgraded.InventoryVersion == 1 && upgraded.Revision == 6 && upgraded.MeteoritePowder == 321 &&
                CardInventory.PremiumCount(upgraded, bronze) == 3 && CardInventory.PremiumCount(upgraded, leader) == 1,
                "legacy all-copy unlock migrates once to full ownership without charges");
            var oldDeck = new DeckModel { Leader = leader, Deck = new List<string> { bronze, bronze, bronze } };
            CardInventory.InitializeDeck(oldDeck, upgraded);
            Check(oldDeck.PremiumCards[bronze] == 3 && oldDeck.PremiumLeader == true, "legacy decks retain premium usage after migration");
            var notReady = new DeckModel { Leader = leader, Deck = new List<string> { bronze } };
            CardInventory.InitializeDeck(notReady, null);
            Check(notReady.PremiumCards == null && notReady.PremiumLeader == null, "unsynchronized client cannot erase legacy deck version choices");
            var missing = await NewUser("copy-missing-revision");
            await db.GetMongoClient().GetDatabase("gwentdiy").GetCollection<MongoDB.Bson.BsonDocument>("premium_collection")
                .InsertOneAsync(new MongoDB.Bson.BsonDocument { { "_id", missing.Id }, { "MeteoritePowder", 123L },
                    { "OwnedCards", MongoDB.Bson.BsonNull.Value }, { "SelectedCards", MongoDB.Bson.BsonNull.Value } });
            var repaired = (await fresh.GetPremiumCollection(missing.UserName)).Collection;
            Check(repaired.InventoryVersion == 1 && repaired.Revision == 1 && repaired.OwnedCards.Count == 0 && repaired.MeteoritePowder == 123,
                "old raw wallet with missing revision and null lists migrates safely");
            foreach (var group in new[] { Cynthia.Card.Group.Silver, Cynthia.Card.Group.Gold, Cynthia.Card.Group.Leader })
            {
                string id = wallet.Costs.Keys.First(x => GwentMap.CardMap[x].Group == group);
                var single = await NewUser("copy-single"); await Grant(single.Id, 1000, "copies");
                var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => fresh.CraftPremiumCopy(single.UserName, id, CraftId())));
                var final = await Account(single);
                Check(results.Count(x => x.Success) == 1 && CardInventory.PremiumCount(final, id) == 1 && final.MeteoritePowder == 1000 - wallet.Costs[id],
                    group + " premium has a one-copy inventory cap and exact atomic debit");
            }
        }
    }
    private sealed class CopyTestAI : Cynthia.Card.AI.RandomAutoAIPlayer
    {
        public override void SetDeckAndName() { PlayerName = "Copy inventory AI"; }
    }
}
