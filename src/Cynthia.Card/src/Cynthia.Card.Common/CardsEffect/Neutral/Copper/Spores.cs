using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14025")]//致命菌菇
	public class Spores : CardEffect
	{//对单排所有单位造成2点伤害，并清除其上的恩泽。
		public Spores(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectRow(Card.PlayerIndex,Card,TurnType.Enemy.GetRow());
			var row = Game.RowToList(Card.PlayerIndex,result).ToList();
			foreach(var card in row)
			{
				if(card.Status.CardRow.IsOnPlace())
				{
					await card.Effect.Damage(2,Card);
				}
			}
			if(Game.GameRowStatus[Game.AnotherPlayer(Card.PlayerIndex)][result.Mirror().MyRowToIndex()].IsBoon())
			{
				await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.None);
			}
			return 0;
		}
	}
}