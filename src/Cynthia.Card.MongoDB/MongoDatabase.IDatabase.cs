using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoDatabase : IDatabase
    {
        public string Name => _database.DatabaseNamespace.DatabaseName;

        public IRepository<TModel> GetRepository<TModel>() where TModel : IModel => GetRepository<TModel>(typeof(TModel).Name);

        public IRepository<TModel> GetRepository<TModel>(string name) where TModel : IModel => new MongoRepository<TModel>(this, name, _database.GetCollection<TModel>(name));

        public IRepository GetRepository(string name) => new MongoRepository(this, name);

        public IEnumerator<IRepository> GetEnumerator() => _database.ListCollections().ToEnumerable().Select(data => new MongoRepository(this, data["name"].ToString())).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}