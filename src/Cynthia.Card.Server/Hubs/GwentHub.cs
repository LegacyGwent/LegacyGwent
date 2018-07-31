using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentServerService GwentServerService { get; set; }
        public bool Register(string username, string password, string playername) => GwentServerService.Register(username, password, playername);
        public UserInfo Login(string username, string password) => GwentServerService.Login(new User(username, Context.ConnectionId), password);
        public bool Match(int cardIndex) => GwentServerService.Match(Context.ConnectionId, cardIndex);
        public Task GameOperation(Operation<UserOperationType> operation) => GwentServerService.GameOperation(operation, Context.ConnectionId);
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GwentServerService.Disconnect(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}