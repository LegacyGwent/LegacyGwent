using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentUserService GwentUserService { get; set; }
        public bool Register(string name, string password)
        {
            return GwentUserService.Register(new UserInfo(name, Context.ConnectionId), password);
        }
        public bool Login(string name, string password)
        {
            return GwentUserService.Login(new UserInfo(name, Context.ConnectionId), password);
        }
        public bool Match(int cardIndex)
        {
            return GwentUserService.Match(Context.ConnectionId, cardIndex);
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GwentUserService.Disconnect(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}