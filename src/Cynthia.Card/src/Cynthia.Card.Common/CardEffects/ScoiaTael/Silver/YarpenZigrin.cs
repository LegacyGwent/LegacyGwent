using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53007")]//亚尔潘·齐格林
    public class YarpenZigrin : CardEffect, IHandlesEvent<AfterUnitDown>
    {//坚韧。 每打出1个友军“矮人”单位，便获得1点增益。
        public YarpenZigrin(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Resilience(Card);
            return 0;
        }
        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target == Card) return;
            if (PlayerIndex == @event.Target.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.HasAllCategorie(Categorie.Dwarf))
            {
                await Boost(1, Card);
            }
        }
    }
}
