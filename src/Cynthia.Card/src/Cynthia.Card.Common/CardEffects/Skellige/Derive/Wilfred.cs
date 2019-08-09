using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Text.RegularExpressions;

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
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.PlayerIndex == Card.PlayerIndex && x.Status.CardRow.IsOnPlace() && x != Card).Mess(Game.RNG).ToList();

            if (cards.Count() == 0)
            {
                return;
            }
            var cardre = cards.First();
            await cardre.Effect.Strengthen(3, Card);
        }
    }
}