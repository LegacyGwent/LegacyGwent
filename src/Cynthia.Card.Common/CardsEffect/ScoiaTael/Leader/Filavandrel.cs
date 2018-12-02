using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("51003")]//菲拉凡德芮
	public class Filavandrel : CardEffect
	{//创造1张银色“特殊”牌。
		public Filavandrel(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}