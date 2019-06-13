using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33022")]//尼弗迦德大门
	public class NilfgaardianGate : CardEffect
	{//从牌组打出1张铜色/银色“军官”牌，并使其获得1点增益。
		public NilfgaardianGate(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
            var list = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Categories.Contains(Categorie.Officer)&&
                (x.Status.Group==Group.Copper||x.Status.Group==Group.Silver))
                .ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0) return 0;
            await cards.Single().MoveToCardStayFirst();
            await cards.Single().Effect.Boost(1, Card);
            return 1;
        }
	}
}