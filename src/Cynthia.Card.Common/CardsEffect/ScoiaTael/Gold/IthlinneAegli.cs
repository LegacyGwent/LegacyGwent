using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52011")]//伊斯琳妮
	public class IthlinneAegli : CardEffect
	{//从牌组打出1张铜色“法术”、恩泽或灾厄牌，重复其效果一次。
		public IthlinneAegli(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}