using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54025")]//蓝山精锐
	public class BlueMountainElite : CardEffect
	{//召唤所有同名牌。 自身移动时获得2点增益。
		public BlueMountainElite(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}