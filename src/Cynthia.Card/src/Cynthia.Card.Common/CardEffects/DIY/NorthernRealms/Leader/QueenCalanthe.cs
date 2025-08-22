using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("xxx")]//Deploy: shuffle a non-gold ally back to your deck without changing its power, then play the lowest non-gold unit from your deck.
    public class QueenCalanthe : CardEffect
    {//Deploy: shuffle a non-gold ally back to your deck without changing its power
        public QueenCalanthe(GameCard card) : base(card) { }

        private bool isPlayCard = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, filter: (x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver), selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return 1;
            // result.Single().Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), result.Single(), refreshPoint: false);
            isPlayCard = true;
            return 1;
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        // then play the lowest non-gold unit from your deck.
        {
            if (!isPlayCard)
            {
                return;
            }
            var list = Game.PlayersDeck[PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver &&
                    x.CardInfo().CardType == CardType.Unit).WhereAllLowest().ToList();
            if (list.Count() == 0) return;
            var moveCard = list.Mess(RNG).First();
            await moveCard.MoveToCardStayFirst();
            return;
        }
    }
}