using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("35002")]//牛尸
	public class CowCarcass : CardEffect
	{//间谍。2回合后，己方回合结束时，摧毁同排所有其他最弱的单位，并放逐自身。
		public CowCarcass(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}