using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13020")]//多瑞加雷
	public class DorregarayOfVole : CardEffect
	{//不限阵营地创造1个铜色/银色“龙兽”或“野兽”单位。
		public DorregarayOfVole(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}