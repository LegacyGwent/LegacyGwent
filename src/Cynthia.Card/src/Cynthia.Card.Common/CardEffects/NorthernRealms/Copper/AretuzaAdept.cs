using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44026")]//艾瑞图萨学院学员
	public class AretuzaAdept : CardEffect
	{//从牌组随机打出1张铜色灾厄牌。
		public AretuzaAdept(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}