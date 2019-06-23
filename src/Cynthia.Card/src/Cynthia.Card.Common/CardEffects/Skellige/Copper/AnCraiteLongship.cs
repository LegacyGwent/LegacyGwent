using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64018")]//奎特家族作战长船
	public class AnCraiteLongship : CardEffect
	{//对1个敌军随机单位造成2点伤害。己方每丢弃1张牌，便触发此能力一次。
		public AnCraiteLongship(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}