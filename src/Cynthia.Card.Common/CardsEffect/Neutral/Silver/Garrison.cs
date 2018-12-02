using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13044")]//军营
	public class Garrison : CardEffect
	{//创造对方起始牌组中的1张铜色/银色单位牌，并使它获得2点增益。
		public Garrison(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}