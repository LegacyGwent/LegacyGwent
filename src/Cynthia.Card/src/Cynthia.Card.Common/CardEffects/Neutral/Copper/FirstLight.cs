using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14009")]//破晓
	public class FirstLight : CardEffect
	{//择一：使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄；或从牌组随机打出1张铜色单位牌。
		public FirstLight(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			//生成晴空或重整
			await Card.CreateAndMoveStay(CardId.ClearSkies,CardId.Rally);
			return 1;
		}
	}
}