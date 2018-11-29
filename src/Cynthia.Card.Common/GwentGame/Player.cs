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
        protected IAsyncDataEndPoint _upstream;
        protected IAsyncDataEndPoint _downstream;
        public Player()
        {
            (_upstream, _downstream) = AsyncDataEndPoint.CreateDuplex();
        }
        public Task SendAsync(Operation<ServerOperationType> operation) => _upstream.SendAsync(operation);
        public Task SendAsync(ServerOperationType type, params object[] data) => _upstream.SendAsync(Operation.Create(type, data));
        public Task<Operation<UserOperationType>> ReceiveAsync() => _upstream.ReceiveAsync<Operation<UserOperationType>>();
        public event Func<ReceiveEventArgs, Task> Receive { add => _upstream.Receive += value; remove => _upstream.Receive -= value; }
    }
}