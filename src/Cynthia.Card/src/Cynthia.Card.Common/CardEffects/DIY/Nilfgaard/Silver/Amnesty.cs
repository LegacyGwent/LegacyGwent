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
            var cards = await Game.GetSelectPlaceCards(
                Card,
                filter: x => !x.Status.IsSpying &&
                    (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver),
                selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0) return 0;
            var target = cards.Single();
            await target.Effect.Lock(Card);
            await target.Effect.Lower_Power_By(target.CardPoint() - 1, Card);
            await Game.ShowCardMove(new CardLocation(RowPosition.EnemyHand, 0), target, refreshPoint: true);
            return 0;
        }
    }
}
