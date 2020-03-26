using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23020")]//戴维娜符文石
    public class DevenaRunestone : CardEffect
    {//创造1张铜色/银色“怪兽”牌。
        public DevenaRunestone(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(
                GwentMap.GetCreateCardsId(
                    x => x.Faction == Faction.Monsters &&
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