using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12035")]//复原
	public class Renew : CardEffect
	{//复活己方1个非领袖金色单位。
		public Renew(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}