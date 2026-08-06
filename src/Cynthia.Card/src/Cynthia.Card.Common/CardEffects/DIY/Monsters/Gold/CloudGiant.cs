using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70170")]// CloudGiant
    public class CloudGiant : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterTurnOver>
    {
        private int _immuneTurns;

        public CloudGiant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            _immuneTurns = Game.GameRowEffect[AnotherPlayer]
                .Count(row => row.RowStatus == RowStatus.ImpenetrableFog);
            Card.Status.IsImmue = _immuneTurns > 0;
            await Game.ShowSetCard(Card);
            return 0;            
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _immuneTurns = System.Math.Max(0, _immuneTurns - 1);
            if (_immuneTurns == 0 && Card.Status.IsImmue)
            {
                Card.Status.IsImmue = false;
                await Game.ShowSetCard(Card);
            }
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var enemyRow = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .WhereAllHighest()
                .ToList();
            if (!enemyRow.TryMessOne(out var target, Game.RNG))
            {
                return;
            }

            var damage = (Card.Status.Strength + 1) / 2;
            await target.Effect.Damage(damage, Card);
            if (!target.Status.CardRow.IsOnPlace() || target.IsDead)
            {
                return;
            }

            var destinationRows = new[]
                {
                    RowPosition.MyRow1,
                    RowPosition.MyRow2,
                    RowPosition.MyRow3
                }
                .Where(row => row != target.Status.CardRow)
                .Where(row => Game.RowToList(target.PlayerIndex, row).Count < Game.RowMaxCount)
                .ToList();
            if (!destinationRows.TryMessOne(out var destinationRow, Game.RNG))
            {
                return;
            }

            await target.Effect.Move(
                new CardLocation(
                    destinationRow,
                    Game.RowToList(target.PlayerIndex, destinationRow).Count),
                Card);
        }
    }
}
