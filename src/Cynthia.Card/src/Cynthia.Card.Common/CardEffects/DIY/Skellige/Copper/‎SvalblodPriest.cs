using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70135")]//斯瓦勃洛牧师 SvalblodPriest
    public class SvalblodPriest : CardEffect, IHandlesEvent<AfterCardHurt>, IHandlesEvent<AfterTurnOver>
    {
        public SvalblodPriest(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Target != Card || !Card.Status.CardRow.IsOnPlace() || @event.DamageType.IsHazard() || @event.Source.PlayerIndex != PlayerIndex )
            {
                return;
            }
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return;
            }
            await target.Effect.Damage(@event.Num, Card);
            return;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.CardPoint() < Card.Status.Strength/2)
            {
                await Card.Effect.Heal(Card);
            }
            return;
        }
    }
}
