using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using Cynthia.Card.Common.Models;
using System.Collections.Generic;

namespace Cynthia.Card.Client
{
    public class ChatSignalR
    {
        private HubConnection _client;
        public ChatSignalR(string url = "http://localhost:5000/hub/chat")
        {
            _client = new HubConnectionBuilder().WithUrl(url).Build();
            _client.On<ChatMessage>("GetChatMessage", msg => MessageProcessing.PrintMessage(msg));
            _client.On<IEnumerable<ChatMessage>>("GetMessageCache", msgs => MessageProcessing.PrintMessage(msgs));
        }
        public async Task Start()
        {
            await _client.StartAsync();
        }
        //发送消息 (触发服务端:转发消息给全部客户端ForwardMessage)
        public async void SendMessage(string name, string msg)
        {
            await _client.InvokeAsync("DistributeMessage", new ChatMessage() { Name = name, Time = DateTime.Now, Content = msg, Id = Guid.NewGuid().ToString() });
        }
        //获得缓存信息 (触发服务端:发送缓存信息SendMessageCache)
        public async Task GetMessageCache()
        {
            await _client.InvokeAsync("SendMessageCache");
        }
        public async Task Stop()
        {
            await _client.StopAsync();
        }
    }
}