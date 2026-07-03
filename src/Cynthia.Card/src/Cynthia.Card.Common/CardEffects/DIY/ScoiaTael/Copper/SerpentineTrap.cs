using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70175")]//
    public class SerpentineTrap : CardEffect
    {//选定敌方单位所在的一排，使其单位数量调整为4，优先移动战力最大的单位。对因此被移动的单位，造成1点伤害。
        public SerpentineTrap(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var row = await Game.GetSelectRow(PlayerIndex, Card, TurnType.All.GetRow());
            var cards = Game.RowToList(PlayerIndex, row).IgnoreConcealAndDead().Where(x => x != Card).OrderByDescending(x => x.Status.Strength + x.Status.HealthStatus).ToList();
            var targetRow = TurnType.My.GetRow();
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
