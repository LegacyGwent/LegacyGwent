using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54031")]//精灵利剑
	public class ElvenBlade : CardEffect
	{//对1个非“精灵”单位造成10点伤害。
		public ElvenBlade(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}