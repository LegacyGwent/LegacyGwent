using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53018")]//莫拉纳符文石
    public class MoranaRunestone : CardEffect
    {//创造1张铜色/银色“松鼠党”牌。
        public MoranaRunestone(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(
                GwentMap.GetCreateCardsId(
                    x => x.Faction == Faction.ScoiaTael &&
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