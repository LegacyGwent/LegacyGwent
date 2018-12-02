using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("21004")]//艾瑞汀
	public class EredinBrAccGlas : CardEffect
	{//生成1个铜色“狂猎”单位。
		public EredinBrAccGlas(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}