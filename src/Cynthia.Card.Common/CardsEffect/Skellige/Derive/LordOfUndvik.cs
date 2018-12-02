using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("65003")]//乌德维克之主
	public class LordOfUndvik : CardEffect
	{//遗愿：使“哈尔玛”获得10点增益。
		public LordOfUndvik(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}