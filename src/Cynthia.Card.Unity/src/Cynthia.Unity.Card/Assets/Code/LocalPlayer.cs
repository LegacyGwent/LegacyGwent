using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Extensions.IO;
using Alsein.Extensions.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alsein.Extensions;
using Autofac;

namespace Cynthia.Card.Client
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(HubConnection hubConnection) : base()
        {
            //接收到通讯层消息,发送到下游
            ((Player)this).Receive += async x => await hubConnection.SendAsync("GameOperation", x.Result);
            hubConnection.On<IList<Operation<ServerOperationType>>>("GameOperation", async x =>
            {
                await _upstream.SendAsync(x);
            });
            hubConnection.Closed += async (e) =>
            {
                if (DependencyResolver.Container.Resolve<GwentClientService>().ClientState == ClientState.Play)
                {
                    await _upstream.SendAsync(Operation.Create(ServerOperationType.GameEnd, new GameResultInfomation("END", "END", new GameStatus())));
                }
            };
        }

        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);

        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));

        public new Task<IList<Operation<ServerOperationType>>> ReceiveAsync() => _downstream.ReceiveAsync<IList<Operation<ServerOperationType>>>();

        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}
