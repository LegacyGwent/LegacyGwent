using System.Collections.Generic;

namespace Cynthia.Card
{
    public interface IDatabaseService : IEnumerable<IDatabase>
    {
        IDatabase this[string name] { get; }
    }
}
