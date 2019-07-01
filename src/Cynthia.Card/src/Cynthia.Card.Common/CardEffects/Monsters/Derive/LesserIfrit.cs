using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("25006")]//次级伊夫利特
	public class LesserIfrit : CardEffect
	{//对1个敌军随机单位造成1点伤害。
		public LesserIfrit(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}