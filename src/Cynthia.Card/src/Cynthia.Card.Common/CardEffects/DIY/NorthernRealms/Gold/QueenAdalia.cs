using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70141")]//阿德莉亚女王
    public class QueenAdalia : CardEffect
    {
        public QueenAdalia(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.Cintra))
                .Select(x => x.CardId).ToArray();

            var createdCount = await Game.CreateAndMoveStay(PlayerIndex, cardsId);
            foreach (var cardId in cardsId)
            {
                await Game.CreateCard(
                    cardId,
                    PlayerIndex,
                    new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[PlayerIndex].Count));
            }

            return createdCount;
        }
    }
}
