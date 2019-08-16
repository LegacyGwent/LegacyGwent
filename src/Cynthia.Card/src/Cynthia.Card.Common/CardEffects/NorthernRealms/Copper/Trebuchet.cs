using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44015")]//投石机
    public class Trebuchet : CardEffect
    {//对3个相邻敌军单位造成1点伤害。 驱动：伤害增加1点。
        public Trebuchet(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (Card.GetLocation().RowPosition.IsOnPlace())
            {
                int damagenum = 1 + Card.GetCrewedCount();
                var result = await Game.GetSelectPlaceCards(Card, range: 1, selectMode: SelectModeType.EnemyRow);
                if (result.Count <= 0)
                {
                    return;
                }
                foreach (var card in result.Single().GetRangeCard(1).ToList())
                {
                    if (card.Status.CardRow.IsOnPlace())
                        await card.Effect.Damage(damagenum, Card);
                }
            }
            return;
        }
    }
}