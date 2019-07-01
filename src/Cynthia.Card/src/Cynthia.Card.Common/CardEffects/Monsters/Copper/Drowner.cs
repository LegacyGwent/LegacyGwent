using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24014")]//水鬼
	public class Drowner : CardEffect
	{//将1个敌军单位拖至对方同排，对其造成2点伤害，若目标排处于灾厄之下，则伤害提高至4点。
		public Drowner(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}