using System;
using System.Globalization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cynthia.Card.Server
{
    public sealed class RewardSettlementService : BackgroundService
    {
        private readonly GwentDatabaseService _database;
        private readonly IHubContext<GwentHub> _hub;
        private readonly ILogger<RewardSettlementService> _logger;
        private readonly Channel<RewardWork> _work = Channel.CreateUnbounded<RewardWork>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        public RewardSettlementService(GwentDatabaseService database, IHubContext<GwentHub> hub,
            ILogger<RewardSettlementService> logger)
        {
            _database = database;
            _hub = hub;
            _logger = logger;
        }

        public bool QueueDailyLogin(string username, string connectionId) =>
            _work.Writer.TryWrite(new RewardWork { Username = username, ConnectionId = connectionId });

        public bool QueueDailyRound(string username, string playerName, string connectionId, string opponentPlayerName,
            string matchId, string roundId, DateTimeOffset settledUtc) =>
            _work.Writer.TryWrite(new RewardWork
            {
                Username = username,
                PlayerName = playerName,
                ConnectionId = connectionId,
                OpponentPlayerName = opponentPlayerName,
                MatchId = matchId,
                RoundId = roundId,
                SettledUtc = settledUtc
            });

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var pending in await _database.GetPendingDailyRoundRewardJobs(stoppingToken))
                    {
                        if (DateTimeOffset.TryParse(pending.SettledUtc, CultureInfo.InvariantCulture,
                            DateTimeStyles.RoundtripKind, out var settledUtc))
                            QueueDailyRound(pending.Username, pending.PlayerName, pending.ConnectionId,
                                pending.OpponentPlayerName, pending.MatchId, pending.Id, settledUtc);
                    }
                    break;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                catch (Exception error)
                {
                    _logger.LogError(error, "Pending daily round rewards will be loaded again in 5 seconds.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            await foreach (var item in _work.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    if (item.RoundId == null)
                    {
                        await _database.GetDailyQuests(item.Username);
                    }
                    else
                    {
                        var job = await _database.PersistDailyRoundRewardJob(item.Username, item.PlayerName,
                            item.ConnectionId, item.OpponentPlayerName, item.MatchId, item.RoundId, item.SettledUtc,
                            stoppingToken);
                        if (!job.Completed)
                        {
                            var settledUtc = DateTimeOffset.Parse(job.SettledUtc, CultureInfo.InvariantCulture,
                                DateTimeStyles.RoundtripKind);
                            // The human-match overload keeps the authoritative opponent/match context for
                            // the same-opponent check; the legacy overload stays for synthetic jobs.
                            var result = string.IsNullOrWhiteSpace(job.MatchId) ||
                                         string.IsNullOrWhiteSpace(job.PlayerName) ||
                                         string.IsNullOrWhiteSpace(job.OpponentPlayerName)
                                ? await _database.AwardDailyCrown(job.Username, job.Id, settledUtc)
                                : await _database.AwardDailyCrown(job.Username, job.PlayerName,
                                    job.OpponentPlayerName, job.MatchId, job.Id, settledUtc);
                            if (!result.Success)
                                throw new InvalidOperationException("Daily round reward returned status " + result.Status + ".");
                            await _database.CompleteDailyRoundRewardJob(job.Id, stoppingToken);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(item.ConnectionId))
                    {
                        try { await _hub.Clients.Client(item.ConnectionId).SendAsync("DailyQuestsChanged", cancellationToken: stoppingToken); }
                        catch (Exception error) { _logger.LogDebug(error, "Daily reward notification was not delivered."); }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                catch (Exception error)
                {
                    if (item.RoundId != null)
                    {
                        try { await _database.FailDailyRoundRewardJob(item.RoundId, error, stoppingToken); }
                        catch (Exception updateError) { _logger.LogError(updateError, "Could not record reward job failure {RoundId}.", item.RoundId); }
                    }
                    _logger.LogError(error, "Reward settlement will retry. User={Username}, Round={RoundId}",
                        item.Username, item.RoundId);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    _work.Writer.TryWrite(item);
                }
            }
        }

        private sealed class RewardWork
        {
            public string Username { get; set; }
            public string PlayerName { get; set; }
            public string ConnectionId { get; set; }
            public string OpponentPlayerName { get; set; }
            public string MatchId { get; set; }
            public string RoundId { get; set; }
            public DateTimeOffset SettledUtc { get; set; }
        }
    }
}
