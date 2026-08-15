using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cynthia.Card
{
    /// <summary>
    /// Pure authoritative projection used by both the server endpoint and tests.
    /// It never persists or mutates the submitted deck.
    /// </summary>
    public static class DeckBuildingProjectionEngine
    {
        public const int MaximumNormalizationPasses = 8;
        public const int AbsoluteCopyLimit = 250;

        public static DeckBuildingProjection Project(
            GameFeatureManifest manifest,
            DeckBuildingProjectionRequest request)
            => ProjectCore(manifest, request, includeCardStates: true);

        private static DeckBuildingProjection ProjectCore(
            GameFeatureManifest manifest,
            DeckBuildingProjectionRequest request,
            bool includeCardStates)
        {
            manifest = manifest ?? new GameFeatureManifest { RulesetVersion = "offline-standard" };
            request = request ?? new DeckBuildingProjectionRequest();
            var submitted = CloneDeck(request.Deck);
            var response = new DeckBuildingProjection
            {
                Revision = request.Revision,
                RulesetVersion = manifest.RulesetVersion ?? "",
                SubmittedDeck = submitted
            };

            var normalized = CloneDeck(submitted);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var allRemovals = new List<DeckRuleTransitionRemoval>();
            ResolvedDeckRuleSet rules = null;
            var stabilized = false;
            var isRuleTransition =
                (string.Equals(request.Action, "add", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(request.Action, "remove", StringComparison.OrdinalIgnoreCase)) &&
                DeckRuleEngine.IsRuleCard(request.CandidateCardId);
            var shouldNormalize = isRuleTransition ||
                string.Equals(request.Action, "normalize", StringComparison.OrdinalIgnoreCase);

            if (shouldNormalize)
                DeduplicateRuleCards(normalized, allRemovals);

            // Removing a prerequisite rule can leave one or more selected rules
            // with an unsatisfied RequiresAll chain. Treat those dependent rules
            // as part of the same authoritative transition so the client can
            // preview and confirm one deterministic cleanup instead of getting
            // stuck behind a rule-resolution error.
            if (shouldNormalize &&
                string.Equals(request.Action, "remove", StringComparison.OrdinalIgnoreCase) &&
                DeckRuleEngine.IsRuleCard(request.CandidateCardId))
            {
                RemoveRulesWithMissingDependencies(manifest, normalized, allRemovals);
            }

            response.FailureCode = shouldNormalize
                ? ApplyNormalizationProposals(manifest, normalized, allRemovals)
                : "";
            rules = Resolve(manifest, normalized);
            // A conflicting/incomplete rule set has no authoritative transition.
            // Never preview destructive cleanup for a combination that cannot be
            // accepted in the first place; report the rule issue against the
            // submitted deck unchanged.
            if (!shouldNormalize)
            {
                // Refresh is a read-only snapshot operation. Existing broken or
                // retired-card decks must never be silently rewritten merely by
                // opening the editor.
                stabilized = true;
            }
            else if (!string.IsNullOrWhiteSpace(response.FailureCode) ||
                (rules.ResolutionIssues?.Count ?? 0) > 0)
            {
                stabilized = true;
            }
            else
            {
                for (var pass = 0; pass < MaximumNormalizationPasses; pass++)
                {
                    var stateKey = DeckKey(normalized);
                    if (!seen.Add(stateKey))
                    {
                        response.FailureCode = "rules.normalization-cycle";
                        break;
                    }

                    rules = Resolve(manifest, normalized);
                    var plan = DeckRuleEngine.PlanTransition(normalized, rules);
                    foreach (var removal in plan.RemovedCards)
                    {
                        removal.SourceRuleCardId = SourceForConstraint(rules, removal.ConstraintId)?.SourceRuleCardId ?? "";
                        allRemovals.Add(removal);
                    }

                    if (DeckEquals(normalized, plan.ResultDeck))
                    {
                        normalized = CloneDeck(plan.ResultDeck);
                        stabilized = true;
                        break;
                    }
                    normalized = CloneDeck(plan.ResultDeck);
                }
            }

            if (!stabilized && string.IsNullOrWhiteSpace(response.FailureCode))
                response.FailureCode = "rules.normalization-limit";

            rules = Resolve(manifest, normalized);
            response.ResolvedRules = rules;
            response.RulesFingerprint = rules.Fingerprint ?? "";
            response.ExecutionOrder = rules.ExecutionOrder ?? new List<ResolvedRuleCardExecution>();
            response.NormalizedDeck = normalized;
            response.RemovedCards = allRemovals;
            response.RequiresConfirmation = allRemovals.Count > 0 && !request.ConfirmNormalization;
            response.Limits = BuildLimits(rules);

            var validationDeck = response.RequiresConfirmation ? submitted : normalized;
            var validationRules = Resolve(manifest, validationDeck);
            var validation = DeckRuleEngine.Validate(validationDeck, validationRules, false);
            response.Issues = validation.Issues.ToList();
            if (!string.IsNullOrWhiteSpace(response.FailureCode))
                response.Issues.Add(new DeckValidationIssue { Code = response.FailureCode });
            response.IsValid = string.IsNullOrWhiteSpace(response.FailureCode) && validation.IsValid && !response.RequiresConfirmation;
            response.IsComplete = response.IsValid && DeckRuleEngine.Validate(validationDeck, validationRules, true).IsComplete;

            response.PoolFingerprint = PoolFingerprint(manifest, normalized, rules);
            // Correctness first: deck/group totals can change many max-copy states,
            // so the first protocol revision always returns a complete snapshot.
            response.FullSnapshot = true;
            response.CardStates = includeCardStates
                ? BuildCardStates(manifest, normalized, rules)
                : new List<DeckBuildingCardState>();
            response.ProjectionFingerprint = Hash(string.Join("\n", new[]
            {
                response.RulesetVersion,
                response.RulesFingerprint,
                response.PoolFingerprint,
                DeckKey(normalized),
                string.Join("|", response.CardStates.Select(x => x.CardId + ":" + x.Selectable + ":" + x.MaxCopies))
            }));
            return response;
        }

        private static void DeduplicateRuleCards(
            DeckModel deck,
            ICollection<DeckRuleTransitionRemoval> removals)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < (deck?.Deck?.Count ?? 0); index++)
            {
                var cardId = deck.Deck[index];
                if (!DeckRuleEngine.IsRuleCard(cardId) || seen.Add(cardId)) continue;
                removals.Add(new DeckRuleTransitionRemoval
                {
                    CardId = cardId,
                    OriginalIndex = index,
                    ReasonCode = "rules.duplicate"
                });
                deck.Deck.RemoveAt(index--);
            }
        }

        private static string ApplyNormalizationProposals(
            GameFeatureManifest manifest,
            DeckModel deck,
            ICollection<DeckRuleTransitionRemoval> removals)
        {
            var selected = new HashSet<string>(
                (deck?.Deck ?? new List<string>()).Where(DeckRuleEngine.IsRuleCard),
                StringComparer.Ordinal);
            var active = (manifest?.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null && x.IsEnabled && selected.Contains(x.Id))
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Id, StringComparer.Ordinal)
                .ToList();
            var proposalCandidates = active
                .Where(x => !DeckRuleEngine.IsSuppressedByHigherPriority(
                    x, DeckRuleEngine.ScopeNormalization, active))
                .SelectMany(x => (x.NormalizationRemovals ?? new List<DeckRuleTransitionRemoval>())
                    .Where(y => y != null && !string.IsNullOrWhiteSpace(y.CardId))
                    .Select(y => new NormalizationCandidate
                    {
                        Rule = x,
                        Removal = new DeckRuleTransitionRemoval
                        {
                            CardId = y.CardId,
                            OriginalIndex = y.OriginalIndex,
                            ReasonCode = string.IsNullOrWhiteSpace(y.ReasonCode) ? "rules.normalized" : y.ReasonCode,
                            ConstraintId = y.ConstraintId ?? "",
                            SourceRuleCardId = string.IsNullOrWhiteSpace(y.SourceRuleCardId) ? x.Id : y.SourceRuleCardId
                        }
                    }))
                .ToList();
            var proposals = new List<NormalizationCandidate>();
            foreach (var group in proposalCandidates.GroupBy(x => x.Removal.OriginalIndex))
            {
                var topPriority = group.Max(x => x.Rule.Priority);
                var winners = group.Where(x => x.Rule.Priority == topPriority).ToList();
                if (winners.Select(x => x.Removal.CardId).Distinct(StringComparer.Ordinal).Count() > 1)
                    return "rules.normalization-conflict";
                proposals.Add(winners
                    .OrderBy(x => x.Rule.SortOrder)
                    .ThenBy(x => x.Rule.Id, StringComparer.Ordinal)
                    .First());
            }
            proposals = proposals.OrderByDescending(x => x.Removal.OriginalIndex).ToList();
            if (proposals.Count == 0) return "";

            var candidate = CloneDeck(deck);
            var ordinary = candidate.Deck
                .Select((cardId, fullIndex) => new { cardId, fullIndex })
                .Where(x => !DeckRuleEngine.IsRuleCard(x.cardId))
                .ToList();
            foreach (var proposal in proposals)
            {
                var index = proposal.Removal.OriginalIndex;
                if (index < 0 || index >= ordinary.Count ||
                    !string.Equals(ordinary[index].cardId, proposal.Removal.CardId, StringComparison.Ordinal))
                    return "rules.normalization-proposal-invalid";
                candidate.Deck.RemoveAt(ordinary[index].fullIndex);
                ordinary.RemoveAt(index);
            }

            deck.Deck = candidate.Deck;
            foreach (var proposal in proposals.OrderBy(x => x.Removal.OriginalIndex))
                removals.Add(proposal.Removal);
            return "";
        }

        private static void RemoveRulesWithMissingDependencies(
            GameFeatureManifest manifest,
            DeckModel deck,
            ICollection<DeckRuleTransitionRemoval> removals)
        {
            var definitions = (manifest?.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);

            while (true)
            {
                var selected = new HashSet<string>(
                    (deck?.Deck ?? new List<string>()).Where(DeckRuleEngine.IsRuleCard),
                    StringComparer.Ordinal);
                var dependentIds = selected
                    .Where(id => definitions.TryGetValue(id, out var definition) &&
                                 (definition.RequiresAll ?? new List<string>()).Any(required => !selected.Contains(required)))
                    .OrderBy(id => id, StringComparer.Ordinal)
                    .ToList();
                if (dependentIds.Count == 0) return;

                foreach (var dependentId in dependentIds)
                {
                    for (var index = deck.Deck.Count - 1; index >= 0; index--)
                    {
                        if (!string.Equals(deck.Deck[index], dependentId, StringComparison.Ordinal)) continue;
                        removals.Add(new DeckRuleTransitionRemoval
                        {
                            CardId = dependentId,
                            OriginalIndex = index,
                            ReasonCode = "rules.requires"
                        });
                        deck.Deck.RemoveAt(index);
                    }
                }
            }
        }

        public static ResolvedDeckRuleSet Resolve(GameFeatureManifest manifest, DeckModel deck)
        {
            manifest = manifest ?? new GameFeatureManifest();
            var selected = (deck?.Deck ?? new List<string>())
                .Where(DeckRuleEngine.IsRuleCard)
                .Distinct(StringComparer.Ordinal);
            return DeckRuleEngine.Resolve(manifest.RuleCards, selected, manifest.RulesetVersion, manifest.CardPools);
        }

        private static List<DeckBuildingLimitState> BuildLimits(ResolvedDeckRuleSet rules)
        {
            var sources = (rules.ConstraintSources ?? new List<ResolvedConstraintSource>())
                .ToDictionary(x => x.ConstraintId, x => x, StringComparer.Ordinal);
            return (rules.Constraints ?? new List<DeckConstraintDefinition>())
                .Where(x => x.Kind == "deck-size" || x.Kind == "card-count" || x.Kind == "copy-count")
                .OrderBy(x => x.Id, StringComparer.Ordinal)
                .Select(x =>
                {
                    sources.TryGetValue(x.Id, out var source);
                    return new DeckBuildingLimitState
                    {
                        ConstraintId = x.Id,
                        Kind = x.Kind,
                        Min = x.Min,
                        Max = x.Max,
                        Groups = (x.Filter?.Groups ?? new List<Group>()).ToList(),
                        SourceRuleCardId = source?.SourceRuleCardId ?? "",
                        Priority = source?.Priority ?? int.MinValue
                    };
                })
                .ToList();
        }

        private static List<DeckBuildingCardState> BuildCardStates(
            GameFeatureManifest manifest,
            DeckModel deck,
            ResolvedDeckRuleSet rules)
        {
            var states = new List<DeckBuildingCardState>();
            if (!GwentMap.CardMap.TryGetValue(deck.Leader ?? "", out var leader) || leader.Group != Group.Leader)
                return states;

            var ruleDefinitions = (manifest.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null && x.IsEnabled && x.PlayerSelectable)
                .ToDictionary(x => x.Id, x => x, StringComparer.Ordinal);
            var ordinaryCards = GwentMap.CardMap.Values
                .Where(x => x.Group != Group.Leader && !DeckRuleEngine.IsRuleCard(x.CardId))
                .Where(x => DiyAiCardPool.IsUserDeckCard(x.CardId) || DeckRuleEngine.IsCardAllowedByPool(x, leader, rules));
            var cardIds = ordinaryCards.Select(x => x.CardId)
                .Concat(manifest.PlayerRuleCardsEnabled ? ruleDefinitions.Keys : Enumerable.Empty<string>())
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal);

            foreach (var cardId in cardIds)
                states.Add(BuildCardState(manifest, deck, rules, leader, ruleDefinitions, cardId));
            return states;
        }

        private static DeckBuildingCardState BuildCardState(
            GameFeatureManifest manifest,
            DeckModel deck,
            ResolvedDeckRuleSet rules,
            GwentCard leader,
            IDictionary<string, RuleCardDefinition> ruleDefinitions,
            string cardId)
        {
            var state = new DeckBuildingCardState { CardId = cardId };
            var isRule = DeckRuleEngine.IsRuleCard(cardId) || ruleDefinitions.ContainsKey(cardId);
            if (isRule)
            {
                var candidate = CloneDeck(deck);
                candidate.Deck.RemoveAll(x => x == cardId);
                candidate.Deck.Add(cardId);
                // A rule transition may intentionally invalidate some of the
                // submitted ordinary cards and then remove them after explicit
                // confirmation. Evaluate the same authoritative transition as
                // the add endpoint, without recursively producing card states,
                // so cards such as an empty-deck rule remain selectable.
                var transition = ProjectCore(manifest, new DeckBuildingProjectionRequest
                {
                    Action = "add",
                    CandidateCardId = cardId,
                    ConfirmNormalization = true,
                    Deck = candidate
                }, includeCardStates: false);
                state.Selectable = DeckRuleEngine.CanPlayerSelectRuleDeck(manifest, candidate) &&
                    transition.IsValid &&
                    (transition.NormalizedDeck?.Deck ?? new List<string>()).Contains(cardId);
                state.MaxCopies = state.Selectable ? 1 : 0;
                ApplyReason(state,
                    new DeckValidationResult
                    {
                        Issues = transition.Issues ?? new List<DeckValidationIssue>()
                    },
                    transition.ResolvedRules);
                return state;
            }

            if (!GwentMap.CardMap.TryGetValue(cardId, out var card) ||
                !DeckRuleEngine.IsCardAllowedByPool(card, leader, rules))
            {
                state.ReasonCode = "card.not-allowed";
                state.ConstraintId = "rules.card-pool";
                state.SourceRuleCardId = PoolSourceFor(card, leader, rules);
                return state;
            }

            var probe = CloneDeck(deck);
            probe.Deck.RemoveAll(x => x == cardId);
            var upper = DeckMaximum(rules);
            for (var count = 1; count <= upper; count++)
            {
                probe.Deck.Add(cardId);
                var validation = DeckRuleEngine.Validate(probe, rules, false);
                if (!validation.IsValid)
                {
                    if (state.MaxCopies == 0) ApplyReason(state, validation, rules);
                    break;
                }
                state.MaxCopies = count;
            }
            state.Selectable = state.MaxCopies > (deck.Deck?.Count(x => x == cardId) ?? 0);
            if (state.MaxCopies == 0 && string.IsNullOrWhiteSpace(state.ReasonCode))
                state.ReasonCode = "card.limit";
            return state;
        }

        private static void ApplyReason(
            DeckBuildingCardState state,
            DeckValidationResult validation,
            ResolvedDeckRuleSet rules)
        {
            var issue = validation?.Issues?.FirstOrDefault(x => x.Code != "count.min");
            if (issue == null) return;
            state.ReasonCode = issue.Code ?? "";
            state.ConstraintId = issue.ConstraintId ?? "";
            var source = SourceForConstraint(rules, state.ConstraintId);
            state.SourceRuleCardId = source?.SourceRuleCardId ?? "";
            state.Priority = source?.Priority ?? 0;
        }

        private static ResolvedConstraintSource SourceForConstraint(ResolvedDeckRuleSet rules, string constraintId)
            => (rules?.ConstraintSources ?? new List<ResolvedConstraintSource>())
                .FirstOrDefault(x => x.ConstraintId == constraintId);

        private static string PoolSourceFor(GwentCard card, GwentCard leader, ResolvedDeckRuleSet rules)
        {
            foreach (var modifier in (rules?.CardPoolModifiers ?? new List<ResolvedCardPoolModifier>())
                .OrderByDescending(x => x.Priority).ThenBy(x => x.RuleCardId, StringComparer.Ordinal))
            {
                if ((modifier.Exclude ?? new List<CardPoolDefinition>()).Any(x => PoolMatches(x, card, leader)))
                    return modifier.RuleCardId;
                if ((modifier.RestrictTo?.Count ?? 0) > 0 && !modifier.RestrictTo.Any(x => PoolMatches(x, card, leader)))
                    return modifier.RuleCardId;
            }
            return "";
        }

        private static bool PoolMatches(CardPoolDefinition pool, GwentCard card, GwentCard leader)
            => (pool?.AnyOf ?? new List<CardFilterDefinition>()).Any(x => DeckRuleEngine.Matches(x, card, leader));

        private static int DeckMaximum(ResolvedDeckRuleSet rules)
        {
            var configured = (rules?.Constraints ?? new List<DeckConstraintDefinition>())
                .Where(x => x.Kind == "deck-size" && x.Max.HasValue)
                .Select(x => x.Max.Value)
                .DefaultIfEmpty(AbsoluteCopyLimit)
                .Min();
            return Math.Max(1, Math.Min(AbsoluteCopyLimit, configured));
        }

        private static string PoolFingerprint(GameFeatureManifest manifest, DeckModel deck, ResolvedDeckRuleSet rules)
        {
            if (!GwentMap.CardMap.TryGetValue(deck.Leader ?? "", out var leader))
                return Hash("leader.invalid");
            var ids = GwentMap.CardMap.Values
                .Where(x => x.Group != Group.Leader && !DeckRuleEngine.IsRuleCard(x.CardId))
                .Where(x => DeckRuleEngine.IsCardAllowedByPool(x, leader, rules))
                .Select(x => x.CardId)
                .OrderBy(x => x, StringComparer.Ordinal);
            var selectableRules = manifest.PlayerRuleCardsEnabled
                ? (manifest.RuleCards ?? new List<RuleCardDefinition>())
                    .Where(x => x.IsEnabled && x.PlayerSelectable)
                    .Select(x => x.Id)
                    .OrderBy(x => x, StringComparer.Ordinal)
                : Enumerable.Empty<string>();
            return Hash(string.Join("|", ids.Concat(selectableRules)));
        }

        private static DeckModel CloneDeck(DeckModel deck)
            => new DeckModel
            {
                Id = deck?.Id,
                SchemaVersion = deck?.SchemaVersion ?? 2,
                Name = deck?.Name ?? "",
                Leader = deck?.Leader ?? "",
                Deck = (deck?.Deck ?? new List<string>()).ToList()
            };

        private static bool DeckEquals(DeckModel first, DeckModel second)
            => string.Equals(first?.Leader, second?.Leader, StringComparison.Ordinal) &&
               (first?.Deck ?? new List<string>()).SequenceEqual(second?.Deck ?? new List<string>());

        private static string DeckKey(DeckModel deck)
            => (deck?.Leader ?? "") + "\n" + string.Join("\n", deck?.Deck ?? new List<string>());

        private static string Hash(string value)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? "")))
                    .Replace("-", "").ToLowerInvariant();
        }

        private sealed class NormalizationCandidate
        {
            public RuleCardDefinition Rule { get; set; }
            public DeckRuleTransitionRemoval Removal { get; set; }
        }
    }
}
