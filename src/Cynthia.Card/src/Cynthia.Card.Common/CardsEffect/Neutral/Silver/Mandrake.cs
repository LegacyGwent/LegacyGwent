using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13040")]//曼德拉草
    public class Mandrake : CardEffect
    {//择一：治愈1个单位，使其获得6点强化；或重置1个单位，使其受到6点削弱。
        public Mandrake(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("曼德拉甘酒", "治愈1个单位，并使其获得6点强化。"),
                ("曼德拉根茎提取物", "重置1个单位，使其受到6点削弱。")
            );
            //选择场上任意一个单位
            var taget = await Game.GetSelectPlaceCards(Card);
            if (taget.Count <= 0) return 0;
            var tagetCard = taget.Single();
            //将单位重置,如果是第一个,强化,第二个削弱
            if (switchCard == 0)
            {
                await tagetCard.Effect.Heal(Card);
                await tagetCard.Effect.Strengthen(6, Card);
            }
            else if (switchCard == 1)
            {
                await tagetCard.Effect.Reset(Card);
                await tagetCard.Effect.Weaken(6, Card);
            }
            return 0;
        }
    }
}