using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13038")]//白霜
	public class WhiteFrost : CardEffect
	{//在对方相邻两排降下灾厄。回合开始时，对所在排最弱的单位造成2点伤害。
		public WhiteFrost(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}