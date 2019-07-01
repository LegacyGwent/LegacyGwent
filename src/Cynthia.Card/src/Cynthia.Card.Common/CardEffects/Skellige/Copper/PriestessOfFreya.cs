using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64032")]//弗蕾雅女祭司
	public class PriestessOfFreya : CardEffect
	{//复活1个铜色“士兵”单位。
		public PriestessOfFreya(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}