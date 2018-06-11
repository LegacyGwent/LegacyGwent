using System.Threading.Tasks;
using Cynthia.Card.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class TestHub : Hub
    {
        public async Task SendMessage(ChatMessage msg)
        {
            await Clients.All.SendAsync("GetChatMessage", msg);
        }
    }
}