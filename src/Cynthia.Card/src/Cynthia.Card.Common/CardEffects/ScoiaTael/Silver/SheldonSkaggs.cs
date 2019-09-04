using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53004")]//谢尔顿·斯卡格斯
    public class SheldonSkaggs : CardEffect
    {//将同排所有友军单位移至随机排。每移动1个单位，便获得1点增益。
        public SheldonSkaggs(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Card.Status.CardRow;
            var cards = Game.RowToList(PlayerIndex, row).IgnoreConcealAndDead().Where(x => x != Card).ToList();
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
            var listCount = cards.Count();
            await Boost(listCount, Card);
            return 0;
        }
    }
}