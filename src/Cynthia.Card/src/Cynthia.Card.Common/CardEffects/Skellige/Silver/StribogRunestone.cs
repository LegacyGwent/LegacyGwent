using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63018")]//史璀伯格符文石
    public class StribogRunestone : CardEffect
    {//创造1张铜色/银色“史凯利格”牌。
        public StribogRunestone(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(GwentMap.GetCreateCardsId(x => x.Faction == Faction.Skellige && x.CardId != Card.Status.CardId && (x.Group == Group.Copper || x.Group == Group.Silver) && !x.HasAnyCategorie(Categorie.Agent), RNG).ToList());
        }
    }
}