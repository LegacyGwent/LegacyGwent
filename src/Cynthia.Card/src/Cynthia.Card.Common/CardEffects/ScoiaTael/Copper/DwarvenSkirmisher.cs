using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54015")] //矮人好斗分子
    public class DwarvenSkirmisher : CardEffect
    {
        //对1个敌军单位造成3点伤害。若没有摧毁目标，则获得3点增益。
        public DwarvenSkirmisher(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!list.Any()) return 0;
            var card = list.Single();
            await card.Effect.Damage(damage,Card);
            if (card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(boost,Card);
            }

            return 0;
        }

        private const int damage = 3;
        private const int boost = 3;
    }
}