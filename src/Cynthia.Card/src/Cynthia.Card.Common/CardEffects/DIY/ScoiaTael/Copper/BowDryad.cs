using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70114")]//长弓树精
    public class BowDryad : CardEffect
    {//摧毁1个基础战力不高于自身的敌军单位
        public BowDryad(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = (await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow));
            if(result.Count!=0)
            {
                if(result.Single().Status.Strength <= Card.Status.Strength)
                {
                    await result.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
