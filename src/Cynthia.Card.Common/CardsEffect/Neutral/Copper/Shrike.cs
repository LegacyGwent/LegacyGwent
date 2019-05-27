using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14014")]//复仇
	public class Shrike : CardEffect
	{//对最多6个敌军随机单位造成2点伤害。
		public Shrike(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var cards = Game.GetAllCard(Card.PlayerIndex).Where(x=>x.Status.CardRow.IsOnPlace()&&x.PlayerIndex!=Card.PlayerIndex).Mess().Take(6).ToList();
			foreach(var card in cards)
			{
				if(card.Status.CardRow.IsOnPlace())
					await card.Effect.Damage(2,Card);
			}
			return 0;
		}
	}
}