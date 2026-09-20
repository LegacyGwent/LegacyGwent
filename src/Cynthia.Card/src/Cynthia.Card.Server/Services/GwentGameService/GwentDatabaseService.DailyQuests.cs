using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Cynthia.Card.Server
{
    public partial class GwentDatabaseService
    {
        // A server-only injectable clock; no hub accepts a date, crown count or reward amount.
        public Func<DateTimeOffset> DailyQuestClock { get; set; } = () => DateTimeOffset.UtcNow;
        private static readonly TimeSpan ChinaOffset = TimeSpan.FromHours(8);
        private static readonly Lazy<DailyQuestConfig> DailyConfig = new Lazy<DailyQuestConfig>(() =>
        {
            var config = JsonConvert.DeserializeObject<DailyQuestConfig>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"DailyQuests.json")));
            if (config == null || config.LoginPowder <= 0 || config.Tiers == null || config.Tiers.Count == 0 ||
                config.Tiers.Any(x => x == null || x.Crowns <= 0 || x.Powder <= 0) ||
                !config.Tiers.Select(x=>x.Crowns).SequenceEqual(config.Tiers.Select(x=>x.Crowns).Distinct().OrderBy(x=>x)))
                throw new InvalidDataException("Daily quest thresholds must be positive and strictly increasing.");
            if ((long)config.LoginPowder + config.Tiers.Sum(x => (long)x.Powder) > int.MaxValue)
                throw new InvalidDataException("Daily quest rewards exceed the supported daily total.");
            return config;
        });
        public Task<DailyQuestResult> GetDailyQuests(string username) => UpdateDailyQuests(username, null, null);

        // Called exclusively by the authoritative round settlement, with a stable match/round key.
        public Task<DailyQuestResult> AwardDailyCrown(string username, string roundId, DateTimeOffset settledUtc)
        {
            if (string.IsNullOrWhiteSpace(roundId) || roundId.Length > 100) throw new ArgumentException("Invalid round key");
            return UpdateDailyQuests(username,roundId,settledUtc);
        }

        private async Task<DailyQuestResult> UpdateDailyQuests(string username, string roundId, DateTimeOffset? settledUtc)
        {
            // Validate before wallet creation or any reward write.
            var config = DailyConfig.Value;
            var wallet = await GetPremiumCollection(username);
            if (!wallet.Success) return new DailyQuestResult { Status = wallet.Status };
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var now = DailyQuestClock().ToUniversalTime();
                string today = now.ToOffset(ChinaOffset).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
                var account = wallet.Collection;
                var prior = account.DailyQuests;
                // Reward attribution uses this processing attempt's server day, not the round's day.
                // Never roll a stored day backwards when the server clock moves behind persisted state.
                if (prior != null && string.CompareOrdinal(prior.Day,today)>0) return DailyResult(account,now,"clock_behind");
                bool newDay = prior == null || prior.Day != today;
                var processedRounds = new HashSet<string>(prior?.ProcessedRoundIds ?? new List<string>());
                // Upgrade existing wallets before replacing their day: their current credited keys
                // predate the persistent cross-day ledger. Previously discarded history is unavailable.
                if (prior?.RoundIds != null) processedRounds.UnionWith(prior.RoundIds);
                var progress = newDay ? new DailyQuestProgress { Day=today } :
                    new DailyQuestProgress { Day=prior.Day,LoginGranted=prior.LoginGranted,Crowns=prior.Crowns,
                        PowderGranted=prior.PowderGranted,RoundIds=new List<string>(prior.RoundIds ?? new List<string>()) };
                int reward = 0;
                bool changed = newDay || prior.RoundIds == null || prior.ProcessedRoundIds == null;
                if (!progress.LoginGranted)
                { progress.LoginGranted=true; reward+=config.LoginPowder; changed=true; }
                bool validRound = settledUtc.HasValue && settledUtc.Value <= now;
                if (validRound && processedRounds.Add(roundId))
                {
                    changed=true;
                    if (progress.Crowns < config.Tiers.Last().Crowns)
                    {
                        progress.RoundIds.Add(roundId); progress.Crowns++;
                        reward += config.Tiers.Where(x=>x.Crowns==progress.Crowns).Sum(x=>x.Powder);
                    }
                }
                if (!changed) return DailyResult(account,now,"ok");
                progress.ProcessedRoundIds = processedRounds.ToList();
                progress.PowderGranted = checked(progress.PowderGranted + reward);
                // Progress, deduplication and powder balance commit together in a single Mongo document.
                // Revision CAS also composes safely with concurrent crafting and admin grants.
                var updated = await PremiumAccounts.FindOneAndUpdateAsync(
                    Builders<PremiumCollection>.Filter.Eq(x=>x.Id,account.Id) & Builders<PremiumCollection>.Filter.Eq(x=>x.Revision,account.Revision),
                    Builders<PremiumCollection>.Update.Set(x=>x.DailyQuests,progress)
                        .Inc(x=>x.MeteoritePowder,reward).Inc(x=>x.Revision,1),
                    new FindOneAndUpdateOptions<PremiumCollection> { ReturnDocument=ReturnDocument.After });
                if (updated != null) return DailyResult(updated,now,"ok");
                wallet.Collection = await PremiumAccounts.Find(x=>x.Id==account.Id).FirstAsync();
            }
            throw new InvalidOperationException("Daily rewards could not settle after concurrent wallet updates.");
        }
        private static DailyQuestResult DailyResult(PremiumCollection account, DateTimeOffset now, string status)
        {
            var config = DailyConfig.Value;
            var midnight = new DateTimeOffset(now.ToOffset(ChinaOffset).Date.AddDays(1),ChinaOffset);
            return new DailyQuestResult { Status=status,ServerUtc=now.ToString("O"),ResetUtc=midnight.ToUniversalTime().ToString("O"),
                LoginPowder=config.LoginPowder,DailyCap=config.LoginPowder+config.Tiers.Sum(x=>x.Powder),Tiers=config.Tiers,
                Wallet=Result("ok",account) };
        }
        private sealed class DailyQuestConfig
        {
            public int LoginPowder { get; set; }
            public List<DailyQuestTier> Tiers { get; set; }
        }
    }
}
