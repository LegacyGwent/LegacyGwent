using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12025")]//杰洛特：亚登法印
    public class GeraltYrden : CardEffect
    {//重置单排所有单位，并移除它们的状态。
        public GeraltYrden(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.All.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result).ToList();
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    await card.Effect.Reset(Card);
                }
            }
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    card.Status.IsImmue = false;
                    if (card.Status.IsSpying)
                        await card.Effect.Spying(Card);
                }
            }
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    if (card.Status.IsLock)
                        await card.Effect.Lock(Card);
                }
            }
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    if (card.Status.IsResilience)
                        await card.Effect.Resilience(Card);
                }
            }
            return 0;
        }
    }
}