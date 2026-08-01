using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.Common.Models;

namespace Cynthia.Card.Server.Services
{
    public class SeasonOverviewService
    {
        private static readonly Faction[] SupportedFactions =
        {
            Faction.NorthernRealms,
            Faction.Nilfgaard,
            Faction.Skellige,
            Faction.ScoiaTael,
            Faction.Monsters
        };

        private readonly GwentDatabaseService _databaseService;
        private readonly object _cacheLock = new object();
        private IReadOnlyList<SeasonOverview> _cache;
        private DateTime _cacheExpiresUtc = DateTime.MinValue;
        private Task<IReadOnlyList<SeasonOverview>> _refreshTask;

        public SeasonOverviewService(GwentDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<IReadOnlyList<SeasonOverview>> GetAsync()
        {
            lock (_cacheLock)
            {
                if (_cache != null && DateTime.UtcNow < _cacheExpiresUtc)
                {
                    return Task.FromResult(_cache);
                }

                if (_refreshTask == null)
                {
                    _refreshTask = Task.Run(BuildAndCache);
                }

                return _refreshTask;
            }
        }

        private IReadOnlyList<SeasonOverview> BuildAndCache()
        {
            try
            {
                var result = Build();
                lock (_cacheLock)
                {
                    _cache = result;
                    _cacheExpiresUtc = DateTime.UtcNow.AddMinutes(2);
                }

                return result;
            }
            finally
            {
                lock (_cacheLock)
                {
                    _refreshTask = null;
                }
            }
        }

        private IReadOnlyList<SeasonOverview> Build()
        {
            var seasons = (_databaseService.QuerySeasons() ?? new List<SeasonInfo>())
                .Where(season => season != null)
                .OrderByDescending(season => season.isActive)
                .ThenByDescending(season => season.SeasonStartTime)
                .ToList();

            var overviews = seasons.ToDictionary(
                season => season,
                season => new SeasonOverview
                {
                    Season = season,
                    Factions = SupportedFactions.Select(faction => new FactionOverview { Faction = faction }).ToList()
                });

            var rankedResults = seasons.Count == 0
                ? new List<GameResult>()
                : _databaseService.GetRankedGameResultsForPeriod(
                    seasons.Min(season => season.SeasonStartTime),
                    seasons.Max(season => season.SeasonEndTime))
                    ?? new List<GameResult>();

            foreach (var result in rankedResults.Where(result => result != null && result.IsEffective()))
            {
                var season = seasons.FirstOrDefault(candidate =>
                    result.Time >= candidate.SeasonStartTime
                    && result.Time < candidate.SeasonEndTime);
                if (season == null)
                {
                    continue;
                }

                var overview = overviews[season];
                overview.MatchCount++;
                var day = result.Time.Date;
                overview.DailyMatches[day] = overview.DailyMatches.TryGetValue(day, out var count)
                    ? count + 1
                    : 1;

                AddPlayerResult(overview, result.RedLeaderId, result.RedPlayerStatus());
                AddPlayerResult(overview, result.BlueLeaderId, result.BluePlayerStatus());
            }

            foreach (var overview in overviews.Values)
            {
                overview.Activity = overview.DailyMatches
                    .OrderBy(item => item.Key)
                    .Select(item => new DayActivity { Date = item.Key, Matches = item.Value })
                    .ToList();
            }

            return seasons.Select(season => overviews[season]).ToList();
        }

        private static void AddPlayerResult(SeasonOverview overview, string leaderId, GameStatus status)
        {
            if (string.IsNullOrWhiteSpace(leaderId)
                || !GwentMap.CardMap.TryGetValue(leaderId, out var leader))
            {
                return;
            }

            var faction = overview.Factions.FirstOrDefault(item => item.Faction == leader.Faction);
            if (faction == null)
            {
                return;
            }

            faction.Picks++;
            switch (status)
            {
                case GameStatus.Win:
                    faction.Wins++;
                    break;
                case GameStatus.Lose:
                    faction.Losses++;
                    break;
                case GameStatus.Draw:
                    faction.Draws++;
                    break;
            }
        }

    }

    public class SeasonOverview
    {
        public SeasonInfo Season { get; set; }
        public int MatchCount { get; set; }
        public Dictionary<DateTime, int> DailyMatches { get; } = new Dictionary<DateTime, int>();
        public IList<DayActivity> Activity { get; set; } = new List<DayActivity>();
        public IList<FactionOverview> Factions { get; set; } = new List<FactionOverview>();
        public int PlayerPicks => Factions.Sum(faction => faction.Picks);
        public int ActiveDays => DailyMatches.Count;
    }

    public class DayActivity
    {
        public DateTime Date { get; set; }
        public int Matches { get; set; }
    }

    public class FactionOverview
    {
        public Faction Faction { get; set; }
        public int Picks { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public double WinRate => Picks == 0 ? 0 : Wins * 100d / Picks;
    }
}
