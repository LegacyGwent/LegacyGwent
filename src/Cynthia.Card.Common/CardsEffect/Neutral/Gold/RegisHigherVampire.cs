using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12022")]//雷吉斯：高等吸血鬼
	public class RegisHigherVampire : CardEffect
	{//检视对方牌组3张铜色单位牌。选择1张吞噬，获得等同于其基础战力的增益。
		public RegisHigherVampire(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}