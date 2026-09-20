using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public sealed class DailyRoundRewardJob
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public string SettledUtc { get; set; }
        public bool Completed { get; set; }
        public int Attempts { get; set; }
        public string LastError { get; set; }
        public string CompletedUtc { get; set; }
    }

    public partial class GwentDatabaseService
    {
        private IMongoCollection<DailyRoundRewardJob> DailyRoundRewardJobs =>
            GetDatabase().GetCollection<DailyRoundRewardJob>("daily_round_reward_jobs");

        public async Task<DailyRoundRewardJob> PersistDailyRoundRewardJob(
            string username, string connectionId, string roundId, DateTimeOffset settledUtc,
            CancellationToken cancellationToken)
        {
            var update = Builders<DailyRoundRewardJob>.Update
                .SetOnInsert(x => x.Username, username)
                .SetOnInsert(x => x.ConnectionId, connectionId)
                .SetOnInsert(x => x.SettledUtc, settledUtc.ToUniversalTime().ToString("O"))
                .SetOnInsert(x => x.Completed, false)
                .SetOnInsert(x => x.Attempts, 0);
            return await DailyRoundRewardJobs.FindOneAndUpdateAsync(
                x => x.Id == roundId,
                update,
                new FindOneAndUpdateOptions<DailyRoundRewardJob>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                }, cancellationToken);
        }

        public Task<List<DailyRoundRewardJob>> GetPendingDailyRoundRewardJobs(CancellationToken cancellationToken) =>
            DailyRoundRewardJobs.Find(x => !x.Completed).ToListAsync(cancellationToken);

        public Task CompleteDailyRoundRewardJob(string roundId, CancellationToken cancellationToken) =>
            DailyRoundRewardJobs.UpdateOneAsync(x => x.Id == roundId,
                Builders<DailyRoundRewardJob>.Update.Set(x => x.Completed, true)
                    .Set(x => x.CompletedUtc, DateTimeOffset.UtcNow.ToString("O"))
                    .Set(x => x.LastError, null), cancellationToken: cancellationToken);

        public Task FailDailyRoundRewardJob(string roundId, Exception error, CancellationToken cancellationToken) =>
            DailyRoundRewardJobs.UpdateOneAsync(x => x.Id == roundId,
                Builders<DailyRoundRewardJob>.Update.Inc(x => x.Attempts, 1)
                    .Set(x => x.LastError, error.GetType().Name + ": " + error.Message), cancellationToken: cancellationToken);
    }
}
