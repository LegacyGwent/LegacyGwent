using System;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Algorithms;

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
            _player1.Deck.Deck = _player1.Deck.Deck.Mess().ToArray();
            _player2.Deck.Deck = _player2.Deck.Deck.Mess().ToArray();
            await _player1.SendViaUpstreamAsync(ServerOperationType.GameStart, new GameInfomation()
            {
                OpponentName = _player2.PlayerName,
                OpponentCardCount = _player2.Deck.Deck.Length,
                YourHandCard = _player1.Deck.Deck.Take(10)
            });
            await _player2.SendViaUpstreamAsync(ServerOperationType.GameStart, new GameInfomation()
            {
                OpponentName = _player1.PlayerName,
                OpponentCardCount = _player1.Deck.Deck.Length,
                YourHandCard = _player2.Deck.Deck.Take(10)
            });
            var r = new Random();
            var end = (r.Next(2) == 1);
            await _player1.SendViaUpstreamAsync(ServerOperationType.GameEnd, end);
            await _player2.SendViaUpstreamAsync(ServerOperationType.GameEnd, !end);
            return true;
        }
    }
}