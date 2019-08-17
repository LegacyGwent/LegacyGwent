using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("63004")]//“阿蓝”卢戈
    public class BlueboyLugos : CardEffect
    {//在对方单排生成1只“幽灵鲸”。
        public BlueboyLugos(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var allmylist = new List<RowPosition>() { RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            var allowlist = new List<RowPosition>();
            foreach (var row in allmylist)
            {
                if (Game.RowToList(AnotherPlayer, row).Count() < Game.RowMaxCount)
                {
                    allowlist.Add(row.Mirror());
                }
            }
            if (allowlist.Count() == 0)
            {
                return 0;
            }
            var resultrow = await Game.GetSelectRow(Card.PlayerIndex, Card, allowlist);
            await Game.CreateCardAtEnd(CardId.SpectralWhale, Card.PlayerIndex, resultrow);
            return 0;
        }
    }
}