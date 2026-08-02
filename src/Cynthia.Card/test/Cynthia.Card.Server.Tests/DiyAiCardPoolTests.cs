using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            Assert.Equal(181, DiyAiCardPool.RetiredCardIds.Count);
            Assert.Equal(10, DiyAiCardPool.SystemCardIds.Count);
            Assert.Empty(DiyAiCardPool.RetiredCardIds.Intersect(DiyAiCardPool.SystemCardIds));
            Assert.DoesNotContain("70041", DiyAiCardPool.RetiredCardIds);
            Assert.DoesNotContain("70042", DiyAiCardPool.RetiredCardIds);

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
            Assert.Equal(new Version(1, 0, 0, 161), GwentMap.CardMapVersion);
            Assert.Equal(715, GwentMap.CardMap.Count);

            var historicalIds = string.Join(",", GwentMap.CardMap.Keys.Take(709));
            var historicalHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(historicalIds)))
                .ToLowerInvariant();

            Assert.Equal(
                "1d92e39fd29ffd178e2998d5c9bc761cebd37bf5c3adee040505840d9fab84a9",
                historicalHash);
            Assert.Equal(
                new[] { "34034", "34035", "34036", "64035", "64036", "64037" },
                GwentMap.CardMap.Keys.Skip(709));
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
        public void LocalizationUpdatePolicyCoversFreshAndStaleClients()
        {
            var current = new Version(1, 0, 0, 161);
            var stale = new Version(1, 0, 0, 160);

            Assert.True(LocalizationUpdatePolicy.ShouldDownloadLocales(false, current, current));
            Assert.True(LocalizationUpdatePolicy.ShouldDownloadLocales(false, stale, current));
            Assert.True(LocalizationUpdatePolicy.ShouldDownloadLocales(true, stale, current));
            Assert.True(LocalizationUpdatePolicy.ShouldDownloadLocales(true, null, current));
            Assert.False(LocalizationUpdatePolicy.ShouldDownloadLocales(true, current, current));
        }

        [Fact]
        public void UnityClientUsesPortableLocaleSynchronization()
        {
            var serviceSource = Regex.Replace(
                File.ReadAllText(FindRepositoryFile(
                    "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Code/GwentClientService.cs")),
                @"\s+",
                " ");
            var handlerSource = Regex.Replace(
                File.ReadAllText(FindRepositoryFile(
                    "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Script/ResourceManagement/TextLocalizationFileHandler.cs")),
                @"\s+",
                " ");

            Assert.Contains(
                "LocalizationUpdatePolicy.ShouldDownloadLocales( fileHandler.AreFilesDownloaded(), localesWereLastUpdatedTo, serverVersion)",
                serviceSource);
            Assert.Contains(
                "PlayerPrefs.GetString(\"LocalizationVersion\", \"0.0.0.0\")",
                serviceSource);
            Assert.DoesNotContain(
                "!fileHandler.AreFilesDownloaded() && clientVersion != serverVersion",
                serviceSource);
            Assert.Contains(
                "#else _directoryPath = Application.persistentDataPath; #endif",
                handlerSource);
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

        [Fact]
        public void AugustSecondBalancePatchMatchesPublishedRules()
        {
            Assert.Equal(5, GwentMap.CardMap[CardId.TrissTelekinesis].Strength);
            Assert.Equal(7, GwentMap.CardMap[CardId.Spotter].Strength);
            Assert.Equal(11, GwentMap.CardMap[CardId.DimunPirate].Strength);
            Assert.Equal(1, GwentMap.CardMap[CardId.DimunCorsair].Strength);
            Assert.Contains("基础战力一半（向下取整）", GwentMap.CardMap[CardId.Spotter].Info);

            var spotterSource = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Nilfgaard/Copper/Spotter.cs"));

            Assert.Contains("Status.Strength / 2", spotterSource);
        }

        [Fact]
        public void GenerateReworkMatchesPublishedRules()
        {
            Assert.Equal(5, GwentMap.CardMap["21003"].Strength);
            Assert.Equal(5, GwentMap.CardMap["42010"].Strength);
            Assert.Equal(1, GwentMap.CardMap["52013"].Strength);
            Assert.Equal(2, GwentMap.CardMap["62012"].Strength);
            Assert.Equal(3, GwentMap.CardMap["33016"].Strength);

            var expectedChineseInfo = new Dictionary<string, string>
            {
                ["12026"] = "生成任意方起始牌组中的1张铜色特殊牌。",
                ["12030"] = "生成1张己方起始牌组之外的铜色/银色“法术”牌。",
                ["12039"] = "根据场上最高战力单位的所在排及当前战力奇偶，生成1个对应奇偶战力的己方起始牌组之外的非领袖金色单位。己方攻城/远程/近战排对应中立/怪兽/尼弗迦德，对方近战/远程/攻城排对应北方领域/松鼠党/史凯利格。并列时按从上到下、同排从左到右取首个；场上没有单位时不生效。",
                ["13020"] = "生成1只“恶熊”、“翼手龙”、“须岩怪”或“水鬼”。",
                ["13023"] = "择一：生成1个己方起始牌组之外的铜色“食腐生物”或“吸血鬼”单位，并使其获得1点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。",
                ["13044"] = "生成对方起始牌组中的1张非间谍铜色/银色单位牌，并使其获得1点增益。",
                ["21003"] = "生成1张铜色/银色“有机”牌。",
                ["23020"] = "若落后，生成1个己方起始牌组之外的本阵营偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["31004"] = "间谍。生成对方阵营的1张非间谍领袖牌，并使其获得1点增益。",
                ["33016"] = "生成1个己方起始牌组之外的铜色尼弗迦德“士兵”单位。",
                ["33019"] = "若落后，生成1个己方起始牌组之外的本阵营偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["41002"] = "生成1个铜色北方领域“诅咒生物”单位。",
                ["42010"] = "择一：生成1张己方起始牌组之外的铜色“炼金”牌；或从牌组打出1张铜色/银色“道具”牌。",
                ["43019"] = "若落后，生成1个己方起始牌组之外的本阵营偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["51003"] = "生成1张己方起始牌组之外的银色中立“特殊”牌。",
                ["52013"] = "择一：从牌组打出1张铜色/银色“特殊”牌；或生成1个己方起始牌组之外的非间谍银色“精灵”单位。",
                ["53018"] = "若落后，生成1个己方起始牌组之外的本阵营偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["53021"] = "择一：生成1个己方起始牌组之外的铜色“矮人”单位；或使1个单位获得7点强化。",
                ["62012"] = "择一：从牌组打出1张铜色/银色“诅咒生物”牌；或生成对方初始牌组中1张非间谍银色单位牌。",
                ["63018"] = "若落后，生成1个己方起始牌组之外的本阵营偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["63020"] = "生成1个己方起始牌组之外的铜色史凯利格“士兵”单位，并使其获得2点强化。"
            };
            Assert.All(expectedChineseInfo, card =>
                Assert.Equal(card.Value, GwentMap.CardMap[card.Key].Info));

            var excluded = GwentMap.GetCards()
                .Where(card => card.Is(Group.Copper, CardType.Unit))
                .Take(2)
                .Select(card => card.CardId)
                .ToArray();
            var expectedCandidates = GwentMap.GetCards()
                .Where(card => card.Is(Group.Copper, CardType.Unit))
                .Where(card => !excluded.Contains(card.CardId))
                .Select(card => card.CardId)
                .ToArray();
            Assert.Equal(
                expectedCandidates,
                GwentMap.GetGenerateCardsId(
                    card => card.Is(Group.Copper, CardType.Unit),
                    excluded));
            Assert.True(expectedCandidates.Length > 3);

            var sourcePaths = new[]
            {
                "Monsters/Leader/WhisperingHillock.cs",
                "Neutral/Gold/AguaraTrueForm.cs",
                "Neutral/Gold/TrissTelekinesis.cs",
                "Neutral/Gold/UmaSCurese.cs",
                "Neutral/Silver/BlackBlood.cs",
                "Neutral/Silver/DorregarayOfVole.cs",
                "Neutral/Silver/Garrison.cs",
                "Nilfgaard/Leader/Usurper.cs",
                "Nilfgaard/Silver/Vreemde.cs",
                "NorthernRealms/Gold/Kiyan.cs",
                "NorthernRealms/Leader/PrincessAdda.cs",
                "ScoiaTael/Gold/IsengrimOutlaw.cs",
                "ScoiaTael/Leader/Filavandrel.cs",
                "ScoiaTael/Silver/MahakamHorn.cs",
                "Skellige/Gold/Hym.cs",
                "Skellige/Silver/OrnamentalSword.cs"
            };
            var cardEffectRoot = "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects";
            Assert.All(sourcePaths, sourcePath =>
            {
                var source = File.ReadAllText(FindRepositoryFile($"{cardEffectRoot}/{sourcePath}"));
                Assert.DoesNotContain("GetCreateCardsId", source);
                Assert.DoesNotContain(".Take(3)", source);
            });

            var umaSource = File.ReadAllText(FindRepositoryFile(
                $"{cardEffectRoot}/Neutral/Gold/UmaSCurese.cs"));
            Assert.True(
                umaSource.IndexOf("RowPosition.EnemyRow3", StringComparison.Ordinal) <
                umaSource.IndexOf("RowPosition.EnemyRow2", StringComparison.Ordinal));
            Assert.True(
                umaSource.IndexOf("RowPosition.EnemyRow2", StringComparison.Ordinal) <
                umaSource.IndexOf("RowPosition.EnemyRow1", StringComparison.Ordinal));
            Assert.True(
                umaSource.IndexOf("RowPosition.EnemyRow1", StringComparison.Ordinal) <
                umaSource.IndexOf("RowPosition.MyRow1", StringComparison.Ordinal));
            Assert.Contains("orderedCards.First", umaSource);
            Assert.Contains("if (orderedCards.Count == 0) return 0", umaSource);

            var runestoneSource = File.ReadAllText(FindRepositoryFile(
                $"{cardEffectRoot}/FactionRunestoneEffect.cs"));
            Assert.Contains("if (myPoint == enemyPoint) return 0", runestoneSource);
            Assert.Contains("myPoint < enemyPoint ? 0 : 1", runestoneSource);

            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
            {
                var locales = localeRoots.Select(localeRoot =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language}.json")))).ToArray();
                Assert.All(expectedChineseInfo.Keys, cardId =>
                {
                    Assert.All(locales.Skip(1), locale =>
                        Assert.Equal(locales[0].CardLocales[cardId].Info, locale.CardLocales[cardId].Info));
                    Assert.False(string.IsNullOrWhiteSpace(locales[0].CardLocales[cardId].Info));
                });
                if (language == "cn")
                {
                    Assert.All(expectedChineseInfo, card =>
                        Assert.Equal(card.Value, locales[0].CardLocales[card.Key].Info));
                }
            });
        }

        [Fact]
        public void TemporaryBalanceVariantsAreIndependentAndMatchTheirPublishedRules()
        {
            var viperIds = new[]
            {
                CardId.ViperWitcher,
                CardId.ViperWitcherA,
                CardId.ViperWitcherB,
                CardId.ViperWitcherC
            };
            var greatswordIds = new[]
            {
                CardId.AnCraiteGreatsword,
                CardId.AnCraiteGreatswordA,
                CardId.AnCraiteGreatswordB,
                CardId.AnCraiteGreatswordC
            };

            Assert.Equal(new[] { 5, 5, 5, 3 }, viperIds.Select(id => GwentMap.CardMap[id].Strength));
            Assert.Equal(new[] { 8, 8, 8, 7 }, greatswordIds.Select(id => GwentMap.CardMap[id].Strength));
            Assert.All(viperIds, id =>
            {
                Assert.Equal("20012400", GwentMap.CardMap[id].CardArtsId);
                Assert.True(DiyAiCardPool.IsUserDeckCard(id));
            });
            Assert.All(greatswordIds, id =>
            {
                Assert.Equal("20004000", GwentMap.CardMap[id].CardArtsId);
                Assert.True(DiyAiCardPool.IsUserDeckCard(id));
            });

            Assert.Contains("每有1张“炼金”牌", GwentMap.CardMap[CardId.ViperWitcher].Info);
            Assert.Contains("每有3张“炼金”牌", GwentMap.CardMap[CardId.ViperWitcherA].Info);
            Assert.Contains("造成3点伤害", GwentMap.CardMap[CardId.ViperWitcherB].Info);
            Assert.Contains("每有1张“炼金”牌", GwentMap.CardMap[CardId.ViperWitcherC].Info);
            Assert.Contains("每2回合", GwentMap.CardMap[CardId.AnCraiteGreatsword].Info);
            Assert.Contains("每3回合", GwentMap.CardMap[CardId.AnCraiteGreatswordA].Info);
            Assert.Contains("获得3点强化", GwentMap.CardMap[CardId.AnCraiteGreatswordB].Info);
            Assert.Contains("每2回合", GwentMap.CardMap[CardId.AnCraiteGreatswordC].Info);

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(ViperWitcher), dataService.GetType(CardId.ViperWitcher));
            Assert.Equal(typeof(ViperWitcherA), dataService.GetType(CardId.ViperWitcherA));
            Assert.Equal(typeof(ViperWitcherB), dataService.GetType(CardId.ViperWitcherB));
            Assert.Equal(typeof(ViperWitcherC), dataService.GetType(CardId.ViperWitcherC));
            Assert.Equal(typeof(AnCraiteGreatsword), dataService.GetType(CardId.AnCraiteGreatsword));
            Assert.Equal(typeof(AnCraiteGreatswordA), dataService.GetType(CardId.AnCraiteGreatswordA));
            Assert.Equal(typeof(AnCraiteGreatswordB), dataService.GetType(CardId.AnCraiteGreatswordB));
            Assert.Equal(typeof(AnCraiteGreatswordC), dataService.GetType(CardId.AnCraiteGreatswordC));

            var viperEffects = new ViperWitcherEffect[]
            {
                new ViperWitcher(new GameCard(null)),
                new ViperWitcherA(new GameCard(null)),
                new ViperWitcherB(new GameCard(null)),
                new ViperWitcherC(new GameCard(null))
            };
            var getDamage = typeof(ViperWitcherEffect).GetMethod(
                "GetDamage",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var skipZeroDamage = typeof(ViperWitcherEffect).GetProperty(
                "SkipTargetWhenNoDamage",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.Equal(new[] { 0, 0, 3, 0 }, viperEffects.Select(effect =>
                (int)getDamage.Invoke(effect, new object[] { 0 })));
            Assert.Equal(new[] { 3, 2, 5, 3 }, viperEffects.Select(effect =>
                (int)getDamage.Invoke(effect, new object[] { 3 })));
            Assert.Equal(new[] { 7, 4, 7, 7 }, viperEffects.Select(effect =>
                (int)getDamage.Invoke(effect, new object[] { 7 })));
            Assert.Equal(new[] { false, true, false, false }, viperEffects.Select(effect =>
                (bool)skipZeroDamage.GetValue(effect)));

            var greatswordEffects = new AnCraiteGreatswordEffect[]
            {
                new AnCraiteGreatsword(new GameCard(null)),
                new AnCraiteGreatswordA(new GameCard(null)),
                new AnCraiteGreatswordB(new GameCard(null)),
                new AnCraiteGreatswordC(new GameCard(null))
            };
            var turnCountdown = typeof(AnCraiteGreatswordEffect).GetProperty(
                "TurnCountdown",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var strengthenAmount = typeof(AnCraiteGreatswordEffect).GetProperty(
                "StrengthenAmount",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.Equal(new[] { 2, 3, 3, 2 }, greatswordEffects.Select(effect =>
                (int)turnCountdown.GetValue(effect)));
            Assert.Equal(new[] { 2, 2, 3, 2 }, greatswordEffects.Select(effect =>
                (int)strengthenAmount.GetValue(effect)));

            var expectedNamesByLanguage = new Dictionary<string, string[]>
            {
                ["cn"] = new[]
                {
                    "毒蛇学派猎魔人A", "毒蛇学派猎魔人B", "毒蛇学派猎魔人C",
                    "奎特家族巨剑士A", "奎特家族巨剑士B", "奎特家族巨剑士C"
                },
                ["en"] = new[]
                {
                    "Viper Witcher A", "Viper Witcher B", "Viper Witcher C",
                    "An Craite Greatsword A", "An Craite Greatsword B", "An Craite Greatsword C"
                },
                ["pl"] = new[]
                {
                    "Wiedźmin Szkoły Żmii A", "Wiedźmin Szkoły Żmii B", "Wiedźmin Szkoły Żmii C",
                    "Rębacz an Craite A", "Rębacz an Craite B", "Rębacz an Craite C"
                },
                ["ru"] = new[]
                {
                    "Ведьмак школы Змеи A", "Ведьмак школы Змеи B", "Ведьмак школы Змеи C",
                    "Ан Крайт: мечник A", "Ан Крайт: мечник B", "Ан Крайт: мечник C"
                }
            };
            var variantIds = viperIds.Skip(1).Concat(greatswordIds.Skip(1)).ToArray();
            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(localeRoots, localeRoot =>
            {
                Assert.All(expectedNamesByLanguage, expectedNames =>
                {
                    var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{expectedNames.Key}.json")));
                    Assert.Equal(
                        expectedNames.Value,
                        variantIds.Select(id => locale.CardLocales[id].Name));
                    Assert.All(variantIds, id => Assert.False(string.IsNullOrWhiteSpace(locale.CardLocales[id].Info)));
                    Assert.All(variantIds, id => Assert.False(string.IsNullOrWhiteSpace(locale.CardLocales[id].Flavor)));
                });
            });

            var familyIds = viperIds.Concat(greatswordIds).ToArray();
            Assert.All(expectedNamesByLanguage.Keys, language =>
            {
                var locales = localeRoots.Select(localeRoot =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language}.json")))).ToArray();
                Assert.All(familyIds, id =>
                {
                    Assert.All(locales.Skip(1), locale =>
                    {
                        Assert.Equal(locales[0].CardLocales[id].Name, locale.CardLocales[id].Name);
                        Assert.Equal(locales[0].CardLocales[id].Info, locale.CardLocales[id].Info);
                        Assert.Equal(locales[0].CardLocales[id].Flavor, locale.CardLocales[id].Flavor);
                    });
                });
            });

            var chineseLocale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.All(familyIds, id =>
            {
                Assert.Equal(GwentMap.CardMap[id].Name, chineseLocale.CardLocales[id].Name);
                Assert.Equal(GwentMap.CardMap[id].Info, chineseLocale.CardLocales[id].Info);
            });
        }

        [Fact]
        public void SelectedDiyAiPotionsUseTheOriginalAlchemyDesign()
        {
            Assert.All(new[] { "70041", "70042" }, cardId =>
            {
                var card = GwentMap.CardMap[cardId];
                Assert.True(DiyAiCardPool.IsUserDeckCard(cardId));
                Assert.False(card.IsDerive);
                Assert.Equal(Group.Copper, card.Group);
                Assert.Equal(CardType.Special, card.CardType);
                Assert.Contains(Categorie.Special, card.Categories);
                Assert.Contains(Categorie.Alchemy, card.Categories);
                Assert.DoesNotContain(Categorie.Item, card.Categories);
                Assert.Equal(2, card.Categories.Length);
            });

            var bidensSource = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY/Neutral/Copper/BidensBipinnata.cs"));
            var albizziaSource = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY/Neutral/Copper/AlbizziaJulibrissin.cs"));
            Assert.Contains("for (var i = 0; i < 4 + count; i++)", bidensSource);
            Assert.Contains("for (var i = 0; i < 4 + count; i++)", albizziaSource);
            Assert.DoesNotContain("Damage(3", bidensSource);
            Assert.DoesNotContain("Boost(3", albizziaSource);
            Assert.Contains("造成2点伤害，重复4次", GwentMap.CardMap["70041"].Info);
            Assert.Contains("获得2点增益，重复4次", GwentMap.CardMap["70042"].Info);

            var expectedInfoByLanguage = new Dictionary<string, string[]>
            {
                ["cn"] = new[]
                {
                    "对最强的敌军单位造成2点伤害，重复4次。己方墓场每有1张“合欢茎魔药”，则额外重复1次。",
                    "使最弱的友军单位获得2点增益，重复4次。己方墓场每有1张“鬼针草煎药”，则额外重复1次。"
                },
                ["en"] = new[]
                {
                    "Damage the highest enemy by 2, four times.\nFor each White Raffard's Decoction in your graveyard, repeat an additional time.",
                    "Boost the lowest ally by 2, four times.\nFor each Giga Scorpion Decoction in your graveyard, repeat an additional time."
                },
                ["pl"] = new[]
                {
                    "Zadaj najsilniejszemu wrogowi 2 pkt obrażeń cztery razy.\nZa każdą kartę „Odwar Raffarda Białego” na swoim cmentarzu powtórz dodatkowy raz.",
                    "Wzmocnij najsłabszego sojusznika o 2 pkt cztery razy.\nZa każdą kartę „Wyciąg z Gigaskorpiona” na swoim cmentarzu powtórz dodatkowy raz."
                },
                ["ru"] = new[]
                {
                    "Четыре раза нанесите 2 ед. урона самому сильному противнику.\nЗа каждую карту «Зелье Раффара Белого» на вашем кладбище повторите ещё один раз.",
                    "Четыре раза усильте самого слабого союзника на 2 ед.\nЗа каждую карту «Отвар из гигаскорпиона» на вашем кладбище повторите ещё один раз."
                }
            };
            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(localeRoots, localeRoot =>
            {
                Assert.All(expectedInfoByLanguage, expectedInfo =>
                {
                    var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{expectedInfo.Key}.json")));
                    Assert.Equal(expectedInfo.Value[0], locale.CardLocales["70041"].Info);
                    Assert.Equal(expectedInfo.Value[1], locale.CardLocales["70042"].Info);
                });
            });
        }

        [Fact]
        public void ActiveChoiceMenusUseLocalizedOptionKeys()
        {
            var expectedChoicesBySource = new Dictionary<string, string[]>
            {
                ["Monsters/Gold/WeavessIncantation.cs"] = new[]
                {
                    "WeavessIncantation_1_Strenghten", "WeavessIncantation_2_PlayRelict"
                },
                ["Neutral/Copper/Mardroeme.cs"] = new[]
                {
                    "Mardroeme_1_Strenghten", "Mardroeme_2_Weaken"
                },
                ["Neutral/Derive/ShupeKnight.cs"] = new[]
                {
                    "ShupeKnight_1_Strengthen", "ShupeKnight_2_Resilience",
                    "ShupeKnight_3_Duel", "ShupeKnight_4_Reset", "ShupeKnight_5_Destroy"
                },
                ["Neutral/Derive/ShupeHunter.cs"] = new[]
                {
                    "ShupeHunter_1_SingleDamage", "ShupeHunter_2_RepeatedDamage",
                    "ShupeHunter_3_Replay", "ShupeHunter_4_PlayFromDeck",
                    "ShupeHunter_5_ClearSky"
                },
                ["Neutral/Derive/ShupeMage.cs"] = new[]
                {
                    "ShupeMage_1_DrawCard", "ShupeMage_2_Charm", "ShupeMage_3_Hazard",
                    "ShupeMage_4_Damage", "ShupeMage_5_Special"
                },
                ["Neutral/Gold/Sihil.cs"] = new[]
                {
                    "Sihil_1_DamageOdd", "Sihil_2_DamageEven", "Sihil_3_PlayUnit"
                },
                ["Neutral/Silver/BlackBlood.cs"] = new[]
                {
                    "BlackBlood_1_CreateVampire", "BlackBlood_2_DestroyVampire"
                },
                ["Neutral/Silver/Mandrake.cs"] = new[]
                {
                    "Mandrake_1_Strenghten", "Mandrake_2_Weaken"
                },
                ["Nilfgaard/Gold/LethoKingslayer.cs"] = new[]
                {
                    "LethoKingslayer_1_Destroy", "LethoKingslayer_2_PlayTactic"
                },
                ["Nilfgaard/Silver/Cadaverine.cs"] = new[]
                {
                    "Cadaverine_1_DamegeCategory", "Cadaverine_2_DestroyNeutral"
                },
                ["NorthernRealms/Gold/Kiyan.cs"] = new[]
                {
                    "Kiyan_1_CreateAlchemy", "Kiyan_2_PlayItem"
                },
                ["NorthernRealms/Silver/VandergriftSBlade.cs"] = new[]
                {
                    "VandergriftSBlade_1_DestroyCursed", "VandergriftSBlade_2_Damage"
                },
                ["ScoiaTael/Gold/IsengrimOutlaw.cs"] = new[]
                {
                    "IsengrimOutlaw_1_PlaySpecial", "IsengrimOutlaw_2_CreateElf"
                },
                ["ScoiaTael/Silver/MahakamHorn.cs"] = new[]
                {
                    "MahakamHorn_1_CreateDwarf", "MahakamHorn_2_Strenghten"
                },
                ["Skellige/Copper/BoneTalisman.cs"] = new[]
                {
                    "BoneTalisman_1_ResurectBeast", "BoneTalisman_2_Strenghten"
                },
                ["Skellige/Gold/Hym.cs"] = new[]
                {
                    "Hym_1_PlayCursed", "Hym_2_PlaySilver"
                },
                ["Neutral/Gold/GaunterODimm.cs"] = new[]
                {
                    "GaunterODimm_1_LowerThanSix", "GaunterODimm_2_EqualToSix",
                    "GaunterODimm_3_HigherThanSix"
                },
                ["Neutral/Gold/Aguara.cs"] = new[]
                {
                    "Aguara_1_BoostLowest", "Aguara_2_DamageHighest",
                    "Aguara_3_BoostHand", "Aguara_4_CharmElf"
                }
            };
            var cardEffectRoot =
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects";
            Assert.All(expectedChoicesBySource, choiceSource =>
            {
                var source = File.ReadAllText(FindRepositoryFile(
                    $"{cardEffectRoot}/{choiceSource.Key}"));
                Assert.All(choiceSource.Value, key => Assert.Contains($"\"{key}\"", source));
            });

            var expectedKeys = expectedChoicesBySource
                .SelectMany(choiceSource => choiceSource.Value)
                .ToArray();
            Assert.Equal(49, expectedKeys.Length);
            Assert.Equal(49, expectedKeys.Distinct(StringComparer.Ordinal).Count());
            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            var menuTextsByLanguage = new Dictionary<string, Dictionary<string, string>>(
                StringComparer.Ordinal);
            Assert.All(localeRoots, localeRoot =>
            {
                Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
                {
                    var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language}.json")));
                    Assert.All(expectedKeys, key =>
                    {
                        Assert.True(locale.MenuLocales.TryGetValue(key, out var text));
                        Assert.False(string.IsNullOrWhiteSpace(text));
                    });
                    Assert.All(expectedChoicesBySource.Values, optionKeys =>
                    {
                        var optionTexts = optionKeys
                            .Select(key => locale.MenuLocales[key])
                            .ToArray();
                        Assert.Equal(
                            optionTexts.Length,
                            optionTexts.Distinct(StringComparer.Ordinal).Count());
                    });
                    Assert.Contains("9", locale.MenuLocales["VandergriftSBlade_2_Damage"]);
                    Assert.DoesNotContain("10", locale.MenuLocales["VandergriftSBlade_2_Damage"]);
                    Assert.Contains("2", locale.MenuLocales["Cadaverine_1_DamegeCategory"]);
                    Assert.DoesNotContain("3", locale.MenuLocales["Cadaverine_1_DamegeCategory"]);

                    var currentTexts = expectedKeys.ToDictionary(
                        key => key,
                        key => locale.MenuLocales[key],
                        StringComparer.Ordinal);
                    if (menuTextsByLanguage.TryGetValue(language, out var canonicalTexts))
                    {
                        Assert.Equal(canonicalTexts, currentTexts);
                    }
                    else
                    {
                        menuTextsByLanguage[language] = currentTexts;
                    }
                });
            });

            var chineseLocale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.Contains("除自身外", chineseLocale.MenuLocales[
                "WeavessIncantation_1_Strenghten"]);
            Assert.Contains("食腐生物", chineseLocale.MenuLocales[
                "BlackBlood_1_CreateVampire"]);
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
