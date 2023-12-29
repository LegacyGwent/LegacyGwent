using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70127")]//莫拉汉姆家仆从 VanMoorlehemServant
    public class VanMoorlehemServant : CardEffect
    {//对方手牌中每有1张金色牌则获得2点增益
        public VanMoorlehemServant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var enemyhand = Game.RowToList(PlayerIndex, RowPosition.EnemyHand);
            var cardscount = enemyhand.Where(x => x.IsAnyGroup(Group.Gold)).Count();
            for (var i = 0; i < cardscount; i++)
            {
                await Card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}
