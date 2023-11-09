using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70096")] //图尔赛克战船
    public class TuirseachWarship : CardEffect
    {
        //选择一个单位，造成等同于自身基础战力的伤害
        public TuirseachWarship (GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (list.Count <= 0) return 0;
            var card = list.Single();
            await card.Effect.Damage(damage,Card);

            return 0;
        }

        private int damage
        {
            get { return Card.Status.Strength; }
        }
    }
}