using System.Collections.Generic;
using System.Linq;
using Cynthia.Card.Common.Models;
using Alsein.Utilities;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Server.Services
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
    }
}