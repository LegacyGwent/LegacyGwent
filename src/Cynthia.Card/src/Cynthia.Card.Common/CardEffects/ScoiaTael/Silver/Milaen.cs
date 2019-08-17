using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53014")]//麦莉
    public class Milaen : CardEffect
    {//选定一排，做左右两侧末端的单位各造成6点伤害。
        public Milaen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var rowIndex = await Game.GetSelectRow(Card.PlayerIndex, Card,
                TurnType.Enemy.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, rowIndex).ToList();
            if (row.Count <= 0)
                return 0;
            var cardLeft = row.First();
            var cardRight = row.Last();
            await cardLeft.Effect.Damage(damage, Card);
            if (cardLeft == cardRight) return 0;
            await cardRight.Effect.Damage(damage, Card);
            return 0;
        }


        private const int damage = 6;
    }
}