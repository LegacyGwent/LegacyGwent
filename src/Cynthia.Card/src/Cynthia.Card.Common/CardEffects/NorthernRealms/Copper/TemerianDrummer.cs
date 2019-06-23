using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44022")]//泰莫利亚鼓手
	public class TemerianDrummer : CardEffect
	{//使1个友军单位获得6点增益。
		public TemerianDrummer(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}