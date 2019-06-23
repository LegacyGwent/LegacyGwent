using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12031")]//雷吉斯
	public class Regis : CardEffect
	{//汲食1个单位的所有增益。
		public Regis(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
            var cards = await Game.GetSelectPlaceCards(Card, 1, filter: x => x.Status.HealthStatus > 0);
            if (cards.Count() == 0) return 0;
            var target = cards.Single();
            await Card.Effect.Drain(target.Status.HealthStatus, target);
			return 0;
		}
	}
}