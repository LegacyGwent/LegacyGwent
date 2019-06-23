using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53019")]//自然的馈赠
	public class NatureSGift : CardEffect
	{//从牌组打出1张铜色/银色“特殊”牌。
		public NatureSGift(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}