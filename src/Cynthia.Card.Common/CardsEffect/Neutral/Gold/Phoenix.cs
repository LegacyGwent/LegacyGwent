using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12028")]//凤凰
	public class Phoenix : CardEffect
	{//复活1个铜色/银色“龙兽”单位。
		public Phoenix(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}