using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13022")]//获奖奶牛
	public class PrizeWinningCow : CardEffect
	{//遗愿：在同排生成1个“羊角魔”。
		public PrizeWinningCow(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task OnCardDeath(GameCard taget, CardLocation soure)
		{
			if(taget!=Card) return;
			await Game.CreateCard(CardId.Chort,PlayerIndex,soure);
			return;
		}
	}
}