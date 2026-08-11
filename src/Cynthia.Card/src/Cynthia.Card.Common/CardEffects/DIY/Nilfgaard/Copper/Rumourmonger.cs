using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Rumourmonger)]
    public class Rumourmonger : CardEffect
    {
        public Rumourmonger(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.IsPlayersPass[AnotherPlayer]) return 0;

            var topCopper = Game.PlayersDeck[AnotherPlayer]
                .FirstOrDefault(x => x.Status.Group == Group.Copper);
            if (topCopper == null) return 0;

            await Game.CreateCard(
                topCopper.Status.CardId,
                AnotherPlayer,
                new CardLocation(RowPosition.MyDeck, 0));
            await Game.DrawCard(1, 1);
            return 0;
        }
    }
}
