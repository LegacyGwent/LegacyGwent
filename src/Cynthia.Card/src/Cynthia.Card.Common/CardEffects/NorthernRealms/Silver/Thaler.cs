using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43001")]//塔勒
	public class Thaler : CardEffect
	{//间谍、力竭。 抽2张牌，保留1张，放回另1张。
		public Thaler(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}