using System.Collections.Generic;

namespace Cynthia.Card
{
    public class LocalizedText
    {
        public string ZhCn { get; set; } = "";
        public string En { get; set; } = "";

        public string Resolve(string language)
            => language != null && language.ToLowerInvariant().StartsWith("en") && !string.IsNullOrWhiteSpace(En)
                ? En
                : ZhCn;
    }

    // A declarative card selector shared by server validation and the Unity editor.
    // Empty lists mean "any". Multiple populated fields are combined with AND.
    public class CardFilterDefinition
    {
        public List<string> CardIds { get; set; } = new List<string>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Faction> Factions { get; set; } = new List<Faction>();
        public List<CardType> CardTypes { get; set; } = new List<CardType>();
        public List<Categorie> AnyCategories { get; set; } = new List<Categorie>();
        public List<Categorie> AllCategories { get; set; } = new List<Categorie>();
        public bool LeaderFactionOrNeutral { get; set; }
    }

    // A reusable, server-authored card pool. Filters inside one pool are OR'ed.
    // An empty AnyOf is an intentional empty pool, not "all cards".
    // This lets a manifest name large/hidden pools without exposing every card as
    // a separate client-side rule.
    public class CardPoolDefinition
    {
        public string Id { get; set; } = "";
        public List<CardFilterDefinition> AnyOf { get; set; } = new List<CardFilterDefinition>();
    }

    public class ResolvedCardPoolModifier
    {
        public string RuleCardId { get; set; } = "";
        public int Priority { get; set; }
        public List<CardPoolDefinition> Add { get; set; } = new List<CardPoolDefinition>();
        public List<CardPoolDefinition> RestrictTo { get; set; } = new List<CardPoolDefinition>();
        public List<CardPoolDefinition> Exclude { get; set; } = new List<CardPoolDefinition>();
    }

    // Supported kinds:
    // deck-size, card-count, copy-count, allow-cards, deny-cards, leader-faction.
    // A rule card may explicitly remove a base constraint by id and add replacements.
    public class DeckConstraintDefinition
    {
        public string Id { get; set; } = "";
        public string Kind { get; set; } = "";
        public CardFilterDefinition Filter { get; set; } = new CardFilterDefinition();
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class RuleCardDefinition
    {
        public string Id { get; set; } = "";
        public LocalizedText Name { get; set; } = new LocalizedText();
        public LocalizedText Description { get; set; } = new LocalizedText();
        public string ArtId { get; set; } = "";
        public string PackageVersion { get; set; } = "1";
        // Higher values win when two cards claim the same constraint or an
        // explicitly overridden scope. User deck order never affects priority.
        public int Priority { get; set; }
        public int SortOrder { get; set; }
        public bool IsEnabled { get; set; } = true;
        // Runtime/server-controlled decks may keep using an enabled rule while
        // this flag is false. This is deliberately separate from IsEnabled.
        public bool PlayerSelectable { get; set; }
        // Known scopes: deck-limits, group-limits, card-limits, card-pool,
        // leader, normalization, or * for every lower-priority rule scope.
        public List<string> OverrideScopes { get; set; } = new List<string>();
        public List<Faction> AllowedLeaderFactions { get; set; } = new List<Faction>();
        public List<string> ExclusiveWith { get; set; } = new List<string>();
        public List<string> RequiresAll { get; set; } = new List<string>();
        public List<string> RemoveConstraintIds { get; set; } = new List<string>();
        public List<DeckConstraintDefinition> AddConstraints { get; set; } = new List<DeckConstraintDefinition>();
        // Pure out-of-game CardEffect handlers may propose deterministic removals.
        // OriginalIndex addresses the ordinary-card sequence (rule cards excluded).
        public List<DeckRuleTransitionRemoval> NormalizationRemovals { get; set; } = new List<DeckRuleTransitionRemoval>();
        // Card-pool order is deterministic: base pool UNION additions, then every
        // active restriction is intersected, then exclusions win.
        public List<string> AddCardPools { get; set; } = new List<string>();
        public List<string> RestrictToCardPools { get; set; } = new List<string>();
        public List<string> ExcludeCardPools { get; set; } = new List<string>();
    }

    public class ResolvedDeckRuleSet
    {
        public string RulesetVersion { get; set; } = "";
        public string Fingerprint { get; set; } = "";
        public List<string> AppliedRuleCards { get; set; } = new List<string>();
        public List<ResolvedRuleCardExecution> ExecutionOrder { get; set; } = new List<ResolvedRuleCardExecution>();
        public List<DeckConstraintDefinition> Constraints { get; set; } = new List<DeckConstraintDefinition>();
        public List<ResolvedConstraintSource> ConstraintSources { get; set; } = new List<ResolvedConstraintSource>();
        public List<ResolvedCardPoolModifier> CardPoolModifiers { get; set; } = new List<ResolvedCardPoolModifier>();
        public List<DeckValidationIssue> ResolutionIssues { get; set; } = new List<DeckValidationIssue>();
    }

    public class ResolvedConstraintSource
    {
        public string ConstraintId { get; set; } = "";
        public string SourceRuleCardId { get; set; } = "";
        public int Priority { get; set; }
    }

    public class ResolvedRuleCardExecution
    {
        public string RuleCardId { get; set; } = "";
        public string PackageVersion { get; set; } = "";
        public int Priority { get; set; }
        public List<string> OverrideScopes { get; set; } = new List<string>();
    }

    public class DeckValidationIssue
    {
        public string Code { get; set; } = "";
        public string ConstraintId { get; set; } = "";
        public string CardId { get; set; } = "";
        public string RelatedId { get; set; } = "";
        public int Current { get; set; }
        public int Limit { get; set; }
    }

    public class DeckValidationResult
    {
        // IsValid means the draft can be saved. IsComplete additionally means it can match.
        public bool IsValid { get; set; }
        public bool IsComplete { get; set; }
        public ResolvedDeckRuleSet Rules { get; set; } = new ResolvedDeckRuleSet();
        public List<DeckValidationIssue> Issues { get; set; } = new List<DeckValidationIssue>();
    }

    public class DeckRuleTransitionRemoval
    {
        public string CardId { get; set; } = "";
        public int OriginalIndex { get; set; }
        public string ReasonCode { get; set; } = "";
        public string ConstraintId { get; set; } = "";
        public string SourceRuleCardId { get; set; } = "";
    }

    public class DeckRuleTransitionPlan
    {
        public DeckModel ResultDeck { get; set; } = new DeckModel();
        public List<DeckRuleTransitionRemoval> RemovedCards { get; set; } = new List<DeckRuleTransitionRemoval>();
        public DeckValidationResult Validation { get; set; } = new DeckValidationResult();
        public bool RequiresConfirmation => RemovedCards.Count > 0;
    }

    public class DeckBuildingProjectionRequest
    {
        public long Revision { get; set; }
        public string PreviousProjectionFingerprint { get; set; } = "";
        public string PreviousPoolFingerprint { get; set; } = "";
        public string Action { get; set; } = "refresh";
        public string CandidateCardId { get; set; } = "";
        public bool ConfirmNormalization { get; set; }
        public int ClientFeatureLevel { get; set; } = 2;
        public DeckModel Deck { get; set; } = new DeckModel();
    }

    public class DeckBuildingLimitState
    {
        public string ConstraintId { get; set; } = "";
        public string Kind { get; set; } = "";
        public int? Min { get; set; }
        public int? Max { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
        public string SourceRuleCardId { get; set; } = "";
        public int Priority { get; set; }
    }

    public class DeckBuildingCardState
    {
        public string CardId { get; set; } = "";
        public bool Selectable { get; set; }
        public int MaxCopies { get; set; }
        public string ReasonCode { get; set; } = "";
        public string ConstraintId { get; set; } = "";
        public string SourceRuleCardId { get; set; } = "";
        public int Priority { get; set; }
    }

    public class DeckBuildingProjection
    {
        public long Revision { get; set; }
        public string RulesetVersion { get; set; } = "";
        public string RulesFingerprint { get; set; } = "";
        public string PoolFingerprint { get; set; } = "";
        public string ProjectionFingerprint { get; set; } = "";
        public bool FullSnapshot { get; set; } = true;
        public bool IsAuthoritative { get; set; } = true;
        public bool IsValid { get; set; }
        public bool IsComplete { get; set; }
        public bool RequiresConfirmation { get; set; }
        public string FailureCode { get; set; } = "";
        public DeckModel SubmittedDeck { get; set; } = new DeckModel();
        public DeckModel NormalizedDeck { get; set; } = new DeckModel();
        // Complete, server-resolved rule snapshot. The editor keeps this snapshot
        // while ordinary cards are edited locally and only asks the server again
        // when the leader or selected rule cards change.
        public ResolvedDeckRuleSet ResolvedRules { get; set; } = new ResolvedDeckRuleSet();
        public List<ResolvedRuleCardExecution> ExecutionOrder { get; set; } = new List<ResolvedRuleCardExecution>();
        public List<DeckBuildingLimitState> Limits { get; set; } = new List<DeckBuildingLimitState>();
        public List<DeckBuildingCardState> CardStates { get; set; } = new List<DeckBuildingCardState>();
        public List<DeckRuleTransitionRemoval> RemovedCards { get; set; } = new List<DeckRuleTransitionRemoval>();
        public List<DeckValidationIssue> Issues { get; set; } = new List<DeckValidationIssue>();
    }

    // A pure out-of-game event. Implementations may live on the same CardEffect
    // class as runtime behavior, but cannot access Game/Card or mutate this input.
    public class DeckBuildingAdjustmentContext
    {
        public long Revision { get; set; }
        public DeckModel Deck { get; set; } = new DeckModel();
        public RuleCardDefinition Rule { get; set; } = new RuleCardDefinition();
        public ResolvedDeckRuleSet LowerPriorityRules { get; set; } = new ResolvedDeckRuleSet();
    }

    public class DeckBuildingRuleProposal
    {
        public List<string> RemoveConstraintIds { get; set; } = new List<string>();
        public List<DeckConstraintDefinition> AddConstraints { get; set; } = new List<DeckConstraintDefinition>();
        public List<string> AddCardPools { get; set; } = new List<string>();
        public List<string> RestrictToCardPools { get; set; } = new List<string>();
        public List<string> ExcludeCardPools { get; set; } = new List<string>();
        public List<DeckRuleTransitionRemoval> NormalizationRemovals { get; set; } = new List<DeckRuleTransitionRemoval>();
        public List<DeckValidationIssue> Issues { get; set; } = new List<DeckValidationIssue>();
    }

    public interface IDeckBuildingRuleEffect
    {
        DeckBuildingRuleProposal OnDeckBuildingAdjust(DeckBuildingAdjustmentContext context);
    }

    public class DynamicCardMarkerDefinition
    {
        public string Id { get; set; } = "";
        public LocalizedText Name { get; set; } = new LocalizedText();
        public LocalizedText Description { get; set; } = new LocalizedText();
        public string ShortLabel { get; set; } = "";
        public string StyleToken { get; set; } = "neutral";
        public string ValueDisplay { get; set; } = "none";
        public bool AllowMultipleInstances { get; set; }
        public int Priority { get; set; }
    }

    public class DynamicCardMarker
    {
        public string DefinitionId { get; set; } = "";
        public string InstanceId { get; set; } = "";
        public int? Value { get; set; }
    }

    public class GameResourceDefinition
    {
        public string Id { get; set; } = "";
        public LocalizedText Name { get; set; } = new LocalizedText();
        public LocalizedText Description { get; set; } = new LocalizedText();
        public string ShortLabel { get; set; } = "";
        public string StyleToken { get; set; } = "neutral";
        public string ValueFormat { get; set; } = "number";
        public int Priority { get; set; }
    }

    public class GameResourceState
    {
        public string DefinitionId { get; set; } = "";
        public int Value { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class GameModeDefinition
    {
        public string Id { get; set; } = "";
        public string Category { get; set; } = "";
        // Optional server-authored presentation. Older manifests may omit it;
        // clients then fall back to the built-in pvp/ai category labels.
        public LocalizedText CategoryName { get; set; } = new LocalizedText();
        public LocalizedText Name { get; set; } = new LocalizedText();
        public LocalizedText Description { get; set; } = new LocalizedText();
        public string IconKey { get; set; } = "";
        public string MatchKind { get; set; } = "pvp";
        // Presentation only. MatchKind remains the authoritative execution path.
        public LocalizedText TypeLabel { get; set; } = new LocalizedText();
        // Matchmaking policy only. Password/custom entry points may deliberately
        // pair decks with different rule cards.
        public string RuleMatchPolicy { get; set; } = "same";
        public string AiProfile { get; set; } = "";
        // Rules owned by the server-controlled AI. They are injected into the
        // AI deck at match creation, so players can enter a rule encounter
        // without enabling or carrying player-selectable rule cards.
        public List<string> AiRuleCards { get; set; } = new List<string>();
        public bool IsRanked { get; set; }
        // Rule-package matches are isolated from ordinary MMR by default. A
        // future mode must opt in explicitly after its rules are accepted for
        // ranked play.
        public bool CountRuleMatchesAsRanked { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool AllowCustomRuleCards { get; set; }
        public List<string> RequiredRuleCards { get; set; } = new List<string>();
        public List<string> AllowedRuleCards { get; set; } = new List<string>();
        public int SortOrder { get; set; }
        public int MinimumClientFeatureLevel { get; set; } = 1;
    }

    public class GameFeatureManifest
    {
        public int SchemaVersion { get; set; } = 2;
        public int FeatureLevel { get; set; } = 2;
        public string RulesetVersion { get; set; } = "";
        // Player deck-building/matchmaking exposure only. Runtime rule cards used
        // by server-controlled AI and the in-game rule zone remain available.
        public bool PlayerRuleCardsEnabled { get; set; }
        public List<DynamicCardMarkerDefinition> CardMarkerDefinitions { get; set; } = new List<DynamicCardMarkerDefinition>();
        public List<GameResourceDefinition> ResourceDefinitions { get; set; } = new List<GameResourceDefinition>();
        public List<CardPoolDefinition> CardPools { get; set; } = new List<CardPoolDefinition>();
        public List<RuleCardDefinition> RuleCards { get; set; } = new List<RuleCardDefinition>();
        public List<GameModeDefinition> Modes { get; set; } = new List<GameModeDefinition>();
    }
}
