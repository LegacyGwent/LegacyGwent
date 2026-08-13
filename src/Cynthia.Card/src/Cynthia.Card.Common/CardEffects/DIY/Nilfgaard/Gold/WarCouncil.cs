using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.WarCouncil)]
    public class WarCouncil : CardEffect
    {
        public WarCouncil(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var handCards = Game.PlayersHandCard[PlayerIndex].ToList();
            var deckCards = Game.PlayersDeck[PlayerIndex].ToList();
            if (handCards.Any() && deckCards.Any() &&
                (await Game.GetSelectMenuCards(PlayerIndex, handCards, 1)).TrySingle(out var swapHandCard) &&
                deckCards.TryMessOne(out var swapDeckCard, RNG))
            {
                await swapHandCard.Effect.Swap(swapDeckCard);
            }

            var playCount = await Card.CreateAndMoveStay(CardId.NilfgaardianGate);

            if (Game.IsPlayersPass[AnotherPlayer]) return playCount;

            await Game.CreateCard(
                CardId.BattlePreparation,
                PlayerIndex,
                new CardLocation(RowPosition.MyHand, int.MaxValue));

            var drawCards = await Game.PlayerDrawCard(
                AnotherPlayer,
                1,
                target => target.Status.Group == Group.Copper);
            foreach (var drawCard in drawCards)
            {
                await drawCard.Effect.Reveal(Card);
            }

            return playCount;
        }
    }
}
