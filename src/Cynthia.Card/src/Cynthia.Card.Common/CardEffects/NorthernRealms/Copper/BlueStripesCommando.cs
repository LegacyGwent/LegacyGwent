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

            var target = @event.PlayedCard;
            if (target.Status.CardId == Card.Status.CardId)
            {
                return;
            }
            int phealth = target.ToHealth().health;
            //获取可触发效果的本卡同名卡列表
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId && x.ToHealth().health == phealth);
            if (list.Count() == 0)
            {
                return;
            }
            //只召唤第一个
            if (Card == list.First())
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
            }

            return;

        }

    }
}