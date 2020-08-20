using System.Collections;
using System.Collections.Generic;

namespace Cynthia.Card
{
    public class Switcher<T> : IEnumerable<T>
    {

        public T Current { get => _content[_currentIndex]; }

        private IList<T> _content = new List<T>();

        private int _currentIndex = 0;

        public void Switch()
        {
            _currentIndex = (_currentIndex + 1) % _content.Count;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        public void Add(T item)
        {
            _content.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _content.GetEnumerator();
        }
    }
}