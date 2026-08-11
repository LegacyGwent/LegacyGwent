using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33009")]//重弩海尔格
    public class HeftyHelge : CardEffect, IHandlesEvent<AfterCardReveal>
    {//对对方半场非同排上的所有敌军单位造成1点伤害。每当被己方揭示时，此能力可多生效1次。
        public HeftyHelge(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var repetitions = Card.Status.Countdown;
            var cards = Game.GetPlaceCards(AnotherPlayer)
                .Where(x => x.Status.Type == CardType.Unit && x.Status.CardRow != Card.Status.CardRow)
                .ToList();

            for (var i = 0; i < repetitions; i++)
            {
                foreach (var card in cards.Where(x => x.IsAliveOnPlance()).ToList())
                {
                    await card.Effect.Damage(1, Card, BulletType.FireBall);
                }
            }

            await Card.Effect.SetCountdown(0);
            return 0;
        }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Target != Card || @event.Source == null ||
                @event.Source.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }

            await Card.Effect.SetCountdown(offset: 1);
        }
    }
}
