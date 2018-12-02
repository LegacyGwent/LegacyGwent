using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("52002")]//萨琪亚萨司
	public class Saesenthessis : CardEffect
	{//增益自身等同于友军“矮人”单位数量；造成等同于友军“精灵”单位数量的伤害。
		public Saesenthessis(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}