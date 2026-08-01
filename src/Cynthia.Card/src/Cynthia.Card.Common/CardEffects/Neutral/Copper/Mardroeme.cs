using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14013")]//致幻菌菇
    public class Mardroeme : CardEffect
    {//择一：重置1个单位，并使其获得3点强化；或重置1个单位，使其受到3点削弱。
        public Mardroeme(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("突变诱发物", "重置1个单位，并使其获得3点强化；"),
                ("毒物", "重置1个单位，使其受到3点削弱；")
            );
            //选择场上任意一个单位
            var target = await Game.GetSelectPlaceCards(Card);
            if (target.Count <= 0) return 0;
            var tagetCard = target.Single();
            //将单位重置,如果是第一个,强化,第二个削弱
            await tagetCard.Effect.Reset(Card);
            if (switchCard == 0)
                await tagetCard.Effect.Strengthen(3, Card);
            else if (switchCard == 1)
                await tagetCard.Effect.Weaken(3, Card);
            return 0;
        }
    }
}