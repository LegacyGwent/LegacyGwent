using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33019")]//达兹伯格符文石
    public class DazhbogRunestone : CardEffect
    {//创造1张铜色/银色“尼弗迦德”牌。
        public DazhbogRunestone(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(
                GwentMap.GetCreateCardsId(
                    x => x.Faction == Faction.Nilfgaard &&
                    x.CardId != Card.Status.CardId &&
                    (x.Group == Group.Copper || x.Group == Group.Silver) &&
                    !x.HasAnyCategorie(Categorie.Agent),
                    RNG
                )
                .ToList()
            );
        }
    }
}