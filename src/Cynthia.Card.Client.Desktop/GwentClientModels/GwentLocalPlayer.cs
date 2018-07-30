using System.Threading.Tasks;
using Cynthia.Card.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    public class GwentLocalPlayer : Player
    {
        public GwentLocalPlayer(HubConnection hubConnection)
        {
            ReceiveFromUpstream += x => hubConnection.SendAsync("GameOperation", x);
        }
    }
}