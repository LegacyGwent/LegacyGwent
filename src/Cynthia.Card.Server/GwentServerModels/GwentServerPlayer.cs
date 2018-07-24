using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentServerPlayer
    {

        public string PlayerName { get; set; }//玩家名
        public string ConnectionId { get; set; }//链接ID
        public GwentDeck Deck { get; set; }//所用卡组
        private Func<IHubContext<GwentHub>> _hub;
        private Resulter<Operation<ClientOperationType>> _clientOperationResulter;
        public GwentServerPlayer(UserInfo user, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = user.PlayerName;
            ConnectionId = user.ConnectionId;
            _clientOperationResulter = new Resulter<Operation<ClientOperationType>>();
            _hub = hub;
        }
        public GwentServerPlayer(string playerName, string connectionId, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = playerName;
            ConnectionId = connectionId;
            _hub = hub;
        }
        public async Task<Operation<ClientOperationType>> GetOperation()
        {
            return await _clientOperationResulter;
        }
        public Task SendOperation(Operation<ServerOperationType> operation)
        {
            return _hub().Clients.Client(ConnectionId).SendAsync("GameOperation", operation);
        }
    }
}