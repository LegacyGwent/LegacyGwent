using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacyGwent
{
    public static class EffectPool
    {
        public static async Task RaiseEvent<TEvent>(this IEnumerable<Effect> effects, TEvent @event, params Effect[] externalEffects)
        where TEvent : Event
        {
            foreach (IHandlesEvent<TEvent> effect in effects)
            {
                await effect.HandleEvent(@event);
            }
            foreach (IHandlesEvent<TEvent> effect in externalEffects)
            {
                await effect.HandleEvent(@event);
            }
        }

        public static Task RaiseEvent<TEvent>(this IEnumerable<Effect> effects, params Effect[] externalEffects)
        where TEvent : Event, new()
        => effects.RaiseEvent(new TEvent(), externalEffects);
    }
}