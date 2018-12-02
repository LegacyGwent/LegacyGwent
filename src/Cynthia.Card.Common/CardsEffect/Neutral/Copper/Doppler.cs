using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14022")]//变形怪
	public class Doppler : CardEffect
	{//随机生成 1 张己方阵营中的铜色单位牌。
		public Doppler(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}