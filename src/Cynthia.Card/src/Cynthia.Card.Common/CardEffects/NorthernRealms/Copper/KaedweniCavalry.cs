using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44004")]//科德温骑兵
	public class KaedweniCavalry : CardEffect
	{//摧毁1个单位的护甲。扣除的护甲值将被转化为自身增益。
		public KaedweniCavalry(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}