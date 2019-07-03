using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("21003")]//呢喃山丘
	public class WhisperingHillock : CardEffect
	{//创造1张铜色/银色“有机”牌。
		public WhisperingHillock(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}