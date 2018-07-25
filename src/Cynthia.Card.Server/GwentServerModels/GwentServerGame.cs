using System.Threading.Tasks;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        private GwentServerPlayer _player1;
        private GwentServerPlayer _player2;

        public GwentServerGame(GwentServerPlayer player1, GwentServerPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
        }

        public async Task<bool> Play()
        {
            await _player1.ServerOperation(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息1"));
            await _player1.ServerOperation(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息2"));
            await _player1.ServerOperation(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "信息3"));
            return true;
        }
    }
}