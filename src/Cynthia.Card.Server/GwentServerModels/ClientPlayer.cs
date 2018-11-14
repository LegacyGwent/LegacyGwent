using System;
using System.Threading.Tasks;
using Alsein.Utilities.IO;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ClientPlayer : Player
    {
        public User CurrentUser { get; set; }
        private (TaskCompletionSource<bool> Task,string Id) _nowOkTask;
        public ClientPlayer(User user, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = user.PlayerName;
            CurrentUser = user;
            base.Receive+=async (x)=>
            {
                var type = ((Operation<UserOperationType>)x.Result);
                if(type.OperationType==UserOperationType.OK)
                {
                    x.IsAsyncReceived = false;
                    if(_nowOkTask.Id==type.Id&&!_nowOkTask.Task.Task.IsCompletedSuccessfully)
                    {
                        _nowOkTask.Task.SetResult(true);
                    }
                }
                else
                {
                    x.IsAsyncReceived = true;
                }
                await Task.CompletedTask;
            };
            Receive += async (x) =>
            {   //收到上游的消息
                var uuid = Guid.NewGuid().ToString();
                _nowOkTask = (new TaskCompletionSource<bool>(),uuid);
                reStar:
                while(user.IsWaitingReConnect) await Task.Delay(100);
                ((Operation<ServerOperationType>)x.Result).Id = uuid;
                await hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", x.Result);
                var timeTask = Task.Delay(500);
                switch(await Task.WhenAny(_nowOkTask.Task.Task,timeTask))
                {
                    case Task<bool> wt when wt == _nowOkTask.Task.Task:
                    var result = await wt;
                        break;
                    case Task tt when tt == timeTask:
                        goto reStar;
                }
            };
        }
        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<ReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}