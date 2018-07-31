using System;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class ClientPlayer : Player
    {
        public User CurrentUser { get; set; }
        public ClientPlayer(User user, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = user.PlayerName;
            CurrentUser = user;
            ReceiveFromDownstream += x => hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", x);
        }
    }
}