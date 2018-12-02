using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("42006")]//凯拉·梅兹
	public class KeiraMetz : CardEffect
	{//生成“阿尔祖落雷术”、“雷霆”或“蟹蜘蛛毒液”。
		public KeiraMetz(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}