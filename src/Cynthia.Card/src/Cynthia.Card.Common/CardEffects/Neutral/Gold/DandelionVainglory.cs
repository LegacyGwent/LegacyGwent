using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12011")]//丹德里恩：虚妄荣光
    public class DandelionVainglory : CardEffect
    {
        public DandelionVainglory(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.PlayersPlace[PlayerIndex].All(row => row.Count >= Game.RowMaxCount))
            {
                return 0;
            }

            var candidates = Game.PlayersHandCard[PlayerIndex]
                .Where(card => card.Status.CardId == CardId.GeraltOfRivia ||
                    card.Status.CardId == CardId.TrissMerigold)
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, candidates, 1, isCanOver: false))
                .TrySingle(out var target))
            {
                return 0;
            }

            var location = await Game.GetPlayCard(target);
            if (location.RowPosition.IsInCemetery())
            {
                await target.Effect.Discard(Card);
                return 0;
            }

            await target.Effect.Play(location);
            // Nested Play queues Deploy; draw only after that queued effect has run.
            await Game.AddTask(async () => await Game.PlayerDrawCard(PlayerIndex, 1));
            return 0;
        }
    }
}
