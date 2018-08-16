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
        public Faction[] PlayersFaction { get; set; } = new Faction[2];
        public const int _Player1Index = 0;
        public const int _Player2Index = 1;
        public GwentServerGame(Player player1, Player player2)
        {
            //初始化游戏信息
            Players[_Player1Index] = player1;
            Players[_Player2Index] = player2;
            PlayersPlace[_Player1Index] = new List<GameCard>[3];
            PlayersPlace[_Player2Index] = new List<GameCard>[3];
            PlayersFaction[_Player1Index] = GwentMap.CardMap[player1.Deck.Leader].Faction;
            PlayersFaction[_Player2Index] = GwentMap.CardMap[player2.Deck.Leader].Faction;
            //----------------------------------------------------
            PlayersPlace[_Player1Index][0] = new List<GameCard>();
            PlayersPlace[_Player2Index][0] = new List<GameCard>();
            PlayersPlace[_Player1Index][1] = new List<GameCard>();
            PlayersPlace[_Player2Index][1] = new List<GameCard>();
            PlayersPlace[_Player1Index][2] = new List<GameCard>();
            PlayersPlace[_Player2Index][2] = new List<GameCard>();
            //----------------------------------------------------
            PlayersCemetery[_Player1Index] = new List<GameCard>();
            PlayersCemetery[_Player2Index] = new List<GameCard>();
            PlayersHandCard[_Player1Index] = new List<GameCard>();
            PlayersHandCard[_Player2Index] = new List<GameCard>();
            PlayersLeader[_Player1Index] = new GameCard(player1.Deck.Leader) { DeckFaction = PlayersFaction[_Player1Index] };
            PlayersLeader[_Player2Index] = new GameCard(player2.Deck.Leader) { DeckFaction = PlayersFaction[_Player2Index] };
            PlayersDeck[_Player1Index] = player1.Deck.Deck.Select(x => new GameCard(x) { DeckFaction = GwentMap.CardMap[player1.Deck.Leader].Faction }).ToList();
            PlayersDeck[_Player2Index] = player2.Deck.Deck.Select(x => new GameCard(x) { DeckFaction = GwentMap.CardMap[player2.Deck.Leader].Faction }).ToList();
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
                MyName = Players[myPlayerIndex].PlayerName,
                MyDeckCount = PlayersDeck[myPlayerIndex].Count(),
                EnemyDeckCount = PlayersDeck[enemyPlayerIndex].Count(),
                MyHandCard = PlayersHandCard[myPlayerIndex],
                EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.IsReveal ? new GameCard() { IsCardBack = true } : x),
                MyPlace = PlayersPlace[myPlayerIndex],
                EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() { IsCardBack = true } : item)
                ).ToArray(),
                MyCemetery = PlayersCemetery[myPlayerIndex],
                EnemyCemetery = PlayersCemetery[enemyPlayerIndex],
            };
        }
        public Task SendGameResult(TwoPlayer player, GameResultInfomation.GameStatus status,
            int roundCount = 0, int myR1Point = 0, int enemyR1Point = 0,
            int myR2Point = 0, int enemyR2Point = 0, int myR3Point = 0, int enemyR3Point = 0)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return Players[_Player1Index].SendAsync(ServerOperationType.GameEnd, new GameResultInfomation
            (
                Players[myPlayerIndex].PlayerName,
                Players[enemyPlayerIndex].PlayerName,
                gameStatu: status,
                roundCount,
                myR1Point,
                enemyR1Point,
                myR2Point,
                enemyR2Point,
                myR3Point,
                enemyR3Point
            ));
        }

        public async Task<bool> Play()
        {
            await Players[_Player1Index].SendAsync(ServerOperationType.GameStart, GetPlayerInfoMation(TwoPlayer.Player1));
            await Players[_Player2Index].SendAsync(ServerOperationType.GameStart, GetPlayerInfoMation(TwoPlayer.Player2));
            var r = new Random();
            var end = r.Next(2);
            await SendGameResult(TwoPlayer.Player1, GameResultInfomation.GameStatus.Win, 1, 1, 0);
            await SendGameResult(TwoPlayer.Player2, GameResultInfomation.GameStatus.Lose, 1, 0, 1);
            return true;
        }
    }
}