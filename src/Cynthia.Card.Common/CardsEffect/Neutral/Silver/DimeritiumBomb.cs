using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13029")]//阻魔金炸弹
	public class DimeritiumBomb : CardEffect
	{//重置单排上所有的受增益单位。
		public DimeritiumBomb(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}