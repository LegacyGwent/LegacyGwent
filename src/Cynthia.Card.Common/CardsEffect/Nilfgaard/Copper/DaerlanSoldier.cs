using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34023")]//迪尔兰士兵
	public class DaerlanSoldier : CardEffect
	{//被己方揭示时直接打出至随机排，然后抽1张牌。
		public DaerlanSoldier(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}