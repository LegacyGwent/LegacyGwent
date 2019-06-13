using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54022")]//玛哈坎守卫
	public class MahakamGuard : CardEffect
	{//使1个友军单位获得7点增益。
		public MahakamGuard(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}