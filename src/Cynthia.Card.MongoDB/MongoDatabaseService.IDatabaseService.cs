using System.Collections;
using System.Collections.Generic;

namespace Cynthia.Card.MongoDB
{
    public partial class MongoDatabaseService : IDatabaseService
    {
        public IDatabase this[string name] => new MongoDatabase(_mongoClient.GetDatabase(name));

        public IEnumerator<IDatabase> GetEnumerator() => databases.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => databases.GetEnumerator();
    }
}