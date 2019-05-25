using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace Cynthia.Card.MongoDB
{
    public partial class MongoDatabaseService : IMongoClient
    {
        public ICluster Cluster => _mongoClient.Cluster;

        public MongoClientSettings Settings => _mongoClient.Settings;

        public void DropDatabase(string name, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.DropDatabase(name, cancellationToken);

        public void DropDatabase(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.DropDatabase(session, name, cancellationToken);

        public Task DropDatabaseAsync(string name, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.DropDatabaseAsync(name, cancellationToken);

        public Task DropDatabaseAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.DropDatabaseAsync(session, name, cancellationToken);

        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null) => _mongoClient.GetDatabase(name, settings);

        public IAsyncCursor<BsonDocument> ListDatabases(CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.ListDatabases(cancellationToken);

        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.ListDatabases(session, cancellationToken);

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.ListDatabasesAsync(cancellationToken);

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.ListDatabasesAsync(session, cancellationToken);

        public IClientSessionHandle StartSession(ClientSessionOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.StartSession(options, cancellationToken);

        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) => _mongoClient.StartSessionAsync(options, cancellationToken);

        public IMongoClient WithReadConcern(ReadConcern readConcern) => _mongoClient.WithReadConcern(readConcern);

        public IMongoClient WithReadPreference(ReadPreference readPreference) => _mongoClient.WithReadPreference(readPreference);

        public IMongoClient WithWriteConcern(WriteConcern writeConcern) => _mongoClient.WithWriteConcern(writeConcern);
    }
}