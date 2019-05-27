using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("25002")]//蟹蜘蛛幼虫
	public class ArachasHatchling : CardEffect
	{//召唤所有“蟹蜘蛛雄蛛”。
		public ArachasHatchling(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}