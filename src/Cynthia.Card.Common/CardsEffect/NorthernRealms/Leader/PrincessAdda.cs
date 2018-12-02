using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("41002")]//雅妲公主
	public class PrincessAdda : CardEffect
	{//创造1个铜色/银色“诅咒生物”单位。
		public PrincessAdda(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}