using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("25005")]//鹰身女妖幼崽
	public class HarpyHatchling : CardEffect
	{//没有特殊技能。
		public HarpyHatchling(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}