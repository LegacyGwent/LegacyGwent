using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43007")]//弗尔泰斯特之傲
    public class FoltestSPride : CardEffect
    {//对1个敌军单位造成2点伤害，并将其上移一排。 驱动：再次触发此能力。
        public FoltestSPride(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Card.Status.CardRow.IsOnPlace())
            {
                for (var i = 0; i < 1 + Card.GetCrewedCount(); i++)
                {
                    var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
                    if (result.Count() <= 0)
                    {
                        continue;
                    }
                    var target = result.Single();
                    await target.Effect.Damage(2, Card);
                    if (target.CardPoint()<=0 || target.Status.CardRow == RowPosition.MyRow3)
                    {
                        continue;
                    }
                    var tagetRow = target.Status.CardRow == RowPosition.MyRow1 ? RowPosition.MyRow2 : RowPosition.MyRow3;
                    if (Game.RowToList(target.PlayerIndex, tagetRow).Count() >= 9)
                    {
                        continue;
                    }
                    else
                    {
                        await target.Effect.Move(new CardLocation(tagetRow, 0), Card);
                    }
                }
            }
            return 0;
        }
    }
}