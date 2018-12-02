using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32013")]//卡西尔·迪弗林
	public class CahirDyffryn : CardEffect
	{//复活1张领袖牌。
		public CahirDyffryn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}