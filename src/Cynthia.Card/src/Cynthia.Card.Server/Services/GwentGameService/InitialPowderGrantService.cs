using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Cynthia.Card.Server
{
    public sealed class InitialPowderGrantService : BackgroundService
    {
        private readonly GwentDatabaseService _database;
        private readonly ILogger<InitialPowderGrantService> _logger;
        private readonly Channel<bool> _scanRequests = Channel.CreateBounded<bool>(1);
        public InitialPowderGrantService(GwentDatabaseService database, ILogger<InitialPowderGrantService> logger)
        { _database = database; _logger = logger; }

        public void RequestScan() => _scanRequests.Writer.TryWrite(true);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var count = await _database.BackfillInitialPowder(stoppingToken,
                        (id, error) => _logger.LogError(error, "Initial powder failed for account {PlayerId}", id));
                    _logger.LogInformation("Initial powder backfill completed: {Granted} accounts granted.", count);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                catch (Exception e) { _logger.LogError(e, "Initial powder backfill will retry in 30 seconds."); }
                try
                {
                    using (var cycle = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken))
                    {
                        cycle.CancelAfter(TimeSpan.FromSeconds(30));
                        await _scanRequests.Reader.ReadAsync(cycle.Token);
                    }
                    while (_scanRequests.Reader.TryRead(out _)) { }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                catch (OperationCanceledException) { }
            }
        }
    }
}
