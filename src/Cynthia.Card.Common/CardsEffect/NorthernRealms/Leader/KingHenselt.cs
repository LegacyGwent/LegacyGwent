using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("41004")]//亨赛特国王
	public class KingHenselt : CardEffect
	{//选择1个友军铜色“机械”或“科德温”单位，从牌组打出所有它的同名牌。 操控。
		public KingHenselt(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}