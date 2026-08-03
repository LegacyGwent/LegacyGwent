using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class RecentGameResultsTests
    {
        [Fact]
        public void RecentTimelineMergesPlayerAndAiCollectionsByTime()
        {
            var playerResults = new List<GameResult>
            {
                ResultAt(1, "player-old"),
                ResultAt(3, "player-new")
            };
            var aiResults = new List<GameResult>
            {
                ResultAt(2, "ai-old"),
                ResultAt(4, "ai-new")
            };

            var recent = GwentDatabaseService.MergeRecentGameResults(
                playerResults,
                aiResults,
                3);

            Assert.Equal(
                new[] { "ai-new", "player-new", "ai-old" },
                recent.Select(result => result.RedPlayerName));
        }

        [Fact]
        public void RecentTimelineHandlesEmptyCollectionsAndNonPositiveLimit()
        {
            Assert.Empty(GwentDatabaseService.MergeRecentGameResults(null, null, 50));
            Assert.Empty(GwentDatabaseService.MergeRecentGameResults(
                new[] { ResultAt(1, "player") },
                Array.Empty<GameResult>(),
                0));
        }

        private static GameResult ResultAt(int minute, string playerName)
        {
            return new GameResult
            {
                Time = new DateTime(2026, 8, 3, 0, minute, 0, DateTimeKind.Utc),
                RedPlayerName = playerName
            };
        }
    }
}
