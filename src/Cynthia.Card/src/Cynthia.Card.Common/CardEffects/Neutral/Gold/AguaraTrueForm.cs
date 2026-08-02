using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12030")]//狐妖：真身
    public class AguaraTrueForm : CardEffect
    {//生成1张己方起始牌组之外的铜色/银色“法术”牌。
        public AguaraTrueForm(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = GwentMap.GetGenerateCardsId(
                x => x.HasAllCategorie(Categorie.Spell) &&
                    x.IsAnyGroup(Group.Copper, Group.Silver),
                Card.GetMyBaseDeck().Select(x => x.CardId)).ToList();
            return await Card.CreateAndMoveStay(list);
        }
    }
}
