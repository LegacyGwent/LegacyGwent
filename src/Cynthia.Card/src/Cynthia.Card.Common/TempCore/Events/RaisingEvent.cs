namespace Cynthia.Card.Events
{
    public class RaisingEvent : Event
    {
        public RaisingEvent(Event @event, IHandlesEvent effect)
        {
            Event = @event;
            Effect = effect;
        }

        public Event Event { get; }

        public IHandlesEvent Effect { get; }

        public bool IsCancelled { get; set; } = false;
    }
}