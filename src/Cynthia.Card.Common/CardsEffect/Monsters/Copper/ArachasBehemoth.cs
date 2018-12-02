using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24010")]//蟹蜘蛛巨兽
	public class ArachasBehemoth : CardEffect
	{//每当友军单位吞噬1个单位，便在随机排生成1只“蟹蜘蛛幼虫”。 一共可生效4次。
		public ArachasBehemoth(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}