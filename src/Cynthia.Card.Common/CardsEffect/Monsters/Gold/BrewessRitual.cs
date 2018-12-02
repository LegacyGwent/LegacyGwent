using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("22014")]//煮婆：仪式
	public class BrewessRitual : CardEffect
	{//复活2个铜色遗愿单位。
		public BrewessRitual(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}