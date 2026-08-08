using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.AI;
using Cynthia.Card.Server.Services.GwentGameService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class RuleCardArchitectureTests
    {
        [Fact]
        public void ServerAuthoredAiRulesAreInjectedOnceWithoutTouchingPlayerDecks()
        {
            var ai = new SoldierTrainAI();
            var originalCards = ai.Deck.Deck.ToList();

            GwentMatchs.ApplyAiRuleCards(ai, new[] { "99004", "99004" });

            Assert.Equal(originalCards.Count + 1, ai.Deck.Deck.Count);
            Assert.Single(ai.Deck.Deck, x => x == "99004");
            Assert.Equal(originalCards, ai.Deck.Deck.Where(x => x != "99004"));
        }

        [Fact]
        public void EventOnlyFeastEchoRuleKeepsStandardGoldAndSilverLimits()
        {
            const string ruleId = "99004";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var silverCards = GwentMap.CardMap.Values
                .Where(x => x.Group == Group.Silver &&
                            (x.Faction == Faction.Neutral || x.Faction == leader.Faction) &&
                            DiyAiCardPool.IsUserDeckCard(x.CardId))
                .GroupBy(x => x.CardId)
                .Select(x => x.First())
                .Take(7)
                .ToList();
            Assert.Equal(7, silverCards.Count);

            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var rules = DeckRuleEngine.Resolve(
                    new[] { new RuleCardDefinition { Id = ruleId, PlayerSelectable = true } },
                    new[] { ruleId },
                    "event-only-v1");
                Assert.Contains(rules.Constraints, x =>
                    x.Id == DeckRuleEngine.StandardGoldCount && x.Max == 4);
                Assert.Contains(rules.Constraints, x =>
                    x.Id == DeckRuleEngine.StandardSilverCount && x.Max == 6);

                var deck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new[] { ruleId }.Concat(silverCards.Take(6).Select(x => x.CardId)).ToList()
                };
                Assert.True(DeckRuleEngine.Validate(deck, rules, false).IsValid);
                Assert.False(DeckRuleEngine.CanAddCard(deck, silverCards[6].CardId, rules));
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void V2DeckCodeStoresDirectCardIdsAndRoundTripsDeckOrder()
        {
            var deck = GwentDeck.CreateBasicDeck(0);
            var code = deck.CompressDeck();
            var restored = code.DeCompressToDeck();

            Assert.StartsWith("V2.", code);
            Assert.Equal(deck.Leader, restored.Leader);
            Assert.Equal(deck.Deck, restored.Deck);
        }

        [Fact]
        public void RuleResolutionSupportsExplicitRelaxationRestrictionAndConflicts()
        {
            var definitions = new[]
            {
                new RuleCardDefinition
                {
                    Id = "rule.large-deck",
                    RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardDeckSize },
                    AddConstraints = new List<DeckConstraintDefinition>
                    {
                        new DeckConstraintDefinition { Id = "rule.large-deck.size", Kind = "deck-size", Min = 25, Max = 100 }
                    },
                    ExclusiveWith = new List<string> { "rule.copper-only" }
                },
                new RuleCardDefinition
                {
                    Id = "rule.copper-only",
                    AddConstraints = new List<DeckConstraintDefinition>
                    {
                        new DeckConstraintDefinition
                        {
                            Id = "rule.copper-only.cards",
                            Kind = "allow-cards",
                            Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                        },
                        new DeckConstraintDefinition
                        {
                            Id = "rule.copper-only.copies",
                            Kind = "copy-count",
                            Max = 1,
                            Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                        }
                    }
                }
            };

            var large = DeckRuleEngine.Resolve(definitions, new[] { "rule.large-deck" }, "rules-1");
            Assert.DoesNotContain(large.Constraints, x => x.Id == DeckRuleEngine.StandardDeckSize);
            Assert.Contains(large.Constraints, x => x.Id == "rule.large-deck.size" && x.Max == 100);

            var conflict = DeckRuleEngine.Resolve(definitions, new[] { "rule.large-deck", "rule.copper-only" }, "rules-1");
            Assert.Contains(conflict.ResolutionIssues, x => x.Code == "rules.conflict");
            Assert.NotEqual(large.Fingerprint, conflict.Fingerprint);
        }

        [Fact]
        public void RuleFingerprintIsStableRegardlessOfSelectionAndDefinitionOrder()
        {
            var first = new RuleCardDefinition
            {
                Id = "rule.first",
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition { Id = "rule.first.size", Kind = "deck-size", Min = 20, Max = 60 }
                }
            };
            var second = new RuleCardDefinition
            {
                Id = "rule.second",
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition { Id = "rule.second.denied", Kind = "deny-cards", Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Gold } } }
                }
            };

            var left = DeckRuleEngine.Resolve(new[] { first, second }, new[] { "rule.second", "rule.first" }, "v1");
            var right = DeckRuleEngine.Resolve(new[] { second, first }, new[] { "rule.first", "rule.second" }, "v1");

            Assert.Equal(left.Fingerprint, right.Fingerprint);
            Assert.Equal(new[] { "rule.first", "rule.second" }, left.AppliedRuleCards);
        }

        [Fact]
        public void CombinedMatchFingerprintIncludesRulePackageVersions()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            player2.Deck.Deck.RemoveAll(x => x == ruleId);
            var room = new GwentRoom(player1, "test-versioned-rules");
            room.AddPlayer(player2);
            var first = new RuleCardDefinition { Id = ruleId, PackageVersion = "1", IsEnabled = true };
            var second = new RuleCardDefinition { Id = ruleId, PackageVersion = "2", IsEnabled = true };

            var firstFingerprint = GwentMatchs.CreateCombinedRuleFingerprint(
                room, new[] { first }, "rules-v1", isActiveRuleCard: id => id == ruleId);
            var secondFingerprint = GwentMatchs.CreateCombinedRuleFingerprint(
                room, new[] { second }, "rules-v1", isActiveRuleCard: id => id == ruleId);

            Assert.NotEqual(firstFingerprint, secondFingerprint);
        }

        [Fact]
        public async Task GameResultKeepsVersionedRulePackagesForEachOwner()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            player2.Deck.Deck.RemoveAll(x => x == ruleId);
            GameResult captured = null;
            var definition = new RuleCardDefinition
            {
                Id = ruleId,
                PackageVersion = "package-7",
                Priority = 42,
                OverrideScopes = new List<string> { DeckRuleEngine.ScopeCardPool }
            };
            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                result => captured = result,
                false,
                id => id == ruleId,
                activeRuleCardIds: new[] { ruleId },
                ruleCardDefinitions: new[] { definition });

            await game.GameOverExecute();

            Assert.NotNull(captured);
            var redUsesRule = captured.RedRuleCards.Contains(ruleId);
            var ownerPackages = redUsesRule ? captured.RedRulePackages : captured.BlueRulePackages;
            var otherPackages = redUsesRule ? captured.BlueRulePackages : captured.RedRulePackages;
            var package = Assert.Single(ownerPackages, x => x.RuleCardId == ruleId);
            Assert.Equal("package-7", package.PackageVersion);
            Assert.Equal(42, package.Priority);
            Assert.Equal(new[] { DeckRuleEngine.ScopeCardPool }, package.OverrideScopes);
            Assert.DoesNotContain(otherPackages, x => x.RuleCardId == ruleId);
        }

        [Fact]
        public void PvpMatchKeysSeparateRuleFingerprintsUnlessTheModeExplicitlyOptsOut()
        {
            var standardMode = new GameModeDefinition
            {
                Id = "pvp.casual",
                MatchKind = "pvp",
                RuleMatchPolicy = "same"
            };
            var first = new ResolvedDeckRuleSet { Fingerprint = "rules-a" };
            var second = new ResolvedDeckRuleSet { Fingerprint = "rules-b" };

            Assert.NotEqual(
                GwentMatchs.CreateModeMatchKey(standardMode, first),
                GwentMatchs.CreateModeMatchKey(standardMode, second));
            Assert.Equal("mode:pvp.casual:rules-a", GwentMatchs.CreateModeMatchKey(standardMode, first));

            standardMode.RuleMatchPolicy = "ignore";
            Assert.Equal(
                GwentMatchs.CreateModeMatchKey(standardMode, first),
                GwentMatchs.CreateModeMatchKey(standardMode, second));
            Assert.Equal("mode:pvp.casual", GwentMatchs.CreateModeMatchKey(standardMode, first));
        }

        [Fact]
        public void RulePackageMatchesAreExcludedFromRankedMmrUnlessModeExplicitlyOptsIn()
        {
            const string ruleId = "test-ranked-rule-package";
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var previous);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
            var mode = new GameModeDefinition { Id = "pvp.ranked", IsRanked = true };
            var standard = new ClientPlayer(
                new User("ranked-standard", "ranked-standard-connection"),
                () => null)
            {
                Deck = new DeckModel { Deck = new List<string> { "ordinary-card" } }
            };
            var rulePlayer = new ClientPlayer(
                new User("ranked-rule", "ranked-rule-connection"),
                () => null)
            {
                Deck = new DeckModel { Deck = new List<string> { ruleId } }
            };
            var room = new GwentRoom(standard, "mode:pvp.ranked");
            room.AddPlayer(rulePlayer);

            Assert.False(GwentMatchs.ShouldCountModeMatchAsRanked(mode, room));

            mode.CountRuleMatchesAsRanked = true;
            Assert.True(GwentMatchs.ShouldCountModeMatchAsRanked(mode, room));

            rulePlayer.Deck.Deck.Clear();
            mode.CountRuleMatchesAsRanked = false;
            Assert.True(GwentMatchs.ShouldCountModeMatchAsRanked(mode, room));

            mode.IsRanked = false;
            Assert.False(GwentMatchs.ShouldCountModeMatchAsRanked(mode, room));
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = previous;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void ExplicitPasswordRoomsAllowDifferentRuleDecksByDesign()
        {
            var owner = new ClientPlayer(
                new User("password-owner", "owner-connection") { PlayerName = "owner" },
                () => null)
            {
                Deck = new DeckModel { Deck = new List<string> { "rule.fingerprint-a" } }
            };
            var challenger = new ClientPlayer(
                new User("password-challenger", "challenger-connection") { PlayerName = "challenger" },
                () => null)
            {
                Deck = new DeckModel { Deck = new List<string> { "rule.fingerprint-b" } }
            };
            var room = new GwentRoom(owner, "RuleTrial-806");

            Assert.True(GwentMatchs.CanJoinExplicitPasswordRoom(room, challenger, "ruletrial-806"));
            Assert.False(GwentMatchs.CanJoinExplicitPasswordRoom(room, challenger, "different-password"));
        }

        [Fact]
        public void LeaderFactionRestrictionBecomesASharedDeclarativeConstraint()
        {
            var rules = DeckRuleEngine.Resolve(new[]
            {
                new RuleCardDefinition
                {
                    Id = "rule.monsters-only",
                    AllowedLeaderFactions = new List<Faction> { Faction.Monsters }
                }
            }, new[] { "rule.monsters-only" }, "v1");

            var constraint = Assert.Single(rules.Constraints, x => x.Kind == "leader-faction");
            Assert.Equal(new[] { Faction.Monsters }, constraint.Filter.Factions);
        }

        [Fact]
        public void NamedCardPoolsCanExpandRestrictAndExcludeTheEditorPool()
        {
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var foreignCopper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction != Faction.Neutral && x.Faction != leader.Faction &&
                DiyAiCardPool.IsUserDeckCard(x.CardId));
            var pools = new[]
            {
                new CardPoolDefinition
                {
                    Id = "foreign",
                    AnyOf = new List<CardFilterDefinition>
                    {
                        new CardFilterDefinition { Factions = new List<Faction> { foreignCopper.Faction } }
                    }
                },
                new CardPoolDefinition
                {
                    Id = "copper",
                    AnyOf = new List<CardFilterDefinition>
                    {
                        new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                    }
                },
                new CardPoolDefinition
                {
                    Id = "blocked",
                    AnyOf = new List<CardFilterDefinition>
                    {
                        new CardFilterDefinition { CardIds = new List<string> { foreignCopper.CardId } }
                    }
                }
            };
            var definition = new RuleCardDefinition
            {
                Id = "rule.pool",
                AddCardPools = new List<string> { "foreign" },
                RestrictToCardPools = new List<string> { "copper" }
            };

            var expanded = DeckRuleEngine.Resolve(new[] { definition }, new[] { definition.Id }, "v1", pools);
            Assert.True(DeckRuleEngine.IsCardAllowedByPool(foreignCopper, leader, expanded));
            Assert.DoesNotContain(expanded.Constraints, x => x.Id == DeckRuleEngine.StandardFaction);

            definition.ExcludeCardPools = new List<string> { "blocked" };
            var excluded = DeckRuleEngine.Resolve(new[] { definition }, new[] { definition.Id }, "v1", pools);
            Assert.False(DeckRuleEngine.IsCardAllowedByPool(foreignCopper, leader, excluded));
            Assert.NotEqual(expanded.Fingerprint, excluded.Fingerprint);
        }

        [Fact]
        public void PlayerRuleCardExposureGateDoesNotDeleteOrChangeRuntimeDefinitions()
        {
            var manifest = new GameFeatureManifest
            {
                PlayerRuleCardsEnabled = false,
                RuleCards = new List<RuleCardDefinition> { new RuleCardDefinition { Id = "rule.hidden" } }
            };
            var deck = new DeckModel { Deck = new List<string> { "rule.hidden" } };

            Assert.False(GameFeatureService.CanPlayerUseDeck(manifest, deck));
            Assert.Single(manifest.RuleCards);
            Assert.Contains(
                DeckRuleEngine.Resolve(manifest.RuleCards, deck.Deck, "gate-v1").ExecutionOrder,
                x => x.RuleCardId == "rule.hidden");

            manifest.PlayerRuleCardsEnabled = true;
            Assert.False(GameFeatureService.CanPlayerUseDeck(manifest, deck));
            manifest.RuleCards[0].PlayerSelectable = true;
            Assert.True(GameFeatureService.CanPlayerUseDeck(manifest, deck));

            manifest.RuleCards[0].IsEnabled = false;
            Assert.False(GameFeatureService.CanPlayerUseDeck(manifest, deck));
            Assert.DoesNotContain(
                DeckRuleEngine.Resolve(manifest.RuleCards, deck.Deck, "gate-v1").ExecutionOrder,
                x => x.RuleCardId == "rule.hidden");
            Assert.Single(manifest.RuleCards);
        }

        [Fact]
        public void HigherPriorityRuleWinsTheSamePropertyWhileEqualPriorityConflicts()
        {
            var low = new RuleCardDefinition
            {
                Id = "rule.low",
                Priority = 10,
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition { Id = "package.deck-size", Kind = "deck-size", Min = 25, Max = 100 }
                }
            };
            var high = new RuleCardDefinition
            {
                Id = "rule.high",
                Priority = 20,
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition { Id = "package.deck-size", Kind = "deck-size", Min = 20, Max = 60 }
                }
            };

            var resolved = DeckRuleEngine.Resolve(new[] { high, low }, new[] { high.Id, low.Id }, "v1");
            var winner = Assert.Single(resolved.Constraints, x => x.Id == "package.deck-size");
            Assert.Equal(60, winner.Max);
            Assert.Equal(new[] { low.Id, high.Id }, resolved.ExecutionOrder.Select(x => x.RuleCardId));
            Assert.DoesNotContain(resolved.ResolutionIssues, x => x.Code == "rules.priority-conflict");

            high.Priority = low.Priority;
            var tied = DeckRuleEngine.Resolve(new[] { high, low }, new[] { high.Id, low.Id }, "v1");
            Assert.Contains(tied.ResolutionIssues, x => x.Code == "rules.priority-conflict" && x.ConstraintId == "package.deck-size");
        }

        [Fact]
        public void HigherPriorityScopeTakeoverSuppressesLowerCardPoolRules()
        {
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var foreign = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction != Faction.Neutral && x.Faction != leader.Faction &&
                DiyAiCardPool.IsUserDeckCard(x.CardId));
            var foreignPool = new CardPoolDefinition
            {
                Id = "foreign",
                AnyOf = new List<CardFilterDefinition>
                {
                    new CardFilterDefinition { CardIds = new List<string> { foreign.CardId } }
                }
            };
            var low = new RuleCardDefinition
            {
                Id = "rule.expansion",
                Priority = 10,
                AddCardPools = new List<string> { foreignPool.Id }
            };
            var high = new RuleCardDefinition
            {
                Id = "rule.standard-pool",
                Priority = 20,
                OverrideScopes = new List<string> { DeckRuleEngine.ScopeCardPool }
            };

            var expanded = DeckRuleEngine.Resolve(new[] { low }, new[] { low.Id }, "v1", new[] { foreignPool });
            Assert.True(DeckRuleEngine.IsCardAllowedByPool(foreign, leader, expanded));

            var takenOver = DeckRuleEngine.Resolve(new[] { low, high }, new[] { low.Id, high.Id }, "v1", new[] { foreignPool });
            Assert.False(DeckRuleEngine.IsCardAllowedByPool(foreign, leader, takenOver));
            Assert.Empty(takenOver.CardPoolModifiers);
        }

        [Fact]
        public void HigherPriorityScopeTakeoverSuppressesEveryConstraintScope()
        {
            var cases = new[]
            {
                new
                {
                    Scope = DeckRuleEngine.ScopeDeckLimits,
                    Constraint = new DeckConstraintDefinition { Id = "low.deck", Kind = "deck-size", Min = 1, Max = 99 }
                },
                new
                {
                    Scope = DeckRuleEngine.ScopeGroupLimits,
                    Constraint = new DeckConstraintDefinition
                    {
                        Id = "low.group", Kind = "card-count", Max = 12,
                        Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Gold } }
                    }
                },
                new
                {
                    Scope = DeckRuleEngine.ScopeCardLimits,
                    Constraint = new DeckConstraintDefinition
                    {
                        Id = "low.card", Kind = "copy-count", Max = 7,
                        Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                    }
                }
            };

            foreach (var item in cases)
            {
                var low = new RuleCardDefinition
                {
                    Id = "rule.low." + item.Scope,
                    Priority = 10,
                    AddConstraints = new List<DeckConstraintDefinition> { item.Constraint }
                };
                var high = new RuleCardDefinition
                {
                    Id = "rule.high." + item.Scope,
                    Priority = 20,
                    OverrideScopes = new List<string> { item.Scope }
                };

                var resolved = DeckRuleEngine.Resolve(new[] { low, high }, new[] { low.Id, high.Id }, "v1");

                Assert.DoesNotContain(resolved.Constraints, x => x.Id == item.Constraint.Id);
            }

            var lowLeader = new RuleCardDefinition
            {
                Id = "rule.low.leader",
                Priority = 10,
                AllowedLeaderFactions = new List<Faction> { Faction.Monsters }
            };
            var highLeader = new RuleCardDefinition
            {
                Id = "rule.high.leader",
                Priority = 20,
                OverrideScopes = new List<string> { DeckRuleEngine.ScopeLeader }
            };
            var leaderRules = DeckRuleEngine.Resolve(
                new[] { lowLeader, highLeader },
                new[] { lowLeader.Id, highLeader.Id },
                "v1");

            Assert.DoesNotContain(leaderRules.Constraints, x => x.Kind == "leader-faction");
        }

        [Fact]
        public void FullScopeTakeoverSuppressesAllLowerPriorityRuleProposalsButKeepsBaseRules()
        {
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var foreign = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction != Faction.Neutral && x.Faction != leader.Faction &&
                DiyAiCardPool.IsUserDeckCard(x.CardId));
            var foreignPool = new CardPoolDefinition
            {
                Id = "all.foreign",
                AnyOf = new List<CardFilterDefinition>
                {
                    new CardFilterDefinition { CardIds = new List<string> { foreign.CardId } }
                }
            };
            var low = new RuleCardDefinition
            {
                Id = "rule.low.all",
                Priority = 10,
                AllowedLeaderFactions = new List<Faction> { Faction.Monsters },
                AddCardPools = new List<string> { foreignPool.Id },
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition { Id = "low.all.deck", Kind = "deck-size", Min = 0, Max = 100 },
                    new DeckConstraintDefinition
                    {
                        Id = "low.all.group", Kind = "card-count", Max = 20,
                        Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Gold } }
                    },
                    new DeckConstraintDefinition
                    {
                        Id = "low.all.card", Kind = "copy-count", Max = 20,
                        Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                    }
                }
            };
            var high = new RuleCardDefinition
            {
                Id = "rule.high.all",
                Priority = 20,
                OverrideScopes = new List<string> { DeckRuleEngine.ScopeAll }
            };

            var resolved = DeckRuleEngine.Resolve(
                new[] { low, high },
                new[] { low.Id, high.Id },
                "v1",
                new[] { foreignPool });

            Assert.Empty(resolved.CardPoolModifiers);
            Assert.DoesNotContain(resolved.Constraints, x => x.Id.StartsWith("low.all.", StringComparison.Ordinal));
            Assert.DoesNotContain(resolved.Constraints, x => x.Kind == "leader-faction");
            Assert.Contains(resolved.Constraints, x => x.Id == DeckRuleEngine.StandardDeckSize);
            Assert.Contains(resolved.Constraints, x => x.Id == DeckRuleEngine.StandardFaction);
        }

        [Fact]
        public void HigherPriorityNormalizationScopeSuppressesLowerRemovalProposals()
        {
            const string lowId = "test-normalization-low";
            const string highId = "test-normalization-high";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var oldLowExists = GwentMap.CardMap.TryGetValue(lowId, out var oldLow);
            var oldHighExists = GwentMap.CardMap.TryGetValue(highId, out var oldHigh);
            GwentMap.CardMap[lowId] = TestRuleCard(lowId);
            GwentMap.CardMap[highId] = TestRuleCard(highId);
            try
            {
                var low = new RuleCardDefinition
                {
                    Id = lowId,
                    IsEnabled = true,
                    PlayerSelectable = true,
                    Priority = 10,
                    NormalizationRemovals = new List<DeckRuleTransitionRemoval>
                    {
                        new DeckRuleTransitionRemoval
                        {
                            CardId = copper.CardId,
                            OriginalIndex = 0,
                            ReasonCode = "test.remove"
                        }
                    }
                };
                var high = new RuleCardDefinition
                {
                    Id = highId,
                    IsEnabled = true,
                    PlayerSelectable = true,
                    Priority = 20,
                    OverrideScopes = new List<string> { DeckRuleEngine.ScopeNormalization }
                };
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "normalization-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition> { low }
                };
                var lowOnlyDeck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { lowId, copper.CardId }
                };

                var removalPreview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 1,
                    Action = "add",
                    CandidateCardId = lowId,
                    Deck = lowOnlyDeck
                });

                Assert.True(removalPreview.RequiresConfirmation);
                var removal = Assert.Single(removalPreview.RemovedCards);
                Assert.Equal(copper.CardId, removal.CardId);
                Assert.Equal(lowId, removal.SourceRuleCardId);
                Assert.DoesNotContain(copper.CardId, removalPreview.NormalizedDeck.Deck);

                manifest.RuleCards.Add(high);
                var takeoverDeck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { lowId, highId, copper.CardId }
                };
                var takeover = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 2,
                    Action = "add",
                    CandidateCardId = highId,
                    Deck = takeoverDeck
                });

                Assert.False(takeover.RequiresConfirmation);
                Assert.Empty(takeover.RemovedCards);
                Assert.Contains(copper.CardId, takeover.NormalizedDeck.Deck);
            }
            finally
            {
                if (oldLowExists) GwentMap.CardMap[lowId] = oldLow;
                else GwentMap.CardMap.Remove(lowId);
                if (oldHighExists) GwentMap.CardMap[highId] = oldHigh;
                else GwentMap.CardMap.Remove(highId);
            }
        }

        [Fact]
        public void DuplicateRuleCardsRequireAVisibleDeterministicCleanupPreview()
        {
            const string ruleId = "test-duplicate-rule";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "duplicates-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition { Id = ruleId, IsEnabled = true, PlayerSelectable = true }
                    }
                };
                var deck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { ruleId, ruleId }
                };

                var preview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 3,
                    Action = "add",
                    CandidateCardId = ruleId,
                    Deck = deck
                });

                Assert.True(preview.RequiresConfirmation);
                var removal = Assert.Single(preview.RemovedCards);
                Assert.Equal(ruleId, removal.CardId);
                Assert.Equal("rules.duplicate", removal.ReasonCode);
                Assert.Single(preview.NormalizedDeck.Deck, x => x == ruleId);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void SharedModeValidationRejectsUnknownModeAndAcceptsCompleteStandardDeck()
        {
            var deck = GwentDeck.CreateBasicDeck(0);
            var manifest = new GameFeatureManifest { RulesetVersion = "v1" };
            var casual = new GameModeDefinition
            {
                Id = "pvp.casual",
                IsEnabled = true,
                AllowCustomRuleCards = false
            };

            var accepted = DeckRuleEngine.ValidateMode(deck, manifest, casual, true);
            var rejected = DeckRuleEngine.ValidateMode(deck, manifest, null, true);

            Assert.True(accepted.IsComplete);
            Assert.False(rejected.IsComplete);
            Assert.Contains(rejected.Issues, x => x.Code == "mode.unknown");
        }

        [Fact]
        public void RemovingARelaxationProducesDeterministicLegalToIllegalCleanupPreview()
        {
            var deck = GwentDeck.CreateBasicDeck(0);
            var leader = GwentMap.CardMap[deck.Leader];
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            deck.Deck = new List<string> { copper.CardId, copper.CardId, copper.CardId, copper.CardId };

            // Isolate the constraint transition from manifest/card-map identity checks.
            var relaxed = DeckRuleEngine.Resolve(Enumerable.Empty<RuleCardDefinition>(), Enumerable.Empty<string>(), "v1");
            relaxed.Constraints.RemoveAll(x => x.Id == DeckRuleEngine.StandardCopperCopies);
            relaxed.Constraints.Add(new DeckConstraintDefinition
            {
                Id = "fixture.four-copies.limit",
                Kind = "copy-count",
                Max = 4,
                Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
            });
            var standard = DeckRuleEngine.Resolve(Enumerable.Empty<RuleCardDefinition>(), Enumerable.Empty<string>(), "v1");

            var relaxedDraft = new DeckModel { Leader = deck.Leader, Deck = deck.Deck.ToList() };
            Assert.True(DeckRuleEngine.Validate(relaxedDraft, relaxed, false).IsValid);
            var transition = DeckRuleEngine.PlanTransition(relaxedDraft, standard);

            Assert.True(transition.RequiresConfirmation);
            Assert.Single(transition.RemovedCards);
            Assert.Equal(3, transition.RemovedCards[0].OriginalIndex);
            Assert.Equal("copies.max", transition.RemovedCards[0].ReasonCode);
            Assert.Equal(3, transition.ResultDeck.Deck.Count(x => x == copper.CardId));
            Assert.True(transition.Validation.IsValid);
        }

        [Fact]
        public void RemovingPrerequisiteRecursivelyPreviewsDependentRuleCleanup()
        {
            const string parentId = "test-rule-parent";
            const string childId = "test-rule-child";
            const string grandchildId = "test-rule-grandchild";
            var ids = new[] { parentId, childId, grandchildId };
            var previous = new Dictionary<string, GwentCard>(StringComparer.Ordinal);
            var previouslyPresent = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in ids)
            {
                if (!GwentMap.CardMap.TryGetValue(id, out var card)) continue;
                previous[id] = card;
                previouslyPresent.Add(id);
            }

            try
            {
                foreach (var id in ids)
                {
                    GwentMap.CardMap[id] = new GwentCard
                    {
                        CardId = id,
                        Name = id,
                        Group = Group.Copper,
                        Faction = Faction.Neutral,
                        CardType = CardType.Special,
                        HideTags = new[] { HideTag.Rule },
                        Categories = new Categorie[0]
                    };
                }

                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "dependency-cleanup-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition { Id = parentId, PlayerSelectable = true },
                        new RuleCardDefinition { Id = childId, PlayerSelectable = true, RequiresAll = new List<string> { parentId } },
                        new RuleCardDefinition { Id = grandchildId, PlayerSelectable = true, RequiresAll = new List<string> { childId } }
                    }
                };
                var submitted = GwentDeck.CreateBasicDeck(0);
                submitted.Deck = new List<string> { childId, grandchildId };

                var preview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "remove",
                    CandidateCardId = parentId,
                    Deck = submitted
                });

                Assert.True(preview.RequiresConfirmation);
                Assert.Equal(new[] { childId, grandchildId }, preview.RemovedCards.Select(x => x.CardId).OrderBy(x => x));
                Assert.Empty(preview.NormalizedDeck.Deck);

                var confirmed = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "remove",
                    CandidateCardId = parentId,
                    ConfirmNormalization = true,
                    Deck = submitted
                });

                Assert.True(confirmed.IsValid);
                Assert.False(confirmed.RequiresConfirmation);
                Assert.Empty(confirmed.NormalizedDeck.Deck);
            }
            finally
            {
                foreach (var id in ids)
                {
                    if (!previouslyPresent.Contains(id)) GwentMap.CardMap.Remove(id);
                    else GwentMap.CardMap[id] = previous[id];
                }
            }
        }

        [Fact]
        public void CyclicRuleDependenciesTerminateWithDeterministicCleanup()
        {
            const string firstId = "test-rule-cycle-a";
            const string secondId = "test-rule-cycle-b";
            var ids = new[] { firstId, secondId };
            var previous = new Dictionary<string, GwentCard>(StringComparer.Ordinal);
            foreach (var id in ids)
            {
                if (GwentMap.CardMap.TryGetValue(id, out var card)) previous[id] = card;
                GwentMap.CardMap[id] = TestRuleCard(id);
            }

            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "dependency-cycle-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = firstId,
                            PlayerSelectable = true,
                            RequiresAll = new List<string> { secondId }
                        },
                        new RuleCardDefinition
                        {
                            Id = secondId,
                            PlayerSelectable = true,
                            RequiresAll = new List<string> { firstId }
                        }
                    }
                };
                var submitted = GwentDeck.CreateBasicDeck(0);
                submitted.Deck = new List<string> { secondId };

                var preview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "remove",
                    CandidateCardId = firstId,
                    Deck = submitted
                });

                Assert.True(preview.RequiresConfirmation);
                Assert.Equal(secondId, Assert.Single(preview.RemovedCards).CardId);
                Assert.Empty(preview.NormalizedDeck.Deck);
                Assert.DoesNotContain(preview.Issues, x =>
                    x.Code == "rules.normalization-cycle" || x.Code == "rules.normalization-limit");

                var repeated = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "remove",
                    CandidateCardId = firstId,
                    Deck = submitted
                });

                Assert.Equal(
                    preview.RemovedCards.Select(x => (x.CardId, x.OriginalIndex, x.ReasonCode)),
                    repeated.RemovedCards.Select(x => (x.CardId, x.OriginalIndex, x.ReasonCode)));
                Assert.Equal(preview.ProjectionFingerprint, repeated.ProjectionFingerprint);
            }
            finally
            {
                foreach (var id in ids)
                {
                    if (previous.TryGetValue(id, out var card)) GwentMap.CardMap[id] = card;
                    else GwentMap.CardMap.Remove(id);
                }
            }
        }

        [Fact]
        public void AdditiveRestrictionPrunesOnlyCardsOutsideTheNewRule()
        {
            var deck = GwentDeck.CreateBasicDeck(0);
            var leader = GwentMap.CardMap[deck.Leader];
            var copper = GwentMap.CardMap.Values.First(x => x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var gold = GwentMap.CardMap.Values.First(x => x.Group == Group.Gold && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            deck.Deck = new List<string> { copper.CardId, gold.CardId, copper.CardId };
            var copperOnly = DeckRuleEngine.Resolve(Enumerable.Empty<RuleCardDefinition>(), Enumerable.Empty<string>(), "v1");
            // Apply just the resulting restriction; the manifest card itself is not part of this isolated fixture.
            copperOnly.Constraints.Add(new DeckConstraintDefinition
            {
                Id = "fixture.copper-only.cards",
                Kind = "allow-cards",
                Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
            });

            var transition = DeckRuleEngine.PlanTransition(deck, copperOnly);

            Assert.Single(transition.RemovedCards);
            Assert.Equal(gold.CardId, transition.RemovedCards[0].CardId);
            Assert.Equal(new[] { copper.CardId, copper.CardId }, transition.ResultDeck.Deck);
        }

        [Fact]
        public void AuthoritativeProjectionReturnsExplainableLimitsAndCardStates()
        {
            var deck = GwentDeck.CreateBasicDeck(0);
            var projection = DeckBuildingProjectionEngine.Project(
                new GameFeatureManifest { RulesetVersion = "projection-v1" },
                new DeckBuildingProjectionRequest { Revision = 7, Deck = deck });

            Assert.Equal(7, projection.Revision);
            Assert.True(projection.IsValid);
            Assert.True(projection.IsComplete);
            Assert.NotEmpty(projection.ProjectionFingerprint);
            Assert.NotEmpty(projection.PoolFingerprint);
            Assert.Contains(projection.Limits, x => x.ConstraintId == DeckRuleEngine.StandardDeckSize && x.Max == 40);
            Assert.Contains(projection.CardStates, x => x.Selectable && x.MaxCopies > 0);
            Assert.All(projection.Limits.Where(x => x.ConstraintId.StartsWith("base.")), x => Assert.Empty(x.SourceRuleCardId));
        }

        [Fact]
        public void RefreshReturnsRuleSnapshotWithoutNormalizingTheDraft()
        {
            const string ruleId = "test-snapshot-only-rule";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "snapshot-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            PlayerSelectable = true,
                            AddConstraints = new List<DeckConstraintDefinition>
                            {
                                new DeckConstraintDefinition
                                {
                                    Id = "snapshot.single-copy",
                                    Kind = "copy-count",
                                    Max = 1,
                                    Filter = new CardFilterDefinition { CardIds = new List<string> { copper.CardId } }
                                }
                            }
                        }
                    }
                };
                var deck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { ruleId, copper.CardId, copper.CardId }
                };

                var snapshot = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "refresh",
                    Deck = deck
                });

                Assert.False(snapshot.RequiresConfirmation);
                Assert.Empty(snapshot.RemovedCards);
                Assert.Equal(deck.Deck, snapshot.NormalizedDeck.Deck);
                Assert.Equal(new[] { ruleId }, snapshot.ResolvedRules.AppliedRuleCards);
                Assert.Contains(snapshot.ResolvedRules.Constraints, x => x.Id == "snapshot.single-copy");
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void CardEffectCanHandlePureDeckBuildingEventWithoutAGameInstance()
        {
            var proposal = new GwentCardDataService().AdjustDeckBuilding("99001", new DeckBuildingAdjustmentContext
            {
                Revision = 1,
                Deck = GwentDeck.CreateBasicDeck(0),
                Rule = new RuleCardDefinition { Id = "99001" }
            });

            Assert.Contains(DeckRuleEngine.StandardDeckSize, proposal.RemoveConstraintIds);
            Assert.Contains(proposal.AddConstraints, x => x.Id == "local.deck-size" && x.Max == 100);
        }

        [Fact]
        public void CardEffectDeckAdjustmentIsAuthoritativeForProjectionValidationModeAndPvpFingerprint()
        {
            const string ruleId = "99001";
            const string modeId = "test.effect-authority";
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            var tempRoot = Path.Combine(Path.GetTempPath(), "gwent-rule-authority-" + Guid.NewGuid().ToString("N"));
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    SchemaVersion = 2,
                    FeatureLevel = 2,
                    RulesetVersion = "effect-authority-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        // Deliberately leave the manifest free of deck-size changes.
                        // The authoritative change exists only in CardEffect 99001.
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            PackageVersion = "authority-1",
                            PlayerSelectable = true
                        }
                    },
                    Modes = new List<GameModeDefinition>
                    {
                        new GameModeDefinition
                        {
                            Id = modeId,
                            MatchKind = "pvp",
                            RuleMatchPolicy = "same",
                            AllowCustomRuleCards = true
                        }
                    }
                };
                var featureDirectory = Path.Combine(tempRoot, "Features");
                Directory.CreateDirectory(featureDirectory);
                File.WriteAllText(
                    Path.Combine(featureDirectory, "game-features.json"),
                    JsonConvert.SerializeObject(manifest));
                var service = new GameFeatureService(
                    new TestWebHostEnvironment { ContentRootPath = tempRoot },
                    new GwentCardDataService());
                var deck = GwentDeck.CreateBasicDeck(0);
                deck.Deck.Add(ruleId);

                var projection = service.ProjectDeckBuilding(new DeckBuildingProjectionRequest
                {
                    Revision = 41,
                    Action = "refresh",
                    Deck = deck
                });
                var resolved = service.Resolve(deck);
                var draftValidation = service.ValidateDeck(deck, false);
                Assert.True(service.TryValidateMode(modeId, deck, out var mode, out var modeValidation));

                foreach (var rules in new[]
                {
                    projection.ResolvedRules,
                    resolved,
                    draftValidation.Rules,
                    modeValidation.Rules
                })
                {
                    Assert.DoesNotContain(rules.Constraints, x => x.Id == DeckRuleEngine.StandardDeckSize);
                    Assert.Contains(rules.Constraints, x =>
                        x.Id == "local.deck-size" && x.Min == 25 && x.Max == 100);
                }
                Assert.Equal(projection.RulesFingerprint, resolved.Fingerprint);
                Assert.Equal(resolved.Fingerprint, draftValidation.Rules.Fingerprint);
                Assert.Equal(resolved.Fingerprint, modeValidation.Rules.Fingerprint);
                Assert.Equal(
                    "mode:" + modeId + ":" + resolved.Fingerprint,
                    GwentMatchs.CreateModeMatchKey(mode, modeValidation.Rules));

                var first = new ClientPlayer(new User("authority-a", "authority-a-connection"), () => null)
                {
                    Deck = deck
                };
                var second = new ClientPlayer(new User("authority-b", "authority-b-connection"), () => null)
                {
                    Deck = new DeckModel
                    {
                        Leader = deck.Leader,
                        Deck = deck.Deck.Where(x => x != ruleId).ToList()
                    }
                };
                var secondRules = service.Resolve(second.Deck);
                Assert.NotEqual(resolved.Fingerprint, secondRules.Fingerprint);
                var room = new GwentRoom(first, "effect-authority");
                room.AddPlayer(second);
                var combined = GwentMatchs.CreateCombinedRuleFingerprint(
                    room,
                    manifest.RuleCards,
                    manifest.RulesetVersion,
                    resolveDeckRules: service.Resolve);
                Assert.Equal(
                    DeckRuleEngine.CombineFingerprints(
                        manifest.RulesetVersion,
                        new[] { resolved.Fingerprint, secondRules.Fingerprint }),
                    combined);

                var swappedRoom = new GwentRoom(second, "effect-authority-swapped");
                swappedRoom.AddPlayer(first);
                Assert.Equal(
                    combined,
                    GwentMatchs.CreateCombinedRuleFingerprint(
                        swappedRoom,
                        manifest.RuleCards,
                        manifest.RulesetVersion,
                        resolveDeckRules: service.Resolve));
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
                if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
            }
        }

        [Fact]
        public void FailingDeckBuildingEffectFailsProjectionSaveAndMatchClosedWithoutThrowing()
        {
            const string ruleId = "99007";
            const string modeId = "test.effect-failure";
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            var tempRoot = Path.Combine(Path.GetTempPath(), "gwent-rule-failure-" + Guid.NewGuid().ToString("N"));
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    SchemaVersion = 2,
                    FeatureLevel = 2,
                    RulesetVersion = "effect-failure-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition { Id = ruleId, PlayerSelectable = true }
                    },
                    Modes = new List<GameModeDefinition>
                    {
                        new GameModeDefinition
                        {
                            Id = modeId,
                            MatchKind = "pvp",
                            RuleMatchPolicy = "same",
                            AllowCustomRuleCards = true
                        }
                    }
                };
                var featureDirectory = Path.Combine(tempRoot, "Features");
                Directory.CreateDirectory(featureDirectory);
                File.WriteAllText(
                    Path.Combine(featureDirectory, "game-features.json"),
                    JsonConvert.SerializeObject(manifest));
                var service = new GameFeatureService(
                    new TestWebHostEnvironment { ContentRootPath = tempRoot },
                    new GwentCardDataService());
                var deck = GwentDeck.CreateBasicDeck(0);
                deck.Deck.Add(ruleId);

                var exception = Record.Exception(() =>
                {
                    var projection = service.ProjectDeckBuilding(new DeckBuildingProjectionRequest
                    {
                        Revision = 51,
                        Action = "refresh",
                        Deck = deck
                    });
                    Assert.False(projection.IsValid);
                    Assert.Equal("rules.deck-building-effect-failed", projection.FailureCode);
                    Assert.Contains(projection.Issues, x =>
                        x.Code == "rules.deck-building-effect-failed" && x.CardId == ruleId);

                    var draftValidation = service.ValidateDeck(deck, false);
                    Assert.False(draftValidation.IsValid);
                    Assert.Contains(draftValidation.Issues, x =>
                        x.Code == "rules.deck-building-effect-failed" && x.CardId == ruleId);
                    Assert.False(GwentServerService.CanSaveDraft(service, deck));

                    Assert.False(service.TryValidateMode(modeId, deck, out _, out var modeValidation));
                    Assert.Contains(modeValidation.Issues, x =>
                        x.Code == "rules.deck-building-effect-failed" && x.CardId == ruleId);
                });
                Assert.Null(exception);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
                if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
            }
        }

        [Fact]
        public void RemovedRuleMayLeaveSavableBrokenDraftButStrictMatchStillRejectsIt()
        {
            const string ruleId = "test-broken-draft-rule";
            const string modeId = "test-broken-draft-mode";
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            var tempRoot = Path.Combine(Path.GetTempPath(), "gwent-broken-draft-" + Guid.NewGuid().ToString("N"));
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    SchemaVersion = 2,
                    FeatureLevel = 2,
                    RulesetVersion = "broken-draft-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            PlayerSelectable = true,
                            RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardCopperCopies }
                        }
                    },
                    Modes = new List<GameModeDefinition>
                    {
                        new GameModeDefinition
                        {
                            Id = modeId,
                            MatchKind = "pvp",
                            RuleMatchPolicy = "same",
                            AllowCustomRuleCards = true
                        }
                    }
                };
                var featureDirectory = Path.Combine(tempRoot, "Features");
                Directory.CreateDirectory(featureDirectory);
                File.WriteAllText(
                    Path.Combine(featureDirectory, "game-features.json"),
                    JsonConvert.SerializeObject(manifest));
                var service = new GameFeatureService(
                    new TestWebHostEnvironment { ContentRootPath = tempRoot },
                    new GwentCardDataService());
                var deck = GwentDeck.CreateBasicDeck(0);
                var copperId = deck.Deck.First(x => GwentMap.CardMap[x].Group == Group.Copper);
                while (deck.Deck.Count(x => x == copperId) < 4) deck.Deck.Add(copperId);
                deck.Deck.Add(ruleId);

                Assert.True(service.ValidateDeck(deck, false).IsValid);
                deck.Deck.Remove(ruleId);

                var brokenDraft = service.ValidateDeck(deck, false);
                Assert.False(brokenDraft.IsValid);
                Assert.Contains(brokenDraft.Issues, x =>
                    x.Code == "copies.max" && x.CardId == copperId);
                Assert.True(GwentServerService.CanSaveDraft(service, deck));
                Assert.False(service.TryValidateMode(modeId, deck, out _, out var matchValidation));
                Assert.False(matchValidation.IsComplete);
                Assert.Contains(matchValidation.Issues, x =>
                    x.Code == "copies.max" && x.CardId == copperId);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
                if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
            }
        }

        [Fact]
        public void FeatureManifestHotReloadsEnabledModesWithoutRestartingTheService()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), "gwent-feature-hot-reload-" + Guid.NewGuid().ToString("N"));
            var featureDirectory = Path.Combine(tempRoot, "Features");
            var manifestPath = Path.Combine(featureDirectory, "game-features.json");
            Directory.CreateDirectory(featureDirectory);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    SchemaVersion = 2,
                    FeatureLevel = 2,
                    RulesetVersion = "hot-reload-v1",
                    Modes = new List<GameModeDefinition>
                    {
                        new GameModeDefinition
                        {
                            Id = "pvp.casual",
                            MatchKind = "pvp",
                            RuleMatchPolicy = "same",
                            SortOrder = 10
                        },
                        new GameModeDefinition
                        {
                            Id = "challenge.feast",
                            MatchKind = "ai",
                            RuleMatchPolicy = "ignore",
                            SortOrder = 20
                        }
                    }
                };
                File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest));
                var firstWriteUtc = File.GetLastWriteTimeUtc(manifestPath);
                var service = new GameFeatureService(
                    new TestWebHostEnvironment { ContentRootPath = tempRoot },
                    new GwentCardDataService());

                Assert.Equal(
                    new[] { "pvp.casual", "challenge.feast" },
                    service.GetManifest().Modes.Select(x => x.Id));

                manifest.RulesetVersion = "hot-reload-v2";
                manifest.Modes.Single(x => x.Id == "challenge.feast").IsEnabled = false;
                File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest));
                File.SetLastWriteTimeUtc(manifestPath, firstWriteUtc.AddSeconds(1));

                var disabled = service.GetManifest();
                Assert.Equal("hot-reload-v2", disabled.RulesetVersion);
                Assert.Equal(new[] { "pvp.casual" }, disabled.Modes.Select(x => x.Id));

                manifest.RulesetVersion = "hot-reload-v3";
                manifest.Modes.Single(x => x.Id == "challenge.feast").IsEnabled = true;
                File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest));
                File.SetLastWriteTimeUtc(manifestPath, firstWriteUtc.AddSeconds(2));

                var restored = service.GetManifest();
                Assert.Equal("hot-reload-v3", restored.RulesetVersion);
                Assert.Equal(
                    new[] { "pvp.casual", "challenge.feast" },
                    restored.Modes.Select(x => x.Id));
            }
            finally
            {
                if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
            }
        }

        [Fact]
        public void ProjectionRevisionGateRejectsStaleAndMismatchedResponses()
        {
            var gate = new DeckBuildingProjectionRevisionGate();
            var first = gate.BeginRequest();
            var second = gate.BeginRequest();

            Assert.False(gate.TryAccept(first, new DeckBuildingProjection { Revision = first }));
            Assert.False(gate.TryAccept(second, new DeckBuildingProjection { Revision = first }));
            Assert.True(gate.TryAccept(second, new DeckBuildingProjection { Revision = second }));
            Assert.Equal(second, gate.LatestAccepted);
        }

        [Fact]
        public void ProjectionPreviewsRuleCleanupAndKeepsPlayerGateOutOfTheCatalog()
        {
            const string ruleId = "test-projection-rule";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = new GwentCard
            {
                CardId = ruleId,
                Name = "Projection rule",
                Group = Group.Copper,
                Faction = Faction.Neutral,
                CardType = CardType.Special,
                HideTags = new[] { HideTag.Rule },
                Categories = new Categorie[0]
            };
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "projection-v2",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            Priority = 50,
                            PlayerSelectable = true,
                            AddConstraints = new List<DeckConstraintDefinition>
                            {
                                new DeckConstraintDefinition
                                {
                                    Id = "test.one-copper",
                                    Kind = "copy-count",
                                    Max = 1,
                                    Filter = new CardFilterDefinition { CardIds = new List<string> { copper.CardId } }
                                }
                            }
                        }
                    }
                };
                var deck = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { ruleId, copper.CardId, copper.CardId }
                };
                var preview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 8,
                    Action = "add",
                    CandidateCardId = ruleId,
                    Deck = deck
                });

                Assert.True(preview.RequiresConfirmation);
                Assert.False(preview.IsValid);
                var removed = Assert.Single(preview.RemovedCards);
                Assert.Equal(copper.CardId, removed.CardId);
                Assert.Equal("test.one-copper", removed.ConstraintId);
                Assert.Equal(ruleId, removed.SourceRuleCardId);
                Assert.Contains(preview.CardStates, x => x.CardId == ruleId && x.MaxCopies == 1);

                manifest.PlayerRuleCardsEnabled = false;
                var hidden = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 9,
                    Deck = new DeckModel { Leader = leader.CardId }
                });
                Assert.DoesNotContain(hidden.CardStates, x => x.CardId == ruleId);
                Assert.Single(manifest.RuleCards);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void RuleCardStateStaysSelectableWhenItsTransitionRequiresCleanup()
        {
            const string ruleId = "test-empty-deck-rule";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "empty-transition-v1",
                    PlayerRuleCardsEnabled = true,
                    CardPools = new List<CardPoolDefinition>
                    {
                        new CardPoolDefinition { Id = "test.empty", AnyOf = new List<CardFilterDefinition>() }
                    },
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            PlayerSelectable = true,
                            RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardDeckSize },
                            RestrictToCardPools = new List<string> { "test.empty" },
                            AddConstraints = new List<DeckConstraintDefinition>
                            {
                                new DeckConstraintDefinition
                                {
                                    Id = "test.empty-size",
                                    Kind = "deck-size",
                                    Min = 0,
                                    Max = 0
                                }
                            }
                        }
                    }
                };
                var current = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { copper.CardId }
                };

                var snapshot = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 90,
                    Deck = current
                });
                var ruleState = Assert.Single(snapshot.CardStates, x => x.CardId == ruleId);
                Assert.True(ruleState.Selectable);
                Assert.Equal(1, ruleState.MaxCopies);

                var candidate = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { copper.CardId, ruleId }
                };
                var preview = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 91,
                    Action = "add",
                    CandidateCardId = ruleId,
                    Deck = candidate
                });
                Assert.True(preview.RequiresConfirmation);
                Assert.Equal(copper.CardId, Assert.Single(preview.RemovedCards).CardId);
                Assert.Equal(new[] { ruleId }, preview.NormalizedDeck.Deck);

                var confirmed = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 92,
                    Action = "add",
                    CandidateCardId = ruleId,
                    ConfirmNormalization = true,
                    Deck = candidate
                });
                Assert.True(confirmed.IsValid);
                Assert.Equal(new[] { ruleId }, confirmed.NormalizedDeck.Deck);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void ConflictingRulesNeverPreviewDestructiveDeckCleanup()
        {
            const string firstRuleId = "test-conflict-cleanup-a";
            const string secondRuleId = "test-conflict-cleanup-b";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var gold = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Gold && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var oldFirst = GwentMap.CardMap.TryGetValue(firstRuleId, out var first);
            var oldSecond = GwentMap.CardMap.TryGetValue(secondRuleId, out var second);
            GwentMap.CardMap[firstRuleId] = TestRuleCard(firstRuleId);
            GwentMap.CardMap[secondRuleId] = TestRuleCard(secondRuleId);
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "conflict-cleanup-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = firstRuleId,
                            PlayerSelectable = true,
                            ExclusiveWith = new List<string> { secondRuleId },
                            AddConstraints = new List<DeckConstraintDefinition>
                            {
                                new DeckConstraintDefinition
                                {
                                    Id = "test.conflict.copper-only",
                                    Kind = "allow-cards",
                                    Filter = new CardFilterDefinition { Groups = new List<Group> { Group.Copper } }
                                }
                            }
                        },
                        new RuleCardDefinition { Id = secondRuleId, PlayerSelectable = true }
                    }
                };
                var submitted = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { firstRuleId, secondRuleId, gold.CardId }
                };

                var projection = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 81,
                    Deck = submitted
                });

                Assert.False(projection.IsValid);
                Assert.False(projection.RequiresConfirmation);
                Assert.Empty(projection.RemovedCards);
                Assert.Equal(submitted.Deck, projection.NormalizedDeck.Deck);
                Assert.Contains(projection.Issues, x => x.Code == "rules.conflict");
            }
            finally
            {
                if (oldFirst) GwentMap.CardMap[firstRuleId] = first;
                else GwentMap.CardMap.Remove(firstRuleId);
                if (oldSecond) GwentMap.CardMap[secondRuleId] = second;
                else GwentMap.CardMap.Remove(secondRuleId);
            }
        }

        [Fact]
        public void InvalidNormalizationProposalFailsWithoutChangingTheSubmittedDeck()
        {
            const string ruleId = "test-invalid-normalization";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var copper = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = TestRuleCard(ruleId);
            try
            {
                var submitted = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string> { ruleId, copper.CardId }
                };
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "invalid-normalization-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            IsEnabled = true,
                            PlayerSelectable = true,
                            NormalizationRemovals = new List<DeckRuleTransitionRemoval>
                            {
                                new DeckRuleTransitionRemoval
                                {
                                    CardId = copper.CardId,
                                    OriginalIndex = 99,
                                    ReasonCode = "test.invalid-index"
                                }
                            }
                        }
                    }
                };

                var projection = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 91,
                    Action = "normalize",
                    Deck = submitted,
                    ConfirmNormalization = true
                });

                Assert.Equal("rules.normalization-proposal-invalid", projection.FailureCode);
                Assert.False(projection.IsValid);
                Assert.False(projection.RequiresConfirmation);
                Assert.Empty(projection.RemovedCards);
                Assert.Equal(submitted.Deck, projection.NormalizedDeck.Deck);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void EqualPriorityNormalizationConflictFailsWithoutChoosingByCardId()
        {
            const string firstRuleId = "test-normalization-conflict-a";
            const string secondRuleId = "test-normalization-conflict-b";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var ordinaryCards = GwentMap.CardMap.Values
                .Where(x => x.Group == Group.Copper && x.Faction == leader.Faction && DiyAiCardPool.IsUserDeckCard(x.CardId))
                .Take(2)
                .ToList();
            Assert.Equal(2, ordinaryCards.Count);
            var oldFirstExists = GwentMap.CardMap.TryGetValue(firstRuleId, out var oldFirst);
            var oldSecondExists = GwentMap.CardMap.TryGetValue(secondRuleId, out var oldSecond);
            GwentMap.CardMap[firstRuleId] = TestRuleCard(firstRuleId);
            GwentMap.CardMap[secondRuleId] = TestRuleCard(secondRuleId);
            try
            {
                var submitted = new DeckModel
                {
                    Leader = leader.CardId,
                    Deck = new List<string>
                    {
                        firstRuleId,
                        secondRuleId,
                        ordinaryCards[0].CardId,
                        ordinaryCards[1].CardId
                    }
                };
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "normalization-conflict-v1",
                    PlayerRuleCardsEnabled = true,
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = firstRuleId,
                            IsEnabled = true,
                            PlayerSelectable = true,
                            Priority = 10,
                            NormalizationRemovals = new List<DeckRuleTransitionRemoval>
                            {
                                new DeckRuleTransitionRemoval { CardId = ordinaryCards[0].CardId, OriginalIndex = 0 }
                            }
                        },
                        new RuleCardDefinition
                        {
                            Id = secondRuleId,
                            IsEnabled = true,
                            PlayerSelectable = true,
                            Priority = 10,
                            NormalizationRemovals = new List<DeckRuleTransitionRemoval>
                            {
                                new DeckRuleTransitionRemoval { CardId = ordinaryCards[1].CardId, OriginalIndex = 0 }
                            }
                        }
                    }
                };

                var projection = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 92,
                    Action = "normalize",
                    Deck = submitted,
                    ConfirmNormalization = true
                });

                Assert.Equal("rules.normalization-conflict", projection.FailureCode);
                Assert.False(projection.IsValid);
                Assert.Empty(projection.RemovedCards);
                Assert.Equal(submitted.Deck, projection.NormalizedDeck.Deck);
            }
            finally
            {
                if (oldFirstExists) GwentMap.CardMap[firstRuleId] = oldFirst;
                else GwentMap.CardMap.Remove(firstRuleId);
                if (oldSecondExists) GwentMap.CardMap[secondRuleId] = oldSecond;
                else GwentMap.CardMap.Remove(secondRuleId);
            }
        }

        private static GwentCard TestRuleCard(string id) => new GwentCard
        {
            CardId = id,
            Name = id,
            Group = Group.Copper,
            Faction = Faction.Neutral,
            CardType = CardType.Special,
            HideTags = new[] { HideTag.Rule },
            Categories = new Categorie[0]
        };

        [Fact]
        public void EmptyOrdinaryDeckAndExplicitlyEmptyPoolCanBeComplete()
        {
            const string ruleId = "test-empty-deck-rule";
            var leader = GwentMap.CardMap.Values.First(x =>
                x.Group == Group.Leader && x.Faction != Faction.Neutral && DiyAiCardPool.IsUserDeckCard(x.CardId));
            var existed = GwentMap.CardMap.TryGetValue(ruleId, out var old);
            GwentMap.CardMap[ruleId] = new GwentCard
            {
                CardId = ruleId,
                Name = "Empty deck rule",
                Group = Group.Copper,
                Faction = Faction.Neutral,
                CardType = CardType.Special,
                HideTags = new[] { HideTag.Rule },
                Categories = new Categorie[0]
            };
            try
            {
                var manifest = new GameFeatureManifest
                {
                    RulesetVersion = "empty-v1",
                    PlayerRuleCardsEnabled = true,
                    CardPools = new List<CardPoolDefinition>
                    {
                        new CardPoolDefinition { Id = "empty", AnyOf = new List<CardFilterDefinition>() }
                    },
                    RuleCards = new List<RuleCardDefinition>
                    {
                        new RuleCardDefinition
                        {
                            Id = ruleId,
                            PlayerSelectable = true,
                            RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardDeckSize },
                            RestrictToCardPools = new List<string> { "empty" },
                            AddConstraints = new List<DeckConstraintDefinition>
                            {
                                new DeckConstraintDefinition { Id = "test.empty.size", Kind = "deck-size", Min = 0, Max = 0 }
                            }
                        }
                    }
                };
                var projection = DeckBuildingProjectionEngine.Project(manifest, new DeckBuildingProjectionRequest
                {
                    Revision = 10,
                    Deck = new DeckModel { Leader = leader.CardId, Deck = new List<string> { ruleId } }
                });

                Assert.True(projection.IsValid);
                Assert.True(projection.IsComplete);
                Assert.DoesNotContain(projection.NormalizedDeck.Deck, x => !DeckRuleEngine.IsRuleCard(x));
                Assert.DoesNotContain(projection.CardStates, x => !DeckRuleEngine.IsRuleCard(x.CardId) && x.Selectable);
            }
            finally
            {
                if (existed) GwentMap.CardMap[ruleId] = old;
                else GwentMap.CardMap.Remove(ruleId);
            }
        }

        [Fact]
        public void RuleCardsAreDeduplicatedAndRemovedFromTheDrawDeckAtGameCreation()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            var player2OnlyRuleId = player2.Deck.Deck.First(x => !player1.Deck.Deck.Contains(x));
            player1.Deck.Deck.Add(ruleId);
            player2.Deck.Deck.Add(ruleId);

            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                _ => { },
                false,
                cardId => cardId == ruleId || cardId == player2OnlyRuleId);

            Assert.Equal(2, game.GameRules.Count);
            Assert.DoesNotContain(game.PlayersDeck.SelectMany(x => x), x => x.Status.CardId == ruleId);
            Assert.All(game.GameRules, x => Assert.Equal(RowPosition.Rule, x.Status.CardRow));
            Assert.All(game.GameRules, x => Assert.DoesNotContain(x, game.GetAllCard(game.Player1Index)));
            var playerInfo = game.GetCardsInfo(TwoPlayer.Player1);
            Assert.Equal(2, playerInfo.Rules.Count());
            var shared = Assert.Single(playerInfo.RuleSources, x => x.CardId == ruleId);
            Assert.True(shared.MyPlayerUses);
            Assert.True(shared.EnemyPlayerUses);
            var enemyOnly = Assert.Single(playerInfo.RuleSources, x => x.CardId == player2OnlyRuleId);
            Assert.False(enemyOnly.MyPlayerUses);
            Assert.True(enemyOnly.EnemyPlayerUses);
            Assert.Equal(2, game.GetCardsInfoForSpectator().Rules.Count());
        }

        [Fact]
        public void DisabledRuntimeRuleIsRemovedFromDrawDeckWithoutEnteringRuleZone()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var disabledRuleId = player1.Deck.Deck.First();

            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                _ => { },
                false,
                cardId => cardId == disabledRuleId,
                activeRuleCardIds: new string[0]);

            Assert.Empty(game.GameRules);
            Assert.DoesNotContain(
                game.PlayersDeck.SelectMany(x => x),
                x => x.Status.CardId == disabledRuleId);
            Assert.Empty(game.GetCardsInfo(TwoPlayer.Player1).RuleSources);
            Assert.Empty(game.GetCardsInfoForSpectator().Rules);
        }

        [Fact]
        public async Task RuleZoneEffectsReceiveTheNormalGameEventStream()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                _ => { },
                false,
                cardId => cardId == ruleId);

            foreach (var card in game.GetAllCard(game.Player1Index, true, true))
                card.Effects.Clear();
            game.GameRules[0].Effects.Clear();
            var probe = new RuleEventProbe(game.GameRules[0]);
            game.GameRules[0].Effects.Add(probe);

            await game.SendEvent(new OnGameStart());

            Assert.Equal(1, probe.Triggered);
        }

        [Fact]
        public async Task OrdinaryCardQueriesExcludeRuleZoneWhileEventsStillReachRules()
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                _ => { },
                false,
                cardId => cardId == ruleId);
            var rule = Assert.Single(game.GameRules);

            Assert.DoesNotContain(rule, game.GetAllCard(game.Player1Index, true, true));
            Assert.Contains(rule, game.RowToList(game.Player1Index, RowPosition.Rule));

            foreach (var card in game.GetAllCard(game.Player1Index, true, true))
                card.Effects.Clear();
            rule.Effects.Clear();
            var probe = new RuleEventProbe(rule);
            rule.Effects.Add(probe);

            await game.SendEvent(new OnGameStart());

            Assert.Equal(1, probe.Triggered);
        }

        [Fact]
        public async Task EmptyDeckRuleCanPopulateFortyDistinctGoldCardsDeterministicallyAtGameStart()
        {
            var candidates = GwentMap.CardMap.Values
                .Where(x => x.Group == Group.Gold && !DeckRuleEngine.IsRuleCard(x.CardId))
                .OrderBy(x => x.CardId, StringComparer.Ordinal)
                .GroupBy(x => x.Name, StringComparer.Ordinal)
                .Select(x => x.First().CardId)
                .ToList();
            Assert.True(candidates.Count >= 40, "The production card pool must contain enough uniquely named gold cards for this test.");

            var firstSetup = CreateEmptyDeckPopulationGame(741852);
            var secondSetup = CreateEmptyDeckPopulationGame(741852);
            var first = firstSetup.Game;
            var second = secondSetup.Game;

            await RunOnlyRuleStartEffect(first, new RandomGoldPopulationProbe(first.GameRules.Single(), candidates));
            await RunOnlyRuleStartEffect(second, new RandomGoldPopulationProbe(second.GameRules.Single(), candidates));

            Assert.Equal(firstSetup.RuleId, secondSetup.RuleId);
            Assert.Equal(40, first.PlayersDeck[first.Player1Index].Count);
            Assert.DoesNotContain(first.PlayersDeck[first.Player2Index], x => x.Status.CardId == firstSetup.RuleId);
            Assert.All(first.PlayersDeck[first.Player1Index], x => Assert.Equal(Group.Gold, x.Status.Group));
            Assert.Equal(40, first.PlayersDeck[first.Player1Index]
                .Select(x => GwentMap.CardMap[x.Status.CardId].Name)
                .Distinct(StringComparer.Ordinal).Count());
            Assert.Equal(
                first.PlayersDeck[first.Player1Index].Select(x => x.Status.CardId),
                second.PlayersDeck[second.Player1Index].Select(x => x.Status.CardId));
        }

        [Fact]
        public void EmptyDeckPopulationFailsAtomicallyWhenDistinctCandidatesAreInsufficient()
        {
            var setup = CreateEmptyDeckPopulationGame(963258);
            var game = setup.Game;
            var ruleId = setup.RuleId;
            var candidates = GwentMap.CardMap.Values
                .Where(x => x.Group == Group.Gold && !DeckRuleEngine.IsRuleCard(x.CardId))
                .OrderBy(x => x.CardId, StringComparer.Ordinal)
                .GroupBy(x => x.Name, StringComparer.Ordinal)
                .Select(x => x.First().CardId)
                .Take(39)
                .ToList();

            Action action = () => game.PopulateDeckToCountDistinctRandom(
                game.Player1Index, candidates, 40, ruleId);
            var error = Assert.Throws<RuleDeckPopulationException>(action);

            Assert.Equal(ruleId, error.SourceRuleCardId);
            Assert.Equal(39, error.AvailableCandidateCount);
            Assert.Empty(game.PlayersDeck[game.Player1Index]);
        }

        [Fact]
        public async Task DynamicMarkersAndResourcesRoundTripThroughAuthoritativeGameSnapshots()
        {
            var markerDefinitions = new[]
            {
                new DynamicCardMarkerDefinition
                {
                    Id = "test.poison",
                    ShortLabel = "毒",
                    StyleToken = "venom",
                    ValueDisplay = "stack",
                    AllowMultipleInstances = false,
                    Priority = 5
                },
                new DynamicCardMarkerDefinition
                {
                    Id = "test.runes",
                    ShortLabel = "符",
                    StyleToken = "arcane",
                    ValueDisplay = "number",
                    AllowMultipleInstances = true,
                    Priority = 10
                }
            };
            var resourceDefinitions = new[]
            {
                new GameResourceDefinition
                {
                    Id = "test.coins",
                    ShortLabel = "币",
                    StyleToken = "amber",
                    ValueFormat = "fraction",
                    Priority = 2
                }
            };
            var game = new GwentServerGame(
                new GeraltNovaAI(),
                new SoldierTrainAI(),
                new GwentCardDataService(),
                _ => { },
                cardMarkerDefinitions: markerDefinitions,
                resourceDefinitions: resourceDefinitions);
            var card = game.PlayersDeck[game.Player1Index].First();

            await game.SetCardMarker(card, "test.poison", 2);
            await game.SetCardMarker(card, "test.poison", 3);
            await game.SetCardMarker(card, "test.runes", 1, "left");
            await game.SetCardMarker(card, "test.runes", 4, "right");
            await game.SetCardMarker(card, "test.unknown-marker", 7);
            await game.SetResource(game.Player1Index, "test.coins", 5, 0, 9);
            await game.AddResource(game.Player1Index, "test.coins", 10, 0, 9);
            await game.SetResource(game.Player1Index, "test.unknown-resource", 1);

            Assert.Equal(4, card.Status.DynamicMarkers.Count);
            Assert.Equal(3, Assert.Single(card.Status.DynamicMarkers, x => x.DefinitionId == "test.poison").Value);
            Assert.Equal(7, Assert.Single(card.Status.DynamicMarkers,
                x => x.DefinitionId == "test.unknown-marker").Value);
            Assert.Equal(new[] { "left", "right" }, card.Status.DynamicMarkers
                .Where(x => x.DefinitionId == "test.runes").Select(x => x.InstanceId).OrderBy(x => x));
            var player1Info = game.GetGameInfo(TwoPlayer.Player1);
            var player2Info = game.GetGameInfo(TwoPlayer.Player2);
            Assert.Equal(2, player1Info.MyResources.Count());
            Assert.Equal(9, Assert.Single(player1Info.MyResources,
                x => x.DefinitionId == "test.coins").Value);
            Assert.Equal(1, Assert.Single(player1Info.MyResources,
                x => x.DefinitionId == "test.unknown-resource").Value);
            Assert.Equal(9, Assert.Single(player2Info.EnemyResources,
                x => x.DefinitionId == "test.coins").Value);
            Assert.Empty(player1Info.EnemyResources);

            var wireOperation = Operation.Create(
                ServerOperationType.SetCard,
                new CardLocation { RowPosition = RowPosition.MyHand, CardIndex = 0 },
                card.Status);
            var wireArguments = wireOperation.Arguments.ToArray();
            var restoredStatus = wireArguments[1].ToType<CardStatus>();
            Assert.Equal(4, restoredStatus.DynamicMarkers.Count);
            Assert.Equal(3, Assert.Single(restoredStatus.DynamicMarkers,
                x => x.DefinitionId == "test.poison").Value);

            await game.RemoveCardMarker(card, "test.runes", "left");
            Assert.DoesNotContain(card.Status.DynamicMarkers, x => x.InstanceId == "left");
        }

        private static (GwentServerGame Game, string RuleId) CreateEmptyDeckPopulationGame(int seed)
        {
            var player1 = new GeraltNovaAI();
            var player2 = new SoldierTrainAI();
            var ruleId = player1.Deck.Deck.First();
            player1.Deck.Deck = new List<string> { ruleId };
            var game = new GwentServerGame(
                player1,
                player2,
                new GwentCardDataService(),
                _ => { },
                false,
                cardId => cardId == ruleId,
                randomSeed: seed);
            return (game, ruleId);
        }

        private static async Task RunOnlyRuleStartEffect(GwentServerGame game, CardEffect effect)
        {
            foreach (var card in game.GetAllCard(game.Player1Index, true, true))
                card.Effects.Clear();
            game.GameRules.Single().Effects.Clear();
            game.GameRules.Single().Effects.Add(effect);
            await game.SendEvent(new OnGameStart());
        }

        private sealed class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string ApplicationName { get; set; } = "Cynthia.Card.Server.Tests";
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
            public string WebRootPath { get; set; } = "";
            public string EnvironmentName { get; set; } = "Test";
            public string ContentRootPath { get; set; } = "";
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }

        private sealed class RuleEventProbe : CardEffect, IHandlesEvent<OnGameStart>
        {
            public int Triggered { get; private set; }
            public RuleEventProbe(GameCard card) : base(card) { }
            public Task HandleEvent(OnGameStart @event)
            {
                Triggered++;
                return Task.CompletedTask;
            }
        }

        private sealed class RandomGoldPopulationProbe : CardEffect, IHandlesEvent<OnGameStart>
        {
            private readonly IReadOnlyCollection<string> _candidates;

            public RandomGoldPopulationProbe(GameCard card, IReadOnlyCollection<string> candidates) : base(card)
            {
                _candidates = candidates;
            }

            public Task HandleEvent(OnGameStart @event)
            {
                foreach (var playerIndex in Game.GetRulePlayerIndexes(Card.Status.CardId))
                    Game.PopulateDeckToCountDistinctRandom(
                        playerIndex,
                        _candidates,
                        40,
                        Card.Status.CardId,
                        RuleDeckInsufficientPolicy.FailMatch);
                return Task.CompletedTask;
            }
        }
    }
}
