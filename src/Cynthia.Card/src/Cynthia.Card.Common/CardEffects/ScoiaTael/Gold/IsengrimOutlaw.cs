using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52013")]//伊森格林：亡命徒
	public class IsengrimOutlaw : CardEffect
	{//择一：从牌组打出1张铜色/银色“特殊”牌；或创造1个银色“精灵”单位。
		public IsengrimOutlaw(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}