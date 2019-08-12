using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44014")]//加强型弩炮
    public class ReinforcedBallista : CardEffect
    {//对1个敌军单位造成2点伤害。 驱动：再次触发此能力。
        public ReinforcedBallista(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (Card.GetLocation().RowPosition.IsOnPlace())
            {
                for (var i = 0; i < 1 + Card.GetCrewedCount(); i++)
                {
                    var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                    if (!selectList.TrySingle(out var target))
                    {
                        return;
                    }
                    await target.Effect.Damage(2, Card);
                }
            }
            return;
        }
    }
}