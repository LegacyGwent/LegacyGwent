using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("25004")]//鹰身女妖蛋
	public class HarpyEgg : CardEffect
	{//使吞噬自身的单位获得额外4点增益。 遗愿：在随机排生成1只“鹰身女妖幼崽”。
		public HarpyEgg(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}