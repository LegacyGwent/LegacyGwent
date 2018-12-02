using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32010")]//叶奈法：女术士
	public class YenneferEnchantress : CardEffect
	{//生成1张己方上张打出的铜色/银色“法术”牌。
		public YenneferEnchantress(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}