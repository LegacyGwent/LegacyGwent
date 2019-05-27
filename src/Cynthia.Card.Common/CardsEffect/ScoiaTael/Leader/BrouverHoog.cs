using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51004")]//布罗瓦尔·霍格
	public class BrouverHoog : CardEffect
	{//从牌组打出1张非间谍银色单位牌或铜色“矮人”牌。
		public BrouverHoog(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}