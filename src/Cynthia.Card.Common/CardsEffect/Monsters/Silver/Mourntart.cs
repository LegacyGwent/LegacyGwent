using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23017")]//莫恩塔特
	public class Mourntart : CardEffect
	{//吞噬己方墓场所有铜色/银色单位。每吞噬1个单位，便获得1点增益。
		public Mourntart(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}