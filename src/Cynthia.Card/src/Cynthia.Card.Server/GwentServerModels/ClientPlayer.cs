using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ClientPlayer : Player
    {
        public User CurrentUser { get; set; }
        // private (TaskCompletionSource<bool> Task, string Id) _nowOkTask;
        public Func<IHubContext<GwentHub>> _hub;
        public IList<object> OperactionList { get; set; } = new List<object>();
        public ClientPlayer(User user, Func<IHubContext<GwentHub>> hub) : base()
        {
            PlayerName = user.PlayerName;
            CurrentUser = user;
            _hub = hub;
            Receive += async x =>
             {
                 // await hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("Test", "debug尝试");
                 lock (OperactionList)
                 {
                     OperactionList.Add(x.Result);
                 }
                 if (OperactionList.Count > 50)
                     await SendOperactionList();
                 //await hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", x.Result);
             };
        }
        public async Task SendOperactionList()
        {
            if (OperactionList.Count == 0) return;
            var tempList = default(IList<object>);
            lock (OperactionList)
            {
                tempList = OperactionList.ToList();
                OperactionList.Clear();
            }
            await _hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", tempList);
        }
        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}