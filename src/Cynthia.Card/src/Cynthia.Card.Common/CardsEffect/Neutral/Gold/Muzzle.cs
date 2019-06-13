using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12036")]//嘴套
	public class Muzzle : CardEffect
	{//魅惑1个战力不高于8点的敌军铜色/银色单位。
		public Muzzle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}