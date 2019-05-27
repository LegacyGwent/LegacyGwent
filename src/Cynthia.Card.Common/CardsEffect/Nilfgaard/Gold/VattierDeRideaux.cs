using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32003")]//瓦提尔·德·李道克斯
	public class VattierDeRideaux : CardEffect
	{//揭示最多2张己方手牌，再随机揭示相同数量的对方卡牌
		public VattierDeRideaux(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}