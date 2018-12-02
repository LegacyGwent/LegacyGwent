using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13008")]//乔尼
	public class Johnny : CardEffect
	{//丢弃1张手牌，并在手牌中添加1张对方起始牌组中颜色相同的原始同名牌。
		public Johnny(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}