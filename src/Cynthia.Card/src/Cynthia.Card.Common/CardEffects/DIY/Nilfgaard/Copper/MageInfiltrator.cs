using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70174")]//
    public class MageInfiltrator : CardEffect
    {//Spying. Reveal two enemy cards, then spawn and play a copy of a different bronze unit in your opponent's hand
        public MageInfiltrator(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {   // Reveal two enemy cards
            var list = Game.PlayersHandCard[Card.PlayerIndex]
                .Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Game.AnotherPlayer(Card.PlayerIndex), list, 2, isEnemyBack: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
            }
            // then spawn and play a copy of a different bronze unit in your opponent's hand
            var enemycards = await Game.GetSelectPlaceCards(Card,1, true, filter: x => (x.Status.IsReveal && x.IsAnyGroup(Group.Copper) && x.Status.CardId != Card.Status.CardId && x.CardInfo().CardType == CardType.Unit), selectMode: SelectModeType.MyHand);
            if (enemycards.Count() == 0) return 0;
            var targetCard = enemycards.Single();
            await Game.CreateToStayFirst(targetCard.Status.CardId, Game.AnotherPlayer(Card.PlayerIndex));
            return 1;
        }
    }
}