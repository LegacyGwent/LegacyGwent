using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70046")]//斯瓦勃洛狂信者
    public class SvalblodFanatic : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，对1个战力最低的敌军单位造成3点伤害，然后对自身造成3点伤害。
        public SvalblodFanatic(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (!Card.Status.CardRow.IsOnPlace()
            || @event.PlayerIndex != PlayerIndex)
            {
                return;
            }
            
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).WhereAllLowest().Mess(RNG).ToList();
            if (cards.Count() == 0)
            {
                return ;
            }
            await cards.Mess(RNG).First().Effect.Damage(3, Card);
            await Card.Effect.Damage(3, Card);
        }
    }
}
