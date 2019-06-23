using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24037")]//飞蜥
	public class Slyzard : CardEffect
	{//从己方墓场吞噬1个非同名铜色单位，并从牌组打出1张它的同名牌。
		public Slyzard(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}