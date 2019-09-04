using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32010")]//叶奈法：女术士
    public class YenneferEnchantress : CardEffect
    {//生成1张己方上张打出的铜色/银色“法术”牌。
        public YenneferEnchantress(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.HistoryList.Where(x => x.PlayerIndex == Card.PlayerIndex &&
                x.CardId.CardInfo().Categories.Contains(Categorie.Spell) &&
                (x.CardId.CardInfo().Group == Group.Copper || x.CardId.CardInfo().Group == Group.Silver));
            if (cards.Count() == 0) return 0;
            return await Card.CreateAndMoveStay(cards.Last().CardId.Status.CardId);
        }
    }
}