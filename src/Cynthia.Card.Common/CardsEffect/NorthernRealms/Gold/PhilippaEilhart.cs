using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("42013")]//菲丽芭·艾哈特
	public class PhilippaEilhart : CardEffect
	{//对敌军单位造成5、4、3、2、1点伤害。每次随机改变目标，无法对同一目标连续造成伤害。
		public PhilippaEilhart(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}