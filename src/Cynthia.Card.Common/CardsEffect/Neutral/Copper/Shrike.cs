using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14014")]//复仇
	public class Shrike : CardEffect
	{//对最多6个敌军随机单位造成2点伤害。
		public Shrike(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}