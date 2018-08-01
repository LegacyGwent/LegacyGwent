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
        public LocalPlayer Player { get; set; }
        public GwentClientService(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            Player = new LocalPlayer(HubConnection);
        }
        public Task<bool> Register(string username, string password, string playername) => HubConnection.InvokeAsync<bool>("Register", username, password, playername);
        public Task<UserInfo> Login(string username, string password) => HubConnection.InvokeAsync<UserInfo>("Login", username, password);
        public Task<bool> Match(int cardIndex) => HubConnection.InvokeAsync<bool>("Match", cardIndex);
        public Task SendOperation(Task<Operation<ServerOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);
        public Task StartAsync() => HubConnection.StartAsync();
        public Task StopAsync() => HubConnection.StopAsync();
    }
}