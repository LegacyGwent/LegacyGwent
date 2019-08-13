using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44027")]//蓝衣铁卫突击队
    public class BlueStripesCommando : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//有战力与自身相同的非同名“泰莫利亚”友军单位被打出时，从牌组召唤1张它的同名牌。
        public BlueStripesCommando(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterUnitPlay @event)
        {


            if (@event.PlayedCard.Status.CardId == Card.Status.CardId || @event.PlayedCard.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }
            int phealth = @event.PlayedCard.ToHealth().health;
            //获取可触发效果的本卡同名卡列表
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId && x.ToHealth().health == phealth);
            if (list.Count() == 0)
            {
                return;
            }
            //只召唤最后一个
            if (Card == list.ToList().Last())
            {
                await Card.Effect.Summon(new CardLocation(Game.GetRandomCanPlayLocation(Card.PlayerIndex).RowPosition, int.MaxValue), Card);
            }

            return;

        }

    }
}