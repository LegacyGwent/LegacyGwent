using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Cynthia.Card;
using Cynthia.Card.AI;
using Cynthia.Card.Common.Models.Localization;
using Cynthia.Card.Server.Services.GwentGameService;
using Newtonsoft.Json;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class DiyAiCardPoolTests
    {
        [Fact]
        public void ResetPoolRetiresOnlyDiyCardsAndPreservesSystemCards()
        {
            Assert.Equal(183, DiyAiCardPool.RetiredCardIds.Count);
            Assert.Equal(10, DiyAiCardPool.SystemCardIds.Count);
            Assert.Empty(DiyAiCardPool.RetiredCardIds.Intersect(DiyAiCardPool.SystemCardIds));

            Assert.All(
                DiyAiCardPool.RetiredCardIds,
                cardId => Assert.True(GwentMap.CardMap[cardId].IsDerive));
            Assert.All(
                DiyAiCardPool.SystemCardIds,
                cardId => Assert.True(GwentMap.CardMap.ContainsKey(cardId)));
            Assert.DoesNotContain(
                GwentMap.CardMap.Values.SelectMany(card => card.LinkedCards ?? Enumerable.Empty<string>()),
                DiyAiCardPool.RetiredCardIds.Contains);
        }

        [Fact]
        public void CardMapOrdinalOrderRemainsHistoricalDecodeCompatible()
        {
            Assert.Equal(new Version(1, 0, 0, 155), GwentMap.CardMapVersion);
            Assert.Equal(709, GwentMap.CardMap.Count);

            var orderedIds = string.Join(",", GwentMap.CardMap.Keys);
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(orderedIds)))
                .ToLowerInvariant();

            Assert.Equal("1d92e39fd29ffd178e2998d5c9bc761cebd37bf5c3adee040505840d9fab84a9", hash);
        }

        [Fact]
        public void OriginalStarterDeckIsPlayableAndRetiredCardsAreRejected()
        {
            var starter = GwentDeck.CreateBasicDeck(0);
            Assert.True(starter.IsBasicDeck());
            Assert.All(starter.Deck, cardId => Assert.True(DiyAiCardPool.IsUserDeckCard(cardId)));

            starter.Deck[0] = "70001";
            Assert.False(starter.IsBasicDeck());
            Assert.False(starter.IsSpecialDeck());
        }

        [Fact]
        public void DatabaseMigrationAllowlistMatchesRuntimeUserCardPool()
        {
            var script = File.ReadAllText(FindRepositoryFile("deploy/diy-ai/reset-card-pool.js"));
            var rangeBlock = Regex.Match(
                script,
                @"var allowedUserCardRanges = \[(?<ranges>.*?)\];",
                RegexOptions.Singleline);
            Assert.True(rangeBlock.Success);

            var migrationIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match range in Regex.Matches(rangeBlock.Groups["ranges"].Value, @"\[(\d+),\s*(\d+)\]"))
            {
                var first = int.Parse(range.Groups[1].Value);
                var last = int.Parse(range.Groups[2].Value);
                Assert.True(first <= last);
                foreach (var id in Enumerable.Range(first, last - first + 1))
                {
                    Assert.True(migrationIds.Add(id.ToString()));
                }
            }

            var runtimeIds = GwentMap.CardMap.Keys
                .Where(DiyAiCardPool.IsUserDeckCard)
                .OrderBy(id => id, StringComparer.Ordinal);
            Assert.Equal(runtimeIds, migrationIds.OrderBy(id => id, StringComparer.Ordinal));
            Assert.DoesNotContain("89009", migrationIds);
            Assert.DoesNotContain("89010", migrationIds);
            Assert.Contains("typeof id === \"string\"", script);
            Assert.Contains(
                "Object.prototype.hasOwnProperty.call(allowedUserCardIds, id)",
                script);
            Assert.DoesNotContain("Number(text)", script);
        }

        [Fact]
        public void ChineseClientDescriptionsFollowTheActiveCardRules()
        {
            var locale = JsonConvert.DeserializeObject<GameLocale>(
                File.ReadAllText(FindRepositoryFile(
                    "src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));

            GwentLocalizationService.ApplyChineseCardRules(locale);

            Assert.All(GwentMap.CardMap, card =>
            {
                Assert.True(locale.CardLocales.TryGetValue(card.Key, out var cardLocale));
                Assert.Equal(card.Value.Name, cardLocale.Name);
                Assert.Equal(card.Value.Info, cardLocale.Info);
            });
            Assert.Equal("对1个敌军造成4点伤害。", locale.CardLocales["44016"].Info);
            Assert.Contains("最强单位", locale.CardLocales["14019"].Info);
            Assert.Contains("最弱单位", locale.CardLocales["14019"].Info);
        }

        [Fact]
        public void AiZeroThroughFiveKeepAllRuntimeCardDependencies()
        {
            var players = new AIPlayer[]
            {
                new GeraltNovaAI(),
                new SoldierTrainAI(),
                new MillAI(),
                new AuberonKingAI(),
                new IronFalconAI(),
                new ReaverHunterAI()
            };

            Assert.All(players, player =>
            {
                Assert.True(GwentMap.CardMap.ContainsKey(player.Deck.Leader));
                Assert.All(player.Deck.Deck, cardId =>
                {
                    Assert.True(GwentMap.CardMap.ContainsKey(cardId));
                    Assert.DoesNotContain(cardId, DiyAiCardPool.RetiredCardIds);
                });
            });
        }

        [Fact]
        public void DiyWeatherRulesRemainExplicitExceptions()
        {
            Assert.Equal(10, GwentMap.CardMap["12009"].Strength);
            Assert.Contains("最强单位", GwentMap.CardMap["14019"].Info);
            Assert.Contains("最弱单位", GwentMap.CardMap["14019"].Info);
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(AleOfTheAncestors)));
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(BloodMoonStatus)));
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(PitTrapStatus)));
        }

        private static string FindRepositoryFile(string relativePath)
        {
            for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
                 directory != null;
                 directory = directory.Parent)
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    relativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException($"Could not find repository file {relativePath}.");
        }
    }
}
