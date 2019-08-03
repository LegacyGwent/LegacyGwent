using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63011")]//“黑手”霍格
    public class HolgerBlackhand : CardEffect
    {//造成6点伤害。若摧毁目标，则使己方墓场中最强的单位获得3点强化。
        public HolgerBlackhand(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
			//以下代码基于：如果有多个最强单位，随机强化其中一个
			//选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(6, Card);
            if (target.IsDead)
            {	
				//列出我方最强墓地单位
				 var list = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit).WhereAllHighest();	
            }
            return 1;
        }
    }
}