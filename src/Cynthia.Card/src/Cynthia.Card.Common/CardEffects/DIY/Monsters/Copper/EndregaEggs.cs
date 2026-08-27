using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70106")]//安德莱格虫卵
    public class EndregaEggs : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardDeath>
    {//在左侧生成1张原始同名牌。遗愿：在同排生成1个“安德莱格幼虫”，使牌组中的“安德莱格女王”获得1点强化。3回合后的回合结束时，摧毁自身。
        public EndregaEggs(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 3);
            // The generated Egg is a normal copy. Do not emulate Doomed with a
            // display-only category: that left stale death/lock event state.
            await Game.CreateCard(CardId.EndregaEggs, PlayerIndex, Card.GetLocation());
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

            // Strengthening a Queen can summon it and therefore mutate the deck.
            // Snapshot first so multiple copies and the 10-strength threshold are safe.
            foreach (var queen in Game.PlayersDeck[PlayerIndex]
                .Where(card => card.Status.CardId == CardId.EndregaQueen)
                .ToList())
            {
                await queen.Effect.Strengthen(1, Card);
            }
        }
    }
}
