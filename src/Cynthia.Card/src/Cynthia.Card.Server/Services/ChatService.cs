using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions.LifetimeAnnotations;

namespace Cynthia.Card.Server
{
    [Singleton]
    public class ChatService
    {
        private readonly IDictionary<string, ChatUser> _usersConnection = new ConcurrentDictionary<string, ChatUser>();

        public async Task Disconnect(string connectionId, Exception exception)
        {
            if (_usersConnection.ContainsKey(connectionId))
            {
                _usersConnection.Remove(connectionId);
            }
            await Task.CompletedTask;
        }
    }
}