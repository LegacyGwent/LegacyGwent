using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43019")]//佐里亚符文石
    public class ZoriaRunestone : CardEffect
    {//创造1张铜色/银色“北方领域”牌。
        public ZoriaRunestone(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(GwentMap.GetCreateCardsId(x => x.Faction == Faction.NorthernRealms && x.CardId != Card.Status.CardId && (x.Group == Group.Copper || x.Group == Group.Silver) &&
                    !x.HasAnyCategorie(Categorie.Agent), RNG).ToList());
        }
    }
}