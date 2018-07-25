using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cynthia.Card.Common;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentServerService GwentUserService { get; set; }
        public bool Register(string name, string password) => GwentUserService.Register(new UserInfo(name, Context.ConnectionId), password);
        public bool Login(string username, string password) => GwentUserService.Login(new UserInfo(username, Context.ConnectionId), password);
        public bool Match(int cardIndex) => GwentUserService.Match(Context.ConnectionId, cardIndex);
        public Task GameOperation(Operation<ClientOperationType> operation) => GwentUserService.GameOperation(operation, Context.ConnectionId);
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GwentUserService.Disconnect(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}