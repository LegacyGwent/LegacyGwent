using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14023")]//乱石飞舞
	public class RockBarrage : CardEffect
	{//对1个敌军单位造成7点伤害，并将其上移一排。若该排已满，则将其摧毁。
		public RockBarrage(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = (await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow));
            if(result.Count<=0) return 0;
			var taget = result.Single();
			await taget.Effect.Damage(7,Card);
			if(!taget.Status.CardRow.IsOnPlace()||Card.Status.CardRow==RowPosition.MyRow3)return 0;
			var tagetRow = taget.Status.CardRow==RowPosition.MyRow1?RowPosition.MyRow2:RowPosition.MyRow3;
			if(Game.RowToList(taget.PlayerIndex,tagetRow).Count>=9)
				await taget.Effect.ToCemetery(CardBreakEffectType.Scorch);
			else
				await taget.Effect.Move(new CardLocation(tagetRow,0),Card);
			return 0;
		}
	}
}