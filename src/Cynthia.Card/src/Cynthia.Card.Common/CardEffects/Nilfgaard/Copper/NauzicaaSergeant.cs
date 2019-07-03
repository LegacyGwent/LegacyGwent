using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34017")]//那乌西卡旅中士
    public class NauzicaaSergeant : CardEffect
    {//移除所在排的灾厄效果，并使1个友军单位或1个手牌中被揭示的单位获得3点增益。
        public NauzicaaSergeant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            // await Game.ApplyWeather(PlayerIndex, Card.Status.CardRow.MyRowToIndex(), RowStatus.None);
            var result = await Game.GetSelectPlaceCards
                            (Card, filter: x => x.Status.IsReveal || x.Status.CardRow.IsOnPlace(), selectMode: SelectModeType.My);
            if (result.Count() == 0) return 0;
            await result.Single().Effect.Boost(3, Card);
            return 0;
        }
    }
}