using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Mirrors the production synchronous enqueue contract: recording must not block
// the game scheduler. Persistence is awaited by the fixture after game actions end.
internal sealed class CrownRewardCapture
{
    private readonly List<(int Winner, string Key, DateTimeOffset Time)> _pending = new();
    private readonly Func<int, string, DateTimeOffset, Task> _persist;
    internal CrownRewardCapture(Func<int, string, DateTimeOffset, Task> persist) => _persist = persist;
    internal void Enqueue(int winner, string key, DateTimeOffset time)
    {
        lock (_pending) _pending.Add((winner, key, time));
    }
    internal async Task DrainAsync(string scenario)
    {
        if (SynchronizationContext.Current != null)
            throw new InvalidOperationException("Reward fixture persistence must run outside the game synchronization context.");
        (int Winner, string Key, DateTimeOffset Time)[] jobs;
        lock (_pending) { jobs = _pending.ToArray(); _pending.Clear(); }
        foreach (var job in jobs)
        {
            try { await _persist(job.Winner, job.Key, job.Time).WaitAsync(TimeSpan.FromSeconds(15)); }
            catch (TimeoutException error) { throw new TimeoutException("Reward fixture persistence timed out: " + scenario, error); }
        }
    }
}
