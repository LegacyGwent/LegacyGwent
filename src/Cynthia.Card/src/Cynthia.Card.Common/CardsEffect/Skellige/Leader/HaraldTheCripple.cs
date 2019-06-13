using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("61001")]//“瘸子”哈罗德
	public class HaraldTheCripple : CardEffect
	{//对对方同排的1个随机敌军单位造成1点伤害，再重复9次。
		public HaraldTheCripple(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}