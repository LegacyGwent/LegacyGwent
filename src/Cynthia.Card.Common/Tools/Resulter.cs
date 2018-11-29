using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public class Resulter
    {
        private TaskCompletionSource<object> _source = new TaskCompletionSource<object>();
        private Task GetTask() => _source.Task;
        public TaskAwaiter GetAwaiter() => GetTask().GetAwaiter();
        public Task Result()
        {
            var oldSource = _source;
            _source = new TaskCompletionSource<object>();
            return Task.Run(() => oldSource.SetResult(null));
        }
    }
}