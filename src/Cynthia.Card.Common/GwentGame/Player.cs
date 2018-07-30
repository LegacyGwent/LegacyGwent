using System;
using System.Threading.Tasks;
using Alsein.Utilities.IO;

namespace Cynthia.Card
{
    public abstract class Player
    {

        public string PlayerName { get; set; }//玩家名
        public GwentDeck Deck { get; set; }//所用卡组
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
        public async Task<Operation<ServerOperationType>> ReceiveFromDownstreamAsync() => (await _downstream.ReceiveAsync<Operation<ServerOperationType>>()).Result;
        public async Task<Operation<UserOperationType>> ReceiveFromUpstreamAsync() => (await _upstream.ReceiveAsync<Operation<UserOperationType>>()).Result;
        public event Func<object, Task> ReceiveFromDownstream { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }
        public event Func<object, Task> ReceiveFromUpstream { add => _upstream.Receive += value; remove => _upstream.Receive -= value; }
    }
}