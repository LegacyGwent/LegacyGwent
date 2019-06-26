using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacyGwent.ConsoleTester
{
    public class OperactionList
    {
        private readonly LinkedList<Func<Task>> _data = new LinkedList<Func<Task>>();
        public bool IsRunning { get; private set; } = false;

        public async Task AddLast(Func<Task> task)
        {
            _data.AddLast(task);
            if (IsRunning) await Task.CompletedTask;
            else await Execute();
        }

        public async Task Execute()
        {
            IsRunning = true;
            while (_data.Count > 0)
            {
                var first = _data.First.Value;
                _data.RemoveFirst();
                await first();
            }
            IsRunning = false;
        }
    }
}