using System.Collections;
using System.Collections.Generic;

namespace LegacyGwent
{
    public partial class AsyncBufferList<T> : IReadOnlyList<T>
    {
        public T this[int index] => ((IReadOnlyList<T>)_buffer)[index];

        public int Count => ((IReadOnlyList<T>)_buffer).Count;

        public IEnumerator<T> GetEnumerator() => ((IReadOnlyList<T>)_buffer).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<T>)_buffer).GetEnumerator();
    }
}