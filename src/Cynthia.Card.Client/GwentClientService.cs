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
        public UserInfo User { get; set; }
        public GwentClientService(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            Player = new LocalPlayer(HubConnection);
        }
        public Task<bool> Register(string username, string password, string playername) => HubConnection.InvokeAsync<bool>("Register", username, password, playername);
        public async Task<UserInfo> Login(string username, string password)
        {
            User = await HubConnection.InvokeAsync<UserInfo>("Login", username, password);
            if (User != null)
                Player.PlayerName = User.PlayerName;
            return User;
        }
        public Task<bool> Match(int cardIndex)
        {
            Player.Deck = User.Decks[cardIndex];
            return HubConnection.InvokeAsync<bool>("Match", cardIndex);
        }
        public Task<bool> AddDeck(DeckModel deck) => HubConnection.InvokeAsync<bool>("AddDeck", deck);
        public Task<bool> RemoveDeck(int cardIndex) => HubConnection.InvokeAsync<bool>("AddDeck", cardIndex);
        public Task<bool> ModifyDeck(int cardIndex, DeckModel deck) => HubConnection.InvokeAsync<bool>("AddDeck", cardIndex, deck);
        public Task SendOperation(Task<Operation<UserOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);
        public Task StartAsync() => HubConnection.StartAsync();
        public Task StopAsync() => HubConnection.StopAsync();
    }
}