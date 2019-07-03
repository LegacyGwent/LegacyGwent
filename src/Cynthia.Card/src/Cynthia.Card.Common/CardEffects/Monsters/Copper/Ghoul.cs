using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24029")]//食尸鬼
	public class Ghoul : CardEffect
	{//吞噬墓场中1个铜色/银色单位，获得等同于其战力的增益。
		public Ghoul(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}