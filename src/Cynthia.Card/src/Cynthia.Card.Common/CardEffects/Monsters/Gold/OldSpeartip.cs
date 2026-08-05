using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22003")]//老矛头
    public class OldSpeartip : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardTransform>
    {//对最多5个敌军同排单位造成2点伤害。己方回合开始时若对方同排单位不足3个，则沉睡。
        public OldSpeartip(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
            if (row.Count() < 3)
            {
                await Card.Effect.Transform(CardId.OldSpeartipAsleep, Card);
            }
        }

        public async Task HandleEvent(AfterCardTransform @event)
        {
            if (@event.Target != Card || !Card.IsAliveOnPlance() ||
                @event.Source.Status.CardId != CardId.OldSpeartip)
            {
                return;
            }

            foreach (var target in Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead().Mess(Game.RNG).Take(5))
            {
                await target.Effect.Damage(2, Card);
            }
        }
    }
}
