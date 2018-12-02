using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13016")]//操作者
	public class Operator : CardEffect
	{//力竭。 休战：为双方各添加1张己方手牌1张铜色单位牌的原始同名牌。
		public Operator(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}