using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33016")]//弗林姆德
    public class Vreemde : CardEffect
    {//生成1个己方起始牌组之外的铜色尼弗迦德“士兵”单位。
        public Vreemde(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            return await Card.CreateAndMoveStay(
                GwentMap.GetGenerateCardsId(
                    x => x.Faction == Faction.Nilfgaard &&
                    x.Is(Group.Copper, CardType.Unit) &&
                    x.HasAllCategorie(Categorie.Soldier),
                    Card.GetMyBaseDeck().Select(x => x.CardId))
                .ToList()
            );
        }
    }
}
