using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13009")]//赛浦利安·威利
	public class CyprianWiley : CardEffect
	{//对1个单位造成4点削弱。
		public CyprianWiley(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}