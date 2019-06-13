using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13013")]//维瑟米尔
	public class Vesemir : CardEffect
	{//召唤“艾斯卡尔”和“兰伯特”。
		public Vesemir(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}