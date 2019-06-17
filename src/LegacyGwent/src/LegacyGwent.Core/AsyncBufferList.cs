using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacyGwent
{
    public partial class AsyncBufferList<T> : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new Enumerator(this, cancellationToken);

        private TaskCompletionSource<object?> _onApproached = new TaskCompletionSource<object?>();

        private bool _finished = false;

        private List<T> _buffer = new List<T>();

        public void Reveice(params T[] items) => Reveice((IEnumerable<T>)items);

        public void Reveice(IEnumerable<T> items)
        {
            TaskCompletionSource<object?> source;

            lock (_buffer)
            {
                if (_finished)
                {
                    throw new InvalidOperationException();
                }

                _buffer.AddRange(items);
                source = _onApproached;
                _onApproached = new TaskCompletionSource<object?>();
            }

            source.SetResult(default);
        }

        public List<T> Finish()
        {
            lock (_buffer)
            {
                _finished = true;
                _onApproached.SetResult(null);
                return _buffer;
            }
        }

        private class Enumerator : IAsyncEnumerator<T>
        {
            private readonly AsyncBufferList<T> _target;

            public Enumerator(AsyncBufferList<T> target, CancellationToken cancellationToken)
            {
                _target = target;
                cancellationToken.Register(() =>
                {
                    _target._finished = true;
                    _target._onApproached.SetException(new TaskCanceledException());
                });
            }

            private int _index = -1;

            public T Current => _target._buffer[_index];

            public async ValueTask DisposeAsync() => await Task.CompletedTask;

            public async ValueTask<bool> MoveNextAsync()
            {
                _index++;
                if (_index >= _target._buffer.Count)
                {
                    if (_target._finished)
                    {
                        return false;
                    }
                    await _target._onApproached.Task;
                    if (_index >= _target._buffer.Count)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}