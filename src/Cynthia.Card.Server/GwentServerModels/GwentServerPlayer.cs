using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentServerPlayer : Player
    {
        public string ConnectionId { get; set; }//链接ID
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
        public override async Task<Operation<ClientOperationType>> ClientOperation()
        {
            return await _clientOperationResulter;
        }
        public override Task ServerOperation(Operation<ServerOperationType> operation)
        {
            return _hub().Clients.Client(ConnectionId).SendAsync("GameOperation", operation);
        }
        public Task UserOperation(Operation<ClientOperationType> clientoperation)
        {
            return _clientOperationResulter.Result(clientoperation);
        }
    }
}