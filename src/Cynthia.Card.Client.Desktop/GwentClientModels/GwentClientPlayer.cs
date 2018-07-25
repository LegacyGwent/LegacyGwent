using System.Threading.Tasks;
using Cynthia.Card.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    public class GwentClientPlayer : Player
    {
        private HubConnection _hubConnection;
        private Resulter<Operation<ClientOperationType>> _clientOperationResulter;
        private Resulter<Operation<ServerOperationType>> _serverOperationResulter;

        public GwentClientPlayer(HubConnection hubConnection)
        {
            _clientOperationResulter = new Resulter<Operation<ClientOperationType>>();
            _serverOperationResulter = new Resulter<Operation<ServerOperationType>>();
            _hubConnection = hubConnection;
        }
        public override async Task<Operation<ClientOperationType>> ClientOperation() => await _clientOperationResulter;
        public override Task ServerOperation(Operation<ServerOperationType> operation) => _serverOperationResulter.Result(operation);
        public Task SendOperation(Operation<ClientOperationType> operation) => _clientOperationResulter.Result(operation);
        public async Task<Operation<ServerOperationType>> GetOperation() => await _serverOperationResulter;
    }
}