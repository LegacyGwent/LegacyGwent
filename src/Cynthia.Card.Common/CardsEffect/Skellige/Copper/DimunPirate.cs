using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64002")]//迪门家族海盗
	public class DimunPirate : CardEffect
	{//丢弃牌组中所有同名牌。
		public DimunPirate(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}