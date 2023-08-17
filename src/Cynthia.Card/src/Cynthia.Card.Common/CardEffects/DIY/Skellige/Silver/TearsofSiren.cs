using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70093")]//女海妖之泪
    public class TearsofSiren : CardEffect
    {//使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄。
        public TearsofSiren(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var tagetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x == x.RowStatus.TorrentialRainStatus())
                .Select(x => x.Key);
            foreach (var rowIndex in tagetRows)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<SkelligeStormStatus>();
            }
            var targetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => !x.Value.RowStatus.SkelligeStormStatus())
                .Select(x => x.Key);
            foreach (var rowIndex in targetRows)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<TorrentialRainStatus>();
            }
            return 0;
        }
    }
}
