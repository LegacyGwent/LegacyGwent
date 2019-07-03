using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32008")]//门诺·库霍恩
    public class MennoCoehoorn : CardEffect
    {
        public MennoCoehoorn(GameCard card) : base(card){}
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {//对1个敌军单位造成4点伤害。若它为间谍单位，则直接将其摧毁。
            var result = (await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow));
            if(result.Count!=0)
            {
                //如果是间谍
                if(result.Single().Status.IsSpying)
                {
                    await result.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
                else
                {
                    await result.Single().Effect.Damage(4,Card);
                }
            }
            return 0;
        }
    }
}