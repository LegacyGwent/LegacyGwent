using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22014")]//煮婆：仪式
	public class BrewessRitual : CardEffect
	{//复活2个铜色遗愿单位。
		public BrewessRitual(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}