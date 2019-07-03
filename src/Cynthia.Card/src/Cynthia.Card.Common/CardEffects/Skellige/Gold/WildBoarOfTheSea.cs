using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62005")]//海上野猪
	public class WildBoarOfTheSea : CardEffect
	{//回合结束时，使左侧单位获得1点强化，右侧单位收到1点伤害。5点护甲。
		public WildBoarOfTheSea(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}