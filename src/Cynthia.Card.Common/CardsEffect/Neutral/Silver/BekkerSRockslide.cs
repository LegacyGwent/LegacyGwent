using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13041")]//贝克尔的岩崩术
	public class BekkerSRockslide : CardEffect
	{//对最多10个随机敌军单位造成2点伤害。
		public BekkerSRockslide(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var cards = Game.GetAllCard(Card.PlayerIndex)
				.Where(x=>x.Status.CardRow.IsOnPlace()&&x.PlayerIndex!=Card.PlayerIndex)
				.Mess().Take(10).ToList();
			foreach(var card in cards)
			{
				if(card.Status.CardRow.IsOnPlace())
					await card.Effect.Damage(2,Card);
			}
			return 0;
		}
	}
}