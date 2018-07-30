using Alsein.Utilities.LifetimeAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using Cynthia.Card.Common;
using System.Threading.Tasks;
using System;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class GwentClientService
    {
        public HubConnection HubConnection { get; set; }
        public GwentLocalPlayer Player { get; set; }
        public GwentClientService(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            Player = new GwentLocalPlayer(HubConnection);
            HubConnection.On<Operation<ServerOperationType>>("GameOperation", async operation => await Player.SendToDownstreamAsync(operation));
            HubConnection.On<Operation<ServerOperationType>>("GameOperation", x => Console.WriteLine("aaa"));
        }
        public async Task<bool> Register(string username, string password) => await HubConnection.InvokeAsync<bool>("Register", username, password);
        public async Task<bool> Login(string username, string password) => await HubConnection.InvokeAsync<bool>("Login", username, password);
        public async Task<bool> Match(int cardIndex) => await HubConnection.InvokeAsync<bool>("Match", cardIndex);
        public Task SendOperation(Task<Operation<ServerOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);
        public async Task StartAsync() => await HubConnection.StartAsync();
        public async Task StopAsync() => await HubConnection.StopAsync();
    }
}