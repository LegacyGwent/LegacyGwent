using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53002")]//艾雷亚斯
    public class EleYas : CardEffect, IHandlesEvent<AfterPlayerDraw>,IHandlesEvent<AfterCardSwap>,IHandlesEvent<AfterMulliganDraw>,IHandlesEvent<AfterMulliganOut>
    {//被抽到或被收回牌组时获得2点增益。
     //被调度走,被调度来,被交换走,被交换来,被抽到
        public EleYas(GameCard card) : base(card) { }

        //被抽到时增益2点
        public async Task HandleEvent(AfterPlayerDraw @event)
        {
            await IsSelfBoost(@event.DrawCard);
        }

        //交换走
        public async Task HandleEvent(AfterCardSwap @event)
        {
            await IsSelfBoost(@event.HandCard);
        }

        public async Task HandleEvent(AfterMulliganOut @event)
        {
            await IsSelfBoost(@event.Target);
        }

        public async Task HandleEvent(AfterMulliganDraw @event)
        {
            await IsSelfBoost(@event.Target);
        }

        private async Task IsSelfBoost(GameCard target)
        {
            if(target!=Card)
            {
                return;
            }
            await Boost(2,Card);
        }
    }
}