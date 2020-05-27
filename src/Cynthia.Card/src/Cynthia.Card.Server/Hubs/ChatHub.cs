using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Cynthia.Card.Server
{
    public class ChatHub : Hub
    {
        public ChatService _chatService;

        public ChatHub(ChatService chatService) => _chatService = chatService;

        // public 

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _chatService.Disconnect(Context.ConnectionId, exception);
        }
    }
}