using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13004")]//爱丽丝的同伴
	public class IrisCompanions : CardEffect
	{//将1张牌从牌组移至手牌，然后随机丢弃1张牌。
		public IrisCompanions(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}