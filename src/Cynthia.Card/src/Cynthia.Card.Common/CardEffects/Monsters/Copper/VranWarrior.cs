using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24022")]//蜥蜴人战士
    public class VranWarrior : CardEffect, IHandlesEvent<AfterTurnStart>
    {//吞噬右侧单位，获得其战力作为增益。 每2回合开始时，重复此能力。
        public VranWarrior(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 2);

            var taget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            if (taget.Count() == 0)
            {
                return 0;
            }

            await Card.Effect.Consume(taget.Single());
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
                    //触发效果
                    var taget = Card.GetRangeCard(1, GetRangeType.HollowRight);
                    if (taget.Count() == 0)
                    {
                        return;
                    }

                    await Card.Effect.Consume(taget.Single());
                }
            }
        }
    }
}