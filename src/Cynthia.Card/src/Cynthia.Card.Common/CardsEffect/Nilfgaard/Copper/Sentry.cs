using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34013")]//哨卫
    public class Sentry : CardEffect
    {//使1个“士兵”单位的所有同名牌获得2点增益。

        public Sentry(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var result = await Game.GetSelectPlaceCards(Card, filter: (x => x.Status.Categories.Contains(Categorie.Soldier)), selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return 0;
            string cardId = result.Single().Status.CardId;
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardId == cardId && x.PlayerIndex == Card.PlayerIndex && x.Status.CardRow.IsOnPlace());
            foreach (var card in cards)
            {
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}