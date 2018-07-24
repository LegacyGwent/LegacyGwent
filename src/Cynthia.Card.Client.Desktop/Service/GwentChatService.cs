using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using Cynthia.Card.Common;
using System.Collections.Generic;
using Alsein.Utilities.LifetimeAnnotations;


namespace Cynthia.Card.Client
{
    [Singleton]
    public class GwentChatService
    {
        public HubConnection HubConnection { get; set; }
        public GwentChatService(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            HubConnection.On<ChatMessage>("GetChatMessage", msg => MessageProcessing.PrintMessage(msg));
            HubConnection.On<IEnumerable<ChatMessage>>("GetMessageCache", msgs => MessageProcessing.PrintMessage(msgs));
        }

        //发送消息 (触发服务端:转发消息给全部客户端ForwardMessage)
        public async void SendMessage(string name, string msg)
        {
            await HubConnection.InvokeAsync("DistributeMessage", new ChatMessage() { Name = name, Time = DateTime.Now, Content = msg, Id = Guid.NewGuid().ToString() });
        }
        //获得缓存信息 (触发服务端:发送缓存信息SendMessageCache)
        public async Task GetMessageCache()
        {
            await HubConnection.InvokeAsync("SendMessageCache");
        }
        public async Task StartAsync()
        {
            await HubConnection.StartAsync();
        }
        public async Task StopAsync()
        {
            await HubConnection.StopAsync();
        }
    }
}