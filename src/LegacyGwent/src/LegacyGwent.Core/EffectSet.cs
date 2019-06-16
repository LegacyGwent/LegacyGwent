using System;
using System.Collections.Generic;

namespace LegacyGwent
{
    public partial class EffectSet
    {
        public IHasEffects Target { get; }

        public EffectSet(IHasEffects target) => Target = target;

        public void Add(Effect item)
        {
            item.OnAttaching();
            item.Target = Target;
            item.Status = EffectStatus.Attached;
            _data.Add(item);
            item.OnAttached();
        }

        public void Clear()
        {
            var temp = new Effect[_data.Count];
            _data.CopyTo(temp);
            foreach (var item in temp)
            {
                DetachEffect(item);
            }
        }

        public bool Remove(Effect item)
        {
            if (!_data.Contains(item))
            {
                return false;
            }

            DetachEffect(item);
            return true;
        }

        private void DetachEffect(Effect item)
        {
            if (item.Status != EffectStatus.Attached)
            {
                throw new InvalidOperationException();
            }

            item.OnDetaching();
            item.Target = null;
            item.Status = EffectStatus.Disposed;
            _data.Remove(item);
            item.OnAttached();
        }
    }
}