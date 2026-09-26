using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

partial class Program
{
    static async Task<bool> AwaitDeckSelection(UserInfo user, string id, Func<PremiumDeckSelection, bool> predicate)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(10);
        while (DateTimeOffset.UtcNow < deadline)
        {
            var account = (await db.GetPremiumCollection(user.UserName)).Collection;
            if (account.DeckSelections.TryGetValue(id, out var selection) && predicate(selection)) return true;
            await Task.Delay(25);
        }
        return false;
    }

    static async Task PremiumLegacyDeckCases()
    {
        using (var wire = await WireClient.Connect(url))
        {
            var user = await Login(wire, "legacy-appearance");
            var server = host.Services.GetRequiredService<GwentServerService>();
            var worker = host.Services.GetRequiredService<PremiumDeckSelectionService>();
            var starter = GwentDeck.CreateBasicDeck(0);
            var wallet = await wire.Invoke<PremiumCollectionResult>("GetPremiumCollection");
            string bronze = starter.Deck.GroupBy(id => id).First(group => group.Count() == 3 &&
                GwentMap.CardMap[group.Key].Group == Cynthia.Card.Group.Copper && wallet.Costs.ContainsKey(group.Key)).Key;
            await Grant(user.Id, 10000, "legacy-appearance-funds");
            await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId());

            // A permanent rejection must not retain the global worker, even if internal
            // callers accidentally submit malformed work that the public API would avoid.
            worker.QueueSave(user.UserName, new DeckModel { Id = null, Leader = starter.Leader,
                PremiumCards = new Dictionary<string, int>() });
            foreach (var id in new[] { "old-deck", "legacy.$deck", "legacy~YWJj" })
            {
                var draft = new DeckModel { Id = id, Leader = starter.Leader,
                    PremiumCards = new Dictionary<string, int>(), PremiumLeader = false };
                Check(await wire.Invoke<bool>("AddDeck", draft), "legacy arbitrary ID is still accepted: " + id);
                Check(await AwaitDeckSelection(user, id, _ => true),
                    "legacy appearance persists after rejected work without blocking global queue: " + id);
                var returned = await wire.Invoke<PremiumCollectionResult>("GetPremiumCollection");
                Check(returned.Collection.DeckSelections.ContainsKey(id), "wire retains original legacy ID: " + id);
                // Exercise another result-producing path, guarding against repeated decoding.
                returned = await wire.Invoke<PremiumCollectionResult>("SelectPremium", bronze, 0);
                Check(returned.Collection.DeckSelections.ContainsKey(id), "selection response retains legacy ID: " + id);
                Check(await wire.Invoke<bool>("RemoveDeck", id), "legacy arbitrary ID can be removed: " + id);
            }

            var legacyDeck = JsonConvert.DeserializeObject<DeckModel>(JsonConvert.SerializeObject(starter));
            legacyDeck.Id = Guid.NewGuid().ToString();
            Check(await wire.Invoke<bool>("AddDeck", legacyDeck), "legacy-shaped complete deck saves without appearance fields");
            var password = "appearance-regression-" + Guid.NewGuid().ToString("N");
            Check(await wire.Invoke<bool>("NewMatchOfPassword", legacyDeck.Id, password, 0), "legacy-shaped deck can queue a match");
            var matches = (GwentMatchs)typeof(GwentServerService).GetField("_gwentMatchs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(server);
            var queued = matches.GwentRooms.Single(room => room.Password == password).Player1;
            var opponent = new SinkPlayer { Deck = starter };
            var game = new GwentServerGame(queued, opponent);
            Check(game.PlayersDeck[0].Count(card => card.Status.CardId == bronze && card.Status.IsPremium == true) == 1,
                "server assigns one owned premium among three legacy-shaped copper copies");
            Check(await wire.Invoke<bool>("StopMatch"), "legacy appearance probe leaves the match queue");

            await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId());
            await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", bronze, CraftId());
            var explicitDeck = JsonConvert.DeserializeObject<DeckModel>(JsonConvert.SerializeObject(starter));
            explicitDeck.Id = Guid.NewGuid().ToString();
            explicitDeck.PremiumCards = new Dictionary<string, int> { [bronze] = 3 };
            explicitDeck.PremiumLeader = false;
            Check(await wire.Invoke<bool>("AddDeck", explicitDeck), "full premium-copy selection saves");
            Check(await AwaitDeckSelection(user, explicitDeck.Id, selection => selection.PremiumCards[bronze] == 3),
                "original premium-copy selection reaches storage");

            var replacement = JsonConvert.DeserializeObject<DeckModel>(JsonConvert.SerializeObject(starter));
            replacement.Id = explicitDeck.Id;
            replacement.Deck.Remove(bronze);
            // Find a valid replacement using the domain validator, keeping the probe a full deck.
            foreach (var candidate in GwentMap.CardMap.Keys.Where(id => id != bronze))
            {
                replacement.Deck.Add(candidate);
                if (replacement.IsBasicDeck()) break;
                replacement.Deck.RemoveAt(replacement.Deck.Count - 1);
            }
            if (!replacement.IsBasicDeck() || replacement.Deck.Count(id => id == bronze) != 2)
                throw new InvalidOperationException("Could not construct a valid reduced-copy fixture.");
            Check(await wire.Invoke<bool>("ModifyDeck", explicitDeck.Id, replacement), "legacy-shaped edit preserves and trims appearance immediately");
            password = "trim-regression-" + Guid.NewGuid().ToString("N");
            Check(await wire.Invoke<bool>("NewMatchOfPassword", explicitDeck.Id, password, 0),
                "reduced premium deck matches immediately without wallet reload");
            queued = matches.GwentRooms.Single(room => room.Password == password).Player1;
            Check(queued.Deck.PremiumCards[bronze] == 2, "in-memory snapshot trims preserved premium copies");
            Check(await wire.Invoke<bool>("StopMatch"), "trim probe leaves the match queue");
            Check(await AwaitDeckSelection(user, explicitDeck.Id, selection => selection.PremiumCards[bronze] == 2),
                "background reconciliation persists the same trimmed count");

            if (wallet.Costs.ContainsKey(starter.Leader))
            {
                await wire.Invoke<PremiumCollectionResult>("CraftPremiumCopy", starter.Leader, CraftId());
                replacement.PremiumCards = new Dictionary<string, int> { [bronze] = 2 };
                replacement.PremiumLeader = true;
                Check(await wire.Invoke<bool>("ModifyDeck", explicitDeck.Id, replacement), "premium leader selection saves");
                Check(await AwaitDeckSelection(user, explicitDeck.Id, selection => selection.PremiumLeader), "premium leader selection persists");
                var newLeader = GwentMap.CardMap.First(entry => entry.Key != starter.Leader &&
                    entry.Value.Group == Cynthia.Card.Group.Leader && entry.Value.Faction == GwentMap.CardMap[starter.Leader].Faction &&
                    new DeckModel { Leader = entry.Key, Deck = replacement.Deck }.IsBasicDeck()).Key;
                var oldShape = new DeckModel { Id = replacement.Id, Leader = newLeader, Deck = replacement.Deck.ToList() };
                Check(await wire.Invoke<bool>("ModifyDeck", oldShape.Id, oldShape), "legacy-shaped leader change saves");
                password = "leader-regression-" + Guid.NewGuid().ToString("N");
                Check(await wire.Invoke<bool>("NewMatchOfPassword", oldShape.Id, password, 0), "changed leader can match before wallet reload");
                queued = matches.GwentRooms.Single(room => room.Password == password).Player1;
                Check(queued.Deck.PremiumLeader == false, "old appearance does not follow a different leader");
                Check(await wire.Invoke<bool>("StopMatch"), "leader probe leaves the match queue");
                Check(await AwaitDeckSelection(user, oldShape.Id, selection => !selection.PremiumLeader && selection.LeaderId == newLeader),
                    "changed leader reconciliation persists standard appearance");
            }
        }
    }
}
