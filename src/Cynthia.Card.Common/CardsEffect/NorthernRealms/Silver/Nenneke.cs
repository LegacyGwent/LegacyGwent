using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("43006")]//南尼克
	public class Nenneke : CardEffect
	{//将墓场3张铜色/银色单位牌放回牌组。
		public Nenneke(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}