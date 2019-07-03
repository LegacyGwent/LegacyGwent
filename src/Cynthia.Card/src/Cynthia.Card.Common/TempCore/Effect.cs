using System;

namespace Cynthia.Card
{
    public abstract class Effect : IDisposable, IHandlesEvent
    {
        public IHasEffects Target { get; internal set; }

        public EffectStatus Status { get; internal set; }

        public void Dispose()
        => Target?.Effects.Remove(this);

        protected internal virtual void OnAttaching() { }

        protected internal virtual void OnAttached() { }

        protected internal virtual void OnDetaching() { }

        protected internal virtual void OnDetached() { }
    }
}