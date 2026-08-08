using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    /// <summary>
    /// Local fixture demonstrating that one CardEffect can own both runtime
    /// event handlers and the pure out-of-game deck-building adjustment.
    /// The card itself is only added when GWENT_ENABLE_RULE_FIXTURES=1.
    /// </summary>
    [CardEffectId("99001")]
    public sealed class HundredCardExperimentRule : CardEffect
    {
        public HundredCardExperimentRule(GameCard card) : base(card) { }

        public override DeckBuildingRuleProposal OnDeckBuildingAdjust(DeckBuildingAdjustmentContext context)
            => new DeckBuildingRuleProposal
            {
                RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardDeckSize },
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition
                    {
                        Id = "local.deck-size",
                        Kind = "deck-size",
                        Min = 25,
                        Max = 100
                    }
                }
            };
    }

    [CardEffectId("99004")]
    public sealed class FeastEchoDevelopmentRule : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        private bool _didSeedVisualMarkers;

        public FeastEchoDevelopmentRule(GameCard card) : base(card) { }

        public async System.Threading.Tasks.Task HandleEvent(AfterTurnStart @event)
        {
            var isRuleOwnerTurn = Game.GetRulePlayerIndexes(Card.Status.CardId).Contains(@event.PlayerIndex);
            // Local visual acceptance fixture: mark the opening hands once on both
            // sides so a player entering an AI rule challenge can inspect the
            // generic badge rail without owning or selecting the hidden rule card.
            // Markers are idempotent and may be refreshed on either player's turn;
            // resource changes remain scoped to the rule owner's turn.
            if (!_didSeedVisualMarkers)
            {
                foreach (var target in Game.PlayersHandCard.SelectMany(hand => hand).ToList())
                {
                    await Game.SetCardMarker(target, "local.poison", 2);
                    await Game.SetCardMarker(target, "local.echo", 1, "turn");
                    await Game.SetCardMarker(target, "local.echo", 3, "deploy");
                    await Game.SetCardMarker(target, "local.echo", 2, "graveyard");
                    await Game.SetCardMarker(target, "local.echo", 4, "weather");
                }
                _didSeedVisualMarkers = true;
            }
            if (!isRuleOwnerTurn) return;
            await Game.AddResource(@event.PlayerIndex, "local.coins", 1, 0, 9);
            await Game.AddResource(@event.PlayerIndex, "local.momentum", 2);
        }
    }

    [CardEffectId("99006")]
    public sealed class BlankContractDevelopmentRule : CardEffect
    {
        public BlankContractDevelopmentRule(GameCard card) : base(card) { }

        public override DeckBuildingRuleProposal OnDeckBuildingAdjust(DeckBuildingAdjustmentContext context)
            => new DeckBuildingRuleProposal
            {
                RemoveConstraintIds = new List<string> { DeckRuleEngine.StandardDeckSize },
                RestrictToCardPools = new List<string> { "local.empty" },
                AddConstraints = new List<DeckConstraintDefinition>
                {
                    new DeckConstraintDefinition
                    {
                        Id = "local.empty-size",
                        Kind = "deck-size",
                        Min = 0,
                        Max = 0
                    }
                }
            };
    }

    /// <summary>
    /// Failure fixture used to verify that experimental out-of-game effects
    /// fail closed without escaping into hub or matchmaking callers. It is not
    /// present in CardMap or any shipped feature manifest.
    /// </summary>
    [CardEffectId("99007")]
    public sealed class FailingDeckBuildingDevelopmentRule : CardEffect
    {
        public FailingDeckBuildingDevelopmentRule(GameCard card) : base(card) { }

        public override DeckBuildingRuleProposal OnDeckBuildingAdjust(DeckBuildingAdjustmentContext context)
            => throw new System.InvalidOperationException("Intentional deck-building rule fixture failure.");
    }
}
