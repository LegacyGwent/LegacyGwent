using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Microsoft.AspNetCore.SignalR;


namespace Cynthia.Card.Server
{
    public class Viewer : Player
    {
        public User CurrentUser { get; set; }
        public IList<object> OperationList { get; set; }
        public Func<IHubContext<GwentHub>> Hub;
        public Viewer(User user, Func<IHubContext<GwentHub>> hub)
        {
            CurrentUser = user;
            OperationList = new List<object>();
            Hub = hub;
            Receive += AddOperation;
        }

        public async Task AddOperation(TubeReceiveEventArgs op)
        {
            lock (OperationList)
            {
                OperationList.Add(op.Result);
            }
            await SendOperationList();
        }

        public async Task SendOperationList()
        {
            if (OperationList.Count == 0) return;
            var tempList = default(IList<object>);
            lock (OperationList)
            {
                tempList = OperationList.ToList();
                OperationList.Clear();
            }
            await Hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("ViewerGameOperation", tempList);
        }

        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}