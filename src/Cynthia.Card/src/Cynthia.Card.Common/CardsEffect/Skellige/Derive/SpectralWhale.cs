using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("65004")]//幽灵鲸
	public class SpectralWhale : CardEffect
	{//回合结束时移至随机排，对同排所有其他单位造成1点伤害。 间谍。
		public SpectralWhale(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}