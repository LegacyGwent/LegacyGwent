using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13027")]//指挥号角
	public class CommanderSHorn : CardEffect
	{//使5个相邻单位获得3点增益。
		public CommanderSHorn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}