using System.Threading.Tasks;

namespace Cynthia.Card
{
    public interface IHandlesEvent<in TEvent> : IHandlesEvent
    where TEvent : Event
    {
        Task HandleEvent(TEvent @event);
    }
}