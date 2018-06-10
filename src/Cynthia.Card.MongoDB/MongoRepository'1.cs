using System;
using System.Linq;
using Cynthia.Card.Common;
using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoRepository<TModel> : MongoRepository
    {
        private IMongoCollection<TModel> _collection;
        private IQueryable<TModel> queryable => _collection.AsQueryable<TModel>();
        private TModel AutoId(TModel item) => AutoId(item, null);
        private TModel AutoId(TModel item, string id)
        {
            if (item.Id == null || item.Id == string.Empty)
            {
                item.Id = id ?? Guid.NewGuid().ToString();
            }
            return item;
        }
        public MongoRepository(MongoDatabase database, string name, IMongoCollection<TModel> collection) : base(database, name)
        {
            _collection = collection;
        }
    }
}