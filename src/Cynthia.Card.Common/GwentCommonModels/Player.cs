using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.Common;

namespace Cynthia.Card.Common
{
    public abstract class Player
    {

        public string PlayerName { get; set; }//玩家名
        public GwentDeck Deck { get; set; }//所用卡组
        public abstract Task<Operation<ClientOperationType>> ClientOperation();
        public abstract Task ServerOperation(Operation<ServerOperationType> operation);
        public Task ServerOperation(ServerOperationType type, params object[] arguments) => ServerOperation(Operation.Create<ServerOperationType>(type, arguments));
    }
}