using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62007")]//凯瑞丝·奎特
    public class Cerys : CardEffect, IHandlesEvent<AfterCardResurrect>
    {//位于墓场中时，在己方复活 4个单位后，复活单位，并获得1点强化。
        public Cerys(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardResurrect @event)
        {

            if (@event.Source.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInCemetery())
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    await Card.Effect.Resurrect(new CardLocation() { RowPosition = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true).RowPosition, CardIndex = int.MaxValue }, Card);
                    //重置计数器，复活到随机排最右侧
                    await Card.Effect.SetCountdown(value: 4);
                    await Card.Effect.Strengthen(1, Card);
                }
            }
            return;
        }
    }
}