using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("43018")]//萨宾娜的幽灵
	public class SabrinaSSpecter : CardEffect
	{//复活1个铜色“诅咒生物”单位。
		public SabrinaSSpecter(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}