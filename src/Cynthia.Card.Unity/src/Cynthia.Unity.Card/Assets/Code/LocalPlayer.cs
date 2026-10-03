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
        private readonly HubConnection _hubConnection;

        public LocalPlayer(HubConnection hubConnection) : base()
        {
            _hubConnection = hubConnection;
            ForwardToServer();
            hubConnection.On<IList<Operation<int>>>("GameOperation", async x =>
            {
                await _upstream.SendAsync(x);
            });
            hubConnection.On<IList<Operation<int>>>("ViewerGameOperation", async x =>
            {
                await _upstream.SendAsync(x);
            });
        }

        //接收到通讯层消息,发送到下游
        private void ForwardToServer()
        {
            ((Player)this).Receive += async x => await _hubConnection.SendAsync("GameOperation", x.Result);
            ((Player)this).Receive += async x => await _hubConnection.SendAsync("ViewerGameOperation", x.Result);
        }

        // Stop the running game on this client, e.g. after the connection was lost for good
        public Task EndGame()
        {
            return _upstream.SendAsync(Operation.Create(ServerOperationType.GameEnd, new GameResultInfomation("END", "END", new GameStatus())));
        }

        public void ResetTube()
        {
            (_upstream, _downstream) = Tube.CreateDuplex();
            ForwardToServer();
        }

        public Task SendAsync(Operation<int> operation) => _downstream.SendAsync(operation);

        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create((int)type, objs));

        public new Task<IList<Operation<ServerOperationType>>> ReceiveAsync()
        {
            return _downstream.ReceiveAsync<IList<Operation<int>>>().ContinueWith(x =>
            {
                IList<Operation<ServerOperationType>> res = new List<Operation<ServerOperationType>>();
                var ans = x.Result;
                foreach (var p in ans)
                {
                    var temp = Operation.Create((ServerOperationType)p.OperationType);
                    temp.Arguments = p.Arguments;
                    res.Add(temp);
                }
                return res;
            });
        }

        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}
