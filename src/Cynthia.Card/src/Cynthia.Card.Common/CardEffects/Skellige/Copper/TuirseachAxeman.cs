using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64022")]//图尔赛克家族斧兵
    public class TuirseachAxeman : CardEffect, IHandlesEvent<AfterCardHurt>
    {//对方同排每有1个敌军单位受到伤害，便获得1点增益。2点护甲。
        public TuirseachAxeman(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }
        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace() || @event.Target.Status.CardRow != Card.Status.CardRow)
            {
                return;
            }
            await Card.Effect.Boost(1, Card);
            return;
        }
    }
}