using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32002")]//沙斯希乌斯
    public class Xarthisius : CardEffect
    {//检视对方牌组，将其中1张牌置于底端。
        public Xarthisius(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.PlayersDeck[AnotherPlayer].ToList();
            var selectCard = await Game.GetSelectMenuCards(PlayerIndex, cards);
            if (selectCard.Count == 0) return 0;
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, cards.Count), selectCard.Single());
            return 0;
        }
    }
}