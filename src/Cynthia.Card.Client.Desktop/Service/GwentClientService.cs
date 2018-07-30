using System;
using System.Threading.Tasks;
using Alsein.Utilities.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;

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
            HubConnection.On<Operation<ServerOperationType>>("GameOperation", x => Player.SendViaUpstreamAsync(x));
        }
        public async Task<bool> Register(string username, string password) => await HubConnection.InvokeAsync<bool>("Register", username, password);
        public async Task<bool> Login(string username, string password) => await HubConnection.InvokeAsync<bool>("Login", username, password);
        public async Task<bool> Match(int cardIndex) => await HubConnection.InvokeAsync<bool>("Match", cardIndex);
        public Task SendOperation(Task<Operation<ServerOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);
        public async Task StartAsync() => await HubConnection.StartAsync();
        public async Task StopAsync() => await HubConnection.StopAsync();
    }
}