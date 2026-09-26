using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Cynthia.Card.Server
{
    public sealed class DailyLoginGrantService : BackgroundService
    {
        private readonly GwentServerService _server;

        public DailyLoginGrantService(GwentServerService server) => _server = server;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var offset = TimeSpan.FromHours(8);
            var observedDay = DateTimeOffset.UtcNow.ToOffset(offset).Date;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                var currentDay = DateTimeOffset.UtcNow.ToOffset(offset).Date;
                if (currentDay == observedDay)
                    continue;
                observedDay = currentDay;
                _server.QueueDailyLoginForOnlineUsers();
            }
        }
    }
}
