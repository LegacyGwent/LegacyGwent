using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44015")]//投石机
	public class Trebuchet : CardEffect
	{//对3个相邻敌军单位造成1点伤害。 驱动：伤害增加1点。
		public Trebuchet(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}