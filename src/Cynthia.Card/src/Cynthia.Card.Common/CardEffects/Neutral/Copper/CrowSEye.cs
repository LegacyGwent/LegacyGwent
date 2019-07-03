using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14021")]//乌鸦眼
	public class CrowSEye : CardEffect
	{//对对方每排最强的敌军单位造成4点伤害。己方墓场每有1张同名牌，便使伤害提高1点。
		public CrowSEye(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var damage = 4+(Game.PlayersCemetery[Card.PlayerIndex].ToList().Where(x=>x.Status.CardId==Card.Status.CardId).Count());
			foreach(var row in Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)].ToList())
			{
				var cards = row.WhereAllHighest();
				if(cards.Count()!=0)
				{
					await cards.Mess().First().Effect.Damage(damage,Card,BulletType.RedLight);
				}
			}
			return 0;
		}
	}
}