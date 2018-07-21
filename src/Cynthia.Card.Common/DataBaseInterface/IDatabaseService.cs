using System.Collections.Generic;

namespace Cynthia.Card.Common
{
    public interface IDatabaseService : IEnumerable<IDatabase>
    {
        IDatabase this[string name] { get; }
    }
}
