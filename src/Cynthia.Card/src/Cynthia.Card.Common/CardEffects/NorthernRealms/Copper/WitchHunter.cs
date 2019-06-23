using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44030")]//女巫猎人
	public class WitchHunter : CardEffect
	{//重置1个单位。若它为“法师”，则从牌组打出1张同名牌。
		public WitchHunter(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}