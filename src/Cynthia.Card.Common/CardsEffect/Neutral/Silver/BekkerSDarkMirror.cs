using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13026")]//贝克尔的黑暗之镜
	public class BekkerSDarkMirror : CardEffect
	{//对场上最强的单位造成最多10点伤害（无视护甲），并使场上最弱的单位获得相同数值的增益。
		public BekkerSDarkMirror(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}