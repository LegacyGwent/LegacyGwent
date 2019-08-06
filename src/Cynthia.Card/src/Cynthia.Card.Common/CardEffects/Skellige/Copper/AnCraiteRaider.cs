using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64024")]//奎特家族突袭者
    public class AnCraiteRaider : CardEffect, IHandlesEvent<AfterCardToCemetery>
    {//被丢弃时复活自身。
        public AnCraiteRaider(GameCard card) : base(card) { }
        //不知道为什么以下写法失效
        // public async Task HandleEvent(AfterCardDiscard @event)
        // {
        //     //如果被丢弃的不是本卡，什么都不发生
        //     if (@event.Target != Card)
        //     {
        //         return;
        //     }


        //     //随机复活到任何位置

        //     await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
        //     return;

        // }
        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            //进入墓地的不是本卡，什么都不发生
            if (@event.Target != Card)
            {
                return;
            }
            //第一类情况，死亡进入墓地，无效果
            if (@event.DeathLocation.RowPosition.IsOnPlace())
            {
                return;
            }
            //第二类情况，丢弃，随机复活到任何位置
            else
            {
                await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
            }

        }
    }
}