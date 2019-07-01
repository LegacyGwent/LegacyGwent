using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("25005")]//鹰身女妖幼崽
	public class HarpyHatchling : CardEffect
	{//没有特殊技能。
		public HarpyHatchling(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}