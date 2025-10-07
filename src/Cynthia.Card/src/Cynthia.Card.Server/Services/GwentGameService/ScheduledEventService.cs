using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Cynthia.Card.Server
{
    public class ScheduledEventService : BackgroundService
    {
        private readonly ILogger<ScheduledEventService> _logger;
        private readonly GwentServerService _gwentServerService;
        private readonly GwentDatabaseService _databaseService;
        public ScheduledEventService(
            ILogger<ScheduledEventService> logger,
            GwentServerService gwentServerService,
            GwentDatabaseService databaseService)
        {
            _logger = logger;
            _gwentServerService = gwentServerService;
            _databaseService = databaseService;
        }
        public int season = SeasonProvider.CurrentSeason; // Current season number

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Load season from DB on startup
            try
            {
                season = _databaseService.GetCurrentSeason();
                SeasonProvider.CurrentSeason = season;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load current season from database; falling back to default");
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    // Check if it's the date of the season end
                    if (now.Month == 10 && now.Day == 23 && now.Hour == 0 && now.Minute == 0)
                    {
                        Console.WriteLine("Executing monthly rank reset and seasonal rewards...");
                        _logger.LogInformation("Executing monthly rank reset and seasonal rewards...");

                        // First award seasonal rewards
                        await AwardSeasonalRewards();

                        // Then reset ranks
                        await ResetPlayerRanks();
                        season++;
                        SeasonProvider.CurrentSeason = season;
                        try
                        {
                            _databaseService.SetCurrentSeason(season);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to persist new season to database");
                        }
                    }

                    // Wait for 1 minute before next check
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing scheduled events");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task AwardSeasonalRewards()
        {
            try
            {
                // Get top players by MMR
                var topPlayers = _databaseService.GetAllPlayers()
                    .OrderByDescending(p => p.MMR)
                    .ToList();

                // Award rewards to top players
                for (int i = 0; i < topPlayers.Count; i++)
                {
                    var player = topPlayers[i];
                    var rank = i + 1;

                    // Award borders
                    if (rank <= 100)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border1");
                    }
                    if (rank <= 50)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border2");
                    }
                    if (rank <= 20)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border3");
                    }
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border4");
                    }
                    if (rank <= 5)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border5");
                    }
                    if (rank == 1)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season2Border6");
                    }

                    // Award titles
                    if (rank <= 100)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "RANGER");
                    }
                    if (rank <= 50)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "TRAPPER");
                    }
                    if (rank <= 20)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "HUNTER");
                    }
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "REBEL");
                    }
                    if (rank <= 5)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "DEFENDER");
                    }
                    if (rank == 1)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "PROTECTOR");
                    }

                    // Award avatar to top 10
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddAvatar(player.PlayerName, "Iorveth");
                    }
                }
                
                _logger.LogInformation("Successfully awarded seasonal rewards");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while awarding seasonal rewards");
                throw;
            }
        }

        private async Task ResetPlayerRanks()
        {
            try
            {
                // Get all players and reset their MMR to the starting value
                var players = _databaseService.GetAllPlayers();
                foreach (var player in players)
                {
                    await _databaseService.ResetPlayerMMR(player.UserName, _databaseService.initMMR);
                }
                
                _logger.LogInformation("Successfully reset ranks for all players");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting player ranks");
                throw;
            }
        }
    }
} 