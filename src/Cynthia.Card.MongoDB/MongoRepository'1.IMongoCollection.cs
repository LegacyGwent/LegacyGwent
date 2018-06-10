using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoRepository<TModel> : IMongoCollection<TModel>
    {
        public CollectionNamespace CollectionNamespace => _collection.CollectionNamespace;

        public IBsonSerializer<TModel> DocumentSerializer => _collection.DocumentSerializer;

        public IMongoIndexManager<TModel> Indexes => _collection.Indexes;

        public MongoCollectionSettings Settings => _collection.Settings;

        IMongoDatabase IMongoCollection<TModel>.Database => _collection.Database;

        public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TModel, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Aggregate(pipeline, options, cancellationToken);
        }

        public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TModel, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Aggregate(session, pipeline, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TModel, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.AggregateAsync(pipeline, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TModel, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.AggregateAsync(session, pipeline, options, cancellationToken);
        }

        public BulkWriteResult<TModel> BulkWrite(IEnumerable<WriteModel<TModel>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.BulkWrite(requests, options, cancellationToken);
        }

        public BulkWriteResult<TModel> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TModel>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.BulkWrite(session, requests, options, cancellationToken);
        }

        public Task<BulkWriteResult<TModel>> BulkWriteAsync(IEnumerable<WriteModel<TModel>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.BulkWriteAsync(requests, options, cancellationToken);
        }

        public Task<BulkWriteResult<TModel>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TModel>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.BulkWriteAsync(session, requests, options, cancellationToken);
        }

        public Task<long> CountAsync(FilterDefinition<TModel> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.CountAsync(filter, options, cancellationToken);
        }

        public Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.CountAsync(session, filter, options, cancellationToken);
        }

        public DeleteResult DeleteMany(FilterDefinition<TModel> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteMany(filter, cancellationToken);
        }

        public DeleteResult DeleteMany(FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteMany(filter, options, cancellationToken);
        }

        public DeleteResult DeleteMany(IClientSessionHandle session, FilterDefinition<TModel> filter, DeleteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteMany(session, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TModel> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteManyAsync(filter, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteManyAsync(filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, DeleteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteManyAsync(session, filter, options, cancellationToken);
        }

        public DeleteResult DeleteOne(FilterDefinition<TModel> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOne(filter, cancellationToken);
        }

        public DeleteResult DeleteOne(FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOne(filter, options, cancellationToken);
        }

        public DeleteResult DeleteOne(IClientSessionHandle session, FilterDefinition<TModel> filter, DeleteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOne(session, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TModel> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOneAsync(filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, DeleteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DeleteOneAsync(session, filter, options, cancellationToken);
        }

        public IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TModel, TField> field, FilterDefinition<TModel> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Distinct(field, filter, options, cancellationToken);
        }

        public IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<TModel, TField> field, FilterDefinition<TModel> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Distinct(session, field, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TModel, TField> field, FilterDefinition<TModel> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DistinctAsync(field, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TModel, TField> field, FilterDefinition<TModel> filter, DistinctOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.DistinctAsync(session, field, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindAsync(filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindAsync(session, filter, options, cancellationToken);
        }

        public TProjection FindOneAndDelete<TProjection>(FilterDefinition<TModel> filter, FindOneAndDeleteOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndDelete(filter, options, cancellationToken);
        }

        public TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, FindOneAndDeleteOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndDelete(session, filter, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TModel> filter, FindOneAndDeleteOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, FindOneAndDeleteOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndDeleteAsync(session, filter, options, cancellationToken);
        }

        public TProjection FindOneAndReplace<TProjection>(FilterDefinition<TModel> filter, TModel replacement, FindOneAndReplaceOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndReplace(filter, replacement, options, cancellationToken);
        }

        public TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, TModel replacement, FindOneAndReplaceOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndReplace(session, filter, replacement, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TModel> filter, TModel replacement, FindOneAndReplaceOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, TModel replacement, FindOneAndReplaceOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndReplaceAsync(session, filter, replacement, options, cancellationToken);
        }

        public TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, FindOneAndUpdateOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndUpdate(filter, update, options, cancellationToken);
        }

        public TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, FindOneAndUpdateOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndUpdate(session, filter, update, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, FindOneAndUpdateOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        }

        public Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, FindOneAndUpdateOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);
        }

        public IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindSync(filter, options, cancellationToken);
        }

        public IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.FindSync(session, filter, options, cancellationToken);
        }

        public void InsertMany(IEnumerable<TModel> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _collection.InsertMany(documents, options, cancellationToken);
        }

        public void InsertMany(IClientSessionHandle session, IEnumerable<TModel> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _collection.InsertMany(session, documents, options, cancellationToken);
        }

        public Task InsertManyAsync(IEnumerable<TModel> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.InsertManyAsync(documents, options, cancellationToken);
        }

        public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TModel> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.InsertManyAsync(session, documents, options, cancellationToken);
        }

        public void InsertOne(TModel document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _collection.InsertOne(document, options, cancellationToken);
        }

        public void InsertOne(IClientSessionHandle session, TModel document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _collection.InsertOne(session, document, options, cancellationToken);
        }

#pragma warning disable
        [Obsolete("Use the new overload of InsertOneAsync with an InsertOneOptions parameter instead.")]
        public Task InsertOneAsync(TModel document, CancellationToken _cancellationToken) => _collection.InsertOneAsync(document, _cancellationToken);
#pragma warning enable

        public Task InsertOneAsync(TModel document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.InsertOneAsync(document, options, cancellationToken);
        }

        public Task InsertOneAsync(IClientSessionHandle session, TModel document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.InsertOneAsync(session, document, options, cancellationToken);
        }

        public IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TModel, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.MapReduce(map, reduce, options, cancellationToken);
        }

        public IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TModel, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.MapReduce(session, map, reduce, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TModel, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.MapReduceAsync(map, reduce, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TModel, TResult> options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.MapReduceAsync(session, map, reduce, options, cancellationToken);
        }

        public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>() where TDerivedDocument : TModel
        {
            return _collection.OfType<TDerivedDocument>();
        }

        public ReplaceOneResult ReplaceOne(FilterDefinition<TModel> filter, TModel replacement, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.ReplaceOne(filter, replacement, options, cancellationToken);
        }

        public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TModel> filter, TModel replacement, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.ReplaceOne(session, filter, replacement, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TModel> filter, TModel replacement, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.ReplaceOneAsync(filter, replacement, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, TModel replacement, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);
        }

        public UpdateResult UpdateMany(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateMany(filter, update, options, cancellationToken);
        }

        public UpdateResult UpdateMany(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateMany(session, filter, update, options, cancellationToken);
        }

        public Task<UpdateResult> UpdateManyAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateManyAsync(filter, update, options, cancellationToken);
        }

        public Task<UpdateResult> UpdateManyAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateManyAsync(session, filter, update, options, cancellationToken);
        }

        public UpdateResult UpdateOne(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateOne(filter, update, options, cancellationToken);
        }

        public UpdateResult UpdateOne(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateOne(session, filter, update, options, cancellationToken);
        }

        public Task<UpdateResult> UpdateOneAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        public Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.UpdateOneAsync(session, filter, update, options, cancellationToken);
        }

        public IAsyncCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<TModel>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Watch(pipeline, options, cancellationToken);
        }

        public IAsyncCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TModel>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.Watch(session, pipeline, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<TModel>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.WatchAsync(pipeline, options, cancellationToken);
        }

        public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TModel>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _collection.WatchAsync(session, pipeline, options, cancellationToken);
        }

        public IMongoCollection<TModel> WithReadConcern(ReadConcern readConcern)
        {
            return _collection.WithReadConcern(readConcern);
        }

        public IMongoCollection<TModel> WithReadPreference(ReadPreference readPreference)
        {
            return _collection.WithReadPreference(readPreference);
        }

        public IMongoCollection<TModel> WithWriteConcern(WriteConcern writeConcern)
        {
            return _collection.WithWriteConcern(writeConcern);
        }

        long IMongoCollection<TModel>.Count(FilterDefinition<TModel> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return _collection.Count(filter, options, cancellationToken);
        }

        long IMongoCollection<TModel>.Count(IClientSessionHandle session, FilterDefinition<TModel> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return _collection.Count(session, filter, options, cancellationToken);
        }
    }
}