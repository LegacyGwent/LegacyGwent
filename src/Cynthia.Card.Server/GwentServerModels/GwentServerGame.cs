using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentServerGame
    {
        public enum TwoPlayer//两个玩家
        {
            Player1,
            Player2
        }
        public Player[] Players { get; set; } = new Player[2]; //玩家数据传输/
        public bool[] IsPlayersLeader { get; set; } = { false, false };//玩家领袖是否使用/
        public GameCard[] PlayersLeader { get; set; } = new GameCard[2];//玩家领袖是?/
        public TwoPlayer GameRound { get; set; }//谁的的回合----
        public int[] PlayersWinCount { get; set; } = { 0, 0 };//玩家胜利场数/
        public IList<GameCard>[] PlayersDeck { get; set; } = new IList<GameCard>[2];//玩家卡组/
        public IList<GameCard>[] PlayersHandCard { get; set; } = new IList<GameCard>[2];//玩家手牌/
        public IList<GameCard>[][] PlayersPlace { get; set; } = new IList<GameCard>[2][];//玩家场地/
        public IList<GameCard>[] PlayersCemetery { get; set; } = new IList<GameCard>[2];//玩家墓地/
        public const int _Player1Index = 0;
        public const int _Player2Index = 1;
        public GwentServerGame(Player player1, Player player2)
        {
            //初始化游戏信息
            Players[_Player1Index] = player1;
            Players[_Player2Index] = player2;
            PlayersPlace[_Player1Index] = new List<GameCard>[3];
            PlayersPlace[_Player2Index] = new List<GameCard>[3];
            PlayersCemetery[_Player1Index] = new List<GameCard>();
            PlayersCemetery[_Player2Index] = new List<GameCard>();
            PlayersHandCard[_Player1Index] = new List<GameCard>();
            PlayersHandCard[_Player2Index] = new List<GameCard>();
            PlayersLeader[_Player1Index] = new GameCard(player1.Deck.Leader);
            PlayersLeader[_Player2Index] = new GameCard(player2.Deck.Leader);
            PlayersDeck[_Player1Index] = player1.Deck.Deck.Select(x => new GameCard(x)).ToList();
            PlayersDeck[_Player2Index] = player2.Deck.Deck.Select(x => new GameCard(x)).ToList();
        }
        public GameInfomation GetPlayerInfoMation(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                MyWinCount = PlayersWinCount[myPlayerIndex],
                EnemyWinCount = PlayersWinCount[enemyPlayerIndex],
                IsMyLeader = IsPlayersLeader[myPlayerIndex],
                IsEnemyLeader = IsPlayersLeader[enemyPlayerIndex],
                MyLeader = PlayersLeader[myPlayerIndex],
                EnemyLeader = PlayersLeader[enemyPlayerIndex],
                EnemyName = Players[enemyPlayerIndex].PlayerName,
                MyDeckCount = PlayersDeck[myPlayerIndex].Count(),
                EnemyDeckCardCount = PlayersDeck[enemyPlayerIndex].Count(),
                MyHandCard = PlayersHandCard[myPlayerIndex],
                EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.Visible ? new GameCard() : x),
                MyPlace = PlayersPlace[myPlayerIndex],
                EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() : item)
                ).ToArray(),
                MyCemetery = PlayersCemetery[myPlayerIndex],
                EnemyCemetery = PlayersCemetery[enemyPlayerIndex],
            };
        }

        public async Task<bool> Play()
        {
            await Players[_Player1Index].SendAsync(ServerOperationType.GameInfomation, GetPlayerInfoMation(TwoPlayer.Player1));
            await Players[_Player2Index].SendAsync(ServerOperationType.GameInfomation, GetPlayerInfoMation(TwoPlayer.Player2));
            var r = new Random();
            var end = (r.Next(2) == 1);
            await Players[_Player1Index].SendAsync(ServerOperationType.GameEnd, end);
            await Players[_Player2Index].SendAsync(ServerOperationType.GameEnd, !end);
            return true;
        }
    }
}