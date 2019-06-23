using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54024")]//战舞者
	public class Wardancer : CardEffect
	{//被交换时自动打出至随机排。
		public Wardancer(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}