using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65005")]//威尔弗雷德
    public class Wilfred : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：使1个友军随机单位获得3点强化。
        public Wilfred(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var cards = Game.GetPlaceCards(Card.PlayerIndex).Where(x => x != Card);
            if (cards.Count() == 0)
            {
                return;
            }
            await cards.Mess(Game.RNG).First().Effect.Strengthen(3, Card);
            return;
        }
    }
}