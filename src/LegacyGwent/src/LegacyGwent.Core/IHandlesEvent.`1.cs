using System.Threading.Tasks;

namespace LegacyGwent
{
    public interface IHandlesEvent<in TEvent> : IHandlesEvent
    where TEvent : Event
    {
        Task HandleEvent(TEvent @event);
    }
}