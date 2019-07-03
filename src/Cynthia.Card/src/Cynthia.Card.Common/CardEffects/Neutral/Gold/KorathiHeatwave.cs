using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12033")]//科拉兹热浪
    public class KorathiHeatwave : CardEffect
    {//灾厄降于对方全场。 回合开始时，对各排最弱的单位造成2点伤害。
        public KorathiHeatwave(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            for (var i = 0; i < 3; i++)
            {
                // await Game.ApplyWeather(AnotherPlayer, i, RowStatus.KorathiHeatwave);
                await Game.GameRowEffect[AnotherPlayer][i]
                .SetStatus<KorathiHeatwaveStatus>();
            }
            return 0;
        }
    }
}