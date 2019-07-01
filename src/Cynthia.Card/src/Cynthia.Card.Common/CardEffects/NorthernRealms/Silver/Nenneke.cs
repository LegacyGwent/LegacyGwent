using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43006")]//南尼克
	public class Nenneke : CardEffect
	{//将墓场3张铜色/银色单位牌放回牌组。
		public Nenneke(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}