using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70138")]//树精的呵护 DryadsCaress
    public class DryadsCaress : CardEffect
    {//
        public DryadsCaress(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            int DryadCount = Game.GetAllCard(Card.PlayerIndex)
                .Where(x => x.Status.CardRow.IsOnPlace() && x.Status.Categories.Contains(Categorie.Dryad)).Count();
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Dryad)).Mess(RNG);
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            if (result.Count() == 0) return 0;
            
            await result.First().Effect.Boost(DryadCount, Card);
            await result.First().MoveToCardStayFirst();
            return 1;
        }
    }
}
