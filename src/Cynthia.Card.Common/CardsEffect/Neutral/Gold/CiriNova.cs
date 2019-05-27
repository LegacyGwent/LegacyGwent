using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12032")]//希里：新星
	public class CiriNova : CardEffect
	{//若每张铜色牌在己方初始牌组中刚好有2张，则基础战力变为22点。
		public CiriNova(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}