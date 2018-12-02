using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13040")]//曼德拉草
	public class Mandrake : CardEffect
	{//择一：治愈1个单位，使其获得6点强化；或重置1个单位，使其受到6点削弱。
		public Mandrake(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}