using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public GwentServerGame(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public async Task<bool> Play()
        {
            Player1.Deck.Deck = Player1.Deck.Deck.Mess().ToArray();
            Player2.Deck.Deck = Player2.Deck.Deck.Mess().ToArray();
            await Player1.SendAsync(ServerOperationType.GameStart, new GameInfomation()
            {
                OpponentName = Player2.PlayerName,
                OpponentCardCount = Player2.Deck.Deck.Length,
                YourHandCard = Player1.Deck.Deck.Take(10)
            });
            await Player2.SendAsync(ServerOperationType.GameStart, new GameInfomation()
            {
                OpponentName = Player1.PlayerName,
                OpponentCardCount = Player1.Deck.Deck.Length,
                YourHandCard = Player2.Deck.Deck.Take(10)
            });
            var r = new Random();
            var end = (r.Next(2) == 1);
            await Player1.SendAsync(ServerOperationType.GameEnd, end);
            await Player2.SendAsync(ServerOperationType.GameEnd, !end);
            return true;
        }
    }
}