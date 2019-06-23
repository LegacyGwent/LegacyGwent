using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12028")]//凤凰
	public class Phoenix : CardEffect
	{//复活1个铜色/银色“龙兽”单位。
		public Phoenix(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}