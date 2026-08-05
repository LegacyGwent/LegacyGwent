using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70113")]//克尔图里斯
    public class Keltullis : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Keltullis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Armor(2, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var alliedRow = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card && !x.Status.IsImmue)
                .WhereAllLowest()
                .ToList();
            if (!alliedRow.TryMessOne(out var sacrifice, Game.RNG))
            {
                return;
            }

            await sacrifice.Effect.ToCemetery(CardBreakEffectType.Scorch);
            await Card.Effect.Boost(1, Card);

            var enemyRow = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .Where(x => !x.Status.IsImmue && x.CardPoint() < Card.CardPoint())
                .WhereAllLowest()
                .ToList();
            if (enemyRow.TryMessOne(out var enemy, Game.RNG))
            {
                await enemy.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
        }
    }
}
