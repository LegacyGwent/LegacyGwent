using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70197")]
    public class NorthernRealmsDraug : CardEffect
    {
        public NorthernRealmsDraug(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Card.Status.CardRow;
            var candidates = Game.PlayersCemetery[PlayerIndex]
                .Where(target => target.CardInfo().CardType == CardType.Unit).ToList();
            var availableSpace = Math.Max(0, Game.RowMaxCount - Game.RowToList(PlayerIndex, row).Count);
            var selectCount = Math.Min(8, Math.Min(availableSpace, candidates.Count));
            if (selectCount == 0) return 0;

            var selected = await Game.GetSelectMenuCards(
                PlayerIndex, candidates, selectCount, "选择最多8个复活目标", isCanOver: true);
            foreach (var target in selected)
            {
                await target.Effect.Transform(CardId.Draugir, Card, x => x.Status.Strength = 1);
                await target.Effect.Resurrect(new CardLocation(row, int.MaxValue), Card);
            }
            return 0;
        }
    }
}
