using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52005")]//卓尔坦·齐瓦
	public class ZoltanChivay : CardEffect
	{//选择3个单位，将它们移至所在半场的此排。使其中的友军单位获得2点强化；对其中的敌军单位造成2点伤害。
		public ZoltanChivay(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}