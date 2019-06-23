using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24023")]//翼手龙
	public class Wyvern : CardEffect
	{//对1个敌军单位造成5点伤害。
		public Wyvern(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}