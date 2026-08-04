using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70179")]//Obtain all boosts and armor from a non-Spying bronze or silver allied unit, then return it to your deck. After that, play a bronze or silver unit from your deck. Crew.
    public class QueenCalanthe : CardEffect
    { 
        public QueenCalanthe(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {   //Deploy: shuffle a non-gold ally back to your deck
            var result = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver, selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return 0;
            var mycard = result.Single();
            // Obtain all boosts and armor
            if (mycard.Status.HealthStatus > 0)
            {
                await Card.Effect.Drain(mycard.Status.HealthStatus, mycard);
            }
            int damagenum = mycard.Status.Armor;
            if (damagenum > 0)
            {
                await mycard.Effect.Damage(damagenum, Card);
                await Card.Effect.Armor(damagenum, Card);
            }
            mycard.Effect.Repair(true);
            

            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), mycard, refreshPoint: true);
            // then play a non-gold unit from your deck.
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit && 
                   (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0)
            {
                return 0;
            }

            //打出
            var playCard = cards.Single();
            await playCard.MoveToCardStayFirst();
            return 1;
        }
    }
}