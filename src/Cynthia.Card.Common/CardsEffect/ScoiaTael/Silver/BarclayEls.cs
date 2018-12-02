using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53017")]//巴克莱·艾尔斯
	public class BarclayEls : CardEffect
	{//从牌组打出1张随机铜色/银色矮人牌，并使其获得3点强化。
		public BarclayEls(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}