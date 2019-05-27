using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13017")]//兰伯特
	public class Lambert : CardEffect
	{//召唤“维瑟米尔”和“艾斯卡尔”。
		public Lambert(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}