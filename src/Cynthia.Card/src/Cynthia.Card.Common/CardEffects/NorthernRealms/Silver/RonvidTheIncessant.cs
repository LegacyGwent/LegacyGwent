using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43004")]//没完没了的朗维德
    public class RonvidTheIncessant : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，复活至随机排，基础战力设为1点。 操控。
        public RonvidTheIncessant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Task.CompletedTask;
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }
            if (Card.Status.Strength > 1)
            {
                int offset = 1 - Card.Status.Strength;
                if (offset > 0)
                {
                    await Card.Effect.Strengthen(offset, Card);
                }
                else if (offset < 0)
                {
                    await Card.Effect.Weaken(-offset, Card);
                }
            }
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            return;

        }
    }
}