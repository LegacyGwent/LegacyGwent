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
            //对方同排列表
            var row = Game.RowToList(PlayerIndex, @event.DeathLocation.RowPosition.Mirror()).ToList();
            foreach (var it in row)
            {
                await it.Effect.Damage(1, Card);

            }
        }
    }
}