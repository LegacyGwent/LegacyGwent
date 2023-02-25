using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70079")]//一个木箱战神
    public class AnCraiteWarlord : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//被丢弃时复活自身。
        public AnCraiteWarlord(GameCard card) : base(card) { }

        private const int boost = 1;

        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex)
            {
                var position = Card.Status.CardRow;
                if (position.IsInDeck() || position.IsInHand() || position.IsOnPlace())
                {
                    await Card.Effect.Boost(boost, Card);
                }
            };
        }
    }
}