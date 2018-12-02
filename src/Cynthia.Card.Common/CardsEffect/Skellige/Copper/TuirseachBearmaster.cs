using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64033")]//图尔赛克家族驯兽师
	public class TuirseachBearmaster : CardEffect
	{//生成1头“熊”。
		public TuirseachBearmaster(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}