using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70175")]//
    public class SerpentineTrap : CardEffect
    {//选定敌方单位所在的一排，使其单位数量调整为4，优先移动战力最大的单位。对因此被移动的单位，造成1点伤害。
        public SerpentineTrap(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect() 
        {
            var row = await Game.GetSelectRow(Card.PlayerIndex, Card, new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3 });
            var cards = Game.RowToList(PlayerIndex, row).IgnoreConcealAndDead().Where(x => x != Card).OrderByDescending(x => x.Status.Strength + x.Status.HealthStatus).ToList();
            int count = cards.Count;
            var targetRow = TurnType.My.GetRow();
            if (count < 4)
            {
                for (int i = 0; i < 4 - count; i++)
                {
                    var enemyboard = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.Status.CardRow != row.Mirror()).WhereAllHighest().ToList();
                    if (enemyboard.Count() == 0)
                    {
                        return 0;
                    }

                    var cardToMove = enemyboard.First();
                    await cardToMove.Effect.Move(new CardLocation(row.Mirror(), int.MaxValue), Card);
                    await cardToMove.Effect.Damage(1, Card);
                }
                return 0;
            }
            
            cards = cards.Take(count - 4).ToList();
                
            targetRow.Remove(row.IsMyRow() ? row : row.Mirror());
            foreach (var card in cards)
            {
                var canMoveRow = targetRow.Where(x => Game.RowToList(card.PlayerIndex, x).Count < Game.RowMaxCount);
                if (!canMoveRow.TryMessOne(out var target, Game.RNG))
                {
                    continue;
                }
                await card.Effect.Move(new CardLocation(target, Game.RowToList(card.PlayerIndex, target).Count), Card);
                await card.Effect.Damage(1, Card);
            }
            return 0;
        }

    }
}
