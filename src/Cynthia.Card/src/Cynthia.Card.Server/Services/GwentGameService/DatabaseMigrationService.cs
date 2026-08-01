using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cynthia.Card.Server
{
    public class DatabaseMigrationService : BackgroundService
    {
        private readonly ILogger<DatabaseMigrationService> _logger;
        private readonly GwentDatabaseService _databaseService;

        public DatabaseMigrationService(
            ILogger<DatabaseMigrationService> logger,
            GwentDatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Running pending database migrations...");
                await _databaseService.RunPendingMigrations();
                _logger.LogInformation("Database migrations completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database migration failed.");
            }
        }
    }
}
