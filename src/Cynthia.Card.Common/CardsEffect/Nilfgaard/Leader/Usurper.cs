using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("31004")]//篡位者
	public class Usurper : CardEffect
	{//间谍。不限阵营地创造1张领袖牌，使其获得2点增益。
		public Usurper(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}