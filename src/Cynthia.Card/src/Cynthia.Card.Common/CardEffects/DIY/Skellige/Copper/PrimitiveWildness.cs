using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70112")]//原始野性
    public class PrimitiveWildness : CardEffect
    {//damage a non-cultist ally by 3 ,and then look at 2 random bronze cultisst, play 1 and strengthen it by 1.
        public PrimitiveWildness(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var targets = await Game.GetSelectPlaceCards(Card, filter: isNotCultist, selectMode: SelectModeType.MyRow);
            if (!targets.TrySingle(out var target))
            {
                return 0;
            }
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper && x.CardInfo().Categories.Contains(Categorie.Cultist)).Mess(RNG).Take(2);
            var playCard = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            if (playCard.Count() == 0) return 0;
            
            await targets.Single().Effect.Damage(3, Card);
            var cardplayed = playCard.Single();
            await cardplayed.Effect.Strengthen(1,Card);
            await cardplayed.MoveToCardStayFirst();
            return 1;
        }
        private bool isNotCultist(GameCard card)
        {
            return !card.CardInfo().Categories.Contains(Categorie.Cultist);
        }
    }
}
