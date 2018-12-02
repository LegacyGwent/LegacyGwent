using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24018")]//赛尔伊诺鹰身女妖
	public class CelaenoHarpy : CardEffect
	{//在左侧生成2枚“鹰身女妖蛋”。
		public CelaenoHarpy(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}