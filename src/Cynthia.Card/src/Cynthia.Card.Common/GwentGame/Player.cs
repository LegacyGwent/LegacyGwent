using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alsein.Extensions.IO;

namespace Cynthia.Card
{
    public abstract class Player
    {

        public string PlayerName { get; set; }//玩家名
        public DeckModel Deck { get; set; }//所用卡组
        protected ITubeEndPoint _upstream;
        protected ITubeEndPoint _downstream;
        public Player()
        {
            (_upstream, _downstream) = Tube.CreateDuplex();
        }
        public Task SendAsync(Operation<ServerOperationType> operation) => _upstream.SendAsync(operation);
        public Task SendAsync(ServerOperationType type, params object[] data) => _upstream.SendAsync(Operation.Create(type, data));
        public async Task<Operation<UserOperationType>> ReceiveAsync()
        {
            return await _upstream.ReceiveAsync<Operation<UserOperationType>>();
        }
        public event Func<TubeReceiveEventArgs, Task> Receive { add => _upstream.Receive += value; remove => _upstream.Receive -= value; }
    }
}