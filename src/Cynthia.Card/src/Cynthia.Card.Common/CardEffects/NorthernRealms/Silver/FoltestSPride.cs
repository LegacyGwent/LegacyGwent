using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43007")]//弗尔泰斯特之傲
    public class FoltestSPride : CardEffect
    {//对1个敌军单位造成3点伤害，并将其上移一排。 驱动：再次触发此能力。
        public FoltestSPride(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Card.Status.CardRow.IsOnPlace())
            {
                for (var i = 0; i < 1 + Card.GetCrewedCount(); i++)
                {
                    var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
                    if (!result.TrySingle(out var target))
                    {
                        continue;
                    }
                    var tagetRow = target.Status.CardRow == RowPosition.MyRow1 ? RowPosition.MyRow2 : RowPosition.MyRow3;
                    if (target.Status.CardRow != RowPosition.MyRow3)
                    {
                        await target.Effect.Move(new CardLocation(tagetRow, 0), Card);
                    }
                    await target.Effect.Damage(3, Card);
                }
            }
            return 0;
        }
    }
}