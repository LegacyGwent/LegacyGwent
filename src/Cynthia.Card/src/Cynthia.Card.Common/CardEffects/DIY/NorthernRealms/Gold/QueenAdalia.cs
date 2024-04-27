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
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId);
            
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (Game.PlayerBaseDeck[PlayerIndex].Deck.Any(x => x.Faction == Faction.Neutral) == true)
            {
                return;
            }
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.Cintra)).Select(x => x.CardId).ToArray();
            for (var i = 0; i < cardsId.Count(); i++)
            {
                await Game.CreateCard(cardsId[i], Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            }
            return;
        }
    }
}
