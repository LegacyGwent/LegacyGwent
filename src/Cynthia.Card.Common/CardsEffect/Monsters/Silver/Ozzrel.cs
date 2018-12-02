using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23015")]//欧兹瑞尔
	public class Ozzrel : CardEffect
	{//吞噬双方墓场中1个铜色/银色单位，获得其战力作为增益。
		public Ozzrel(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}