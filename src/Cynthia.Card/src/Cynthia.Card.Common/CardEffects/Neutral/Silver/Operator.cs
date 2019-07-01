using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13016")]//操作者
	public class Operator : CardEffect
	{//力竭。 休战：为双方各添加1张己方手牌1张铜色单位牌的原始同名牌。
		public Operator(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}