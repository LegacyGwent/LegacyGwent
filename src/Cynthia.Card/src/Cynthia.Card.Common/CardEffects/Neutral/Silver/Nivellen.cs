using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13006")]//纳威伦
    public class Nivellen : CardEffect
    {//将单排上的所有单位移至随机排。
        public Nivellen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = await Game.GetSelectRow(PlayerIndex, Card, TurnType.All.GetRow());
            var cards = Game.RowToList(PlayerIndex, row).IgnoreConcealAndDead().Where(x => x != Card);
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
            }
            return 0;
        }
    }
}