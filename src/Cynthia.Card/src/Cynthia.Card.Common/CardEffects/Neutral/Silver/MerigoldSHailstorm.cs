using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13032")]//特莉丝雹暴术
	public class MerigoldSHailstorm : CardEffect
	{//使单排所有铜色和银色单位的战力减半。
		public MerigoldSHailstorm(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectRow(Card.PlayerIndex,Card,TurnType.All.GetRow());
			var row = Game.RowToList(Card.PlayerIndex,result).ToList();
			foreach(var card in row)
			{
				if(card.Status.CardRow.IsOnPlace()&&(card.Status.Group==Group.Copper||card.Status.Group==Group.Silver))
				{
					var health = card.ToHealth().health;
					var damage = health%2!=0?health/2+1:health/2;
					await card.Effect.Damage(damage,Card,isPenetrate:true);
				}
			}
			return 0;
		}
	}
}