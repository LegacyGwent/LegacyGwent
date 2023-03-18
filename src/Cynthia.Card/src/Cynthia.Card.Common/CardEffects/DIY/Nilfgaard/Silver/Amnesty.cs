using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70115")]//大赦 Amnesty
    public class Amnesty : CardEffect
    {//使1个铜色/银色敌军单位返回敌方手牌。
        public Amnesty(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = await Game.GetSelectPlaceCards
            (Card, filter: x => x.Status.Group == Group.Copper &&
                x.PlayerIndex != PlayerIndex && x.CardInfo().CardUseInfo == CardUseInfo.MyRow);
            if (cards.Count == 0) return 0;
            var targets = cards.Single();
            await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), targets, refreshPoint:true);
            return 0;
        }
    }
}
