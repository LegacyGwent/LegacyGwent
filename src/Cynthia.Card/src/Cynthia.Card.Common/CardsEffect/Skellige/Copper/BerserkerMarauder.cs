using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64005")]//狂战士掠夺者
	public class BerserkerMarauder : CardEffect
	{//场上每有1个受伤、或为“诅咒生物”的友军单位，便获得1点增益。
		public BerserkerMarauder(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}