using System.Threading.Tasks;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        private GwentClientPlayer _player1;
        private GwentClientPlayer _player2;

        public GwentServerGame(GwentClientPlayer player1, GwentClientPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
        }

        public async Task<bool> Play()
        {
            await _player1.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息1"));
            await _player1.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息2"));
            await _player1.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息3"));
            await _player2.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息1"));
            await _player2.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息2"));
            await _player2.SendToDownstreamAsync(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息3"));
            return true;
        }
    }
}