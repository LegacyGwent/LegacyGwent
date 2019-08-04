using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63005")]//莫克瓦格
    public class Morkvarg : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterRoundOver>
    {//进入墓场时，复活自身，但战力削弱一半。
        public Morkvarg(GameCard card) : base(card) { }

        private bool _RoundoverResurrectFlag = false;

        private RowPosition _RoundOverResurrectRowPosition;

        //以下代码基于:死亡时或者小回合，本卡复活到原位。丢弃时，本卡复活到随机我方位置

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
            }
            //第二类情况，丢弃，随机复活到任何位置
            else
            {
                await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
            }

        }

        //小回合结束后，记录位置,让flag为真
        public async Task HandleEvent(AfterRoundOver @event)
        {
            //小回合结束时，如果不在场上，什么都不做
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            //在场上时，让flag为true，准备复活，记录rowposition
            _RoundoverResurrectFlag = true;
            _RoundOverResurrectRowPosition = Game.GetCardLocation(Card).RowPosition;
            await Task.CompletedTask;
            return;
        }


        public async Task HandleEvent(AfterTurnStart @event)
        {
            //flag为false时，什么都不做
            if (!_RoundoverResurrectFlag)
            {
                return;
            }
            //重置flag
            _RoundoverResurrectFlag = false;
            //削弱值向上取整
            var WeakenValue = (Card.Status.Strength + 1) / 2;
            //放逐，不复活
            if (Card.Status.Strength == WeakenValue)
            {
                await Card.Effect.Weaken(WeakenValue, Card);
                return;
            }
            await Card.Effect.Weaken(WeakenValue, Card);
            //复活到原位置所在行的
            await Card.Effect.Resurrect(new CardLocation(_RoundOverResurrectRowPosition, 0), Card);
            return;



        }




    }
}