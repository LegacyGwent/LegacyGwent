using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44003")]//科德温中士
	public class KaedweniSergeant : CardEffect
	{//移除所在排的灾厄。 3点护甲。 操控。
		public KaedweniSergeant(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}