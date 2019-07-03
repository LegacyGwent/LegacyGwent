using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13039")]//过期的麦酒
	public class ExpiredAle : CardEffect
	{//对敌军每排最强的单位造成6点伤害。
		public ExpiredAle(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			foreach(var row in Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)].ToList())
			{
				var cards = row.WhereAllHighest();
				if(cards.Count()!=0)
				{
					await cards.Mess().First().Effect.Damage(6,Card,BulletType.RedLight);
				}
			}
			return 0;
		}
	}
}