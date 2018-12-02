using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("52001")]//萨琪亚
	public class Saskia : CardEffect
	{//用最多2张牌交换同等数量的铜色牌。
		public Saskia(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}