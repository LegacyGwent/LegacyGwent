using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12021")]//杰洛特：阿尔德法印
    public class GeraltAard : CardEffect
    {//选择3个敌军单位各造成3点伤害，并将它们上移1排。
        public GeraltAard(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 3, selectMode: SelectModeType.EnemyRow);
            foreach (var card in cards)
            {
                var currentrow = card.Status.CardRow;
                var row = (currentrow.MyRowToIndex() + 1).IndexToMyRow();
                // await Game.Debug($"currentrow:{currentrow},targetrow:{row}");
                // Siege row cannot move up (row becomes Banish); handle bonus before !IsOnPlace early-exit.
                if (currentrow == RowPosition.MyRow3)
                {
                    await card.Effect.Damage(2, Card);
                    continue;
                }
                if (!row.IsOnPlace())
                {
                    await Game.Debug("已经是最上一排,不产生位移");
                    continue;
                }
                // await Game.Debug($"产生位移,移动至目标:{row}");
                await card.Effect.Move(new CardLocation(row, int.MaxValue), Card);
            }
            foreach (var card in cards)
            {
                await card.Effect.Damage(3, Card);
            }
            return 0;
        }
    }
}