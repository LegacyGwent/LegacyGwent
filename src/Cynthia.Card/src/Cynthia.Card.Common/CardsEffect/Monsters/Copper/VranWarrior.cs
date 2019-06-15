using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24022")]//蜥蜴人战士
    public class VranWarrior : CardEffect
    {//吞噬右侧单位，获得其战力作为增益。 每2回合开始时，重复此能力。
        public VranWarrior(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var taget = Card.GetRangeCard(1,GetRangeType.HollowRight);
            Consume(taget.Single());
            return 0;
        }

        public override async Task OnTurnStart(int playerIndex)
        {
            if(playerIndex == Card.PlayerIndex&&Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.SetCountdown(offset:-1);
                if(Card.Effect.Countdown<=0)
                {
                    //触发效果
                    var taget = Card.GetRangeCard(1,GetRangeType.HollowRight);
                    Consume(taget.Single());
                    //重新倒计时
                    await Card.Effect.SetCountdown(value:2);
                }
            }
        }
    }
}