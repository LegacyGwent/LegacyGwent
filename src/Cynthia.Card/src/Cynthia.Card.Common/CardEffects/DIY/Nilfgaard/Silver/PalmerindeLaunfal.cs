using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70111")]//PalmerindeLaunfal
    public class PalmerindeLaunfal : CardEffect
    {//打出1张铜色士兵牌，使其在手牌、牌组或己方半场所有同名牌获得2点增益。
        public PalmerindeLaunfal(GameCard card) : base(card) { }
   public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Soldier) &&
                     x.Status.Group == Group.Copper)
                  .Mess(Game.RNG)
                  .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
   
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0)
            {
                return 0;
            }

            //打出
            var playCard = cards.Single();
            

            var cardslist = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == playCard.Status.CardId).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cardslist)
            {
                await card.Effect.Boost(2, Card);
            }
            await playCard.MoveToCardStayFirst();

            return 1;
        }
 
    }
}


