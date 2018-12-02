using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34016")]//射石机
	public class Mangonel : CardEffect
	{//对1个敌军随机单位造成2点伤害。己方每揭示1张牌，便再次触发此能力。
		public Mangonel(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}