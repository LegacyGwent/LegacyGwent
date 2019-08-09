using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("65001")]//汉姆多尔
	public class Hemdall : CardEffect
	{//摧毁场上所有单位，并移除所有恩泽和灾厄。
		public Hemdall(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}