using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53002")]//艾雷亚斯
    public class EleYas : CardEffect, IHandlesEvent<AfterPlayerDraw>
    {//被抽到或被收回牌组时获得2点增益。
     //被调度走,被调度来,被交换走,被交换来,被抽到
        public EleYas(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return 0;
        }

        //被抽到时增益2点
        public async Task HandleEvent(AfterPlayerDraw @event)
        {
            if (@event.DrawCard != Card)
            {
                return;
            }
            await Boost(2, Card);
        }
    }
}