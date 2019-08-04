using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65006")]//威尔海姆
    public class Wilhelm : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：对对方同排所有单位造成1点伤害。
        public Wilhelm(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var row = Game.RowToList(AnotherPlayer, Card.GetLocation().RowPosition).ToList();
            foreach (var it in row)
            {
                await it.Effect.Damage(1, Card);
            }
            return;
        }
    }
}