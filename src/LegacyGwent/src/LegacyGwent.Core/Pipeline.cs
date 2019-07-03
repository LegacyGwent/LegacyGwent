using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacyGwent
{
    public class Pipeline
    {
        private readonly LinkedList<Func<Task>> _data = new LinkedList<Func<Task>>();

        private Task? _running;

        public Task AddFirst(params Func<Task>[] actions)
        {
            for (var i = actions.Length - 1; i >= 0; i--)
            {
                _data.AddFirst(actions[i]);
            }
            if (_running == null)
            {
                _running = Execute();
            }
            return _running;
        }

        public Task AddLast(params Func<Task>[] actions)
        {
            for (var i = 0; i < actions.Length; i++)
            {
                _data.AddLast(actions[i]);
            }
            if (_running == null)
            {
                _running = Execute();
            }
            return _running;
        }

        private async Task Execute()
        {
            while (_data.Count > 0)
            {
                var first = _data.First.Value;
                _data.RemoveFirst();
                await first();
            }
            _running = null;
        }
    }
}