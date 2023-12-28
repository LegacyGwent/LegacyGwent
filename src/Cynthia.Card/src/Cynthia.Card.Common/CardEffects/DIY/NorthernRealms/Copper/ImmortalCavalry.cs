using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70101")]//不朽者骑兵 ImmortalCavalry
    public class ImmortalCavalry : CardEffect, IHandlesEvent<AfterTurnOver>
    {//锁定自身，回合结束时使1个随机友军单位获得2点增益。
        public ImmortalCavalry(GameCard card) : base(card){}
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Lock(Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).Mess(RNG).Take(1).ToList();
            foreach (var card in cards)
            {
                if (card.Status.CardRow.IsOnPlace())
                    await card.Effect.Boost(2, Card);
            }
            return;

            
        }
    }
}
