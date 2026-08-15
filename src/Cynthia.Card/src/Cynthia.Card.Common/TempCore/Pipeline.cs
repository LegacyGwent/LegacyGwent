using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public class Pipeline
    {
        private readonly LinkedList<Func<Task>> _data = new LinkedList<Func<Task>>();
        private readonly int _maxTasksPerDrain;

        public Pipeline(int maxTasksPerDrain = 4096)
        {
            if (maxTasksPerDrain <= 0) throw new ArgumentOutOfRangeException(nameof(maxTasksPerDrain));
            _maxTasksPerDrain = maxTasksPerDrain;
        }

        public bool IsRunning { get; private set; }

        public async Task AddLast(params Func<Task>[] tasks)
        {
            foreach (var task in tasks)
            {
                _data.AddLast(task);
            }
            if (IsRunning) await Task.CompletedTask;
            else await Execute();
        }

        private async Task Execute()
        {
            IsRunning = true;
            var executed = 0;
            try
            {
                while (_data.Count > 0)
                {
                    if (++executed > _maxTasksPerDrain)
                        throw new GameResolutionLimitException(
                            $"A single resolution queued more than {_maxTasksPerDrain} tasks.");

                    var first = _data.First.Value;
                    _data.RemoveFirst();
                    await first();
                }
            }
            finally
            {
                // An effect exception must not leave this game's queue permanently
                // marked as running. Discard the remainder of the failed chain so a
                // later safe-abort operation cannot accidentally resume stale work.
                _data.Clear();
                IsRunning = false;
            }
        }
    }

    public sealed class GameResolutionLimitException : InvalidOperationException
    {
        public GameResolutionLimitException(string message) : base(message) { }
    }
}
