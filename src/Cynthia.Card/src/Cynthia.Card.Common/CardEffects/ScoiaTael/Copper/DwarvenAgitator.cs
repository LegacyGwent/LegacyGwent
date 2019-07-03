using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54029")] //矮人煽动分子
    public class DwarvenAgitator : CardEffect
    {
        //随机生成1张牌组中非同名铜色“矮人”牌的原始同名牌。
        public DwarvenAgitator(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var deck = Game.PlayersDeck[PlayerIndex];
            var myId = Card.CardInfo().CardId;
            var cardsToPlay = deck.Where(x =>
                x.CardInfo().CardId != myId && x.CardInfo().Categories.Contains((Categorie.Dwarf)));
            var list = cardsToPlay.Mess().ToList();
            if (!list.Any()) return 0;
            var card = list.First();
            await Card.CreateAndMoveStay(card.CardInfo().CardId);
            return 1;
        }
    }
}