using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34012")]//奴隶猎人

	public class SlaveHunter : CardEffect
	{//魅惑1个战力不高于3点的铜色敌军单位。

		public SlaveHunter(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}