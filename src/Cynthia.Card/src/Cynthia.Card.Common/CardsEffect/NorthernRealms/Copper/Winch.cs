using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44033")]//绞盘
	public class Winch : CardEffect
	{//使所有己方半场的“机械”单位获得3点增益。
		public Winch(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}