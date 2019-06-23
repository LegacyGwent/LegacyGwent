using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44003")]//科德温中士
	public class KaedweniSergeant : CardEffect
	{//移除所在排的灾厄。 3点护甲。 操控。
		public KaedweniSergeant(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}