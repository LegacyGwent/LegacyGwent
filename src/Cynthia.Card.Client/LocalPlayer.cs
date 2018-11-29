using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Utilities.IO;
using System;

namespace Cynthia.Card.Client
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(HubConnection hubConnection)
        {
            ((Player)this).Receive += x => hubConnection.SendAsync("GameOperation", x.Result);
            hubConnection.On<Operation<ServerOperationType>>("GameOperation", x =>
            {
                x.Arguments = x.Arguments.Select(item => item.ToType<string>());
                SendAsync(x);
            });
        }
        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<ReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}