using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("52008")]//米尔瓦
	public class Milva : CardEffect
	{//将双方最强的铜色/银色单位收回各自牌组。
		public Milva(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}