using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130060")]//纳威伦：晋升
    public class NivellenPro : CardEffect
    {//将单排以及对方同排的所有单位移至随机排。
        public NivellenPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = await Game.GetSelectRow(PlayerIndex, Card, TurnType.All.GetRow());
            var cards = Game.RowToList(PlayerIndex, row).IgnoreConcealAndDead().Where(x => x != Card);
            var cards2 = Game.RowToList(AnotherPlayer, row).IgnoreConcealAndDead().Where(x => x != Card);
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
            foreach (var card in cards2)
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