using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Alsein.Utilities;
using Cynthia.Card.Common;
using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoRepository<TModel> : IRepository<TModel> where TModel : IModel
    {
        public Type ElementType => queryable.ElementType;

        public Expression Expression => queryable.Expression;

        public IQueryProvider Provider => queryable.Provider;

        public int Count => queryable.Count();

        public bool IsReadOnly => false;

        public TModel this[string id]
        {
            get => queryable.Where(item => item.Id == id).SingleOrDefault();
            set => _collection.ReplaceOne(item => item.Id == id, AutoId(value, id));
        }

        public void Add(TModel item) => _collection.InsertOne(item.To(AutoId));

        public void Add(IEnumerable<TModel> items)
        {
            if (items.Count() > 0)
                _collection.InsertMany(items.Select(AutoId));
        }

        public void Clear() => _collection.Database.DropCollection(Name);

        public bool Contains(TModel item) => queryable.Contains(item);

        public void CopyTo(TModel[] array, int arrayIndex) => queryable.ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<TModel> GetEnumerator() => queryable.GetEnumerator();

        public bool Remove(TModel item) => _collection.DeleteOne(x => x.Id == item.Id).DeletedCount == 1;

        public int Remove(Expression<Func<TModel, bool>> predicate) => (int)_collection.DeleteMany(predicate).DeletedCount;

        private UpdateDefinitionBuilder<TModel> _updateBuilder { get; } = new UpdateDefinitionBuilder<TModel>();

        private UpdateDefinition<TModel> UpdateDefinition(TModel item)
        {
            var update = default(UpdateDefinition<TModel>);
            item.GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any() && p.CanRead)
                .Select(p => (name: p.Name, value: p.GetValue(item)))
                .Where(p => p.value != null)
                .ForAll(p => update = update == null ? _updateBuilder.Set(p.name, p.value) : update.Set(p.name, p.value));
            return update;
        }

        public bool Update(TModel item) => _collection.ReplaceOne(x => x.Id == item.Id, item).ModifiedCount == 1;

        public int Update(IEnumerable<TModel> items) => items.Select(Update).Count(r => r == true);

        public int Update(Expression<Func<TModel, bool>> predicate, TModel item) => (int)_collection.UpdateMany(predicate, item.To(UpdateDefinition)).ModifiedCount;

        IEnumerator IEnumerable.GetEnumerator() => queryable.GetEnumerator();
    }
}