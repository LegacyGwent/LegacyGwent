using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34028")]//大使
    public class Ambassador : CardEffect
    {//间谍。使1个友军单位获得12点增益。
        public Ambassador(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, isEnemySwitch: true, selectMode: SelectModeType.EnemyRow);
            if (result.Count() == 0) return 0;
            await result.Single().Effect.Boost(12, Card);
            return 0;
        }
    }
}
