using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54029")]//矮人煽动分子
	public class DwarvenAgitator : CardEffect
	{//随机生成1张牌组中非同名铜色“矮人”牌的原始同名牌。
		public DwarvenAgitator(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}