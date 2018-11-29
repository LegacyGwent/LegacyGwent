using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoDatabase : IMongoDatabase
    {
        public IMongoClient Client => _database.Client;

        public DatabaseNamespace DatabaseNamespace => _database.DatabaseNamespace;

        public MongoDatabaseSettings Settings => _database.Settings;

        public void CreateCollection(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.CreateCollection(name, options, cancellationToken);
        }

        public void CreateCollection(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.CreateCollection(session, name, options, cancellationToken);
        }

        public Task CreateCollectionAsync(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.CreateCollectionAsync(name, options, cancellationToken);
        }

        public Task CreateCollectionAsync(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.CreateCollectionAsync(session, name, options, cancellationToken);
        }

        public void CreateView<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.CreateView(viewName, viewOn, pipeline, options, cancellationToken);
        }

        public void CreateView<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.CreateView(session, viewName, viewOn, pipeline, options, cancellationToken);
        }

        public Task CreateViewAsync<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.CreateViewAsync(viewName, viewOn, pipeline, options, cancellationToken);
        }

        public Task CreateViewAsync<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.CreateViewAsync(session, viewName, viewOn, pipeline, options, cancellationToken);
        }

        public void DropCollection(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.DropCollection(name, cancellationToken);
        }

        public void DropCollection(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.DropCollection(session, name, cancellationToken);
        }

        public Task DropCollectionAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.DropCollectionAsync(name, cancellationToken);
        }

        public Task DropCollectionAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.DropCollectionAsync(session, name, cancellationToken);
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string name, MongoCollectionSettings settings = null)
        {
            return _database.GetCollection<TDocument>(name, settings);
        }

        public IAsyncCursor<BsonDocument> ListCollections(ListCollectionsOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.ListCollections(options, cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListCollections(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.ListCollections(session, options, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(ListCollectionsOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.ListCollectionsAsync(options, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.ListCollectionsAsync(session, options, cancellationToken);
        }

        public void RenameCollection(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.RenameCollection(oldName, newName, options, cancellationToken);
        }

        public void RenameCollection(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _database.RenameCollection(session, oldName, newName, options, cancellationToken);
        }

        public Task RenameCollectionAsync(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RenameCollectionAsync(oldName, newName, options, cancellationToken);
        }

        public Task RenameCollectionAsync(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RenameCollectionAsync(session, oldName, newName, options, cancellationToken);
        }

        public TResult RunCommand<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RunCommand(command, readPreference, cancellationToken);
        }

        public TResult RunCommand<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RunCommand(session, command, readPreference, cancellationToken);
        }

        public Task<TResult> RunCommandAsync<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RunCommandAsync(command, readPreference, cancellationToken);
        }

        public Task<TResult> RunCommandAsync<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _database.RunCommandAsync(session, command, readPreference, cancellationToken);
        }

        public IMongoDatabase WithReadConcern(ReadConcern readConcern)
        {
            return _database.WithReadConcern(readConcern);
        }

        public IMongoDatabase WithReadPreference(ReadPreference readPreference)
        {
            return _database.WithReadPreference(readPreference);
        }

        public IMongoDatabase WithWriteConcern(WriteConcern writeConcern)
        {
            return _database.WithWriteConcern(writeConcern);
        }
    }
}