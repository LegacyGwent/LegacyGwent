using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13041")]//贝克尔的岩崩术
	public class BekkerSRockslide : CardEffect
	{//对最多10个随机敌军单位造成2点伤害。
		public BekkerSRockslide(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}