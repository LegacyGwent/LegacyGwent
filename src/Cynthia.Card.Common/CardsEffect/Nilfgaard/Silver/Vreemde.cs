using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33016")]//弗林姆德
    public class Vreemde : CardEffect
    {//创造1个铜色尼弗迦德“士兵”单位。
        public Vreemde(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return await Card.CreateAndMoveStay(
                GwentMap.GetCreateCardsId(
                    x => x.Faction == Faction.Nilfgaard &&
                    (x.Group == Group.Copper) &&
                    (x.Categories.Contains(Categorie.Soldier))
                )
                .ToList()
            );
        }
    }
}