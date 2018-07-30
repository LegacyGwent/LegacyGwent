using System.Collections.Generic;

namespace Cynthia.Card
{
    public class Operation
    {
        public static Operation<TOperationType> Create<TOperationType>(TOperationType type, params object[] arguments) => new Operation<TOperationType>(type, arguments);
    }
    public class Operation<TOperationType>
    {
        public TOperationType OperationType { get; }
        public IEnumerable<object> Arguments { get; }
        public Operation(TOperationType operationType, IEnumerable<object> arguments)
        {
            OperationType = operationType;
            Arguments = arguments;
        }
    }
}