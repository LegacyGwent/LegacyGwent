using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("65003")]//乌德维克之主
	public class LordOfUndvik : CardEffect
	{//遗愿：使“哈尔玛”获得10点增益。
		public LordOfUndvik(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}