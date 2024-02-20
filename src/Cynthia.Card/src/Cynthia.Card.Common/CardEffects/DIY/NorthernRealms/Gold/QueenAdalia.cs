using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70141")]//阿德莉亚女王
    public class QueenAdalia : CardEffect
    {//生成1个铜色辛特拉单位。
        public QueenAdalia(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.Cintra))
                .Select(x => x.CardId);
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
            
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (Game.PlayerBaseDeck[PlayerIndex].Deck.Any(x => x.Faction == Faction.Neutral) == true)
            {
                return;
            }
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count)), Card);
            return;
        }
    }
}
