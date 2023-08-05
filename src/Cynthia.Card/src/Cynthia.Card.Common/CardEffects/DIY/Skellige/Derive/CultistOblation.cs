using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70090")]//巨熊祭品 CultistOblation
    public class CultistOblation : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardDeath>
    {//2回合后的回合开始时，放逐自身。遗愿：治愈对方半场的1个受伤量最大的单位，并使其获得1点强化。
        public CultistOblation(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            if (!Card.Status.IsCountdown)
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                await Card.Effect.Banish();
            }
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target == Card)
            {
                var cards = Game.GetAllCard(PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace());
                var mycard = cards
                                .Where(x => x.PlayerIndex != PlayerIndex)
                                .OrderByDescending(x => x.Status.Strength - x.CardPoint())
                                .FirstOrDefault();
                if (mycard != null)
                {
                    await mycard.Effect.Heal(mycard);
                    await mycard.Effect.Strengthen(1,mycard);
                }
            }
        }
    }
}