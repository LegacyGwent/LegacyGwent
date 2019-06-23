using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34031")]//文登达尔精锐
	public class VenendalElite : CardEffect
	{//与1个被揭示的单位互换战力。
		public VenendalElite(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}