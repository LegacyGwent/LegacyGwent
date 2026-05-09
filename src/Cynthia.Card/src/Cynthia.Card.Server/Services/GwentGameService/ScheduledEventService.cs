using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Cynthia.Card.Common.Models;
using Cynthia.Card;

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
            await _databaseService.RefreshSeasons();
            var seasonData = await _databaseService.QuerySeasonData();
            // Ensure trinkets for the currently active season are released
            TrinketMap.ReleaseSeasonRewards(seasonData?.seasonalRewards);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    if (seasonData != null)
                    {
                        if (seasonData.SeasonEndTime != DateTime.MinValue)
                        {
                            if (now >= seasonData.SeasonEndTime)
                            {
                                Console.WriteLine("Executing monthly rank reset and seasonal rewards...");
                                _logger.LogInformation("Executing monthly rank reset and seasonal rewards...");

                                await ResetSeason();
                                seasonData = await _databaseService.QuerySeasonData();
                            }
                        }
                    }


                    // Wait for 1 minute before next check
                    await Task.Delay(TimeSpan.FromMinutes(1.0f), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing scheduled events");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task ResetSeason(){

            // First award seasonal rewards
            await AwardSeasonalRewards();

            await SaveSeasonRanks();
                        
            // Then reset ranks(with stats)
            await ResetPlayerRanks();

            await _databaseService.RefreshSeasons();

            var season_data = await _databaseService.QuerySeasonData();
            // When a new season becomes active, release its associated trinkets
            TrinketMap.ReleaseSeasonRewards(season_data?.seasonalRewards);
            
            while (DateTime.UtcNow > season_data.SeasonEndTime)
            {
                await ResetSeason();
            }
        }

        private async Task SaveSeasonRanks()
        {
            var temp = _databaseService.GetSeasonInfo();

            var activeSeason = await temp.AsQueryable()
                .Where(x => x.isActive == true)
                .OrderByDescending(x => x.SeasonId)
                .FirstOrDefaultAsync();

            if (activeSeason != null)
            {
                var standings =  _databaseService.QueryAllMMRExtended(0, 100);
                var standingsString = Season.EncodeRanklist(standings);

                var result = await temp.UpdateOneAsync(
                    Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, activeSeason.SeasonId),
                    Builders<SeasonInfo>.Update.Set(x => x.rankingHistory, standingsString)
                );
            }

        }

        private async Task AwardSeasonalRewards()
        {
            try
            {
                bool granted = await _gwentServerService.GiveAwaySeasonalRewards();
                if (granted)
                {
                    _logger.LogInformation("Successfully awarded seasonal rewards");
                    var temp = _databaseService.GetSeasonInfo();

                    var activeSeason = await temp.AsQueryable()
                        .Where(x => x.isActive == true)
                        .OrderByDescending(x => x.SeasonId)
                        .FirstOrDefaultAsync();

                    if (activeSeason != null)
                    {
                        var result = await temp.UpdateOneAsync(
                            Builders<SeasonInfo>.Filter.Eq(x => x.SeasonId, activeSeason.SeasonId),
                            Builders<SeasonInfo>.Update.Set(x => x.areRewardsGranted, true)
                        );
                    }
                }

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
                    await _databaseService.ResetPlayerStreak(player.UserName);
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