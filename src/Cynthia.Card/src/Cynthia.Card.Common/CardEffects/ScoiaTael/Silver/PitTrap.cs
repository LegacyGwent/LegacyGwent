using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53020")]//陷坑
	public class PitTrap : CardEffect
	{//在对方单排降下灾厄，对所有被影响的单位造成3点伤害。
		public PitTrap(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectRow(Card.PlayerIndex, Card, new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3});
            // await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.TorrentialRain);
            await Game.GameRowEffect[AnotherPlayer][result.Mirror().MyRowToIndex()]
                .SetStatus<PitTrapStatus>();
            return 0;
			
		}
	}
}