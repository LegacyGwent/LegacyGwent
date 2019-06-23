using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14011")]//蔽日浓雾
	public class ImpenetrableFog : CardEffect
	{//在对方单排降下灾厄。回合开始时，对所在排最强的单位造成2点伤害。
		public ImpenetrableFog(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectRow(Card.PlayerIndex,Card,new List<RowPosition>(){RowPosition.EnemyRow1,RowPosition.EnemyRow2,RowPosition.EnemyRow3});
			await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.ImpenetrableFog);
			return 0;
		}
	}
}