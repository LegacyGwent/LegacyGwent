using System;

namespace Cynthia.Card.Server
{
    public class CounterService
    {
        private int _value = 0;
        public event Action<int> OnValueChanged;

        public void Click()
        {
            _value++;
            OnValueChanged?.Invoke(_value);
        }
        public int GetValue()
        {
            return _value;
        }
    }
}