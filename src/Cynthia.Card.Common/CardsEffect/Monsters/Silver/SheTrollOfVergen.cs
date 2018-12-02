using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23019")]//维尔金的女巨魔
	public class SheTrollOfVergen : CardEffect
	{//从牌组打出1个铜色“遗愿”单位。吞噬它并获得其基础战力的增益。
		public SheTrollOfVergen(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}