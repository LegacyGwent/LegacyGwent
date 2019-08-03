using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63005")]//莫克瓦格
    public class Morkvarg : CardEffect, IHandlesEvent<AfterCardDiscard>, IHandlesEvent<AfterCardDeath>
    {//进入墓场时，复活自身，但战力削弱一半。
        public Morkvarg(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDiscard @event)
        {
            //丢弃的不是本卡，什么都不发生
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
            //削弱并复活到卡牌所有者可打出位置中的随机一个
            await Card.Effect.Weaken(WeakenValue, Card);
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
            return;
        }


        public async Task HandleEvent(AfterCardDeath @event)
        {
            //死亡的不是本卡，什么都不发生
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
            //复活到原位置
            await Card.Effect.Weaken(WeakenValue, Card);
            await Card.Effect.Resurrect(@event.DeathLocation, Card);
            return;
        }




    }
}