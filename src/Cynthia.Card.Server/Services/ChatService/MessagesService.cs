using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Server
{
    [Singleton]
    internal class MessageService : IMessagesService
    {
        private List<ChatMessage> ContextData = new List<ChatMessage>();
        public int Count
        {
            get
            {
                return ContextData.Count;
            }
        }
        public void AddMessage(ChatMessage data)
        {
            ContextData.Add(data);
        }
        public void AddMessage(IEnumerable<ChatMessage> data)
        {
            data.ForAll(x => ContextData.Add(x));
        }
        public IEnumerable<ChatMessage> GetMessage(int count)
        {
            return ContextData.Skip(count);
        }
        public IEnumerable<ChatMessage> GetLastMessage(int count = 60)
        {
            return ContextData.Skip(ContextData.Count - count);
        }
    }
}