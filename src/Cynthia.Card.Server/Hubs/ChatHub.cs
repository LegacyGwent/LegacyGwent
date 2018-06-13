using System.Threading.Tasks;
using Cynthia.Card.Common.Models;
using Cynthia.Card.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ChatHub : Hub
    {
        public IMessagesService message { get; set; }
        //将消息转发给全部用户 (触发全部用户的GetChatMessage)
        public async Task DistributeMessage(ChatMessage msg)
        {
            message.AddMessage(msg);
            await Clients.All.SendAsync("GetChatMessage", msg);
        }
        //将消息转发给请求者 (触发请求者的GetMessageCache)
        public async Task SendMessageCache()
        {
            await Clients.Caller.SendAsync("GetMessageCache", message.GetLastMessage());
        }
    }
}