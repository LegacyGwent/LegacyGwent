using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70106")]//安德莱格虫卵
    public class EndregaEggs : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardDeath>
    {//在左侧生成1张原始同名牌。遗愿：在同排生成1张“安德莱格幼虫”。3回合后，回合结束时，摧毁自身。
        public EndregaEggs(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 3);
            for (var i = 0; i < 1; i++)
            {
                await Game.CreateCard(CardId.EndregaEggs, PlayerIndex, Card.GetLocation()/*, card => card.IsDoomed = true*/);
            }
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
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
                await Card.Effect.ToCemetery(CardBreakEffectType.ToCemetery);
            }

        }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.EndregaLarva, PlayerIndex, @event.DeathLocation);
            return;
        }
    }
}