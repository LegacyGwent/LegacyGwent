using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        public enum PlayerRound//玩家回合
        {
            Player1,
            player2
        }
        public Player Player1 { get; set; } //玩家1 数据传输/
        public Player Player2 { get; set; } //玩家2 数据传输/
        public bool IsPlayer1Leader { get; set; } = false;//玩家1领袖是否使用/
        public bool IsPlayer2Leader { get; set; } = false;//玩家2领袖是否使用/
        public GameCard Player1Leader { get; set; }//玩家1领袖是?/
        public GameCard Player2Leader { get; set; }//玩家2领袖是?/
        public PlayerRound GameRound { get; set; }//谁的的回合----
        public int Player1WinCount { get; set; } = 0;//玩家1胜利场数/
        public int Player2WinCount { get; set; } = 0;//玩家2胜利场数/
        public IList<GameCard> Player1Deck { get; set; }//玩家1卡组/
        public IList<GameCard> Player2Deck { get; set; }//玩家2卡组/
        public IList<GameCard> Player1HandCard { get; set; }//玩家1手牌/
        public IList<GameCard> Player2HandCard { get; set; }//玩家2手牌/
        public IList<GameCard>[] Player1Place { get; set; }//玩家1场地/
        public IList<GameCard>[] Player2Place { get; set; }//玩家2场地/
        public IList<GameCard> Player1Cemetery { get; set; }//玩家1墓地/
        public IList<GameCard> Player2Cemetery { get; set; }//玩家2墓地/
        public GwentServerGame(Player player1, Player player2)
        {
            //初始化游戏信息
            Player1 = player1;
            Player2 = player2;
            Player1Place = new List<GameCard>[3];
            Player2Place = new List<GameCard>[3];
            Player1Cemetery = new List<GameCard>();
            Player2Cemetery = new List<GameCard>();
            Player1HandCard = new List<GameCard>();
            Player2HandCard = new List<GameCard>();
            Player1Leader = new GameCard() { CardIndex = player1.Deck.Leader };
            Player1Leader = new GameCard() { CardIndex = player1.Deck.Leader };
            Player1Deck = Player1.Deck.Deck.Select(x => new GameCard() { CardIndex = x }).ToList();
            Player2Deck = Player2.Deck.Deck.Select(x => new GameCard() { CardIndex = x }).ToList();
        }
        public GameInfomation GetPlayer1InfoMation()
        {
            return new GameInfomation()
            {
                IsMyLeader = IsPlayer1Leader,
                IsEnemyLeader = IsPlayer2Leader,
                MyLeader = Player1Leader,
                EnemyLeader = Player2Leader,
                EnemyName = Player2.PlayerName,
                MyDeckCount = Player1Deck.Count(),
                EnemyDeckCardCount = Player2Deck.Count(),
                MyHandCard = Player1HandCard,
                EnemyHandCard = Player2HandCard.Select(x => x.Visible ? new GameCard() : x),
                MyPlace = Player1Place,
                EnemyPlace = Player2Place.Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() { Conceal = true } : item)
                ).ToArray(),
                MyCemetery = Player1Cemetery,
                EnemyCemetery = Player2Cemetery,
            };
        }
        public GameInfomation GetPlayer2InfoMation()
        {
            return new GameInfomation()
            {
                IsMyLeader = IsPlayer2Leader,
                IsEnemyLeader = IsPlayer1Leader,
                MyLeader = Player2Leader,
                EnemyLeader = Player1Leader,
                EnemyName = Player1.PlayerName,
                MyDeckCount = Player2Deck.Count(),
                EnemyDeckCardCount = Player1Deck.Count(),
                MyHandCard = Player2HandCard,
                EnemyHandCard = Player1HandCard.Select(x => x.Visible ? new GameCard() : x),
                MyPlace = Player2Place,
                EnemyPlace = Player1Place.Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() { Conceal = true } : item)
                ).ToArray(),
                MyCemetery = Player2Cemetery,
                EnemyCemetery = Player1Cemetery,
            };
        }

        public async Task<bool> Play()
        {
            await Player1.SendAsync(ServerOperationType.GameInfomation, GetPlayer1InfoMation());
            await Player2.SendAsync(ServerOperationType.GameInfomation, GetPlayer2InfoMation());
            var r = new Random();
            var end = (r.Next(2) == 1);
            await Player1.SendAsync(ServerOperationType.GameEnd, end);
            await Player2.SendAsync(ServerOperationType.GameEnd, !end);
            return true;
        }
    }
}