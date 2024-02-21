using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70157")]//紫藤花 Wisteria
    public class Wisteria : CardEffect
    {//
        public Wisteria(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.AllRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                if(target.CardPoint()%2 == 0)
                {
                    await target.Effect.Boost(6, Card);
                }
                if(target.CardPoint()%2 == 1)
                {
                    await target.Effect.Damage(6, Card);
                }
                
            }
            return 0;
        }
    }
}
