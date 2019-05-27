using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32015")]//暗算
	public class Assassination : CardEffect
	{//对1个敌军单位造成8点伤害，再对1个敌军单位造成8点伤害。
		public Assassination(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}