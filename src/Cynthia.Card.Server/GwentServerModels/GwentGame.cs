using System.Threading.Tasks;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class GwentGame
    {
        private GwentServerPlayer _player1;
        private GwentServerPlayer _player2;

        public GwentGame(GwentServerPlayer player1, GwentServerPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
        }

        public async Task<bool> Play()
        {
            await _player1.SendOperation(Operation.Create<ServerOperationType>(ServerOperationType.GameStart, "aaa"));
            return true;
        }
    }
}