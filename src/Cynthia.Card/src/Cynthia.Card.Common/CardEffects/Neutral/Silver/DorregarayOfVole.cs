using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13020")]//多瑞加雷
    public class DorregarayOfVole : CardEffect
    {//不限阵营地创造1个铜色/银色“龙兽”或“野兽”单位。
        public DorregarayOfVole(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = GwentMap.GetCards()
                .Where(x => (x.Categories.Contains(Categorie.Draconid) || x.Categories.Contains(Categorie.Beast))
                        && (x.Group == Group.Copper || x.Group == Group.Silver))
                .Mess(RNG).Take(3).Select(x => x.CardId)
                .ToList();
            return await Card.CreateAndMoveStay(list);
        }
    }
}