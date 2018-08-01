using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public class Operation
    {
        public static Operation<TOperationType> Create<TOperationType>(TOperationType type, params object[] arguments) => new Operation<TOperationType>(type, arguments);
    }
    public class Operation<TOperationType>
    {
        public TOperationType OperationType { get; }
        public IEnumerable<string> Arguments { get; set; }
        public Operation(TOperationType operationType, IEnumerable<object> arguments)
        {
            OperationType = operationType;
            Arguments = arguments.Select(x => x.ToJson());
        }
    }
}