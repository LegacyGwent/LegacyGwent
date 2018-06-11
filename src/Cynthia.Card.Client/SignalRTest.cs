using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using Cynthia.Card.Common.Models;

namespace Cynthia.Card.Client
{
    public class SignalRTest
    {
        private HubConnection _client;
        public SignalRTest(string url = "http://localhost:5000/test")
        {
            _client = new HubConnectionBuilder().WithUrl(url).Build();
            _client.On<ChatMessage>("GetChatMessage", msg =>
            {
                Console.WriteLine($"---------------------------------------------------");
                Console.WriteLine($"{msg.Name}   {msg.Time.ToShortTimeString()}");
                Console.WriteLine($"{msg.Content}");
            });
            _client.StartAsync().Wait();
        }
        public async void SendMessage(string name, string msg)
        {
            await _client.InvokeAsync("SendMessage", new ChatMessage() { Name = name, Time = DateTime.Now, Content = msg, Id = Guid.NewGuid().ToString() });
        }
        public async Task Stop()
        {
            await _client.StopAsync();
        }
    }
}