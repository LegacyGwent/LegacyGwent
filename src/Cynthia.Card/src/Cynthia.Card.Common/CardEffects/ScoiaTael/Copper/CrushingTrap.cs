using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54032")] //碎骨陷阱
    public class CrushingTrap : CardEffect
    {
        //使对方单排左右两侧末端的单位各受到6点伤害。
        public CrushingTrap(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardUseEffect()
        {
            var rowIndex = await Game.GetSelectRow(Card.PlayerIndex, Card,
                TurnType.Enemy.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, rowIndex).ToList();
            if (row.Count <= 0)
                return 0;
            var cardLeft = row.First();
            var cardRight = row.Last();
            await cardLeft.Effect.Damage(damage);
            if (cardLeft == cardRight) return 0;
            await cardRight.Effect.Damage(damage);
            return 0;
        }

        private const int damage = 6;
    }
}