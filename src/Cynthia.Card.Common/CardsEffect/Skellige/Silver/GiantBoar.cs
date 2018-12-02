using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63008")]//大野猪
	public class GiantBoar : CardEffect
	{//随机摧毁1个友军单位，然后获得10点增益。
		public GiantBoar(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}