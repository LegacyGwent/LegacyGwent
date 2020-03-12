using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64040")]//奎特家族盔甲匠
    public class DiyCard1 : CardEffect
    {//治愈2个友军单位，并使其获得3点护甲。
        public DiyCard1(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选取至多两个单位，如果不选，什么都不做
            var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.MyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            //同时治愈和加护甲
            foreach (var target in targets)
            {
                await target.Effect.Heal(Card);
                await target.Effect.Armor(3, Card);
            }

            return 0;
        }
    }
}