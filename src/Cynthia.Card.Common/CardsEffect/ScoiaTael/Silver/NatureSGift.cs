using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53019")]//自然的馈赠
	public class NatureSGift : CardEffect
	{//从牌组打出1张铜色/银色“特殊”牌。
		public NatureSGift(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}