using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    // Serializes game continuations, not whole asynchronous tasks: both players
    // may wait for input at once, while a reconnect observes one coherent boundary.
    internal sealed class GameExecutionContext : SynchronizationContext
    {
        private readonly Queue<Action> _queue = new Queue<Action>();
        private bool _running;

        public override void Post(SendOrPostCallback callback, object state)
        {
            lock (_queue)
            {
                _queue.Enqueue(() => callback(state));
                if (_running) return;
                _running = true;
            }
            ThreadPool.QueueUserWorkItem(_ => Drain());
        }

        private void Drain()
        {
            while (true)
            {
                Action action;
                lock (_queue)
                {
                    if (_queue.Count == 0) { _running = false; return; }
                    action = _queue.Dequeue();
                }
                var previous = Current;
                SetSynchronizationContext(this);
                try { action(); }
                // Only an async void callback can throw here. It belongs to no
                // Run call, so it must not fail the later ones of this game.
                catch (Exception error) { Console.WriteLine($"Game continuation failed: {error}"); }
                finally { SetSynchronizationContext(previous); }
            }
        }

        public Task<T> Run<T>(Func<Task<T>> action)
        {
            if (Current == this) return action();
            var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            Post(async _ =>
            {
                try { completion.TrySetResult(await action()); }
                catch (Exception error) { completion.TrySetException(error); }
            }, null);
            return completion.Task;
        }

        public Task Run(Func<Task> action) => Run(async () => { await action(); return true; });
    }
}
