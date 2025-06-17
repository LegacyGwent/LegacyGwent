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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    
                    // Check if it's the date of the season end
                    if (now.Month == 6 && now.Day == 23 && now.Hour == 0 && now.Minute == 0)
                    {
                        Console.WriteLine("Executing monthly rank reset and seasonal rewards...");
                        _logger.LogInformation("Executing monthly rank reset and seasonal rewards...");
                        
                        // First award seasonal rewards
                        await AwardSeasonalRewards();
                        
                        // Then reset ranks
                        await ResetPlayerRanks();
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
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border1");
                    }
                    if (rank <= 50)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border2");
                    }
                    if (rank <= 20)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border3");
                    }
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border4");
                    }
                    if (rank <= 5)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border5");
                    }
                    if (rank == 1)
                    {
                        await _gwentServerService.AddBorder(player.PlayerName, "Season1Border6");
                    }

                    // Award titles
                    if (rank <= 100)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "MAN-AT-ARMS");
                    }
                    if (rank <= 50)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "MERCENARY");
                    }
                    if (rank <= 20)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "BOUNTYHUNTER");
                    }
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "VETERAN");
                    }
                    if (rank <= 5)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "CHAMPION");
                    }
                    if (rank == 1)
                    {
                        await _gwentServerService.AddTitle(player.PlayerName, "HERO");
                    }

                    // Award avatar to top 10
                    if (rank <= 10)
                    {
                        await _gwentServerService.AddAvatar(player.PlayerName, "Imlerith_Unmasked");
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