using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70101")]//不朽者骑兵 ImmortalCavalry
    public class ImmortalCavalry : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<OnGameStart>
    {//对局开始时改变自身锁定状态。每2回合开始时，改变自身锁定状态。
        public ImmortalCavalry(GameCard card) : base(card){}

        private int _ownerTurnStarts;

        public async Task HandleEvent(OnGameStart @event)
        {
            await Card.Effect.Lock(Card);
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _ownerTurnStarts++;
            if (_ownerTurnStarts >= 2)
            {
                _ownerTurnStarts = 0;
                await Card.Effect.Lock(Card);
            }
        }
    }
}
