using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63005")]//莫克瓦格
    public class Morkvarg : CardEffect, IHandlesEvent<AfterCardToCemetery>
    {//进入墓场时，复活自身，但战力削弱一半。
        public Morkvarg(GameCard card) : base(card) { }

        //以下代码基于:死亡时，本卡复活到原位。小局结束或丢弃时，本卡复活到随机我方位置

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            //进入墓地的不是本卡，什么都不发生
            if (@event.Target != Card)
            {
                return;
            }

            //削弱值向上取整
            var WeakenValue = (Card.Status.Strength + 1) / 2;
            //放逐，不复活
            if (Card.Status.Strength == WeakenValue)
            {
                await Card.Effect.Weaken(WeakenValue, Card);
                return;
            }
            await Card.Effect.Weaken(WeakenValue, Card);
            //第一类情况，死亡进入墓地
            //复活到原位置
            if (@event.DeathLocation.RowPosition.IsOnPlace())
            {
                await Card.Effect.Resurrect(@event.DeathLocation, Card);
                return;
            }
            //第二类情况，丢弃，小局结束进入墓地，随机复活到任何位置
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
            return;

        }



    }
}