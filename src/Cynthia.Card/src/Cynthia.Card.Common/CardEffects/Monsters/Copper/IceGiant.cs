using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24021")]//寒冰巨人
    public class IceGiant : CardEffect, IHandlesEvent<AfterWeatherApply>
    {//场上每有一个“刺骨冰霜“灾厄效果，便获得3点增益。每有一个“刺骨冰霜“灾厄效果出现在场上，便获得3点增益。
        public IceGiant(GameCard card) : base(card) { }
        private const int increment = 3;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var count = Game.GameRowEffect.SelectMany(x => x.Select(x => x.RowStatus)).Where(x => x == RowStatus.BitingFrost).Count();
            await Boost(count * increment, Card);
            return 0;
        }
		public async Task HandleEvent(AfterWeatherApply @event)
        {
            //如果在场上且特效是霜
            if (Card.Status.CardRow.IsOnPlace() && @event.Type == RowStatus.BitingFrost)
            {
                await Boost(increment, Card);
            }
            return;
        }
    }
}