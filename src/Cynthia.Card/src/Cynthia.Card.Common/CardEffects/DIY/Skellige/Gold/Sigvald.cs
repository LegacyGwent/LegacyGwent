using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70038")]//西格瓦尔德
    public class Sigvald : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        private int _ownerTurnOvers;

        public Sigvald(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }

            _ownerTurnOvers++;
            if (_ownerTurnOvers < 2)
            {
                return;
            }

            _ownerTurnOvers = 0;
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            await Card.Effect.Strengthen(1, Card);
        }
    }
}
