using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public partial class GwentDatabaseService
    {
        // Stable business identity: changing Amount must never turn this into another grant.
        public const string InitialPowderRewardId = "initial-meteorite-powder-v1";
        private const long MaxSafePowder = 9007199254740991;
        private readonly Lazy<InitialPowderOptions> _initialPowder;

        private async Task<bool> EnsureInitialPowder(string playerId, CancellationToken cancellationToken = default)
        {
            var options = _initialPowder.Value; // Validate before creating or mutating a wallet.
            var amount = options.Amount;
            var accounts = PremiumAccounts.WithWriteConcern(WriteConcern.WMajority);
            var f = Builders<PremiumCollection>.Filter;
            var identity = f.Eq(x => x.Id, playerId);
            var update = Builders<PremiumCollection>.Update
                .SetOnInsert(x => x.MeteoritePowder, 0).SetOnInsert(x => x.Revision, 0)
                .SetOnInsert(x => x.InitialPowderGranted, false)
                .SetOnInsert(x => x.OwnedCards, new List<string>()).SetOnInsert(x => x.SelectedCards, new List<string>())
                .SetOnInsert(x => x.InventoryVersion, 1).SetOnInsert(x => x.PremiumCopies, new Dictionary<string, int>())
                .SetOnInsert(x => x.CraftReceipts, new List<PremiumCraftReceipt>())
                .SetOnInsert(x => x.Rewards, new List<PowderReward>())
                .SetOnInsert(x => x.DeckSelections, new Dictionary<string, PremiumDeckSelection>());
            try
            {
                await accounts.UpdateOneAsync(x => x.Id == playerId, update,
                    new UpdateOptions { IsUpsert = true }, cancellationToken);
            }
            catch (MongoWriteException e) when (e.WriteError.Category == ServerErrorCategory.DuplicateKey) { }
            if (!options.Enabled) return false; // Pause without consuming eligibility.

            // Old documents may have a missing or explicit-null reward ledger.
            await accounts.UpdateOneAsync(identity & f.Eq(x => x.Rewards, null),
                Builders<PremiumCollection>.Update.Set(x => x.Rewards, new List<PowderReward>()).Inc(x => x.Revision, 1),
                cancellationToken: cancellationToken);
            // Receipt was the original idempotency marker. Promote it to the explicit flag
            // without paying again when an installation upgrades this schema.
            await accounts.UpdateOneAsync(identity & f.Ne(x => x.InitialPowderGranted, true)
                    & f.Eq("Rewards.RewardId", InitialPowderRewardId),
                Builders<PremiumCollection>.Update.Set(x => x.InitialPowderGranted, true).Inc(x => x.Revision, 1),
                cancellationToken: cancellationToken);
            var eligible = identity & f.Ne(x => x.InitialPowderGranted, true)
                & f.Ne("Rewards.RewardId", InitialPowderRewardId);
            var result = await accounts.UpdateOneAsync(eligible & f.Lte(x => x.MeteoritePowder, MaxSafePowder - amount),
                Builders<PremiumCollection>.Update.Inc(x => x.MeteoritePowder, amount).Inc(x => x.Revision, 1)
                    .Set(x => x.InitialPowderGranted, true)
                    .Push(x => x.Rewards, new PowderReward
                    {
                        RewardId = InitialPowderRewardId, Amount = amount,
                        Reason = "One-time initial meteorite powder", GrantedUtc = DateTimeOffset.UtcNow.ToString("O")
                    }), cancellationToken: cancellationToken);
            if (result.ModifiedCount != 0) return true;
            if (await accounts.Find(eligible).AnyAsync(cancellationToken))
                throw new InvalidDataException("Initial powder cannot be granted to wallet " + playerId + ": balance exceeds safe range.");
            return false;
        }

        public async Task<long> BackfillInitialPowder(CancellationToken cancellationToken = default,
            Action<string, Exception> onError = null)
        {
            if (!_initialPowder.Value.Enabled) return 0;
            long granted = 0, errors = 0;
            // Stream IDs instead of materializing all accounts or loading passwords/decks.
            using (var cursor = await GetUserInfo().Find(Builders<UserInfo>.Filter.Empty)
                .Project(x => x.Id).ToCursorAsync(cancellationToken))
            {
                while (await cursor.MoveNextAsync(cancellationToken))
                    foreach (var id in cursor.Current)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try { if (await EnsureInitialPowder(id, cancellationToken)) granted++; }
                        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
                        catch (Exception e) { errors++; onError?.Invoke(id, e); }
                    }
            }
            if (errors != 0) throw new InvalidOperationException("Initial powder backfill has " + errors + " failed accounts; retry is safe.");
            return granted;
        }
    }
}
