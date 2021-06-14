using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("130180")]//德鲁伊控天者：晋升
	public class VaedermakarPro : CardEffect
	{//生成1张中立铜色/银色“灾厄”牌、“晴空”或“阿尔祖落雷术”。
		public VaedermakarPro(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return await Card.CreateAndMoveStay(CardId.SkelligeStorm,CardId.WhiteFrost,CardId.TorrentialRain,CardId.BitingFrost,CardId.ImpenetrableFog,CardId.AlzurSThunder);
		}
	}
}