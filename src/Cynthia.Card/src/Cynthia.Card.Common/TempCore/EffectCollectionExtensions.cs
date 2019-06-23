using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cynthia.Card.Events;

namespace Cynthia.Card
{
    public static class EffectPoolExtensions
    {
        public static async Task<TEvent> RaiseEvent<TEvent>(this IHasEffects hasEffects, TEvent @event)
        where TEvent : Event
        {
            return await hasEffects.Effects.RaiseEvent(@event);
        }

        public static async Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, TEvent @event)
        where TEvent : Event
        {
            foreach (var item in effects.ToList())
            {
                if (item is IHandlesEvent<TEvent> he)
                {
                    if (!(@event is RaisingEvent))
                    {
                        if ((await effects.RaiseEvent<RaisingEvent>(@event, item)).IsCancelled)
                        {
                            continue;
                        }
                    }
                    await he.HandleEvent(@event);
                }
            }
            return @event;
        }

        public static Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects)
        where TEvent : Event, new()
        => effects.RaiseEvent(new TEvent());

        public static Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, Action<TEvent> action)
        where TEvent : Event, new()
        {
            var @event = new TEvent();
            action(@event);
            return effects.RaiseEvent(@event);
        }

        public static async Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, Func<TEvent, Task> action)
        where TEvent : Event, new()
        {
            var @event = new TEvent();
            await action(@event);
            return await effects.RaiseEvent(@event);
        }

        public static async Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, params object[] arguments)
        where TEvent : Event
        => await effects.RaiseEvent((TEvent)Activator.CreateInstance(typeof(TEvent), arguments));

        public static async Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, Action<TEvent> action, params object[] arguments)
        where TEvent : Event
        {
            var @event = (TEvent)Activator.CreateInstance(typeof(TEvent), arguments);
            action(@event);
            return await effects.RaiseEvent(@event);
        }

        public static async Task<TEvent> RaiseEvent<TEvent>(this IEnumerable<IHandlesEvent> effects, Func<TEvent, Task> action, params object[] arguments)
        where TEvent : Event
        {
            var @event = (TEvent)Activator.CreateInstance(typeof(TEvent), arguments);
            await action(@event);
            return await effects.RaiseEvent(@event);
        }
    }
}