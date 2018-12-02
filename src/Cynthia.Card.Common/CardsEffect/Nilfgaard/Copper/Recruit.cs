using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34032")]//新兵
	public class Recruit : CardEffect
	{//从牌组随机打出1张非同名“铜色”士兵牌。
		public Recruit(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}