using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53008")]//玛丽娜
	public class Malena : CardEffect
	{//伏击：2回合后的回合开始时：翻开，在战力不超过5点的铜色/银色敌军单位中魅惑其中最强的一个。
		public Malena(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}