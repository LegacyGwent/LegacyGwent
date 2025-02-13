using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70174")]//
    public class MageInfiltrator : CardEffect
    {//Spying. Reveal two enemy cards
        public MageInfiltrator(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)]
                .Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, isEnemyBack: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
            }

            return 0;
            var enemycard = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.IsReveal, filter: x=>(x.IsAnyGroup(Group.Coper)), selectMode: SelectModeType.Enemy);
            await Game.CreateCard(enemycard.CardId.Status.CardId, Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));

        }
    }
}