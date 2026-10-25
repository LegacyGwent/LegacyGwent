using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    // Explicitly propagated by an activation and its nested countdown requests.
    // Physical card identity matters: distinct same-name copies may each activate.
    public sealed class CountdownTriggerChain
    {
        private readonly List<GameCard> _activatedCards = new List<GameCard>();

        public bool TryActivate(GameCard card)
        {
            if (_activatedCards.Any(activated => ReferenceEquals(activated, card))) return false;
            _activatedCards.Add(card);
            return true;
        }
    }

    // Requests one step of the target's countdown ability, including its zero
    // resolution. This is not a synthetic turn/Consume event.
    public sealed class TriggerCountdown : Event
    {
        public GameCard Target { get; }
        public CountdownTriggerChain Chain { get; }

        public TriggerCountdown(GameCard target, CountdownTriggerChain chain = null)
        {
            Target = target;
            Chain = chain ?? new CountdownTriggerChain();
        }
    }
}
