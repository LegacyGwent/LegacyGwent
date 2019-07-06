using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12018")]//伊瓦拉夸克斯
    public class Ihuarraquax : CardEffect, IHandlesEvent<AfterTurnOver>
    {//对自身造成5点伤害。 当前战力等同于基础战力时，在回合结束时对3个敌方随机单位造成7点伤害。
        public Ihuarraquax(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(1);
            await Card.Effect.Damage(5, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace() || Card.Status.HealthStatus != 0 || Card.Status.Countdown > 0)
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            var cards = Game.GetPlaceCards(AnotherPlayer).Mess(Game.RNG).Take(3);
            foreach (var card in cards)
            {
                await card.Effect.Damage(7, Card);
            }
        }
    }
}