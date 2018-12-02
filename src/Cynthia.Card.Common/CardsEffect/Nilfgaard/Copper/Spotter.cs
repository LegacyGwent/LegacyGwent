using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34021")]//侦察员
	public class Spotter : CardEffect
	{//获得等同于1张被揭示铜色/银色单位牌基础战力的增益。
		public Spotter(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}