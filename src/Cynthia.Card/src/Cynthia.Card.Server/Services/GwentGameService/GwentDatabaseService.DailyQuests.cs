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
            if (config == null) throw new InvalidDataException("Daily quest configuration is required.");
            if (config.LoginPowder <= 0 || config.Tiers == null || config.Tiers.Count == 0 ||
                config.Tiers.Any(x => x == null || x.Crowns <= 0 || x.Powder <= 0) ||
                !config.Tiers.Select(x=>x.Crowns).SequenceEqual(config.Tiers.Select(x=>x.Crowns).Distinct().OrderBy(x=>x)) ||
                config.GGPowder <= 0 || config.GGDailyCap <= 0 || config.GGDailyCap < config.GGPowder ||
                config.GGDailyCap % config.GGPowder != 0)
                throw new InvalidDataException("Daily quest thresholds must be positive and strictly increasing.");
            if ((long)config.LoginPowder + config.Tiers.Sum(x => (long)x.Powder) + config.GGDailyCap > int.MaxValue)
                throw new InvalidDataException("Daily quest rewards exceed the supported daily total.");
            return config;
        });
        private int _dailyRewardIndexesReady;
        private readonly object _dailyRewardIndexesLock = new object();

        private void EnsureDailyRewardIndexes()
        {
            if (_dailyRewardIndexesReady != 0) return;
            lock (_dailyRewardIndexesLock)
            {
                if (_dailyRewardIndexesReady != 0) return;
                var results = GetDatabase().GetCollection<GameResult>("gameresults");
                results.Indexes.CreateOne(new CreateIndexModel<GameResult>(Builders<GameResult>.IndexKeys
                    .Ascending(x => x.RedPlayerName).Ascending(x => x.BluePlayerName).Ascending(x => x.Time),
                    new CreateIndexOptions { Name = "daily_reward_players_time" }));
                _dailyRewardIndexesReady = 1;
            }
        }

        private bool HasPriorHumanMatch(string playerName, string opponentPlayerName, string currentMatchId,
            DateTimeOffset processingNow, DateTimeOffset currentEventUtc)
        {
            if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(opponentPlayerName) ||
                string.IsNullOrWhiteSpace(currentMatchId)) return false;
            EnsureDailyRewardIndexes();
            var localDay = processingNow.ToOffset(ChinaOffset).Date;
            var dayStartUtc = new DateTimeOffset(localDay, ChinaOffset).ToUniversalTime().UtcDateTime;
            var dayEndUtc = new DateTimeOffset(localDay.AddDays(1), ChinaOffset).ToUniversalTime().UtcDateTime;
            var beforeUtc = currentEventUtc.ToUniversalTime().UtcDateTime;
            if (beforeUtc < dayStartUtc) return false;
            if (beforeUtc >= dayEndUtc) beforeUtc = dayEndUtc.AddMilliseconds(-1);
            var results = GetDatabase().GetCollection<GameResult>("gameresults");
            var filter = Builders<GameResult>.Filter.And(
                Builders<GameResult>.Filter.Gte(x => x.Time, dayStartUtc),
                Builders<GameResult>.Filter.Lte(x => x.Time, beforeUtc),
                Builders<GameResult>.Filter.Ne(x => x.Id, currentMatchId),
                Builders<GameResult>.Filter.Or(
                    Builders<GameResult>.Filter.And(
                        Builders<GameResult>.Filter.Eq(x => x.RedPlayerName, playerName),
                        Builders<GameResult>.Filter.Eq(x => x.BluePlayerName, opponentPlayerName)),
                    Builders<GameResult>.Filter.And(
                        Builders<GameResult>.Filter.Eq(x => x.BluePlayerName, playerName),
                        Builders<GameResult>.Filter.Eq(x => x.RedPlayerName, opponentPlayerName))));
            return results.Find(filter).Limit(1).Any();
        }

        public async Task<DailyQuestResult> GetDailyQuests(string username) => (await UpdateDailyQuests(username, null, null, null, null, null, null, null)).Result;

        // Called exclusively by the authoritative round settlement, with a stable match/round key.
        public async Task<DailyQuestResult> AwardDailyCrown(string username, string roundId, DateTimeOffset settledUtc)
        {
            if (string.IsNullOrWhiteSpace(roundId) || roundId.Length > 100) throw new ArgumentException("Invalid round key");
            return (await UpdateDailyQuests(username,roundId,settledUtc,null,null,null,null,null)).Result;
        }

        // Authoritative human-match path. The prior-match query is performed at settlement time;
        // the current result is excluded by its server-issued GameResult.Id.
        public async Task<DailyQuestResult> AwardDailyCrown(string username, string playerName, string opponentPlayerName,
            string matchId, string roundId, DateTimeOffset settledUtc)
        {
            if (string.IsNullOrWhiteSpace(roundId) || roundId.Length > 100 ||
                string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(opponentPlayerName) ||
                string.IsNullOrWhiteSpace(matchId)) throw new ArgumentException("Invalid human match reward context");
            return (await UpdateDailyQuests(username,roundId,settledUtc,null,null,
                playerName, opponentPlayerName, matchId)).Result;
        }

        // Called exclusively by the authenticated hub GG path, with the server-issued finished-match id.
        // The recipient wallet owns the dedup ledger, so a replayed match id never pays twice on any day.
        public async Task<DailyGGAwardResult> AwardDailyGG(string username, string matchId, DateTimeOffset settledUtc)
        {
            if (string.IsNullOrWhiteSpace(matchId) || matchId.Length > 100) throw new ArgumentException("Invalid GG match key");
            var outcome = await UpdateDailyQuests(username, null, null, matchId, settledUtc, null, null, null);
            return new DailyGGAwardResult { Result = outcome.Result, Processed = outcome.ProcessedGG };
        }

        public async Task<DailyGGAwardResult> AwardDailyGG(string username, string playerName, string opponentPlayerName,
            string matchId, DateTimeOffset settledUtc)
        {
            if (string.IsNullOrWhiteSpace(matchId) || matchId.Length > 100 ||
                string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(opponentPlayerName))
                throw new ArgumentException("Invalid human match reward context");
            var outcome = await UpdateDailyQuests(username, null, null, matchId, settledUtc,
                playerName, opponentPlayerName, matchId);
            return new DailyGGAwardResult { Result = outcome.Result, Processed = outcome.ProcessedGG };
        }

        private async Task<(DailyQuestResult Result, bool ProcessedGG)> UpdateDailyQuests(string username, string roundId, DateTimeOffset? settledUtc, string ggMatchId, DateTimeOffset? ggSettledUtc, string playerName, string opponentPlayerName, string currentMatchId)
        {
            // Validate before wallet creation or any reward write.
            var config = DailyConfig.Value;
            var wallet = await GetPremiumCollection(username);
            if (!wallet.Success) return (new DailyQuestResult { Status = wallet.Status }, false);
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var now = DailyQuestClock().ToUniversalTime();
                string today = now.ToOffset(ChinaOffset).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
                var account = wallet.Collection;
                var prior = account.DailyQuests;
                // Reward attribution uses this processing attempt's server day, not the event's day.
                // Never roll a stored day backwards when the server clock moves behind persisted state.
                if (prior != null && string.CompareOrdinal(prior.Day,today)>0) return (DailyResult(account,now,"clock_behind"), false);
                bool newDay = prior == null || prior.Day != today;
                bool? sameOpponentAllowed = null;
                if (playerName != null || opponentPlayerName != null || currentMatchId != null)
                {
                    // Re-evaluate against this CAS attempt's server day and event position.
                    var eventUtc = ggSettledUtc ?? settledUtc.Value;
                    sameOpponentAllowed = !string.IsNullOrWhiteSpace(playerName) &&
                        !string.IsNullOrWhiteSpace(opponentPlayerName) &&
                        !string.IsNullOrWhiteSpace(currentMatchId) &&
                        !HasPriorHumanMatch(playerName, opponentPlayerName, currentMatchId, now, eventUtc);
                }
                var processedRounds = new HashSet<string>(prior?.ProcessedRoundIds ?? new List<string>());
                // Upgrade existing wallets before replacing their day: their current credited keys
                // predate the persistent cross-day ledger. Previously discarded history is unavailable.
                if (prior?.RoundIds != null) processedRounds.UnionWith(prior.RoundIds);
                // The GG ledger survives daily resets exactly like the crown ledger.
                var processedGGs = new HashSet<string>(prior?.ProcessedGGIds ?? new List<string>());
                var progress = newDay ? new DailyQuestProgress { Day=today } :
                    new DailyQuestProgress { Day=prior.Day,LoginGranted=prior.LoginGranted,Crowns=prior.Crowns,
                        PowderGranted=prior.PowderGranted,GGReceived=prior.GGReceived,GGPowderGranted=prior.GGPowderGranted,
                        RoundIds=new List<string>(prior.RoundIds ?? new List<string>()) };
                int reward = 0;
                bool changed = newDay || prior.RoundIds == null || prior.ProcessedRoundIds == null || prior.ProcessedGGIds == null;
                if (!progress.LoginGranted)
                { progress.LoginGranted=true; reward+=config.LoginPowder; changed=true; }
                bool validRound = settledUtc.HasValue && settledUtc.Value <= now;
                if (validRound && processedRounds.Add(roundId))
                {
                    changed=true;
                    if (sameOpponentAllowed != false && progress.Crowns < config.Tiers.Last().Crowns)
                    {
                        progress.RoundIds.Add(roundId); progress.Crowns++;
                        reward += config.Tiers.Where(x=>x.Crowns==progress.Crowns).Sum(x=>x.Powder);
                    }
                }
                // A GG settles once per finished match on the recipient wallet. Capped events are
                // still recorded so replaying the same server-issued match id on a later day cannot pay.
                bool processedGG = false;
                bool validGG = ggSettledUtc.HasValue && ggSettledUtc.Value <= now;
                if (validGG && processedGGs.Add(ggMatchId))
                {
                    changed = true; processedGG = true;
                    int maxGG = config.GGDailyCap / config.GGPowder;
                    if (sameOpponentAllowed != false && progress.GGReceived < maxGG && progress.GGPowderGranted + config.GGPowder <= config.GGDailyCap)
                    {
                        progress.GGReceived++;
                        progress.GGPowderGranted = checked(progress.GGPowderGranted + config.GGPowder);
                        reward += config.GGPowder;
                    }
                }
                if (!changed) return (DailyResult(account,now,"ok"), false);
                progress.ProcessedRoundIds = processedRounds.ToList();
                progress.ProcessedGGIds = processedGGs.ToList();
                progress.PowderGranted = checked(progress.PowderGranted + reward);
                // Progress, deduplication and powder balance commit together in a single Mongo document.
                // Revision CAS also composes safely with concurrent crafting and admin grants.
                var updated = await PremiumAccounts.FindOneAndUpdateAsync(
                    Builders<PremiumCollection>.Filter.Eq(x=>x.Id,account.Id) & Builders<PremiumCollection>.Filter.Eq(x=>x.Revision,account.Revision),
                    Builders<PremiumCollection>.Update.Set(x=>x.DailyQuests,progress)
                        .Inc(x=>x.MeteoritePowder,reward).Inc(x=>x.Revision,1),
                    new FindOneAndUpdateOptions<PremiumCollection> { ReturnDocument=ReturnDocument.After });
                if (updated != null) return (DailyResult(updated,now,"ok"), processedGG);
                wallet.Collection = await PremiumAccounts.Find(x=>x.Id==account.Id).FirstAsync();
            }
            throw new InvalidOperationException("Daily rewards could not settle after concurrent wallet updates.");
        }
        private static DailyQuestResult DailyResult(PremiumCollection account, DateTimeOffset now, string status)
        {
            var config = DailyConfig.Value;
            var midnight = new DateTimeOffset(now.ToOffset(ChinaOffset).Date.AddDays(1),ChinaOffset);
            return new DailyQuestResult { Status=status,ServerUtc=now.ToString("O"),ResetUtc=midnight.ToUniversalTime().ToString("O"),
                LoginPowder=config.LoginPowder,DailyCap=config.LoginPowder+config.Tiers.Sum(x=>x.Powder)+config.GGDailyCap,
                GGPowder=config.GGPowder,GGDailyCap=config.GGDailyCap,Tiers=config.Tiers,
                Wallet=Result("ok",account) };
        }
        private sealed class DailyQuestConfig
        {
            public int LoginPowder { get; set; }
            public int GGPowder { get; set; } = 5;
            public int GGDailyCap { get; set; } = 30;
            public List<DailyQuestTier> Tiers { get; set; }
        }
    }

    // Server-only outcome of a GG settlement. Processed means this exact finished match had not
    // been seen before, so social display/count may be updated exactly once for it.
    public sealed class DailyGGAwardResult
    {
        public DailyQuestResult Result { get; set; }
        public bool Processed { get; set; }
        public bool Success => Result != null && Result.Success;
    }
}
