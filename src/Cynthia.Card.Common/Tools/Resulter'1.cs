using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cynthia.Card.Common
{
    public class Resulter<T>
    {
        private TaskCompletionSource<T> _source = new TaskCompletionSource<T>();
        public TaskAwaiter<T> GetAwaiter() => _source.Task.GetAwaiter();
        public Task Result(T result)
        {
            var oldSource = _source;
            _source = new TaskCompletionSource<T>();
            return Task.Run(() => oldSource.SetResult(result));
        }
    }
}