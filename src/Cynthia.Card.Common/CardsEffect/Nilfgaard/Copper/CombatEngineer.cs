using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34018")]//作战工程师
	public class CombatEngineer : CardEffect
	{//使1个友军单位获得5点增益。操控。
		public CombatEngineer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}