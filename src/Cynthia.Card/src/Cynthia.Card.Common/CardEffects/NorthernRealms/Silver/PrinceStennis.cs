using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43016")]//斯坦尼斯王子
	public class PrinceStennis : CardEffect
	{//从牌组随机打出1张铜色/银色非间谍单位牌，使其获得5点护甲。 3点护甲。
		public PrinceStennis(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}