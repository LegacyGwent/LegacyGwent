using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52001")]//萨琪亚
	public class Saskia : CardEffect
	{//用最多2张牌交换同等数量的铜色牌。
		public Saskia(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}