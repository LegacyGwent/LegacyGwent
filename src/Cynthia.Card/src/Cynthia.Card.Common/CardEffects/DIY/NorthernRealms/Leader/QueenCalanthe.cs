using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70179")]//Deploy: Play the lowest non-gold unit from your deck, thenshuffle a non-gold ally back to your deck without changing its power.
    public class QueenCalanthe : CardEffect
    { 
        public QueenCalanthe(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {   
            
            // Play the lowest non-gold unit from your deck.
            var list = Game.PlayersDeck[PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) &&
                    x.CardInfo().CardType == CardType.Unit).WhereAllLowest().ToList();
            if (list.Count() == 0) return 0;
            var moveCard = list.Mess(RNG).First();
            await moveCard.MoveToCardStayFirst();

            //then shuffle a non-gold ally back to your deck without changing its power
            var result = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver, selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return 0;
            var mycard = result.Single();
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), mycard, refreshPoint: false);
            return 1;
        }
    }
}