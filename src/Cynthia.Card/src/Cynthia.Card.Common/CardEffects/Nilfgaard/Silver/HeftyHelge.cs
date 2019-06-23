using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33009")]//重弩海尔格
    public class HeftyHelge : CardEffect
    {//对对方半场非同排上的所有敌军单位造成1点伤害。若被揭示，则对所有敌军单位造成1点伤害。
        public HeftyHelge(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}