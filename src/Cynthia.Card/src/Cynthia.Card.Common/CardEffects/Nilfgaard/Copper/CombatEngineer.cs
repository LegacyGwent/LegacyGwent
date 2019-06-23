using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34018")]//作战工程师
	public class CombatEngineer : CardEffect
	{//使1个友军单位获得5点增益。操控。
		public CombatEngineer(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}