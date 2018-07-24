using System.Collections.Generic;
using System.Threading.Tasks;
using Cynthia.Card.Common;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ChatHub : Hub
    {
        public IMessagesService MessageService { get; set; }
        //将消息转发给全部用户 (触发全部用户的GetChatMessage)
        public async Task DistributeMessage(ChatMessage msg)
        {
            lock (MessageService)
            {
                MessageService.AddMessage(msg);
            }
            await Clients.All.SendAsync("GetChatMessage", msg);
        }
        //将消息转发给请求者 (触发请求者的GetMessageCache)
        public async Task SendMessageCache()
        {
            IEnumerable<ChatMessage> messageCache;
            lock (MessageService)
            {
                messageCache = MessageService.GetLastMessage();
            }
            await Clients.Caller.SendAsync("GetMessageCache", messageCache);
        }
    }
}