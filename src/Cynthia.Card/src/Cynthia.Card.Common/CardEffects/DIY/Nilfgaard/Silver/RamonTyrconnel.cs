using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70202")]//拉蒙·蒂尔康奈尔 Ramon Tyrconnel
    public class RamonTyrconnel : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardReveal>
    {
        public RamonTyrconnel(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(4, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInHand())
            {
                return;
            }
            await Card.Effect.Reveal(Card);
        }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Target != Card || @event.Source == null ||
                @event.Source.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return;
            }
            await target.Effect.Damage(4, Card);
        }
    }
}
