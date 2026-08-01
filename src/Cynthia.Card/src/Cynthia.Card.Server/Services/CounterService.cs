using System;
using System.Threading;

namespace Cynthia.Card.Server
{
    public class CounterService
    {
        private int _value = 0;
        public event Action<int> OnValueChanged;

        public void Click()
        {
            var value = Interlocked.Increment(ref _value);
            OnValueChanged?.Invoke(value);
        }
        public int GetValue()
        {
            return Volatile.Read(ref _value);
        }
    }
}
