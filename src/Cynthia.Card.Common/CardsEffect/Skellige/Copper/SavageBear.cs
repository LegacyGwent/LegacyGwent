using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64008")]//恶熊
	public class SavageBear : CardEffect
	{//对后续打出至对方半场的单位造成1点伤害。
		public SavageBear(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}