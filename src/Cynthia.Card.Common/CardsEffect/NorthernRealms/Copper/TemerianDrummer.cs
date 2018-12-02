using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44022")]//泰莫利亚鼓手
	public class TemerianDrummer : CardEffect
	{//使1个友军单位获得6点增益。
		public TemerianDrummer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}