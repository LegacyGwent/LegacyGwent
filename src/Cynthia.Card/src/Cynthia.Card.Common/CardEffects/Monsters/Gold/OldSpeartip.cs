using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22003")]//老矛头
    public class OldSpeartip :CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardTransform>
    {//对最多5个敌军同排单位造成2点伤害。己方回合开始时若对方同排单位不足3个，则沉睡
    // Damage up to 5 enemy units on the opposite row by 2. At the start of your turn, if there are fewer than 3 enemy units on the opposite row, Sleep.
        public OldSpeartip(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnStart @event)
        {// if there are fewer than 3 enemy units on the opposite row, transform into Old Speartip: Asleep
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            
            
            if (@event.PlayerIndex != PlayerIndex)
            {
                var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
                if (row.Count() < 3)
                {
                    await Card.Effect.Transform(CardId.OldSpeartipAsleep, Card);
                }
            }
            return;
        }

        public async Task HandleEvent(AfterCardTransform @event)
        {
            if (@event.Target != Card || !Card.IsAliveOnPlance() || @event.Source.Status.CardId != CardId.OldSpeartipAsleep)
            {
                return;
            }
            foreach (var card in Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).Mess(Game.RNG).Take(5))
            {
                await card.Effect.Damage(2, Card);
            }
            return;
        }
    }
}