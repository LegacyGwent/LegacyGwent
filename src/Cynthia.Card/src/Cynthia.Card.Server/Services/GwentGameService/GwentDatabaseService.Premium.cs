using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Cynthia.Card.Server
{
    public partial class GwentDatabaseService
    {
        private IMongoCollection<PremiumCollection> PremiumAccounts => GetDatabase().GetCollection<PremiumCollection>("premium_collection");
        private static readonly Lazy<Dictionary<string, int>> PremiumCosts = new Lazy<Dictionary<string, int>>(() =>
        {
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "PremiumCrafting.json"));
            var config = JsonConvert.DeserializeObject<PremiumCraftingConfig>(json);
            if (config.Copper <= 0 || config.Silver <= 0 || config.Gold <= 0 || config.Leader <= 0)
                throw new InvalidDataException("Premium crafting costs must be positive.");
            var arts = new HashSet<string>(config.AvailableArtIds);
            return GwentMap.CardMap.Where(x => arts.Contains(x.Value.CardArtsId))
                .ToDictionary(x => x.Key, x => x.Value.Group == Group.Copper ? config.Copper :
                    x.Value.Group == Group.Silver ? config.Silver : x.Value.Group == Group.Leader ? config.Leader : config.Gold);
        });

        public async Task<PremiumCollectionResult> GetPremiumCollection(string username)
        {
            var user = await GetUserInfo().Find(x => x.UserName == username).FirstOrDefaultAsync();
            if (user == null) return new PremiumCollectionResult { Status = "unauthenticated" };
            await EnsureInitialPowder(user.Id);
            while (true)
            {
                var account = await PremiumAccounts.Find(x => x.Id == user.Id).FirstAsync();
                if (account.InventoryVersion >= 1) return Result("ok", account);
                var copies = (account.OwnedCards ?? new List<string>()).Distinct()
                    .Where(x => CardInventory.Limit(x) > 0).ToDictionary(x => x, CardInventory.Limit);
                var revision = Builders<PremiumCollection>.Filter.Eq(x => x.Revision, account.Revision);
                if (account.Revision == 0) revision |= Builders<PremiumCollection>.Filter.Exists(x => x.Revision, false);
                var updated = await PremiumAccounts.FindOneAndUpdateAsync(
                    Builders<PremiumCollection>.Filter.Eq(x => x.Id, user.Id) & revision,
                    Builders<PremiumCollection>.Update.Set(x => x.InventoryVersion, 1)
                        .Set(x => x.PremiumCopies, copies).Set(x => x.CraftReceipts, new List<PremiumCraftReceipt>())
                        .Set(x => x.OwnedCards, account.OwnedCards ?? new List<string>())
                        .Set(x => x.SelectedCards, account.SelectedCards ?? new List<string>())
                        .Inc(x => x.Revision, 1),
                    new FindOneAndUpdateOptions<PremiumCollection> { ReturnDocument = ReturnDocument.After });
                if (updated != null) return Result("ok", updated);
            }
        }

        private static PremiumCollectionResult Result(string status, PremiumCollection account) =>
            new PremiumCollectionResult { Status = status, Collection = account, Costs = PremiumCosts.Value };

        // Legacy clients have no operation ID. Retain their one-unlock behavior on retries.
        public Task<PremiumCollectionResult> CraftPremium(string username, string cardId) =>
            CraftPremiumCore(username, cardId, null);

        public Task<PremiumCollectionResult> CraftPremiumCopy(string username, string cardId, string requestId)
        {
            if (!Guid.TryParseExact(requestId, "N", out _))
                return Task.FromResult(new PremiumCollectionResult { Status = "invalid_request" });
            return CraftPremiumCore(username, cardId, requestId);
        }

        private async Task<PremiumCollectionResult> CraftPremiumCore(string username, string cardId, string requestId)
        {
            var current = await GetPremiumCollection(username);
            if (!current.Success) return current;
            if (cardId == null || !PremiumCosts.Value.TryGetValue(cardId, out var cost)) return Result("unavailable", current.Collection);
            var f = Builders<PremiumCollection>.Filter;
            while (true)
            {
                var account = current.Collection;
                var receipt = requestId == null ? null : account.CraftReceipts?.FirstOrDefault(x => x.RequestId == requestId);
                if (receipt != null) return Result(receipt.CardId == cardId ? "ok" : "request_conflict", account);
                int count = CardInventory.PremiumCount(account, cardId);
                if (count >= CardInventory.Limit(cardId) || (requestId == null && count > 0))
                    return Result("already_owned", account);
                if (account.MeteoritePowder < cost) return Result("insufficient_powder", account);
                var copies = new Dictionary<string, int>(account.PremiumCopies) { [cardId] = count + 1 };
                var change = Builders<PremiumCollection>.Update.Inc(x => x.MeteoritePowder, -cost).Inc(x => x.Revision, 1)
                    .Set(x => x.PremiumCopies, copies).AddToSet(x => x.OwnedCards, cardId).AddToSet(x => x.SelectedCards, cardId);
                if (requestId != null) change = change.Push(x => x.CraftReceipts, new PremiumCraftReceipt { RequestId = requestId, CardId = cardId });
                var updated = await PremiumAccounts.FindOneAndUpdateAsync(
                    f.Eq(x => x.Id, account.Id) & f.Eq(x => x.Revision, account.Revision) & f.Gte(x => x.MeteoritePowder, cost), change,
                    new FindOneAndUpdateOptions<PremiumCollection> { ReturnDocument = ReturnDocument.After });
                if (updated != null) return Result("ok", updated);
                current = Result("ok", await PremiumAccounts.Find(x => x.Id == account.Id).FirstAsync());
            }
        }

        public async Task<PremiumCollectionResult> SelectPremium(string username, string cardId, bool premium)
        {
            var current = await GetPremiumCollection(username);
            if (!current.Success) return current;
            if (cardId == null || !GwentMap.CardMap.ContainsKey(cardId)) return Result("unavailable", current.Collection);
            var f = Builders<PremiumCollection>.Filter;
            var filter = f.Eq(x => x.Id, current.Collection.Id);
            if (premium) filter &= f.AnyEq(x => x.OwnedCards, cardId);
            var change = premium ? Builders<PremiumCollection>.Update.AddToSet(x => x.SelectedCards, cardId) :
                Builders<PremiumCollection>.Update.Pull(x => x.SelectedCards, cardId);
            var updated = await PremiumAccounts.FindOneAndUpdateAsync(filter, change.Inc(x => x.Revision, 1),
                new FindOneAndUpdateOptions<PremiumCollection> { ReturnDocument = ReturnDocument.After });
            return Result(updated == null ? "not_owned" : "ok", updated ?? current.Collection);
        }

        public async Task<bool> SaveDeckSelection(string username, string deckId, PremiumDeckSelection selection)
        {
            if (!Guid.TryParse(deckId, out _) || selection == null)
                return false;
            var current = await GetPremiumCollection(username);
            if (!current.Success)
                return false;
            var premiumCards = selection.PremiumCards ?? new Dictionary<string, int>();
            if (premiumCards.Any(entry => entry.Value < 0 ||
                    CardInventory.PremiumCount(current.Collection, entry.Key) < entry.Value))
                return false;
            if (selection.PremiumLeader && CardInventory.PremiumCount(current.Collection, selection.LeaderId) == 0)
                return false;
            var normalized = new PremiumDeckSelection
            {
                PremiumCards = new Dictionary<string, int>(premiumCards),
                LeaderId = selection.LeaderId,
                PremiumLeader = selection.PremiumLeader
            };
            var update = Builders<PremiumCollection>.Update
                .Set("DeckSelections." + deckId, normalized)
                .Inc(x => x.Revision, 1);
            var result = await PremiumAccounts.UpdateOneAsync(x => x.Id == current.Collection.Id, update);
            return result.MatchedCount == 1;
        }

        public async Task<bool> ReconcileDeckSelection(string username, DeckModel deck)
        {
            if (deck == null || !Guid.TryParse(deck.Id, out _))
                return false;
            var current = await GetPremiumCollection(username);
            if (!current.Success)
                return false;
            if (current.Collection.DeckSelections == null ||
                !current.Collection.DeckSelections.TryGetValue(deck.Id, out var selection) || selection == null)
                return true;
            var available = (deck.Deck ?? new List<string>()).GroupBy(id => id)
                .ToDictionary(group => group.Key, group => group.Count());
            var reconciled = new PremiumDeckSelection
            {
                LeaderId = deck.Leader,
                PremiumLeader = selection.LeaderId == deck.Leader && selection.PremiumLeader,
                PremiumCards = (selection.PremiumCards ?? new Dictionary<string, int>())
                    .Where(entry => available.TryGetValue(entry.Key, out var count) && entry.Value > 0)
                    .ToDictionary(entry => entry.Key, entry => Math.Min(entry.Value, available[entry.Key]))
            };
            return await SaveDeckSelection(username, deck.Id, reconciled);
        }

        public async Task RemoveDeckSelection(string username, string deckId)
        {
            if (!Guid.TryParse(deckId, out _))
                return;
            var user = await GetUserInfo().Find(x => x.UserName == username).Project(x => x.Id).FirstOrDefaultAsync();
            if (user == null)
                return;
            await PremiumAccounts.UpdateOneAsync(x => x.Id == user,
                Builders<PremiumCollection>.Update.Unset("DeckSelections." + deckId).Inc(x => x.Revision, 1));
        }

        private sealed class PremiumCraftingConfig
        {
            public int Copper { get; set; }
            public int Silver { get; set; }
            public int Gold { get; set; }
            public int Leader { get; set; }
            public string[] AvailableArtIds { get; set; }
        }
    }
}
