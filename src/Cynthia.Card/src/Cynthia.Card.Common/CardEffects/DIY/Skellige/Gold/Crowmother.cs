using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect, IHandlesEvent<AfterCardDeath>
    {//
        public Crowmother(GameCard card) : base(card) { }
        // 在随机一行生成2只乌鸦。每当你的回合内有1只友方乌鸦被摧毁，便额外生成1只乌鸦。
        // Spawn 2 Crows on a random row. Spawn an additional Crow for every ally Crow destroyed during your turns.
        private int DestroyedCrowCount = 0;
        // public async Task HandleEvent(OnGameStart @event)
        // {
        //     await Card.Effect.SetCountdown(value: 0);
        // }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (int i = 0; i < 2 + DestroyedCrowCount; i++)
            {
                await Game.CreateCard(CardId.Crow, PlayerIndex, Game.GetRandomCanPlayLocation(PlayerIndex, true));
            }
            return 0;
        }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (Game.GameRound.ToPlayerIndex(Game) == PlayerIndex && @event.Target.Status.CardId == CardId.Crow && @event.Target.PlayerIndex == Card.PlayerIndex)
            {
                DestroyedCrowCount++;
                await Card.Effect.SetCountdown(value: DestroyedCrowCount);
            }
            return;
        }
    }
}
