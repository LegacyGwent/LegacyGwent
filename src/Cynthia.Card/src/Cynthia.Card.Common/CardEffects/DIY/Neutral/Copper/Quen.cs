using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70001")]//昆恩护盾
    public class Quen : CardEffect
    {
        public Quen(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var unitHandCard = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.Status.Type == CardType.Unit &&
                    x.IsAnyGroup(Group.Copper, Group.Silver) &&
                    !HasPendingQuen(x)).ToList();
            if (unitHandCard.Count == 0) { return 0; }

            var targetcards = await Game.GetSelectMenuCards(PlayerIndex, unitHandCard, isCanOver: false);
            if (!targetcards.TrySingle(out var target)) { return 0; }

            var sameNameCards = Game.PlayersHandCard[PlayerIndex]
                .Concat(Game.PlayersDeck[PlayerIndex])
                .Where(x => x.CardInfo().CardId == target.CardInfo().CardId);
            foreach (var card in sameNameCards)
            {
                if (!HasPendingQuen(card))
                {
                    card.Effects.Add(new PendingQuenEffect(card));
                }
            }
            return 0;
        }

        private static bool HasPendingQuen(GameCard card)
        {
            return card.Effects.OfType<PendingQuenEffect>().Any(effect => effect.IsPending);
        }
    }

    public sealed class PendingQuenEffect : Effect, IHandlesEvent<AfterUnitLanded>
    {
        private readonly GameCard _card;

        public PendingQuenEffect(GameCard card)
        {
            _card = card;
        }

        public bool IsPending { get; private set; } = true;

        public async Task HandleEvent(AfterUnitLanded @event)
        {
            if (!TryConsume(@event))
            {
                return;
            }

            _card.Status.IsShield = true;
            await _card.Effect.Boost(2, null);
        }

        private bool TryConsume(AfterUnitLanded @event)
        {
            if (!IsPending || @event.Target != _card || !_card.IsAliveOnPlance() ||
                !_card.Status.CardRow.IsMyRow())
            {
                return false;
            }

            IsPending = false;
            Dispose();
            return true;
        }
    }
}
