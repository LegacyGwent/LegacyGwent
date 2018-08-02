using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Utilities.IO;

namespace Cynthia.Card
{
    public abstract class Player
    {

        public string PlayerName { get; set; }//玩家名
        public DeckModel Deck { get; set; }//所用卡组
        private IAsyncDataEndPoint _upstream;
        private IAsyncDataEndPoint _downstream;
        public Player()
        {
            (_upstream, _downstream) = AsyncDataEndPoint.CreateDuplex();
        }
        public Task SendViaDownstreamAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendViaUpstreamAsync(Operation<ServerOperationType> operation) => _upstream.SendAsync(operation);
        public Task SendViaDownstreamAsync(UserOperationType type, params object[] data) => _downstream.SendAsync(Operation.Create(type, data));
        public Task SendViaUpstreamAsync(ServerOperationType type, params object[] data) => _upstream.SendAsync(Operation.Create(type, data));
        public Task<Operation<ServerOperationType>> ReceiveFromDownstreamAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public Task<Operation<UserOperationType>> ReceiveFromUpstreamAsync() => _upstream.ReceiveAsync<Operation<UserOperationType>>();
        public event Func<ReceiveEventArgs, Task> ReceiveFromDownstream { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
        public event Func<ReceiveEventArgs, Task> ReceiveFromUpstream { add => _upstream.Receive += value; remove => _upstream.Receive -= value; }
    }
}