using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Extensions.IO;
using Alsein.Extensions.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Alsein.Extensions;
using Autofac;

namespace Cynthia.Card.Client
{
    public class LocalPlayer : Player
    {
        private readonly HubConnection _hubConnection;
        private readonly object _lifetime = new object();
        private readonly CancellationTokenSource _transportLifetime = new CancellationTokenSource();
        private readonly TaskCompletionSource<bool> _retired = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly IDisposable _gameSubscription;
        private readonly IDisposable _viewerSubscription;
        public bool IsRetired => _retired.Task.IsCompleted;

        public LocalPlayer(HubConnection hubConnection) : base()
        {
            _hubConnection = hubConnection;
            ForwardToServer();
            _gameSubscription = hubConnection.On<IList<Operation<int>>>("GameOperation", async x =>
            {
                if (!IsRetired) await _upstream.SendAsync(x);
            });
            _viewerSubscription = hubConnection.On<IList<Operation<int>>>("ViewerGameOperation", async x =>
            {
                if (!IsRetired) await _upstream.SendAsync(x);
            });
        }

        //接收到通讯层消息,发送到下游
        private void ForwardToServer()
        {
            ((Player)this).Receive += x => Forward("GameOperation", x.Result);
            ((Player)this).Receive += x => Forward("ViewerGameOperation", x.Result);
        }

        private Task Forward(string method, object result)
        {
            lock (_lifetime)
                return IsRetired ? Task.CompletedTask : _hubConnection.SendAsync(method, result, _transportLifetime.Token);
        }

        // Stop the running game on this client, e.g. after the connection was lost for good
        public Task EndGame()
        {
            return _upstream.SendAsync(Operation.Create(ServerOperationType.GameEnd, new GameResultInfomation("END", "END", new GameStatus())));
        }

        // Old scene callbacks keep this instance; they must never write to a new session.
        public void Retire()
        {
            lock (_lifetime)
                if (!_retired.TrySetResult(true)) return;
            _transportLifetime.Cancel();
            _gameSubscription.Dispose();
            _viewerSubscription.Dispose();
        }

        public Task SendAsync(Operation<int> operation)
        {
            lock (_lifetime)
                return IsRetired ? Task.CompletedTask : _downstream.SendAsync(operation);
        }

        public Task SendAsync(UserOperationType type, params object[] objs) => SendAsync(Operation.Create((int)type, objs));

        public new async Task<IList<Operation<ServerOperationType>>> ReceiveAsync()
        {
                if (IsRetired) return new List<Operation<ServerOperationType>>();
                var receive = _downstream.ReceiveAsync<IList<Operation<int>>>();
                if (await Task.WhenAny(receive, _retired.Task) != receive || IsRetired)
                    return new List<Operation<ServerOperationType>>();
                IList<Operation<ServerOperationType>> res = new List<Operation<ServerOperationType>>();
                var ans = await receive;
                foreach (var p in ans)
                {
                    var temp = Operation.Create((ServerOperationType)p.OperationType);
                    temp.Arguments = p.Arguments;
                    res.Add(temp);
                }
                return res;
        }

        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
    }
}
