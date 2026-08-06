using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cynthia.Card
{
    public static class DeckRuleEngine
    {
        public const string StandardDeckSize = "base.deck-size";
        public const string StandardFaction = "base.faction";
        public const string StandardGoldCount = "base.gold-count";
        public const string StandardSilverCount = "base.silver-count";
        public const string StandardGoldCopies = "base.gold-copies";
        public const string StandardSilverCopies = "base.silver-copies";
        public const string StandardCopperCopies = "base.copper-copies";
        public const string StandardNoLeader = "base.no-leader-in-deck";
        public const string ScopeDeckLimits = "deck-limits";
        public const string ScopeGroupLimits = "group-limits";
        public const string ScopeCardLimits = "card-limits";
        public const string ScopeCardPool = "card-pool";
        public const string ScopeLeader = "leader";
        public const string ScopeNormalization = "normalization";
        public const string ScopeAll = "*";

        public static List<DeckConstraintDefinition> StandardConstraints() => new List<DeckConstraintDefinition>
        {
            Constraint(StandardDeckSize, "deck-size", null, 25, 40),
            Constraint(StandardFaction, "allow-cards", new CardFilterDefinition { LeaderFactionOrNeutral = true }),
            Constraint(StandardGoldCount, "card-count", Groups(Group.Gold), null, 4),
            Constraint(StandardSilverCount, "card-count", Groups(Group.Silver), null, 6),
            Constraint(StandardGoldCopies, "copy-count", Groups(Group.Gold), null, 1),
            Constraint(StandardSilverCopies, "copy-count", Groups(Group.Silver), null, 1),
            Constraint(StandardCopperCopies, "copy-count", Groups(Group.Copper), null, 3),
            Constraint(StandardNoLeader, "deny-cards", Groups(Group.Leader))
        };

        public static ResolvedDeckRuleSet Resolve(
            IEnumerable<RuleCardDefinition> definitions,
            IEnumerable<string> selectedRuleCards,
            string rulesetVersion = "",
            IEnumerable<CardPoolDefinition> cardPools = null)
        {
            var result = new ResolvedDeckRuleSet { RulesetVersion = rulesetVersion ?? "" };
            var catalog = (definitions ?? Enumerable.Empty<RuleCardDefinition>())
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
            result.AppliedRuleCards = (selectedRuleCards ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();

            var selected = new HashSet<string>(result.AppliedRuleCards, StringComparer.Ordinal);
            var reportedConflicts = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in result.AppliedRuleCards)
            {
                if (!catalog.TryGetValue(id, out var definition))
                {
                    result.ResolutionIssues.Add(Issue("rules.unknown", cardId: id));
                    continue;
                }
                if (!definition.IsEnabled)
                    result.ResolutionIssues.Add(Issue("rules.disabled", cardId: id));
                foreach (var required in definition.RequiresAll ?? new List<string>())
                    if (!selected.Contains(required))
                        result.ResolutionIssues.Add(Issue("rules.requires", cardId: id, relatedId: required));
                foreach (var exclusive in definition.ExclusiveWith ?? new List<string>())
                {
                    var pair = string.CompareOrdinal(id, exclusive) < 0 ? id + "\n" + exclusive : exclusive + "\n" + id;
                    if (selected.Contains(exclusive) && reportedConflicts.Add(pair))
                        result.ResolutionIssues.Add(Issue("rules.conflict", cardId: id, relatedId: exclusive));
                }
            }

            var activeDefinitions = result.AppliedRuleCards
                .Where(catalog.ContainsKey)
                .Select(x => catalog[x])
                .Where(x => x.IsEnabled)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Id, StringComparer.Ordinal)
                .ToList();
            result.ExecutionOrder = activeDefinitions.Select(x => new ResolvedRuleCardExecution
            {
                RuleCardId = x.Id,
                PackageVersion = x.PackageVersion ?? "",
                Priority = x.Priority,
                OverrideScopes = (x.OverrideScopes ?? new List<string>()).Distinct(StringComparer.Ordinal).OrderBy(y => y, StringComparer.Ordinal).ToList()
            }).ToList();
            var poolCatalog = (cardPools ?? Enumerable.Empty<CardPoolDefinition>())
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
            foreach (var definition in activeDefinitions)
            {
                if (IsSuppressedByHigherPriority(definition, ScopeCardPool, activeDefinitions)) continue;
                var modifier = new ResolvedCardPoolModifier { RuleCardId = definition.Id, Priority = definition.Priority };
                ResolvePools(definition.AddCardPools, poolCatalog, modifier.Add, result, definition.Id);
                ResolvePools(definition.RestrictToCardPools, poolCatalog, modifier.RestrictTo, result, definition.Id);
                ResolvePools(definition.ExcludeCardPools, poolCatalog, modifier.Exclude, result, definition.Id);
                if (modifier.Add.Count > 0 || modifier.RestrictTo.Count > 0 || modifier.Exclude.Count > 0)
                    result.CardPoolModifiers.Add(modifier);
            }
            var operations = StandardConstraints()
                .Select(x => new ConstraintOperation { Constraint = x, Priority = int.MinValue, SourceId = "base" })
                .ToList();
            foreach (var definition in activeDefinitions)
            {
                foreach (var removedId in definition.RemoveConstraintIds ?? new List<string>())
                {
                    var scope = ConstraintScopeForId(removedId, activeDefinitions);
                    if (!IsSuppressedByHigherPriority(definition, scope, activeDefinitions))
                        operations.Add(new ConstraintOperation { ConstraintId = removedId, IsRemoval = true, Priority = definition.Priority, SourceId = definition.Id });
                }
                if ((definition.AllowedLeaderFactions?.Count ?? 0) > 0 &&
                    !IsSuppressedByHigherPriority(definition, ScopeLeader, activeDefinitions))
                {
                    operations.Add(new ConstraintOperation
                    {
                        Constraint = new DeckConstraintDefinition
                        {
                            Id = "rule." + definition.Id + ".leader-faction",
                            Kind = "leader-faction",
                            Filter = new CardFilterDefinition { Factions = definition.AllowedLeaderFactions.Distinct().ToList() }
                        },
                        Priority = definition.Priority,
                        SourceId = definition.Id
                    });
                }
                foreach (var constraint in definition.AddConstraints ?? new List<DeckConstraintDefinition>())
                {
                    if (constraint == null || string.IsNullOrWhiteSpace(constraint.Id) || string.IsNullOrWhiteSpace(constraint.Kind))
                    {
                        result.ResolutionIssues.Add(Issue("rules.constraint-invalid", cardId: definition.Id));
                        continue;
                    }
                    if (IsSuppressedByHigherPriority(definition, ConstraintScope(constraint), activeDefinitions)) continue;
                    operations.Add(new ConstraintOperation { Constraint = constraint, Priority = definition.Priority, SourceId = definition.Id });
                }
            }
            var constraints = new List<DeckConstraintDefinition>();
            var constraintSources = new List<ResolvedConstraintSource>();
            foreach (var group in operations.GroupBy(x => x.Id, StringComparer.Ordinal))
            {
                var topPriority = group.Max(x => x.Priority);
                var winners = group.Where(x => x.Priority == topPriority).OrderBy(x => x.SourceId, StringComparer.Ordinal).ToList();
                var additions = winners.Where(x => !x.IsRemoval).ToList();
                if (winners.Any(x => x.IsRemoval) && additions.Count > 0)
                {
                    result.ResolutionIssues.Add(Issue("rules.priority-conflict", constraintId: group.Key));
                    continue;
                }
                if (additions.Select(x => ConstraintCanonical(x.Constraint)).Distinct(StringComparer.Ordinal).Count() > 1)
                {
                    result.ResolutionIssues.Add(Issue("rules.priority-conflict", constraintId: group.Key));
                    continue;
                }
                if (additions.Count > 0)
                {
                    constraints.Add(additions[0].Constraint);
                    constraintSources.Add(new ResolvedConstraintSource
                    {
                        ConstraintId = additions[0].Constraint.Id,
                        SourceRuleCardId = additions[0].SourceId == "base" ? "" : additions[0].SourceId,
                        Priority = additions[0].Priority
                    });
                }
            }
            if (result.CardPoolModifiers.Count > 0)
                constraints.RemoveAll(x => x.Id == StandardFaction);
            result.Constraints = constraints.OrderBy(x => x.Id, StringComparer.Ordinal).ToList();
            result.ConstraintSources = constraintSources
                .Where(x => result.Constraints.Any(y => y.Id == x.ConstraintId))
                .OrderBy(x => x.ConstraintId, StringComparer.Ordinal)
                .ToList();
            result.Fingerprint = Fingerprint(result);
            return result;
        }

        public static DeckValidationResult Validate(DeckModel deck, ResolvedDeckRuleSet rules, bool requireComplete)
        {
            var result = new DeckValidationResult { Rules = rules ?? new ResolvedDeckRuleSet() };
            result.Issues.AddRange(result.Rules.ResolutionIssues ?? new List<DeckValidationIssue>());
            if (deck == null || deck.Deck == null || string.IsNullOrWhiteSpace(deck.Leader))
            {
                result.Issues.Add(Issue("deck.missing"));
                return result;
            }

            var ruleCards = deck.Deck.Where(IsRuleCard).ToList();
            if (ruleCards.Count != ruleCards.Distinct(StringComparer.Ordinal).Count())
                result.Issues.Add(Issue("rules.duplicate"));
            if (!ruleCards.Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(result.Rules.AppliedRuleCards))
                result.Issues.Add(Issue("rules.selection-mismatch"));

            if (!DiyAiCardPool.IsUserDeckCard(deck.Leader) ||
                !GwentMap.CardMap.TryGetValue(deck.Leader, out var leader) || leader.Group != Group.Leader)
            {
                result.Issues.Add(Issue("leader.invalid", cardId: deck.Leader));
                return result;
            }

            foreach (var cardId in deck.Deck.Where(x => !IsKnownDeckCard(x, result.Rules, leader)).Distinct())
                result.Issues.Add(Issue("card.invalid", cardId: cardId));
            if (result.Issues.Any(x => x.Code == "card.invalid"))
                return result;

            var cards = deck.Deck.Where(x => !IsRuleCard(x)).Select(x => GwentMap.CardMap[x]).ToList();
            if ((result.Rules.CardPoolModifiers?.Count ?? 0) > 0)
                foreach (var card in cards.Where(x => !IsCardAllowedByPool(x, leader, result.Rules)))
                    result.Issues.Add(Issue("card.not-allowed", "rules.card-pool", card.CardId));
            foreach (var constraint in result.Rules.Constraints ?? new List<DeckConstraintDefinition>())
                ValidateConstraint(result, constraint, cards, leader, requireComplete);

            var nonMinimumIssues = result.Issues.Where(x => x.Code != "count.min").ToList();
            result.IsValid = nonMinimumIssues.Count == 0 && (!requireComplete || result.Issues.Count == 0);
            result.IsComplete = result.Issues.Count == 0 && MinimumsSatisfied(result.Rules.Constraints, cards, leader);
            return result;
        }

        public static DeckValidationResult ValidateMode(
            DeckModel deck,
            GameFeatureManifest manifest,
            GameModeDefinition mode,
            bool requireComplete)
        {
            manifest = manifest ?? new GameFeatureManifest();
            var selectedRuleCards = (deck?.Deck ?? new List<string>())
                .Where(IsRuleCard)
                .Distinct(StringComparer.Ordinal);
            var rules = Resolve(manifest.RuleCards, selectedRuleCards, manifest.RulesetVersion, manifest.CardPools);
            var result = Validate(deck, rules, requireComplete);
            if (mode == null || !mode.IsEnabled)
            {
                result.Issues.Add(Issue("mode.unknown", relatedId: mode?.Id));
                result.IsValid = false;
                result.IsComplete = false;
                return result;
            }

            var selected = new HashSet<string>(rules.AppliedRuleCards, StringComparer.Ordinal);
            foreach (var required in mode.RequiredRuleCards ?? new List<string>())
                if (!selected.Contains(required))
                    result.Issues.Add(Issue("mode.rule-required", cardId: required, relatedId: mode.Id));

            if (!mode.AllowCustomRuleCards)
            {
                var required = new HashSet<string>(mode.RequiredRuleCards ?? new List<string>(), StringComparer.Ordinal);
                foreach (var unexpected in selected.Where(x => !required.Contains(x)))
                    result.Issues.Add(Issue("mode.rule-not-allowed", cardId: unexpected, relatedId: mode.Id));
            }
            else if ((mode.AllowedRuleCards?.Count ?? 0) > 0)
            {
                var allowed = new HashSet<string>(mode.AllowedRuleCards, StringComparer.Ordinal);
                foreach (var unexpected in selected.Where(x => !allowed.Contains(x)))
                    result.Issues.Add(Issue("mode.rule-not-allowed", cardId: unexpected, relatedId: mode.Id));
            }

            var completeBeforeMode = result.IsComplete;
            result.IsValid = result.Issues.Count == 0 || (!requireComplete && result.Issues.All(x => x.Code == "count.min"));
            result.IsComplete = completeBeforeMode && result.Issues.Count == 0;
            return result;
        }

        public static bool CanPlayerSelectRuleDeck(GameFeatureManifest manifest, DeckModel deck)
        {
            var definitions = (manifest?.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null)
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
            var selected = (deck?.Deck ?? new List<string>())
                .Where(x => IsRuleCard(x) || definitions.ContainsKey(x))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            if (selected.Count == 0) return true;
            if (manifest?.PlayerRuleCardsEnabled != true) return false;
            return selected.All(x => definitions.TryGetValue(x, out var definition) &&
                                     definition.IsEnabled &&
                                     definition.PlayerSelectable);
        }

        public static bool CanAddCard(DeckModel deck, string cardId, ResolvedDeckRuleSet rules)
        {
            if (deck == null || string.IsNullOrWhiteSpace(cardId)) return false;
            var copy = CloneDeck(deck);
            copy.Deck.Add(cardId);
            return Validate(copy, rules, false).IsValid;
        }

        // Produces a deterministic preview. It never mutates the supplied deck.
        // Earlier copies win; invalid/excess cards are removed from the tailward additions.
        public static DeckRuleTransitionPlan PlanTransition(DeckModel deck, ResolvedDeckRuleSet newRules)
        {
            var plan = new DeckRuleTransitionPlan { ResultDeck = CloneDeck(deck) };
            plan.ResultDeck.Deck = newRules?.AppliedRuleCards?.Distinct(StringComparer.Ordinal).ToList() ?? new List<string>();

            var original = (deck?.Deck ?? new List<string>()).Where(x => !IsRuleCard(x)).ToList();
            for (var index = 0; index < original.Count; index++)
            {
                var cardId = original[index];
                var candidate = CloneDeck(plan.ResultDeck);
                candidate.Deck.Add(cardId);
                var validation = Validate(candidate, newRules, false);
                if (validation.IsValid)
                {
                    plan.ResultDeck.Deck.Add(cardId);
                    continue;
                }
                var issue = validation.Issues.LastOrDefault() ?? Issue("card.invalid", cardId: cardId);
                plan.RemovedCards.Add(new DeckRuleTransitionRemoval
                {
                    CardId = cardId,
                    OriginalIndex = index,
                    ReasonCode = issue.Code,
                    ConstraintId = issue.ConstraintId
                });
            }
            plan.Validation = Validate(plan.ResultDeck, newRules, false);
            return plan;
        }

        public static bool Matches(CardFilterDefinition filter, GwentCard card, GwentCard leader)
        {
            filter = filter ?? new CardFilterDefinition();
            if ((filter.CardIds?.Count ?? 0) > 0 && !filter.CardIds.Contains(card.CardId)) return false;
            if ((filter.Groups?.Count ?? 0) > 0 && !filter.Groups.Contains(card.Group)) return false;
            if ((filter.Factions?.Count ?? 0) > 0 && !filter.Factions.Contains(card.Faction)) return false;
            if ((filter.CardTypes?.Count ?? 0) > 0 && !filter.CardTypes.Contains(card.CardType)) return false;
            var categories = card.Categories ?? new Categorie[0];
            if ((filter.AnyCategories?.Count ?? 0) > 0 && !categories.Intersect(filter.AnyCategories).Any()) return false;
            if ((filter.AllCategories?.Count ?? 0) > 0 && filter.AllCategories.Any(x => !categories.Contains(x))) return false;
            if (filter.LeaderFactionOrNeutral && card.Faction != Faction.Neutral && card.Faction != leader.Faction) return false;
            return true;
        }

        public static bool IsCardAllowedByPool(GwentCard card, GwentCard leader, ResolvedDeckRuleSet rules)
        {
            if (string.IsNullOrWhiteSpace(card.CardId) || string.IsNullOrWhiteSpace(leader.CardId) ||
                IsRuleCard(card.CardId) || card.Group == Group.Leader) return false;
            var modifiers = rules?.CardPoolModifiers ?? new List<ResolvedCardPoolModifier>();
            var allowed = DiyAiCardPool.IsUserDeckCard(card.CardId) &&
                (card.Faction == Faction.Neutral || card.Faction == leader.Faction);
            if (modifiers.SelectMany(x => x.Add ?? new List<CardPoolDefinition>()).Any(x => PoolMatches(x, card, leader)))
                allowed = true;
            foreach (var restriction in modifiers.Where(x => (x.RestrictTo?.Count ?? 0) > 0))
                allowed = allowed && restriction.RestrictTo.Any(x => PoolMatches(x, card, leader));
            if (modifiers.SelectMany(x => x.Exclude ?? new List<CardPoolDefinition>()).Any(x => PoolMatches(x, card, leader)))
                allowed = false;
            return allowed;
        }

        public static bool IsCardSelectable(GwentCard card, GwentCard leader, ResolvedDeckRuleSet rules)
        {
            if (!IsCardAllowedByPool(card, leader, rules)) return false;
            foreach (var constraint in rules?.Constraints ?? new List<DeckConstraintDefinition>())
            {
                var kind = (constraint.Kind ?? "").ToLowerInvariant();
                if (kind == "allow-cards" && !Matches(constraint.Filter, card, leader)) return false;
                if (kind == "deny-cards" && Matches(constraint.Filter, card, leader)) return false;
            }
            return true;
        }

        public static string Fingerprint(ResolvedDeckRuleSet rules)
        {
            var lines = new List<string> { "v=" + (rules?.RulesetVersion ?? "") };
            lines.AddRange((rules?.AppliedRuleCards ?? new List<string>()).OrderBy(x => x).Select(x => "r=" + x));
            foreach (var execution in rules?.ExecutionOrder ?? new List<ResolvedRuleCardExecution>())
                lines.Add("x=" + execution.RuleCardId + "|" + execution.PackageVersion + "|" + execution.Priority + "|" +
                    string.Join(",", execution.OverrideScopes ?? new List<string>()));
            foreach (var c in (rules?.Constraints ?? new List<DeckConstraintDefinition>()).OrderBy(x => x.Id))
                lines.Add("c=" + ConstraintCanonical(c));
            foreach (var modifier in (rules?.CardPoolModifiers ?? new List<ResolvedCardPoolModifier>()).OrderBy(x => x.RuleCardId))
                lines.Add("p=" + PoolModifierCanonical(modifier));
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", lines))))
                    .Replace("-", "").Substring(0, 20).ToLowerInvariant();
        }

        private static void ValidateConstraint(DeckValidationResult result, DeckConstraintDefinition constraint, List<GwentCard> cards, GwentCard leader, bool requireComplete)
        {
            var filter = constraint.Filter ?? new CardFilterDefinition();
            switch ((constraint.Kind ?? "").ToLowerInvariant())
            {
                case "deck-size":
                    AddBounds(result, constraint, cards.Count, requireComplete);
                    break;
                case "card-count":
                    AddBounds(result, constraint, cards.Count(x => Matches(filter, x, leader)), requireComplete);
                    break;
                case "copy-count":
                    foreach (var group in cards.Where(x => Matches(filter, x, leader)).GroupBy(x => x.CardId))
                        if (constraint.Max.HasValue && group.Count() > constraint.Max.Value)
                            result.Issues.Add(Issue("copies.max", constraint.Id, group.Key, current: group.Count(), limit: constraint.Max.Value));
                    break;
                case "allow-cards":
                    foreach (var card in cards.Where(x => !Matches(filter, x, leader)))
                        result.Issues.Add(Issue("card.not-allowed", constraint.Id, card.CardId));
                    break;
                case "deny-cards":
                    foreach (var card in cards.Where(x => Matches(filter, x, leader)))
                        result.Issues.Add(Issue("card.denied", constraint.Id, card.CardId));
                    break;
                case "leader-faction":
                    if ((filter.Factions?.Count ?? 0) > 0 && !filter.Factions.Contains(leader.Faction))
                        result.Issues.Add(Issue("rules.faction", constraint.Id));
                    break;
                default:
                    result.Issues.Add(Issue("rules.constraint-kind-unknown", constraint.Id));
                    break;
            }
        }

        private static void AddBounds(DeckValidationResult result, DeckConstraintDefinition constraint, int count, bool requireComplete)
        {
            if (constraint.Max.HasValue && count > constraint.Max.Value)
                result.Issues.Add(Issue("count.max", constraint.Id, current: count, limit: constraint.Max.Value));
            if (requireComplete && constraint.Min.HasValue && count < constraint.Min.Value)
                result.Issues.Add(Issue("count.min", constraint.Id, current: count, limit: constraint.Min.Value));
        }

        private static bool MinimumsSatisfied(IEnumerable<DeckConstraintDefinition> constraints, List<GwentCard> cards, GwentCard leader)
        {
            foreach (var c in constraints ?? Enumerable.Empty<DeckConstraintDefinition>())
            {
                if (!c.Min.HasValue) continue;
                var count = c.Kind == "deck-size" ? cards.Count : cards.Count(x => Matches(c.Filter, x, leader));
                if (count < c.Min.Value) return false;
            }
            return true;
        }

        private static string ConstraintCanonical(DeckConstraintDefinition c)
        {
            var f = c.Filter ?? new CardFilterDefinition();
            return string.Join("|", new[]
            {
                c.Id, c.Kind, c.Min?.ToString() ?? "", c.Max?.ToString() ?? "",
                string.Join(",", (f.CardIds ?? new List<string>()).OrderBy(x => x)),
                string.Join(",", (f.Groups ?? new List<Group>()).OrderBy(x => x)),
                string.Join(",", (f.Factions ?? new List<Faction>()).OrderBy(x => x)),
                string.Join(",", (f.CardTypes ?? new List<CardType>()).OrderBy(x => x)),
                string.Join(",", (f.AnyCategories ?? new List<Categorie>()).OrderBy(x => x)),
                string.Join(",", (f.AllCategories ?? new List<Categorie>()).OrderBy(x => x)),
                f.LeaderFactionOrNeutral ? "1" : "0"
            });
        }

        private static void ResolvePools(
            IEnumerable<string> ids,
            IDictionary<string, CardPoolDefinition> catalog,
            ICollection<CardPoolDefinition> destination,
            ResolvedDeckRuleSet result,
            string ruleCardId)
        {
            foreach (var id in (ids ?? Enumerable.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.Ordinal))
            {
                if (catalog.TryGetValue(id, out var pool)) destination.Add(pool);
                else result.ResolutionIssues.Add(Issue("rules.pool-unknown", cardId: ruleCardId, relatedId: id));
            }
        }

        private static bool PoolMatches(CardPoolDefinition pool, GwentCard card, GwentCard leader)
            => (pool?.AnyOf ?? new List<CardFilterDefinition>()).Any(x => Matches(x, card, leader));

        private static bool IsKnownDeckCard(string cardId, ResolvedDeckRuleSet rules, GwentCard leader)
        {
            if (IsRuleCard(cardId)) return true;
            if (!GwentMap.CardMap.TryGetValue(cardId, out var card)) return false;
            if ((rules?.CardPoolModifiers?.Count ?? 0) == 0) return DiyAiCardPool.IsUserDeckCard(cardId);
            return IsCardAllowedByPool(card, leader, rules);
        }

        private static string PoolModifierCanonical(ResolvedCardPoolModifier modifier)
            => string.Join("|", new[]
            {
                modifier.RuleCardId ?? "",
                PoolListCanonical(modifier.Add),
                PoolListCanonical(modifier.RestrictTo),
                PoolListCanonical(modifier.Exclude)
            });

        private static string PoolListCanonical(IEnumerable<CardPoolDefinition> pools)
            => string.Join(";", (pools ?? Enumerable.Empty<CardPoolDefinition>())
                .OrderBy(x => x.Id, StringComparer.Ordinal)
                .Select(x => (x.Id ?? "") + "[" + string.Join(",", (x.AnyOf ?? new List<CardFilterDefinition>())
                    .Select(FilterCanonical).OrderBy(y => y, StringComparer.Ordinal)) + "]"));

        private static string FilterCanonical(CardFilterDefinition f)
            => ConstraintCanonical(new DeckConstraintDefinition { Filter = f ?? new CardFilterDefinition() });

        internal static bool IsSuppressedByHigherPriority(
            RuleCardDefinition definition,
            string scope,
            IEnumerable<RuleCardDefinition> activeDefinitions)
            => activeDefinitions.Any(x => x.Priority > definition.Priority && ClaimsScope(x, scope));

        internal static bool ClaimsScope(RuleCardDefinition definition, string scope)
        {
            var scopes = definition.OverrideScopes ?? new List<string>();
            return scopes.Contains(ScopeAll) || (!string.IsNullOrWhiteSpace(scope) && scopes.Contains(scope));
        }

        private static string ConstraintScopeForId(string constraintId, IEnumerable<RuleCardDefinition> definitions)
        {
            var standard = StandardConstraints().FirstOrDefault(x => x.Id == constraintId);
            if (standard != null) return ConstraintScope(standard);
            var custom = definitions.SelectMany(x => x.AddConstraints ?? new List<DeckConstraintDefinition>())
                .FirstOrDefault(x => x?.Id == constraintId);
            return custom == null ? "constraints" : ConstraintScope(custom);
        }

        private static string ConstraintScope(DeckConstraintDefinition constraint)
        {
            switch ((constraint?.Kind ?? "").ToLowerInvariant())
            {
                case "deck-size": return ScopeDeckLimits;
                case "card-count": return ScopeGroupLimits;
                case "copy-count": return ScopeCardLimits;
                case "allow-cards":
                case "deny-cards": return ScopeCardPool;
                case "leader-faction": return ScopeLeader;
                default: return "constraints";
            }
        }

        private sealed class ConstraintOperation
        {
            public DeckConstraintDefinition Constraint { get; set; }
            public string ConstraintId { get; set; }
            public bool IsRemoval { get; set; }
            public int Priority { get; set; }
            public string SourceId { get; set; }
            public string Id => Constraint?.Id ?? ConstraintId ?? "";
        }

        private static DeckConstraintDefinition Constraint(string id, string kind, CardFilterDefinition filter = null, int? min = null, int? max = null)
            => new DeckConstraintDefinition { Id = id, Kind = kind, Filter = filter ?? new CardFilterDefinition(), Min = min, Max = max };
        private static CardFilterDefinition Groups(params Group[] groups) => new CardFilterDefinition { Groups = groups.ToList() };
        private static DeckValidationIssue Issue(string code, string constraintId = "", string cardId = "", string relatedId = "", int current = 0, int limit = 0)
            => new DeckValidationIssue { Code = code, ConstraintId = constraintId ?? "", CardId = cardId ?? "", RelatedId = relatedId ?? "", Current = current, Limit = limit };
        public static bool IsRuleCard(string cardId)
            => !string.IsNullOrWhiteSpace(cardId) &&
               GwentMap.CardMap.TryGetValue(cardId, out var card) &&
               (card.HideTags ?? new HideTag[0]).Contains(HideTag.Rule);

        private static DeckModel CloneDeck(DeckModel deck) => new DeckModel
        {
            Id = deck?.Id,
            Name = deck?.Name ?? "",
            SchemaVersion = deck?.SchemaVersion ?? 2,
            Leader = deck?.Leader ?? "",
            Deck = deck?.Deck?.ToList() ?? new List<string>()
        };
    }
}
