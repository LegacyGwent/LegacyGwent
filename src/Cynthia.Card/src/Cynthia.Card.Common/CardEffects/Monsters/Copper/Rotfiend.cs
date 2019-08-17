using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24011")]//腐食魔
    public class Rotfiend : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：对对方同排所有单位造成2点伤害。
        public Rotfiend(GameCard card) : base(card) { }

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
                await it.Effect.Damage(2, Card);

            }
            return;
        }
    }
}