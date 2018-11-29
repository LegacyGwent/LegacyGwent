using MongoDB.Driver;

namespace Cynthia.Card.MongoDB
{
    internal partial class MongoDatabase
    {
        private readonly IMongoDatabase _database;
        public MongoDatabase(IMongoDatabase database) => _database = database;
        public override bool Equals(object obj) => obj is MongoDatabase database && database._database == _database;
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => _database.DatabaseNamespace.DatabaseName;
    }
}