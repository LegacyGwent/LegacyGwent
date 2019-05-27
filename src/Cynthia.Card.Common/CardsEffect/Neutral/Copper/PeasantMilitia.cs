using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14027")]//农民民兵
	public class PeasantMilitia : CardEffect
	{//在己方单排生成3个“农民”单位。
		public PeasantMilitia(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectRow(Card.PlayerIndex,Card,TurnType.My.GetRow());
			var row = Game.RowToList(Card.PlayerIndex,result);
			for(var i = 0; i<3; i++)
			{
				if(row.Count<9)
					await Game.CreateCard("15011",Card.PlayerIndex,new CardLocation(result,row.Count));
			}
			return 0;
		}
	}
}