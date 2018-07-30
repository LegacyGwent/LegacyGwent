using System.Threading.Tasks;
using Cynthia.Card.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    public class GwentClientPlayer : Player
    {
        public GwentClientPlayer(HubConnection hubConnection)
        {
            ReceiveFromDownstream += x => hubConnection.SendAsync("GameOperation", x);
        }
    }
}