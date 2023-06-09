using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13018")]//德鲁伊控天者
	public class Vaedermakar : CardEffect
	{//生成“刺骨冰霜”、“蔽日浓雾”或“阿尔祖落雷术”。
		public Vaedermakar(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return await Card.CreateAndMoveStay(CardId.BitingFrost,CardId.TorrentialRain, CardId.AlzurSThunder);
		}
	}
}