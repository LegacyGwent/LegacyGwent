using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("61004")]//布兰王
	public class BranTuirseach : CardEffect
	{//从牌组丢弃最多3张牌，其中的单位牌获得1点强化。
		public BranTuirseach(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}