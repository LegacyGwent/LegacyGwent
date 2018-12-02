using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12004")]//利维亚的杰洛特
	public class GeraltOfRivia : CardEffect
	{//没有特殊技能。
		public GeraltOfRivia(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}