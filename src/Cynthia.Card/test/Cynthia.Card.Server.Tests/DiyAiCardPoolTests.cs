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
            Assert.Equal(84, DiyAiCardPool.RetiredCardIds.Count);
            Assert.Equal(10, DiyAiCardPool.SystemCardIds.Count);
            Assert.Empty(DiyAiCardPool.RetiredCardIds.Intersect(DiyAiCardPool.SystemCardIds));
            Assert.DoesNotContain("70041", DiyAiCardPool.RetiredCardIds);
            Assert.DoesNotContain("70042", DiyAiCardPool.RetiredCardIds);
            Assert.DoesNotContain(CardId.Quen, DiyAiCardPool.RetiredCardIds);
            Assert.True(DiyAiCardPool.IsUserDeckCard(CardId.Quen));

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
            Assert.Equal(new Version(1, 0, 0, 176), GwentMap.CardMapVersion);
            Assert.Equal(718, GwentMap.CardMap.Count);

            var historicalIds = string.Join(",", GwentMap.CardMap.Keys.Take(709));
            var historicalHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(historicalIds)))
                .ToLowerInvariant();

            Assert.Equal(
                "1d92e39fd29ffd178e2998d5c9bc761cebd37bf5c3adee040505840d9fab84a9",
                historicalHash);
            Assert.Equal(
                new[] { "34034", "34035", "34036", "64035", "64036", "64037", "70191", "70192", "70193" },
                GwentMap.CardMap.Keys.Skip(709));
        }

        [Fact]
        public void OriginalStarterDeckIsPlayableAndRetiredCardsAreRejected()
        {
            var starter = GwentDeck.CreateBasicDeck(0);
            Assert.True(starter.IsBasicDeck());
            Assert.All(starter.Deck, cardId => Assert.True(DiyAiCardPool.IsUserDeckCard(cardId)));

            starter.Deck[0] = "70003";
            Assert.False(starter.IsBasicDeck());
            Assert.False(starter.IsSpecialDeck());
        }

        [Fact]
        public void LeaderOnlyDeckIsAValidDraftButNotAPlayableDeck()
        {
            var draft = new DeckModel
            {
                Leader = CardId.QueenCalanthe,
                Deck = new List<string>(),
                Name = "未完成卡组",
                Id = Guid.NewGuid().ToString()
            };

            Assert.True(draft.IsHalfBasicDeck());
            Assert.True(draft.IsHalfSpecialDeck());
            Assert.False(draft.IsBasicDeck());
            Assert.False(draft.IsSpecialDeck());
        }

        [Fact]
        public void AugustThirdCardBatchIsAvailableAndMatchesPublishedValues()
        {
            var deckableDiyCards = new[]
            {
                "70002", "70005", "70011", "70026", "70027", "70059",
                "70062", "70070", "70091", "70110", "70119", "70131",
                "70133", "70155", "70157", "70161", "70172", "70190"
            };
            Assert.All(deckableDiyCards, id => Assert.True(DiyAiCardPool.IsUserDeckCard(id)));

            var generatedCards = new[] { "70006", "70071", "70162" };
            Assert.All(generatedCards, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(GwentMap.CardMap[id].IsDerive);
                Assert.False(DiyAiCardPool.IsUserDeckCard(id));
            });

            Assert.Equal(2, GwentMap.CardMap["21003"].Strength);
            Assert.Equal(22, GwentMap.CardMap["70006"].Strength);
            Assert.Equal(5, GwentMap.CardMap["70002"].Strength);
            Assert.Equal(5, GwentMap.CardMap["22001"].Strength);
            Assert.Equal(3, GwentMap.CardMap["70110"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70161"].Strength);
            Assert.Equal(9, GwentMap.CardMap["70131"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70155"].Strength);
            Assert.Equal(2, GwentMap.CardMap["70172"].Strength);
            Assert.Contains(Categorie.Organic, GwentMap.CardMap["70157"].Categories);
            Assert.DoesNotContain(Categorie.Alchemy, GwentMap.CardMap["70157"].Categories);
            Assert.Equal("克鲁姆国王", GwentMap.CardMap["70190"].Name);

            Assert.Equal("生成1张银色“有机”牌。", GwentMap.CardMap["21003"].Info);
            Assert.Contains("随机非间谍单位", GwentMap.CardMap["12027"].Info);
            Assert.Contains("伤害减半", GwentMap.CardMap["70062"].Info);
            Assert.DoesNotContain("向上取整", GwentMap.CardMap["70062"].Info);
            Assert.Contains("重复3次", GwentMap.CardMap["70119"].Info);
            Assert.Contains("额外获得3点增益", GwentMap.CardMap["70133"].Info);
            Assert.Contains("每个回合开始时", GwentMap.CardMap["70172"].Info);

            var aguaraSource = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Neutral/Gold/Aguara.cs"));
            Assert.Contains("Aguara_1_BoostLowest", aguaraSource);
            Assert.Contains("Aguara_3_BoostHand", aguaraSource);
            Assert.Contains("CardUseInfo.MyRow", aguaraSource);
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
            var current = new Version(1, 0, 0, 168);
            var stale = new Version(1, 0, 0, 167);

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
        public void QuenAvailabilityAndDescriptionMatchTheImmediateShieldRule()
        {
            const string expectedInfo =
                "选择手牌中的1个铜色/银色单位，使其及手牌、牌组中的同名牌获得2点增益和护盾。护盾可阻挡1次伤害；已有护盾的单位不能被选中。";
            Assert.Equal(expectedInfo, GwentMap.CardMap[CardId.Quen].Info);
            Assert.True(DiyAiCardPool.IsUserDeckCard(CardId.Quen));
            Assert.False(GwentMap.CardMap[CardId.Quen].IsDerive);

            var expectedInfoByLanguage = new Dictionary<string, string>
            {
                ["cn"] = expectedInfo,
                ["en"] = "Choose a Bronze or Silver unit in your hand. Boost it and all copies of it in your hand and deck by 2, then give them Shield. Shield blocks one instance of damage; units that already have Shield cannot be chosen.",
                ["pl"] = "Wybierz brązową lub srebrną jednostkę w swojej ręce. Wzmocnij ją oraz wszystkie jej kopie w ręce i talii o 2 i daj im Tarczę. Tarcza blokuje jedno źródło obrażeń; nie można wybrać jednostki, która już ma Tarczę.",
                ["ru"] = "Выберите бронзовый или серебряный отряд в руке. Усильте его и все его копии в руке и колоде на 2 и дайте им щит. Щит блокирует один случай урона; нельзя выбрать отряд, у которого уже есть щит."
            };
            var quenLocaleRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(quenLocaleRoots, localeRoot =>
            {
                Assert.All(expectedInfoByLanguage, language =>
                {
                    var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language.Key}.json")));
                    Assert.Equal(language.Value, locale.CardLocales[CardId.Quen].Info);
                });
            });

        }

        [Fact]
        public void SaesenthessisBlazeDescriptionMatchesItsResurrectionRefillRule()
        {
            const string expectedChineseInfo =
                "放逐所有手牌，抽同等数量的牌。如果抽牌过程中牌组为空，则将墓场中的所有非领袖和非间谍单位放回牌组后继续抽牌，该效果视为复活。";
            Assert.Equal(
                expectedChineseInfo,
                GwentMap.CardMap[CardId.SaesenthessisBlaze].Info);

            var expectedInfoByLanguage = new Dictionary<string, string>
            {
                ["cn"] = expectedChineseInfo,
                ["en"] = "Deploy: Banish your hand, then draw that many cards. If your deck becomes empty while drawing, shuffle all non-Leader, non-Spying units from your graveyard into your deck and continue drawing. This counts as Resurrecting them.",
                ["pl"] = "Rozmieszczenie: Wygnaj wszystkie karty ze swojej ręki i dobierz tyle samo kart. Jeśli podczas dobierania twoja talia będzie pusta, wtasuj do niej wszystkie jednostki niebędące Dowódcami ani Szpiegami ze swojego cmentarza i kontynuuj dobieranie. Jest to traktowane jako Wskrzeszenie.",
                ["ru"] = "Размещение: изгоните все карты из руки и возьмите столько же карт. Если во время добора колода опустеет, замешайте в неё все отряды, не являющиеся лидерами или шпионами, со своего кладбища и продолжите добор. Это считается воскрешением."
            };
            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(localeRoots, localeRoot =>
            {
                Assert.All(expectedInfoByLanguage, language =>
                {
                    var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language.Key}.json")));
                    Assert.Equal(
                        language.Value,
                        locale.CardLocales[CardId.SaesenthessisBlaze].Info);
                });
            });

            var source = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Neutral/Gold/SaesenthessisBlaze.cs"));
            Assert.Contains("unit.Effect.Resurrect", source);
            Assert.Contains("await Game.PlayerDrawCard(PlayerIndex)", source);
        }

        [Fact]
        public void AugustSecondBalancePatchMatchesPublishedRules()
        {
            Assert.Equal(5, GwentMap.CardMap[CardId.TrissTelekinesis].Strength);
            Assert.Equal(7, GwentMap.CardMap[CardId.Spotter].Strength);
            Assert.Equal(11, GwentMap.CardMap[CardId.DimunPirate].Strength);
            Assert.Equal(1, GwentMap.CardMap[CardId.DimunCorsair].Strength);
            Assert.Contains("基础战力一半", GwentMap.CardMap[CardId.Spotter].Info);
            Assert.DoesNotContain("向下取整", GwentMap.CardMap[CardId.Spotter].Info);

            var spotterSource = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Nilfgaard/Copper/Spotter.cs"));

            Assert.Contains("(result.Single().Status.Strength + 1) / 2", spotterSource);
        }

        [Fact]
        public void GenerateReworkMatchesPublishedRules()
        {
            Assert.Equal(2, GwentMap.CardMap["21003"].Strength);
            Assert.Equal(5, GwentMap.CardMap["42010"].Strength);
            Assert.Equal(1, GwentMap.CardMap["52013"].Strength);
            Assert.Equal(2, GwentMap.CardMap["62012"].Strength);
            Assert.Equal(3, GwentMap.CardMap["33016"].Strength);

            var expectedChineseInfo = new Dictionary<string, string>
            {
                ["12026"] = "生成任意方起始牌组中的1张铜色特殊牌。",
                ["12030"] = "生成1张己方起始牌组之外的铜色/银色“法术”牌。",
                ["12039"] = "根据场上最高战力单位的所在排及当前战力奇偶，生成1个对应奇偶战力的己方起始牌组之外的非领袖金色单位。己方攻城/远程/近战排对应中立/怪兽/尼弗迦德，对方近战/远程/攻城排对应北方领域/松鼠党/史凯利格。",
                ["13020"] = "生成1只“恶熊”、“翼手龙”、“须岩怪”或“水鬼”。",
                ["13023"] = "择一：生成1个己方起始牌组之外的铜色“食腐生物”或“吸血鬼”单位，并使其获得1点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。",
                ["13044"] = "生成对方起始牌组中的1张非间谍铜色/银色“士兵”或“军官”牌，并使其获得1点增益。",
                ["21003"] = "生成1张银色“有机”牌。",
                ["23020"] = "若落后，生成1个己方起始牌组之外的怪兽偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["31004"] = "间谍。生成对方阵营的1张非间谍领袖牌，并使其获得1点增益。",
                ["33016"] = "生成1个己方起始牌组之外的铜色尼弗迦德“士兵”单位。",
                ["33019"] = "若落后，生成1个己方起始牌组之外的尼弗迦德偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["41002"] = "生成1个铜色北方领域“诅咒生物”单位。",
                ["42010"] = "择一：生成1张己方起始牌组之外的铜色“炼金”牌；或从牌组打出1张铜色/银色“道具”牌。",
                ["43019"] = "若落后，生成1个己方起始牌组之外的北方领域偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["51003"] = "生成1张己方起始牌组之外的银色中立“特殊”牌。",
                ["52013"] = "择一：从牌组打出1张铜色/银色“特殊”牌；或生成1个己方起始牌组之外的非间谍银色“精灵”单位。",
                ["53018"] = "若落后，生成1个己方起始牌组之外的松鼠党偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
                ["53021"] = "择一：生成1个己方起始牌组之外的铜色“矮人”单位；或使1个单位获得7点强化。",
                ["62012"] = "择一：从牌组打出1张铜色/银色“诅咒生物”牌；或生成对方初始牌组中1张非间谍银色单位牌。",
                ["63018"] = "若落后，生成1个己方起始牌组之外的史凯利格偶数战力铜色单位；若领先，改为奇数战力；平局不生效。",
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
        public void IceTrollUsesSelfDamageInsteadOfAGlobalDuelShieldException()
        {
            Assert.Equal(5, GwentMap.CardMap[CardId.IceTroll].Strength);
            Assert.Equal(
                "对自身造成1点伤害，随后与1个敌军单位对决。若它位于“刺骨冰霜”之下，则己方对决伤害翻倍。",
                GwentMap.CardMap[CardId.IceTroll].Info);
        }

        [Fact]
        public void TemporaryBalanceVariantsAreRetiredWithoutReusingTheirHistoricalSlots()
        {
            var originalIds = new[]
            {
                CardId.ViperWitcher,
                CardId.AnCraiteGreatsword
            };
            var retiredVariantIds = new[]
            {
                CardId.ViperWitcherA,
                CardId.ViperWitcherB,
                CardId.ViperWitcherC,
                CardId.AnCraiteGreatswordA,
                CardId.AnCraiteGreatswordB,
                CardId.AnCraiteGreatswordC
            };

            Assert.All(originalIds, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(DiyAiCardPool.IsUserDeckCard(id));
                Assert.False(GwentMap.CardMap[id].IsDerive);
            });
            Assert.All(retiredVariantIds, id =>
            {
                Assert.Contains(id, DiyAiCardPool.RetiredCardIds);
                Assert.False(DiyAiCardPool.IsUserDeckCard(id));
                Assert.True(GwentMap.CardMap[id].IsDerive);
            });

            Assert.Contains("每有1张“炼金”牌", GwentMap.CardMap[CardId.ViperWitcher].Info);
            Assert.Contains("每2回合", GwentMap.CardMap[CardId.AnCraiteGreatsword].Info);

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(ViperWitcher), dataService.GetType(CardId.ViperWitcher));
            Assert.Equal(typeof(AnCraiteGreatsword), dataService.GetType(CardId.AnCraiteGreatsword));
        }

        [Fact]
        public void AugustFourthLeaderBatchIsDeckableAndMatchesPublishedValues()
        {
            var leaderIds = new[]
            {
                CardId.Meve,
                CardId.AnnaHenrietta,
                CardId.QueenCalanthe,
                CardId.DanaMeadbh
            };
            Assert.All(leaderIds, id => Assert.True(DiyAiCardPool.IsUserDeckCard(id)));
            Assert.Equal(new[] { 8, 6, 7, 3 }, leaderIds.Select(id => GwentMap.CardMap[id].Strength));
            Assert.Equal(Faction.ScoiaTael, GwentMap.CardMap[CardId.DanaMeadbh].Faction);
            Assert.Equal("203195", GwentMap.CardMap[CardId.DanaMeadbh].CardArtsId);
            Assert.Equal("从牌组打出1张中立牌。", GwentMap.CardMap[CardId.DanaMeadbh].Info);
            Assert.Equal(
                "获得1个友军铜色/银色非间谍单位的所有增益和护甲，随后将其收回牌组。然后从牌组打出1张铜色/银色单位牌。操控。",
                GwentMap.CardMap[CardId.QueenCalanthe].Info);

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(Mave), dataService.GetType(CardId.Meve));
            Assert.Equal(typeof(AnnaHenrietta), dataService.GetType(CardId.AnnaHenrietta));
            Assert.Equal(typeof(QueenCalanthe), dataService.GetType(CardId.QueenCalanthe));
            Assert.Equal(typeof(DanaMeadbh), dataService.GetType(CardId.DanaMeadbh));

            Assert.True(File.Exists(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Addressables/Cards/203195.png")));
            Assert.True(File.Exists(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Addressables/Miniatures/203195_slot.png")));
            var cardAddressableGroup = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/AddressableAssetsData/AssetGroups/Default Local Group.asset"));
            var miniatureAddressableGroup = File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/AddressableAssetsData/AssetGroups/Miniatures.asset"));
            Assert.Contains("m_Address: 203195", cardAddressableGroup);
            Assert.Contains("m_Address: 203195_slot", miniatureAddressableGroup);
        }

        [Fact]
        public void AugustFourthSecondBatchIsAvailableAndMatchesPublishedValues()
        {
            var deckableIds = new[]
            {
                "70007", "70008", "70025", "70032", "70072",
                "70125", "70128", "70156", "70158", "70180"
            };
            Assert.All(deckableIds, id => Assert.True(DiyAiCardPool.IsUserDeckCard(id)));

            var wingIds = new[] { "70181", "70182" };
            Assert.All(wingIds, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(GwentMap.CardMap[id].IsDerive);
                Assert.False(DiyAiCardPool.IsUserDeckCard(id));
            });

            Assert.Equal(11, GwentMap.CardMap["70025"].Strength);
            Assert.Equal(3, GwentMap.CardMap["70072"].Strength);
            Assert.Equal(4, GwentMap.CardMap["70125"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70158"].Strength);
            Assert.Contains("非间谍", GwentMap.CardMap["70027"].Info);
            Assert.Equal(
                "每2回合结束时，造成等同于受伤量的伤害。",
                GwentMap.CardMap["70025"].Info);
            Assert.Contains("其他最弱的友军猎魔人", GwentMap.CardMap["70158"].Info);
            Assert.Equal(
                "免疫，生成左翼和右翼。在对方同排降下“刺骨冰霜”，每2回合开始时，重复此能力。若己方没有左右翼，摧毁自身。",
                GwentMap.CardMap["70180"].Info);

            var chineseLocale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.All(deckableIds, id =>
                Assert.Equal(chineseLocale.CardLocales[id].Info, GwentMap.CardMap[id].Info));

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(Gascon), dataService.GetType("70032"));
            Assert.Equal(typeof(Albastra), dataService.GetType("70180"));
            Assert.Equal(typeof(AlbastraRightWing), dataService.GetType("70181"));
            Assert.Equal(typeof(AlbastraLeftWing), dataService.GetType("70182"));
            Assert.Equal(typeof(Syanna), dataService.GetType("70025"));
            Assert.Equal(typeof(CoënofPoviss), dataService.GetType("70158"));

            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            var changedInfoIds = new[] { "70025", "70027", "70158", "70180" };
            Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
            {
                var locales = localeRoots.Select(localeRoot =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{localeRoot}/{language}.json")))).ToArray();
                Assert.All(changedInfoIds, cardId =>
                    Assert.All(locales.Skip(1), locale =>
                        Assert.Equal(locales[0].CardLocales[cardId].Info, locale.CardLocales[cardId].Info)));
            });
        }

        [Fact]
        public void AugustFifthMonsterBatchIsAvailableAndMatchesPublishedValues()
        {
            var restoredDeckableIds = new[]
            {
                CardId.OlgierdImmortal,
                CardId.DetlaffCrimsonCurse,
                CardId.Keltullis,
                CardId.Orianna,
                CardId.IrisShade,
                CardId.Tatterwing,
                CardId.CloudGiant,
                CardId.SirScratchALot
            };
            Assert.All(restoredDeckableIds, id => Assert.True(DiyAiCardPool.IsUserDeckCard(id)));

            Assert.True(DiyAiCardPool.IsUserDeckCard(CardId.OldSpeartipAsleep));
            Assert.False(DiyAiCardPool.IsUserDeckCard(CardId.OldSpeartip));
            Assert.True(GwentMap.CardMap[CardId.OldSpeartip].IsDerive);
            Assert.Equal("老矛头：觉醒", GwentMap.CardMap[CardId.OldSpeartip].Name);

            Assert.Equal(6, GwentMap.CardMap[CardId.GeraltAard].Strength);
            Assert.Contains("攻城排", GwentMap.CardMap[CardId.GeraltAard].Info);
            Assert.Contains("摧毁并放逐", GwentMap.CardMap[CardId.GeraltProfessional].Info);
            Assert.Equal(9, GwentMap.CardMap[CardId.OlgierdVonEverec].Strength);
            Assert.Equal(7, GwentMap.CardMap[CardId.IrisShade].Strength);
            Assert.Contains("添加2张", GwentMap.CardMap[CardId.IrisShade].Info);
            Assert.Equal(7, GwentMap.CardMap[CardId.Orianna].Strength);
            Assert.Equal(8, GwentMap.CardMap[CardId.CloudGiant].Strength);
            Assert.Equal(9, GwentMap.CardMap[CardId.Keltullis].Strength);
            Assert.DoesNotContain("铜色/银色", GwentMap.CardMap[CardId.DetlaffCrimsonCurse].Info);
            Assert.Contains("所有敌军单位", GwentMap.CardMap[CardId.Tatterwing].Info);
            Assert.Contains("有友军野兽单位被打出时", GwentMap.CardMap[CardId.SirScratchALot].Info);

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(GeraltAard), dataService.GetType(CardId.GeraltAard));
            Assert.Equal(typeof(GeraltProfessional), dataService.GetType(CardId.GeraltProfessional));
            Assert.Equal(typeof(OlgierdVonEverec), dataService.GetType(CardId.OlgierdVonEverec));
            Assert.Equal(typeof(OldSpeartipAsleep), dataService.GetType(CardId.OldSpeartipAsleep));
            Assert.Equal(typeof(OldSpeartip), dataService.GetType(CardId.OldSpeartip));
            Assert.Equal(typeof(Imlerith), dataService.GetType(CardId.Imlerith));
            Assert.Equal(typeof(Keltullis), dataService.GetType(CardId.Keltullis));
            Assert.Equal(typeof(SirScratchALot), dataService.GetType(CardId.SirScratchALot));
            Assert.Equal(typeof(CloudGiant), dataService.GetType(CardId.CloudGiant));

            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            var changedChineseIds = new[]
            {
                CardId.GeraltProfessional, CardId.GeraltAard, CardId.OldSpeartip,
                CardId.DetlaffCrimsonCurse, CardId.Keltullis, CardId.IrisShade,
                CardId.CloudGiant, CardId.SirScratchALot, "70180"
            };
            var chineseLocales = localeRoots.Select(localeRoot =>
                JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                    FindRepositoryFile($"{localeRoot}/cn.json")))).ToArray();
            Assert.All(changedChineseIds, cardId =>
            {
                Assert.Equal(GwentMap.CardMap[cardId].Name, chineseLocales[0].CardLocales[cardId].Name);
                Assert.Equal(GwentMap.CardMap[cardId].Info, chineseLocales[0].CardLocales[cardId].Info);
                Assert.All(chineseLocales.Skip(1), locale =>
                {
                    Assert.Equal(chineseLocales[0].CardLocales[cardId].Name, locale.CardLocales[cardId].Name);
                    Assert.Equal(chineseLocales[0].CardLocales[cardId].Info, locale.CardLocales[cardId].Info);
                });
            });
        }

        [Fact]
        public void AugustSixthMonsterBatchIsAvailableAndMatchesPublishedValues()
        {
            var deckableIds = new[]
            {
                "70009", "70010", "70022", "70023", "70058", "70083", "70085", "70088",
                "70106", "70124", "70129", "70132", "70146", "70148", "70168", "70169",
                "70176", "70183", "70185", "70192"
            };
            Assert.All(deckableIds, id => Assert.True(DiyAiCardPool.IsUserDeckCard(id)));

            var generatedIds = new[] { "70107", "70108", "70147", "70186", "70187" };
            Assert.All(generatedIds, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(GwentMap.CardMap[id].IsDerive);
                Assert.False(DiyAiCardPool.IsUserDeckCard(id));
            });

            Assert.Equal("70108", GwentMap.CardMap["70108"].CardId);
            Assert.Equal(6, GwentMap.CardMap["70083"].Strength);
            Assert.Equal(Group.Silver, GwentMap.CardMap["70083"].Group);
            Assert.Equal(4, GwentMap.CardMap["70009"].Strength);
            Assert.Equal(6, GwentMap.CardMap["70010"].Strength);
            Assert.Equal(4, GwentMap.CardMap["70107"].Strength);
            Assert.Equal(5, GwentMap.CardMap["70108"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70168"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70169"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70170"].Strength);
            Assert.Equal(7, GwentMap.CardMap["70185"].Strength);
            Assert.Equal("沼泽鬼火", GwentMap.CardMap["70088"].Name);
            Assert.Contains("3个位于“刺骨冰霜”", GwentMap.CardMap["70083"].Info);
            Assert.Contains("造成6点伤害", GwentMap.CardMap["70085"].Info);
            Assert.Contains("黄金酒沫", GwentMap.CardMap["70146"].Info);
            Assert.Contains("3回合后的回合开始时", GwentMap.CardMap["70168"].Info);
            Assert.Contains("改为获得强化", GwentMap.CardMap["70169"].Info);
            Assert.Contains("自身基础战力一半", GwentMap.CardMap["70170"].Info);
            Assert.Contains("触发1个友军铜色单位的遗愿", GwentMap.CardMap["70176"].Info);
            Assert.All(GwentMap.CardMap.Values, card => Assert.DoesNotContain("说明", card.Info));

            var ignis = GwentMap.CardMap[CardId.IgnisFatuus];
            Assert.Equal("伊格尼斯·法图斯", ignis.Name);
            Assert.Equal(6, ignis.Strength);
            Assert.Equal(Group.Gold, ignis.Group);
            Assert.Equal(Faction.Monsters, ignis.Faction);
            Assert.Contains(Categorie.Relict, ignis.Categories);
            Assert.Equal("202680", ignis.CardArtsId);
            Assert.Contains("伤害改为削弱", ignis.Info);
            Assert.Equal(typeof(IgnisFatuus), new GwentCardDataService().GetType(CardId.IgnisFatuus));

            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            var changedIds = new[]
            {
                "24021", "70083", "70085", "70088", "70106", "70113", "70133", "70146",
                "70147", "70158", "70168", "70169", "70170", "70176", "70192"
            };
            Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
            {
                var locales = localeRoots.Select(root =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{root}/{language}.json")))).ToArray();
                Assert.All(changedIds, id =>
                    Assert.All(locales.Skip(1), locale =>
                    {
                        Assert.Equal(locales[0].CardLocales[id].Name, locale.CardLocales[id].Name);
                        Assert.Equal(locales[0].CardLocales[id].Info, locale.CardLocales[id].Info);
                    }));
            });

            var chinese = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.All(changedIds, id =>
            {
                Assert.Equal(GwentMap.CardMap[id].Name, chinese.CardLocales[id].Name);
                Assert.Equal(GwentMap.CardMap[id].Info, chinese.CardLocales[id].Info);
            });

            Assert.True(File.Exists(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Addressables/Cards/202680.png")));
            Assert.True(File.Exists(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Addressables/Miniatures/202680_slot.png")));
            Assert.Contains("m_Address: 202680", File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/AddressableAssetsData/AssetGroups/Default Local Group.asset")));
            Assert.Contains("m_Address: 202680_slot", File.ReadAllText(FindRepositoryFile(
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/AddressableAssetsData/AssetGroups/Miniatures.asset")));
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
            Assert.Contains("造成2点伤害，随后重复3次", GwentMap.CardMap["70041"].Info);
            Assert.Contains("获得2点增益，随后重复3次", GwentMap.CardMap["70042"].Info);

            var expectedInfoByLanguage = new Dictionary<string, string[]>
            {
                ["cn"] = new[]
                {
                    "对最强的敌军单位造成2点伤害，随后重复3次。己方墓场每有1张“合欢茎魔药”，则额外重复1次。",
                    "使最弱的友军单位获得2点增益，随后重复3次。己方墓场每有1张“鬼针草煎药”，则额外重复1次。"
                },
                ["en"] = new[]
                {
                    "Damage the highest enemy by 2, then repeat 3 times.\nFor each White Raffard's Decoction in your graveyard, repeat an additional time.",
                    "Boost the lowest ally by 2, then repeat 3 times.\nFor each Giga Scorpion Decoction in your graveyard, repeat an additional time."
                },
                ["pl"] = new[]
                {
                    "Zadaj najsilniejszemu wrogowi 2 pkt obrażeń, a następnie powtórz 3 razy.\nZa każdą kartę „Odwar Raffarda Białego” na swoim cmentarzu powtórz dodatkowy raz.",
                    "Wzmocnij najsłabszego sojusznika o 2 pkt, a następnie powtórz 3 razy.\nZa każdą kartę „Wyciąg z Gigaskorpiona” na swoim cmentarzu powtórz dodatkowy raz."
                },
                ["ru"] = new[]
                {
                    "Нанесите 2 ед. урона самому сильному противнику, затем повторите 3 раза.\nЗа каждую карту «Зелье Раффара Белого» на вашем кладбище повторите ещё один раз.",
                    "Усильте самого слабого союзника на 2 ед., затем повторите 3 раза.\nЗа каждую карту «Отвар из гигаскорпиона» на вашем кладбище повторите ещё один раз."
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

        [Fact]
        public void AugustSixthNilfgaardBatchMatchesRulesPoolAndLocales()
        {
            var restoredIds = new[]
            {
                "70004", "70012", "70103", "70111", "70115", "70123", "70127",
                "70150", "70151", "70152", "70153", "70165", "70174", "70184"
            };
            Assert.All(restoredIds, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(DiyAiCardPool.IsUserDeckCard(id));
            });

            Assert.Equal(5, GwentMap.CardMap["34024"].Strength);
            Assert.Equal(6, GwentMap.CardMap[CardId.Hybrid].Strength);
            Assert.Contains("士兵”或“军官", GwentMap.CardMap["13044"].Info);
            Assert.Equal("暗杀", GwentMap.CardMap["32015"].Name);
            Assert.Contains("被揭示的非间谍敌军单位牌", GwentMap.CardMap["33009"].Info);
            Assert.Contains("同排2个敌军", GwentMap.CardMap["33020"].Info);
            Assert.Contains("品质最低", GwentMap.CardMap["34004"].Info);
            Assert.Contains("佚亡原始同名牌", GwentMap.CardMap["70072"].Info);
            Assert.Contains("随机隐匿1张铜色手牌", GwentMap.CardMap["70152"].Info);
            Assert.Contains("下回合开始时，吞噬右侧单位", GwentMap.CardMap[CardId.Hybrid].Info);
            Assert.Contains("本小局", GwentMap.CardMap["70184"].Info);
            Assert.Equal(
                "检视对方牌组，将其中1张牌置于底端，并改变它的锁定状态。",
                GwentMap.CardMap["32002"].Info);
            Assert.Equal(
                "对1个敌军单位造成8点无视护甲的伤害，再对1个敌军单位造成8点无视护甲的伤害。",
                GwentMap.CardMap["32015"].Info);
            Assert.Equal(
                "对1个敌军单位造成7点无视护甲的伤害，若其具有增益则改为造成10点无视护甲的伤害。",
                GwentMap.CardMap["70156"].Info);
            Assert.DoesNotContain(
                GwentMap.CardMap.Values,
                card => (card.Info ?? string.Empty).Contains("汲取", StringComparison.Ordinal));

            var masquerade = GwentMap.CardMap[CardId.Masquerade];
            Assert.Equal("化妆舞会", masquerade.Name);
            Assert.Equal(Group.Copper, masquerade.Group);
            Assert.Equal(Faction.Nilfgaard, masquerade.Faction);
            Assert.Equal(CardType.Special, masquerade.CardType);
            Assert.Contains(Categorie.Tactic, masquerade.Categories);
            Assert.Equal("d19930000", masquerade.CardArtsId);
            Assert.Contains(CardId.Masquerade, DiyAiCardPool.RetiredCardIds);
            Assert.True(masquerade.IsDerive);
            Assert.False(DiyAiCardPool.IsUserDeckCard(CardId.Masquerade));
            Assert.Equal(typeof(Masquerade), new GwentCardDataService().GetType(CardId.Masquerade));
            Assert.True(File.Exists(FindRepositoryFile(
                "src/Cynthia.Card/src/Cynthia.Card.Server/wwwroot/scale/d19930000.png")));

            Assert.All(GwentMap.CardMap.Values, card =>
            {
                Assert.DoesNotContain("（向上取整）", card.Info);
                Assert.DoesNotContain("（向下取整）", card.Info);
                Assert.DoesNotContain("说明", card.Info);
            });

            var halfSources = new Dictionary<string, string>
            {
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Nilfgaard/Copper/Spotter.cs"] = "(result.Single().Status.Strength + 1) / 2",
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/NorthernRealms/Silver/VincentMeis.cs"] = "(damageint + 1) / 2",
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/Skellige/Copper/AnCraiteWarcrier.cs"] = "target.Status.HealthStatus + 1) / 2",
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY/Skellige/Silver/KnutTheCallous.cs"] = "(DTarget.CardPoint() + 1) / 2",
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY/ScoiaTael/Gold/TheGreatOak.cs"] = "(Dtarget.Status.Strength + 1) / 2",
                ["src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY/Nilfgaard/Gold/VincentvanMoorlehem.cs"] = "(num + 1) / 2"
            };
            Assert.All(halfSources, source => Assert.Contains(
                source.Value,
                File.ReadAllText(FindRepositoryFile(source.Key))));

            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            var changedIds = new[]
            {
                "13044", "32002", "32015", "33009", "33020", "34004", "34021", "34024", "70062", "70072",
                "70103", "70132", "70145", "70148", "70152", "70156", "70168", "70170", "70174", "70176",
                "70183", "70184", "70193"
            };
            Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
            {
                var locales = localeRoots.Select(root =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{root}/{language}.json")))).ToArray();
                Assert.All(changedIds, id => Assert.All(locales.Skip(1), locale =>
                {
                    Assert.Equal(locales[0].CardLocales[id].Name, locale.CardLocales[id].Name);
                    Assert.Equal(locales[0].CardLocales[id].Info, locale.CardLocales[id].Info);
                }));
                Assert.All(new[]
                {
                    "Masquerade_ChangeGold", "Masquerade_ChangeSilver", "Masquerade_ChangeCopper"
                }, key => Assert.All(locales.Skip(1), locale =>
                    Assert.Equal(locales[0].MenuLocales[key], locale.MenuLocales[key])));
            });

            var chinese = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.All(changedIds, id =>
            {
                Assert.Equal(GwentMap.CardMap[id].Name, chinese.CardLocales[id].Name);
                Assert.Equal(GwentMap.CardMap[id].Info, chinese.CardLocales[id].Info);
            });
            Assert.All(localeRoots, root =>
            {
                var locale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                    FindRepositoryFile($"{root}/cn.json")));
                Assert.DoesNotContain(
                    locale.CardLocales.Values,
                    card => (card.Info ?? string.Empty).Contains("汲取", StringComparison.Ordinal));
            });
        }

        [Fact]
        public void AugustSeventhFirstBatchMatchesRulesPoolAndLocales()
        {
            var restoredIds = new[]
            {
                "70017", "70024", "70033", "70050", "70076", "70077", "70078",
                "70086", "70094", "70095", "70101", "70104", "70118", "70126",
                "70130", "70141", "70142", "70143", "70144", "70163", "70188"
            };
            Assert.All(restoredIds, id =>
            {
                Assert.DoesNotContain(id, DiyAiCardPool.RetiredCardIds);
                Assert.True(DiyAiCardPool.IsUserDeckCard(id));
                Assert.False(GwentMap.CardMap[id].IsDerive);
            });

            Assert.Equal(7, GwentMap.CardMap["13012"].Strength);
            Assert.Equal("造成3、2、1点伤害。", GwentMap.CardMap["13012"].Info);
            Assert.Equal(10, GwentMap.CardMap["43003"].Strength);
            Assert.Contains(Categorie.Soldier, GwentMap.CardMap["43003"].Categories);
            Assert.Equal(5, GwentMap.CardMap["43015"].Strength);
            Assert.Equal(2, GwentMap.CardMap["43017"].Strength);
            Assert.Equal(7, GwentMap.CardMap["70076"].Strength);
            Assert.Equal(8, GwentMap.CardMap["70094"].Strength);
            Assert.Contains("一半", GwentMap.CardMap["70094"].Info);
            Assert.Equal(7, GwentMap.CardMap["70126"].Strength);
            Assert.Equal(7, GwentMap.CardMap["70130"].Strength);
            Assert.Equal(new[] { Categorie.Soldier }, GwentMap.CardMap["70101"].Categories);
            Assert.Contains("每2回合开始时", GwentMap.CardMap["70101"].Info);
            Assert.True(GwentMap.CardMap["70146"].IsCountdown);
            Assert.Equal(3, GwentMap.CardMap["70146"].Countdown);
            Assert.Contains("3回合后的回合开始时", GwentMap.CardMap["70146"].Info);
            Assert.Contains("随后使其获得1点增益", GwentMap.CardMap["70152"].Info);
            Assert.False(GwentMap.CardMap["70163"].IsCountdown);
            Assert.DoesNotContain("生效3次", GwentMap.CardMap["70163"].Info);
            Assert.Contains("所有增益和护甲", GwentMap.CardMap["70179"].Info);
            Assert.DoesNotContain("生成并打出", GwentMap.CardMap["70188"].Info);

            var dataService = new GwentCardDataService();
            Assert.Equal(typeof(Myrgtabrakke), dataService.GetType("13012"));
            Assert.Equal(typeof(Roach), dataService.GetType("13001"));
            Assert.Equal(typeof(Ves), dataService.GetType("43002"));
            Assert.Equal(typeof(Trollololo), dataService.GetType("43003"));
            Assert.Equal(typeof(Winch), dataService.GetType("44033"));
            Assert.Equal(typeof(Gael), dataService.GetType("70146"));
            Assert.Equal(typeof(QueenCalanthe), dataService.GetType("70179"));
            Assert.Equal(typeof(Mantlet), dataService.GetType("70130"));
            Assert.Equal(typeof(ImmortalCavalry), dataService.GetType("70101"));

            var changedIds = new[]
            {
                "13001", "13012", "43002", "43003", "43015", "43017", "44025",
                "44033", "70076", "70094", "70101", "70126", "70130", "70146",
                "70152", "70163", "70179", "70188"
            };
            var localeRoots = new[]
            {
                "src/Cynthia.Card/src/Cynthia.Card.Server/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Resources/Locales",
                "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/StreamingFile/Locales"
            };
            Assert.All(new[] { "cn", "en", "pl", "ru" }, language =>
            {
                var locales = localeRoots.Select(root =>
                    JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                        FindRepositoryFile($"{root}/{language}.json")))).ToArray();
                Assert.All(changedIds, id => Assert.All(locales.Skip(1), locale =>
                {
                    Assert.Equal(locales[0].CardLocales[id].Name, locale.CardLocales[id].Name);
                    Assert.Equal(locales[0].CardLocales[id].Info, locale.CardLocales[id].Info);
                }));
            });

            var chinese = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(
                FindRepositoryFile("src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json")));
            Assert.All(changedIds, id =>
            {
                Assert.Equal(GwentMap.CardMap[id].Name, chinese.CardLocales[id].Name);
                Assert.Equal(GwentMap.CardMap[id].Info, chinese.CardLocales[id].Info);
            });
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
