using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34027")]//奴隶步兵
	public class SlaveInfantry : CardEffect
	{//在己方其他排生成1张佚亡原始同名牌。
		public SlaveInfantry(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}