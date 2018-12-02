using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23016")]//阿巴亚
	public class Abaya : CardEffect
	{//生成“倾盆大雨”、“晴空”或“蟹蜘蛛毒液”。
		public Abaya(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}