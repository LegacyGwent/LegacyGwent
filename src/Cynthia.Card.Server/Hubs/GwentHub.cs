using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentServerService GwentServerService { get; set; }
        public bool Register(string name, string password) => GwentServerService.Register(new UserInfo(name, Context.ConnectionId), password);
        public bool Login(string username, string password) => GwentServerService.Login(new UserInfo(username, Context.ConnectionId), password);
        public bool Match(int cardIndex) => GwentServerService.Match(Context.ConnectionId, cardIndex);
        public Task GameOperation(Operation<UserOperationType> operation) => GwentServerService.GameOperation(operation, Context.ConnectionId);
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GwentServerService.Disconnect(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}