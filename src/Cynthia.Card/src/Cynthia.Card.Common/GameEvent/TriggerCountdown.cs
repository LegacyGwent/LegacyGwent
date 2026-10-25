namespace Cynthia.Card
{
    // Requests one step of the target's countdown ability, including its zero
    // resolution. This is not a synthetic turn/Consume event.
    public sealed class TriggerCountdown : Event
    {
        public GameCard Target { get; }

        public TriggerCountdown(GameCard target)
        {
            Target = target;
        }
    }
}
