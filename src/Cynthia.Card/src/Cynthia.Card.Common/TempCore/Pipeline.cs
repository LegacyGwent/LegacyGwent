using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public class Pipeline
    {
        private readonly LinkedList<Func<Task>> _data = new LinkedList<Func<Task>>();

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