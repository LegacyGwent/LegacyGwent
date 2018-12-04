using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14006")]//惊悚咆哮
	public class BloodcurdlingRoar : CardEffect
	{//摧毁1个友军单位。 生成1头“熊”。
		public BloodcurdlingRoar(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
			await Game.CreatCard("15010",Card.PlayerIndex,new CardLocation(RowPosition.MyStay,0));
			return 1;
		}
	}
}