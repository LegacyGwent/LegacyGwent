using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44032")]//掠夺者斥候
	public class ReaverScout : CardEffect
	{//选择1个非同名友军铜色单位，从牌组打出1张它的同名牌。
		public ReaverScout(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}