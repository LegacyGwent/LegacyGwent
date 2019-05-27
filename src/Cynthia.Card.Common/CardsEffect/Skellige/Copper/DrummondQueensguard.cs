using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64025")]//德拉蒙家族女王卫队
	public class DrummondQueensguard : CardEffect
	{//复活所有同名牌。
		public DrummondQueensguard(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}