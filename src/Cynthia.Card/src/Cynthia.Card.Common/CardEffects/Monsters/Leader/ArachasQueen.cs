using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("21002")]//蟹蜘蛛女王
	public class ArachasQueen : CardEffect
	{//吞噬3个友军单位，获得其战力作为增益。 免疫。
		public ArachasQueen(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}