using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Cynthia.Card.Server
{
    public class ChatHub : Hub
    {
        public GwentServerService _gwentServerService;

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _gwentServerService.Disconnect(Context.ConnectionId, exception);//, true);
        }
    }
}