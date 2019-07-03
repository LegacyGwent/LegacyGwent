using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("21004")]//艾瑞汀
	public class EredinBrAccGlas : CardEffect
	{//生成1个铜色“狂猎”单位。
		public EredinBrAccGlas(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}