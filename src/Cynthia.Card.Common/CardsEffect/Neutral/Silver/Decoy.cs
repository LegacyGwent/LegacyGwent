using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13028")]//诱饵
	public class Decoy : CardEffect
	{//重新打出1个铜色/银色友军单位，并使它获得3点增益。
		public Decoy(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}