using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62010")]//凯瑞丝：无所畏惧
    public class CerysFearless : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//复活己方下张丢弃的单位牌。
        public CerysFearless(GameCard card) : base(card) { }
        //以下代码基于：自己丢弃自己的牌的时候才可以复活。



        //打出后获得复活计数

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 1);
            return 0;

        }


        public async Task HandleEvent(AfterCardDiscard @event)
        {

            if (@event.Target.PlayerIndex == Card.PlayerIndex && @event.Source.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown == 1 && @event.Target.Status.CardRow.IsInCemetery() )
            {
                await Card.Effect.SetCountdown(value: 0);
                await @event.Target.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);

            }
            return;
        }

    }
}