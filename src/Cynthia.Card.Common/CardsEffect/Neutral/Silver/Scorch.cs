using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13034")]//烧灼
	public class Scorch : CardEffect
	{//摧毁所有最强的单位。
		public Scorch(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
			foreach(var card in cards)
			{
				await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
			}
			return 0;
		}
	}
}