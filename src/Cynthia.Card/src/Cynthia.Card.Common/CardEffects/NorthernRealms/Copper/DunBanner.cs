using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44023")]//褐旗营
	public class DunBanner : CardEffect
	{//回合开始时，若落后25点战力以上，则召唤此单位至随机排。
		public DunBanner(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}