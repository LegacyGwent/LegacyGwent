using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14025")]//致命菌菇
	public class Spores : CardEffect
	{//对单排所有单位造成2点伤害，并清除其上的恩泽。
		public Spores(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}