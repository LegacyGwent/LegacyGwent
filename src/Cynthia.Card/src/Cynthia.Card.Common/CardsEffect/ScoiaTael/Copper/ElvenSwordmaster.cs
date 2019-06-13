using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54021")]//精灵剑术大师
	public class ElvenSwordmaster : CardEffect
	{//对1个敌军单位造成等同自身战力的伤害。
		public ElvenSwordmaster(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}