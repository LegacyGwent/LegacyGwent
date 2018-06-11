using System.Threading.Tasks;
using Cynthia.Card.Common.Models;
using Cynthia.Card.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class TestHub : Hub
    {
        public IMessagesService message { get; set; }
        public async Task SendMessage(ChatMessage msg)
        {
            message.AddMessage(msg);
            await Clients.All.SendAsync("GetChatMessage", msg);
        }
        public async Task SendCache()
        {
            await Clients.Caller.SendAsync("GetCacheMessage", message.GetMessage(0));
        }
    }
}