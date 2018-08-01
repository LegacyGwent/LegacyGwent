using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(HubConnection hubConnection)
        {
            ReceiveFromUpstream += x => hubConnection.SendAsync("GameOperation", x);
            hubConnection.On<Operation<ServerOperationType>>("GameOperation", x =>
            {
                x.Arguments = x.Arguments.Select(item => item.ToType<string>());
                SendViaUpstreamAsync(x);
            });
        }
    }
}