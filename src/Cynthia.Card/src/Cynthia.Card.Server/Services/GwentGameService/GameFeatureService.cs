using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Cynthia.Card.Server.Services.GwentGameService
{
    public class GameFeatureService
    {
        private readonly string _manifestPath;
        private readonly GwentCardDataService _cardData;
        private readonly object _sync = new object();
        private DateTime _lastWriteUtc = DateTime.MinValue;
        private GameFeatureManifest _manifest;

        public GameFeatureService(IWebHostEnvironment environment, GwentCardDataService cardData)
        {
            _cardData = cardData;
            var overridePath = Environment.GetEnvironmentVariable("GWENT_FEATURE_MANIFEST");
            _manifestPath = string.IsNullOrWhiteSpace(overridePath)
                ? Path.Combine(environment.ContentRootPath, "Features", "game-features.json")
                : Path.GetFullPath(Path.IsPathRooted(overridePath)
                    ? overridePath
                    : Path.Combine(environment.ContentRootPath, overridePath));
        }

        public GameFeatureManifest GetManifest(int clientFeatureLevel = int.MaxValue)
        {
            EnsureLoaded();
            lock (_sync)
            {
                // Re-serialize to avoid callers mutating the cached singleton instance.
                var copy = JsonConvert.DeserializeObject<GameFeatureManifest>(
                    JsonConvert.SerializeObject(_manifest)) ?? new GameFeatureManifest();
                copy.Modes = copy.Modes
                    .Where(x => x.IsEnabled && x.MinimumClientFeatureLevel <= clientFeatureLevel)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Id, StringComparer.Ordinal)
                    .ToList();
                copy.RuleCards = copy.RuleCards
                    .Where(x => x.IsEnabled)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Id, StringComparer.Ordinal)
                    .ToList();
                return copy;
            }
        }

        public ResolvedDeckRuleSet Resolve(DeckModel deck)
        {
            var manifest = GetManifest();
            return ResolveEffective(manifest, deck);
        }

        internal ResolvedDeckRuleSet ResolveRuntime(DeckModel deck)
        {
            var manifest = GetManifest();
            var activeRuleIds = new HashSet<string>(
                (manifest.RuleCards ?? new List<RuleCardDefinition>()).Select(x => x.Id),
                StringComparer.Ordinal);
            var runtimeDeck = CloneDeck(deck);
            runtimeDeck.Deck = runtimeDeck.Deck
                .Where(cardId => !DeckRuleEngine.IsRuleCard(cardId) || activeRuleIds.Contains(cardId))
                .ToList();
            return ResolveEffective(manifest, runtimeDeck);
        }

        private ResolvedDeckRuleSet ResolveEffective(
            GameFeatureManifest manifest,
            DeckModel deck,
            long revision = 0)
        {
            var effectIssues = new List<DeckValidationIssue>();
            ApplyDeckBuildingEffects(manifest, new DeckBuildingProjectionRequest
            {
                Revision = revision,
                Deck = CloneDeck(deck)
            }, effectIssues);
            var ruleCards = (deck?.Deck ?? new List<string>())
                .Where(DeckRuleEngine.IsRuleCard)
                .Distinct(StringComparer.Ordinal);
            var rules = DeckRuleEngine.Resolve(
                manifest.RuleCards,
                ruleCards,
                manifest.RulesetVersion,
                manifest.CardPools);
            rules.ResolutionIssues.AddRange(effectIssues);
            return rules;
        }

        public DeckValidationResult ValidateDeck(DeckModel deck, bool requireComplete)
            => DeckRuleEngine.Validate(deck, Resolve(deck), requireComplete);

        public DeckBuildingProjection ProjectDeckBuilding(DeckBuildingProjectionRequest request)
        {
            request = request ?? new DeckBuildingProjectionRequest();
            var manifest = GetManifest(request.ClientFeatureLevel);
            var effectIssues = new List<DeckValidationIssue>();
            ApplyDeckBuildingEffects(manifest, request, effectIssues);
            var projection = DeckBuildingProjectionEngine.Project(manifest, request);
            if (effectIssues.Count > 0)
            {
                projection.Issues.AddRange(effectIssues);
                projection.IsValid = false;
                projection.IsComplete = false;
                if (string.IsNullOrWhiteSpace(projection.FailureCode))
                    projection.FailureCode = "rules.deck-building-effect-failed";
            }
            return projection;
        }

        public bool TryValidateMode(string modeId, DeckModel deck, out GameModeDefinition mode, out DeckValidationResult validation)
        {
            var manifest = GetManifest();
            mode = manifest.Modes.FirstOrDefault(x => x.Id == modeId && x.IsEnabled);
            var rules = ResolveEffective(manifest, deck);
            validation = DeckRuleEngine.ValidateMode(deck, manifest, mode, rules, true);
            if (!CanPlayerUseDeck(manifest, deck))
            {
                validation.Issues.Add(new DeckValidationIssue { Code = "rules.player-disabled" });
                validation.IsValid = false;
                validation.IsComplete = false;
            }
            return validation.IsComplete;
        }

        public bool CanPlayerUseDeck(DeckModel deck) => CanPlayerUseDeck(GetManifest(), deck);

        public static bool CanPlayerUseDeck(GameFeatureManifest manifest, DeckModel deck)
            => DeckRuleEngine.CanPlayerSelectRuleDeck(manifest, deck);

        private void EnsureLoaded()
        {
            lock (_sync)
            {
                if (!File.Exists(_manifestPath))
                    throw new FileNotFoundException("Game feature manifest is missing.", _manifestPath);
                var writeUtc = File.GetLastWriteTimeUtc(_manifestPath);
                if (_manifest != null && writeUtc == _lastWriteUtc) return;
                var loaded = JsonConvert.DeserializeObject<GameFeatureManifest>(File.ReadAllText(_manifestPath));
                if (loaded == null || loaded.SchemaVersion < 2 || string.IsNullOrWhiteSpace(loaded.RulesetVersion))
                    throw new InvalidDataException("Game feature manifest is invalid.");
                EnsureUnique(loaded.Modes.Select(x => x.Id), "mode");
                EnsureUnique(loaded.RuleCards.Select(x => x.Id), "rule card");
                EnsureUnique(loaded.CardPools.Select(x => x.Id), "card pool");
                EnsureUnique((loaded.CardMarkerDefinitions ?? new List<DynamicCardMarkerDefinition>()).Select(x => x.Id), "card marker");
                EnsureUnique((loaded.ResourceDefinitions ?? new List<GameResourceDefinition>()).Select(x => x.Id), "resource");
                ValidatePresentationDefinitions(loaded);
                if (loaded.Modes.Any(x => string.IsNullOrWhiteSpace(x.Id)) ||
                    loaded.RuleCards.Any(x => string.IsNullOrWhiteSpace(x.Id)))
                    throw new InvalidDataException("Game feature manifest contains a blank id.");
                foreach (var rule in loaded.RuleCards)
                {
                    if (!GwentMap.CardMap.ContainsKey(rule.Id) || !DeckRuleEngine.IsRuleCard(rule.Id))
                        throw new InvalidDataException($"Rule card '{rule.Id}' is missing from CardMap or lacks HideTag.Rule.");
                    var knownScopes = new HashSet<string>(new[]
                    {
                        DeckRuleEngine.ScopeDeckLimits, DeckRuleEngine.ScopeGroupLimits,
                        DeckRuleEngine.ScopeCardLimits, DeckRuleEngine.ScopeCardPool,
                        DeckRuleEngine.ScopeLeader, DeckRuleEngine.ScopeNormalization,
                        DeckRuleEngine.ScopeAll
                    }, StringComparer.Ordinal);
                    var unknownScope = (rule.OverrideScopes ?? new List<string>()).FirstOrDefault(x => !knownScopes.Contains(x));
                    if (unknownScope != null)
                        throw new InvalidDataException($"Rule card '{rule.Id}' has unknown override scope '{unknownScope}'.");
                }
                var ruleIds = new HashSet<string>(loaded.RuleCards.Select(x => x.Id), StringComparer.Ordinal);
                var poolIds = new HashSet<string>(loaded.CardPools.Select(x => x.Id), StringComparer.Ordinal);
                foreach (var pool in loaded.CardPools)
                {
                    if (string.IsNullOrWhiteSpace(pool.Id))
                        throw new InvalidDataException("Game feature manifest contains a card pool with a blank id.");
                    var missingCard = pool.AnyOf.SelectMany(x => x.CardIds ?? new List<string>())
                        .FirstOrDefault(x => !GwentMap.CardMap.ContainsKey(x));
                    if (missingCard != null)
                        throw new InvalidDataException($"Card pool '{pool.Id}' references unknown card '{missingCard}'.");
                }
                foreach (var rule in loaded.RuleCards)
                {
                    var missingPool = (rule.AddCardPools ?? new List<string>())
                        .Concat(rule.RestrictToCardPools ?? new List<string>())
                        .Concat(rule.ExcludeCardPools ?? new List<string>())
                        .FirstOrDefault(x => !poolIds.Contains(x));
                    if (missingPool != null)
                        throw new InvalidDataException($"Rule card '{rule.Id}' references unknown card pool '{missingPool}'.");
                }
                foreach (var mode in loaded.Modes)
                {
                    if (!string.Equals(mode.MatchKind, "pvp", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(mode.MatchKind, "ai", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException($"Mode '{mode.Id}' has unsupported MatchKind '{mode.MatchKind}'.");
                    if (!string.Equals(mode.RuleMatchPolicy, "same", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(mode.RuleMatchPolicy, "ignore", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException($"Mode '{mode.Id}' has unsupported RuleMatchPolicy '{mode.RuleMatchPolicy}'.");
                    var references = (mode.RequiredRuleCards ?? new List<string>())
                        .Concat(mode.AllowedRuleCards ?? new List<string>())
                        .Concat(mode.AiRuleCards ?? new List<string>())
                        .Distinct(StringComparer.Ordinal);
                    var missing = references.FirstOrDefault(x => !ruleIds.Contains(x));
                    if (missing != null)
                        throw new InvalidDataException($"Mode '{mode.Id}' references unknown rule card '{missing}'.");
                    if (!string.Equals(mode.MatchKind, "ai", StringComparison.OrdinalIgnoreCase) &&
                        (mode.AiRuleCards?.Count ?? 0) > 0)
                        throw new InvalidDataException($"Non-AI mode '{mode.Id}' cannot declare AI rule cards.");
                }
                _manifest = loaded;
                _lastWriteUtc = writeUtc;
            }
        }

        private void ApplyDeckBuildingEffects(
            GameFeatureManifest manifest,
            DeckBuildingProjectionRequest request,
            ICollection<DeckValidationIssue> issues)
        {
            var selected = new HashSet<string>(
                (request.Deck?.Deck ?? new List<string>()).Where(DeckRuleEngine.IsRuleCard),
                StringComparer.Ordinal);
            var ordered = (manifest.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null && x.IsEnabled && selected.Contains(x.Id))
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Id, StringComparer.Ordinal)
                .ToList();
            var processed = new List<string>();

            foreach (var rule in ordered)
            {
                try
                {
                    var proposal = _cardData.AdjustDeckBuilding(rule.Id, new DeckBuildingAdjustmentContext
                    {
                        Revision = request.Revision,
                        Deck = CloneDeck(request.Deck),
                        Rule = rule,
                        LowerPriorityRules = DeckRuleEngine.Resolve(
                            manifest.RuleCards,
                            processed,
                            manifest.RulesetVersion,
                            manifest.CardPools)
                    });
                    MergeProposal(rule, proposal);
                    foreach (var issue in proposal.Issues ?? new List<DeckValidationIssue>())
                    {
                        if (string.IsNullOrWhiteSpace(issue.CardId)) issue.CardId = rule.Id;
                        issues.Add(issue);
                    }
                }
                catch
                {
                    // Bad experimental rules fail closed for this projection;
                    // the hub and unrelated matchmaking remain alive.
                    issues.Add(new DeckValidationIssue
                    {
                        Code = "rules.deck-building-effect-failed",
                        CardId = rule.Id
                    });
                }
                processed.Add(rule.Id);
            }
        }

        private static void MergeProposal(RuleCardDefinition rule, DeckBuildingRuleProposal proposal)
        {
            if (proposal == null) return;
            rule.RemoveConstraintIds = (rule.RemoveConstraintIds ?? new List<string>())
                .Concat(proposal.RemoveConstraintIds ?? new List<string>())
                .Distinct(StringComparer.Ordinal)
                .ToList();
            foreach (var constraint in proposal.AddConstraints ?? new List<DeckConstraintDefinition>())
            {
                if (constraint == null || string.IsNullOrWhiteSpace(constraint.Id)) continue;
                if (rule.AddConstraints == null) rule.AddConstraints = new List<DeckConstraintDefinition>();
                rule.AddConstraints.RemoveAll(x => x.Id == constraint.Id);
                rule.AddConstraints.Add(constraint);
            }
            rule.AddCardPools = MergeIds(rule.AddCardPools, proposal.AddCardPools);
            rule.RestrictToCardPools = MergeIds(rule.RestrictToCardPools, proposal.RestrictToCardPools);
            rule.ExcludeCardPools = MergeIds(rule.ExcludeCardPools, proposal.ExcludeCardPools);
            rule.NormalizationRemovals = (rule.NormalizationRemovals ?? new List<DeckRuleTransitionRemoval>())
                .Concat(proposal.NormalizationRemovals ?? new List<DeckRuleTransitionRemoval>())
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.CardId))
                .Select(x => new DeckRuleTransitionRemoval
                {
                    CardId = x.CardId,
                    OriginalIndex = x.OriginalIndex,
                    ReasonCode = string.IsNullOrWhiteSpace(x.ReasonCode) ? "rules.normalized" : x.ReasonCode,
                    ConstraintId = x.ConstraintId ?? "",
                    SourceRuleCardId = rule.Id
                })
                .ToList();
        }

        private static List<string> MergeIds(IEnumerable<string> first, IEnumerable<string> second)
            => (first ?? Enumerable.Empty<string>()).Concat(second ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList();

        private static DeckModel CloneDeck(DeckModel deck)
            => new DeckModel
            {
                Id = deck?.Id,
                SchemaVersion = deck?.SchemaVersion ?? 2,
                Name = deck?.Name ?? "",
                Leader = deck?.Leader ?? "",
                Deck = (deck?.Deck ?? new List<string>()).ToList()
            };

        private static void EnsureUnique(IEnumerable<string> values, string label)
        {
            var ids = values.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (ids.Count != ids.Distinct(StringComparer.Ordinal).Count())
                throw new InvalidDataException($"Duplicate {label} id in game feature manifest.");
        }

        private static void ValidatePresentationDefinitions(GameFeatureManifest manifest)
        {
            var styles = new HashSet<string>(new[]
            {
                "neutral", "amber", "frost", "venom", "blood", "arcane", "nature", "steel"
            }, StringComparer.OrdinalIgnoreCase);
            var markerDisplays = new HashSet<string>(new[]
            {
                "none", "number", "stack", "countdown"
            }, StringComparer.OrdinalIgnoreCase);
            var resourceFormats = new HashSet<string>(new[]
            {
                "number", "fraction", "compact"
            }, StringComparer.OrdinalIgnoreCase);

            foreach (var marker in manifest.CardMarkerDefinitions ?? new List<DynamicCardMarkerDefinition>())
            {
                ValidatePresentationIdentity(marker.Id, marker.ShortLabel, "card marker");
                if (!styles.Contains(marker.StyleToken ?? ""))
                    throw new InvalidDataException($"Card marker '{marker.Id}' has unsupported style '{marker.StyleToken}'.");
                if (!markerDisplays.Contains(marker.ValueDisplay ?? ""))
                    throw new InvalidDataException($"Card marker '{marker.Id}' has unsupported value display '{marker.ValueDisplay}'.");
            }
            foreach (var resource in manifest.ResourceDefinitions ?? new List<GameResourceDefinition>())
            {
                ValidatePresentationIdentity(resource.Id, resource.ShortLabel, "resource");
                if (!styles.Contains(resource.StyleToken ?? ""))
                    throw new InvalidDataException($"Resource '{resource.Id}' has unsupported style '{resource.StyleToken}'.");
                if (!resourceFormats.Contains(resource.ValueFormat ?? ""))
                    throw new InvalidDataException($"Resource '{resource.Id}' has unsupported value format '{resource.ValueFormat}'.");
            }
        }

        private static void ValidatePresentationIdentity(string id, string shortLabel, string label)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Length > 64)
                throw new InvalidDataException($"A {label} id must contain 1 through 64 characters.");
            if ((shortLabel ?? "").Length > 8)
                throw new InvalidDataException($"{label} '{id}' short label cannot exceed 8 characters.");
        }
    }
}
