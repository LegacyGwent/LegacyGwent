using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24007")]//狼人头领
	public class AlphaWerewolf : CardEffect
	{//接触“满月”效果时，在自身两侧各生成1只“狼”。
		public AlphaWerewolf(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
		public override async Task OnWeatherApply(int playerIndex, RowPosition row, RowStatus type)
		{
			// 这个判断是否同排有没有简单的方法呀
			if (((playerIndex == Card.PlayerIndex && row == Card.Status.CardRow) // 己方
				|| (playerIndex != Card.PlayerIndex && row.Mirror() == Card.Status.CardRow)) // 敌方
				&& type == RowStatus.FullMoon) // 满月
			{
				var left = Card.GetLocation(Card.PlayerIndex); // 左侧
				await Game.CreatCard("25007", Card.PlayerIndex, left);
				var right = Card.GetLocation(Card.PlayerIndex); // 右侧
				right.CardIndex += 1;
				await Game.CreatCard("25007", Card.PlayerIndex, right);
			}
		}
	}
}