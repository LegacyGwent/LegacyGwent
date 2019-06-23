using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54021")] //精灵剑术大师
    public class ElvenSwordmaster : CardEffect
    {
        //对1个敌军单位造成等同自身战力的伤害。
        public ElvenSwordmaster(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (list.Count <= 0) return 0;
            var card = list.Single();
            await card.Effect.Damage(damage,Card);

            return 0;
        }

        private int damage
        {
            get { return Card.CardPoint(); }
        }
    }
}