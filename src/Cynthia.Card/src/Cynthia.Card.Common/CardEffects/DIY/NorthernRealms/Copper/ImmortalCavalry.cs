using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70101")]//不朽者骑兵 ImmortalCavalry
    public class ImmortalCavalry : CardEffect, IHandlesEvent<AfterCardBoost>, IHandlesEvent<AfterCardHurt>
    {//每当处于锁定状态的单位获得增益或受到伤害，获得1点增益。
        public ImmortalCavalry(GameCard card) : base(card){}

        public async Task HandleEvent(AfterCardBoost @event)
        {
            await BoostForLockedTarget(@event.Target);
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            await BoostForLockedTarget(@event.Target);
        }

        private async Task BoostForLockedTarget(GameCard target)
        {
            if (Card.Status.CardRow.IsOnPlace() && target.Status.IsLock)
            {
                await Card.Effect.Boost(1, Card);
            }
        }
    }
}
