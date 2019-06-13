using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33011")]//彼得·萨尔格温利
	public class PeterSaarGwynleve : CardEffect
	{//重置1个友军单位，使其获得3点强化；或重置1个敌军单位，使其受到3点削弱。
		public PeterSaarGwynleve(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
            var cards = await Game.GetSelectPlaceCards(Card);
            if (cards.Count() == 0) return 0;
            var card = cards.Single();
            await card.Effect.Reset(Card);
            if (card.PlayerIndex == Card.PlayerIndex)
                await card.Effect.Strengthen(3, Card);
            else await card.Effect.Weaken(3, Card);
			return 0;
		}
	}
}