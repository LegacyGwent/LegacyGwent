using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13039")]//过期的麦酒
	public class ExpiredAle : CardEffect
	{//对敌军每排最强的单位造成6点伤害。
		public ExpiredAle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}