using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70162")]//圣甲虫 Scarab
    public class Scarab : CardEffect,IHandlesEvent<AfterPlayerPass>
    {
        public Scarab(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterPlayerPass @event)
        {
            if (Card.PlayerIndex == @event.PlayerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Charm(Card);
        }
    }
}
