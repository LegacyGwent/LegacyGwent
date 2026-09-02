using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22002")]//战灵
    public class Draug : CardEffect
    {//将死去的单位复活为战力为1的“战鬼”，直至填满此排。
        public Draug(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Card.Status.CardRow;
            var availableSpace = Math.Max(0, Game.RowMaxCount - Game.RowToList(PlayerIndex, row).Count);
            var selectCount = Math.Min(8, Math.Min(availableSpace, Game.PlayersCemetery[PlayerIndex].Count));
            if (selectCount <= 0)
            {
                return 0;
            }

            var selected = await Game.GetSelectMenuCards(
                PlayerIndex,
                Game.PlayersCemetery[PlayerIndex].ToList(),
                selectCount,
                "选择最多8个复活目标",
                isCanOver: true);

            foreach (var target in selected)
            {
                await target.Effect.Transform(CardId.Draugir, Card, x => x.Status.Strength = 1);
                await target.Effect.Resurrect(new CardLocation(row, int.MaxValue), Card);
            }
            return 0;
        }
    }
}
