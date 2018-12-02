using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("43001")]//塔勒
	public class Thaler : CardEffect
	{//间谍、力竭。 抽2张牌，保留1张，放回另1张。
		public Thaler(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}