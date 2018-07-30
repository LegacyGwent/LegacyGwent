using System;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentClientPlayer : Player
    {
        public UserInfo CurrentUser { get; set; }
        public GwentClientPlayer(UserInfo user, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = user.PlayerName;
            CurrentUser = user;
            ReceiveFromDownstream += x => hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", x);
        }
    }
}