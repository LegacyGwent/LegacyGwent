using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70093")]//暴风雨
    public class Tempest : CardEffect
    {//使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄。
        public Tempest(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var tagetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus == RowStatus.TorrentialRain)
                .Select(x => x.Key);
            foreach (var rowIndex in tagetRows)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<SkelligeStormStatus>();
            }
            var tagetRow = Game.GameRowEffect[Game.AnotherPlayer(Card.PlayerIndex)].Indexed()
                .Where(x => x.Value.RowStatus == RowStatus.TorrentialRain)
                .Select(x => x.Key);
            foreach (var rowIndex in tagetRow)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[AnotherPlayer][rowIndex].SetStatus<SkelligeStormStatus>();
            }            
            var targetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus != RowStatus.SkelligeStorm)
                .Select(x => x.Key);
            foreach (var rowIndex in targetRows)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<TorrentialRainStatus>();
            }
            var targetRow = Game.GameRowEffect[Game.AnotherPlayer(Card.PlayerIndex)].Indexed()
                .Where(x => x.Value.RowStatus != RowStatus.SkelligeStorm)
                .Select(x => x.Key);
            foreach (var rowIndex in targetRow)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[AnotherPlayer][rowIndex].SetStatus<TorrentialRainStatus>();
            }
            return 0;
        }
    }
}