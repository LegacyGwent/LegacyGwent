using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using Cynthia.Card.Common.Models;
using System.Collections.Generic;

namespace Cynthia.Card.Client
{
    public class SignalRTest
    {
        private HubConnection _client;
        public SignalRTest(string url = "http://localhost:5000/hub/test")
        {
            _client = new HubConnectionBuilder().WithUrl(url).Build();
            _client.On<ChatMessage>("GetChatMessage", msg => MessageProcessing.PrintMessage(msg));
            _client.On<IEnumerable<ChatMessage>>("GetCacheMessage", msgs => MessageProcessing.PrintMessage(msgs));
        }
        public async Task Start()
        {
            await _client.StartAsync();
        }
        public async void SendMessage(string name, string msg)
        {
            await _client.InvokeAsync("SendMessage", new ChatMessage() { Name = name, Time = DateTime.Now, Content = msg, Id = Guid.NewGuid().ToString() });
        }
        public async Task GetCacheMessage()
        {
            await _client.InvokeAsync("SendCache");
        }
        public async Task Stop()
        {
            await _client.StopAsync();
        }
    }
}