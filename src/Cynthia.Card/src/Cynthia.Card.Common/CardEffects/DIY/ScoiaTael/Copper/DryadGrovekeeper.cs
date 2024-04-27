using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70140")]//树精林卫 DryadGrovekeeper
    public class DryadGrovekeeper : CardEffect
    {//
        public DryadGrovekeeper(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow, filter: x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var targetpoint = target.CardPoint();
            await target.Effect.Transform(CardId.DryadGrovekeeper, Card, x => x.Status.Strength = targetpoint);
            var BoostList = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card && x.CardPoint() < Card.CardPoint()).ToList();
            foreach (var targets in BoostList)
            {
                await targets.Effect.Boost(1, Card);
            }
            return 0;
        }
    }
}
