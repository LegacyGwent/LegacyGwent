using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentClientPlayer : Player
    {
        public UserInfo CurrentUser { get; set; }
        public GwentClientPlayer(UserInfo user, Func<IHubContext<GwentHub>> hub)
        {
            PlayerName = user.PlayerName;
            CurrentUser = user;
            ReceiveFromUpstream += x => hub().Clients.Client(CurrentUser.ConnectionId).SendAsync("GameOperation", x);
        }
    }
}