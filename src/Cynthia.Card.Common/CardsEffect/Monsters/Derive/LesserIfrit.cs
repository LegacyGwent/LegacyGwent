using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("25006")]//次级伊夫利特
	public class LesserIfrit : CardEffect
	{//对1个敌军随机单位造成1点伤害。
		public LesserIfrit(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}