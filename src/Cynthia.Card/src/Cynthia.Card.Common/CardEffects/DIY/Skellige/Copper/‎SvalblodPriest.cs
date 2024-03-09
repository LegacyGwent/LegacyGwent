using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70135")]//斯瓦勃洛牧师 SvalblodPriest
    public class SvalblodPriest : CardEffect, IHandlesEvent<AfterCardHurt>
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
    }
}
