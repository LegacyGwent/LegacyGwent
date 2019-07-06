using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("21001")]//达冈
	public class Dagon : CardEffect
	{//生成“蔽日浓雾”或“倾盆大雨”。
		public Dagon(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return await Card.CreateAndMoveStay(CardId.BitingFrost, CardId.TorrentialRain);
		}
	}
}