using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            //客户端调用SendMessage
            //服务端调用所有客户端上的ReceiveMessage
            await Clients.All.SendAsync("ReceiveMessage", $"{user}发送了消息:{message}");
        }
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