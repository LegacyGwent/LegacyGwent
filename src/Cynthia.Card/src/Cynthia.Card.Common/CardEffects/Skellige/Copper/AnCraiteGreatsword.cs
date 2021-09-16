using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64009")]//奎特家族巨剑士
    public class AnCraiteGreatsword : CardEffect, IHandlesEvent<AfterTurnStart>
    {//每2回合，若受伤，则在回合开始时治愈自身，并获得1点强化。
        public AnCraiteGreatsword(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 3);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    //重新倒计时
                    await Card.Effect.SetCountdown(value: 2);
                    //如果受伤，触发效果
                    if (Card.Status.HealthStatus < 0)
                    {
                       await Card.Effect.Heal(Card);
                       await Card.Effect.Strengthen(1, Card);

                    }
                }
            }
        }
    }
}