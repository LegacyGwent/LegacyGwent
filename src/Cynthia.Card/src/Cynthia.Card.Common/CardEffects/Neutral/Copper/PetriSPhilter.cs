using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14010")]//佩特里的魔药
	public class PetriSPhilter : CardEffect
	{//使最多6个友军随机单位获得2点增益。
		public PetriSPhilter(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var cards = Game.GetAllCard(Card.PlayerIndex).Where(x=>x.Status.CardRow.IsOnPlace()&&x.PlayerIndex==Card.PlayerIndex).Mess().Take(6).ToList();
			foreach(var card in cards)
			{
				if(card.Status.CardRow.IsOnPlace())
					await card.Effect.Boost(2,Card);
			}
			return 0;
		}
	}
}