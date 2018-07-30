using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        private ClientPlayer _player1;
        private ClientPlayer _player2;

        public GwentServerGame(ClientPlayer player1, ClientPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
        }

        public async Task<bool> Play()
        {
            await _player1.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息1");
            await _player1.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息2");
            await _player1.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息3");
            await _player2.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息1");
            await _player2.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息2");
            await _player2.SendViaUpstreamAsync(ServerOperationType.GameStart, "信息3");
            return true;
        }
    }
}