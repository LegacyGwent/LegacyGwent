using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12020")]//刚特·欧迪姆
    public class GaunterODimm : CardEffect 
    {//Look at the top three cards of your deck, if there is one, play a unit with higher strength than self, then put all remaining cards at the bottom of your deck in any order.    public GaunterODimm(GameCard card) : base(card) { }
        public GaunterODimm(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //Look at the top three cards of your deck
            var list = Game.PlayersDeck[Card.PlayerIndex].Take(3).Where(x => x.CardPoint() > 7);
            //if there is one, play a unit with higher strength than self
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();

            //then put all remaining cards at the bottom of your deck in any order
            var remainingCards = list.Except(result);
            foreach (var remainingcard in remainingCards)
            {
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[Card.PlayerIndex].Count), remainingcard);
            }
            return 1;
        }
    }
}