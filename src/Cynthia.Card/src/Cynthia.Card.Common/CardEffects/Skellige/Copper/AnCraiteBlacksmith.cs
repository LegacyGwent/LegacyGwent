using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64007")]//奎特家族盾牌匠
    public class AnCraiteBlacksmith : CardEffect
    {//使1个友军单位获得2点强化和2点护甲。
        public AnCraiteBlacksmith(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            await targets.First().Effect.Strengthen(2, Card);
            await targets.First().Effect.Armor(2, Card);
            return 0;
        }
    }
}