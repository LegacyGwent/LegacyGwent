using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("15013")]//晴空
    public class ClearSkies : CardEffect
    {//使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄。
        public ClearSkies(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var tagetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus.IsHazard())
                .Select(x => x.Key);
            foreach (var rowIndex in tagetRows)
            {
                foreach (var card in Game.PlayersPlace[Card.PlayerIndex][rowIndex])
                {
                    if (card.Status.HealthStatus < 0)
                    {
                        await card.Effect.Boost(2);
                    }
                }
            }
            foreach (var rowIndex in tagetRows)
            {
                // await Game.ApplyWeather(Card.PlayerIndex, rowIndex.IndexToMyRow(), RowStatus.None);
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
            }
            return 0;
        }
    }
}