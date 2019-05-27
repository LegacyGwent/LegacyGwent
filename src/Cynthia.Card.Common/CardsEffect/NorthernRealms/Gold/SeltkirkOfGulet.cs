using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42003")]//古雷特的赛尔奇克
	public class SeltkirkOfGulet : CardEffect
	{//与1个敌军单位对决。 3点护甲。
		public SeltkirkOfGulet(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}