using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34031")]//文登达尔精锐
    public class VenendalElite : CardEffect
    {//与1个被揭示的单位互换战力。
        public VenendalElite(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.IsReveal, selectMode: SelectModeType.AllHand);
            if (cards.Count == 0)
            {
                return 0;
            }
            var target = cards.Single();
            var point = target.CardPoint() - Card.CardPoint();
            if (point < 0)
            {
                await Card.Effect.Damage(-point, target, isPenetrate: true);
                await target.Effect.Boost(-point, Card);
            }
            else if (point > 0)
            {
                await Card.Effect.Boost(point, target);
                await target.Effect.Damage(point, Card, isPenetrate: true);
            }
            return 0;
        }
    }
}