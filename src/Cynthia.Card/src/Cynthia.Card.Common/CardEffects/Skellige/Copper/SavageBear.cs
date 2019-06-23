using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64008")]//恶熊
	public class SavageBear : CardEffect
	{//对后续打出至对方半场的单位造成1点伤害。
		public SavageBear(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}