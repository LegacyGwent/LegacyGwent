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
            Player1 = 0,
            Player2 = 1
        }
        public Player[] Players { get; set; } = new Player[2]; //玩家数据传输/
        public bool[] IsPlayersLeader { get; set; } = { true, true };//玩家领袖是否可用/
        public GameCard[] PlayersLeader { get; set; } = new GameCard[2];//玩家领袖是?/
        public TwoPlayer GameRound { get; set; }//谁的的回合----
        public int RoundCount { get; set; } = 0;//有效比分的回合数
        public int CurrentRoundCount { get; set; } = 0;//当前小局
        public int[] PlayersWinCount { get; set; } = new int[2] { 0, 0 };//玩家胜利场数/
        public int[][] PlayersRoundResult { get; set; } = new int[3][];//三局r1 r2 r3
        public IList<GameCard>[] PlayersDeck { get; set; } = new IList<GameCard>[2];//玩家卡组/
        public IList<GameCard>[] PlayersHandCard { get; set; } = new IList<GameCard>[2];//玩家手牌/
        public IList<GameCard>[][] PlayersPlace { get; set; } = new IList<GameCard>[2][];//玩家场地/
        public IList<GameCard>[] PlayersCemetery { get; set; } = new IList<GameCard>[2];//玩家墓地/
        public Faction[] PlayersFaction { get; set; } = new Faction[2];//玩家们的势力
        public bool[] IsPlayersPass { get; set; } = new bool[2] { false, false };
        public const int _Player1Index = 0;
        public const int _Player2Index = 1;
        public async Task<bool> Play()
        {
            DrawCard(_Player1Index, 10);
            DrawCard(_Player2Index, 10);
            await SetAllInfo();
            //---------------------------------------------------------------------------------------
            await PlayerRound(_Player1Index);
            await PlayerRound(_Player2Index);
            await PlayerRound(_Player1Index);
            await PlayerRound(_Player2Index);
            await PlayerRound(_Player1Index);
            await PlayerRound(_Player2Index);
            //---------------------------------------------------------------------------------------
            await GameOverExecute();
            return true;
        }
        public async Task PlayerRound(int playerIndex)
        {
            //让玩家选择拖拽,或者Pass
            await Players[playerIndex].SendAsync(ServerOperationType.GetDragOrPass);
            //获取信息
            var cardInfo = (await Players[playerIndex].ReceiveAsync()).Arguments.ToArray()[0].ToType<string>().ToType<RoundInfo>();//接收玩家的选择,提取结果
            if (cardInfo.IsPass)
            {//Pass时候执行
                PlayersRoundResult[0][playerIndex] = 99999;
                IsPlayersPass[playerIndex] = true;
                if (IsPlayersPass[playerIndex == 0 ? 1 : 0] == true)
                {
                    //双方都已经pass,进入下一轮或者结束
                }
                //发送信息
            }
            else
            {//放置卡牌时执行
                await PlayCard(playerIndex, cardInfo);
                await Players[playerIndex].SendAsync(ServerOperationType.MyCardEffectEnd);
                await Players[playerIndex == 0 ? 1 : 0].SendAsync(ServerOperationType.EnemyCardEffectEnd);
            }
            //宣告回合结束
            await Players[playerIndex].SendAsync(ServerOperationType.RoundEnd);
        }
        public async Task<bool> PlayCard(int playerIndex, RoundInfo cardInfo)//哪一位玩家,打出第几张手牌,打到了第几排,第几列
        {
            //将放置信息发送给对手
            var enemyRowIndex = cardInfo.RowIndex == -3 ? cardInfo.RowIndex :
            (
                cardInfo.RowIndex == -2 ? -1 :
                (
                    cardInfo.RowIndex == -1 ? -2 :
                    (
                        cardInfo.RowIndex >= 3 ? cardInfo.RowIndex - 3 : cardInfo.RowIndex + 3
                    )
                )
            );
            var enemyCardInfo = new RoundInfo()
            {
                HandCardIndex = cardInfo.HandCardIndex,
                CardIndex = cardInfo.CardIndex,
                RowIndex = enemyRowIndex
            };
            //------------------------------------------------------------
            var card = default(GameCard);
            if (cardInfo.HandCardIndex == -1)//如果是-1,视为领袖卡
            {
                if (IsPlayersLeader[playerIndex] == false)
                    return false;
                card = PlayersLeader[playerIndex];
                IsPlayersLeader[playerIndex] = false;
            }
            else
            {
                if (cardInfo.HandCardIndex < 0 || cardInfo.HandCardIndex > PlayersHandCard[playerIndex].Count)//判断手牌合法
                    return false;
                card = PlayersHandCard[playerIndex][cardInfo.HandCardIndex];
                PlayersHandCard[playerIndex].RemoveAt(cardInfo.HandCardIndex);
            }
            //以上获得了卡牌,并且提取了出来
            //---可以进行发送
            await Players[playerIndex == 0 ? 1 : 0].SendAsync(ServerOperationType.EnemyCardDrag, enemyCardInfo, card);
            //这句话测试用
            if (cardInfo.RowIndex == -3)
            {
                //需要进行处理后进入墓地,如果是佚亡直接消除
                card.Armor = 0; //护甲归零
                card.HealthStatus = 0;//没有增益和受伤
                card.IsCardBack = false; //没有背面
                card.IsResilience = false;//没有坚韧
                card.IsGray = false;   //没有灰
                card.IsShield = false; //没有昆恩
                card.IsSpying = false; //没有间谍
                card.Conceal = false;  //没有隐藏
                card.IsReveal = false; //没有解释
                PlayersCemetery[playerIndex].Add(card);
                await SetCountInfo();
            }
            else if (cardInfo.RowIndex == -1 || cardInfo.RowIndex == -2)
            {
                //法术卡的话
                //执行效果代码之后...进入墓地
                PlayersCemetery[playerIndex].Add(card);
                await SetCountInfo();
            }
            else
            {
                //单位卡
                //放在了...玩家1还是玩家2的场地?
                var playerPlace = cardInfo.RowIndex >= 3 ? (playerIndex == 0 ? 1 : 0) : playerIndex;
                var rowIndex = cardInfo.RowIndex >= 3 ? cardInfo.RowIndex - 3 : cardInfo.RowIndex;
                PlayersPlace[playerPlace][rowIndex].Insert(cardInfo.CardIndex, card);
                await SetPointInfo();
                //----效果----?
            }
            PlayersRoundResult[CurrentRoundCount][playerIndex] += card.Strength;
            return true;
        }
        //玩家抽卡
        public void DrawCard(int playerIndex, int count)
        {
            if (count > PlayersDeck[playerIndex].Count) count = PlayersDeck[playerIndex].Count;
            for (var i = 0; i < count; i++)
            {
                PlayersHandCard[playerIndex].Add(PlayersDeck[playerIndex][0]);
                PlayersDeck[playerIndex].RemoveAt(0);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        //下面是发送数据包,或者进行一些初始化信息
        //根据当前信息,处理游戏结果
        public async Task GameOverExecute()
        {
            RoundCount = 1;
            if (PlayersRoundResult[0][_Player1Index] >= PlayersRoundResult[0][_Player2Index])
                PlayersWinCount[_Player1Index]++;
            if (PlayersRoundResult[0][_Player1Index] <= PlayersRoundResult[0][_Player2Index])
                PlayersWinCount[_Player2Index]++;
            if (PlayersRoundResult[1][_Player1Index] >= PlayersRoundResult[1][_Player2Index])
                PlayersWinCount[_Player1Index]++;
            if (PlayersRoundResult[1][_Player1Index] <= PlayersRoundResult[1][_Player2Index])
                PlayersWinCount[_Player2Index]++;
            if (PlayersRoundResult[2][_Player1Index] >= PlayersRoundResult[2][_Player2Index])
                PlayersWinCount[_Player1Index]++;
            if (PlayersRoundResult[2][_Player1Index] <= PlayersRoundResult[2][_Player2Index])
                PlayersWinCount[_Player2Index]++;
            int result = 0;//0为平, 1为玩家1胜利, 2为玩家2胜利
            if (PlayersWinCount[_Player1Index] == PlayersWinCount[_Player2Index])
                result = 0;
            if (PlayersWinCount[_Player1Index] > PlayersWinCount[_Player2Index])
                result = 1;
            if (PlayersWinCount[_Player1Index] < PlayersWinCount[_Player2Index])
                result = 2;
            await SendGameResult
            (
                TwoPlayer.Player1,
                result == 0 ? GameResultInfomation.GameStatus.Draw :
                (result == 1 ? GameResultInfomation.GameStatus.Win : GameResultInfomation.GameStatus.Lose),
                RoundCount,
                PlayersRoundResult[0][_Player1Index],
                PlayersRoundResult[0][_Player2Index],
                PlayersRoundResult[1][_Player1Index],
                PlayersRoundResult[1][_Player2Index],
                PlayersRoundResult[2][_Player1Index],
                PlayersRoundResult[2][_Player2Index]
            );
            await SendGameResult
            (
                TwoPlayer.Player2,
                result == 0 ? GameResultInfomation.GameStatus.Draw :
                (result == 1 ? GameResultInfomation.GameStatus.Lose : GameResultInfomation.GameStatus.Win),
                RoundCount,
                PlayersRoundResult[0][_Player2Index],
                PlayersRoundResult[0][_Player1Index],
                PlayersRoundResult[1][_Player2Index],
                PlayersRoundResult[1][_Player1Index],
                PlayersRoundResult[2][_Player2Index],
                PlayersRoundResult[2][_Player1Index]
            );
        }
        public GwentServerGame(Player player1, Player player2)
        {
            //初始化游戏信息
            PlayersRoundResult[0] = new int[2];
            PlayersRoundResult[1] = new int[2];
            PlayersRoundResult[2] = new int[2];
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
            IsPlayersLeader[_Player1Index] = true;
            IsPlayersLeader[_Player2Index] = true;
            PlayersLeader[_Player1Index] = new GameCard(player1.Deck.Leader) { DeckFaction = PlayersFaction[_Player1Index], Strength = GwentMap.CardMap[player1.Deck.Leader].Strength };
            PlayersLeader[_Player2Index] = new GameCard(player2.Deck.Leader) { DeckFaction = PlayersFaction[_Player2Index], Strength = GwentMap.CardMap[player2.Deck.Leader].Strength };
            //打乱牌组
            PlayersDeck[_Player1Index] = player1.Deck.Deck.Select(x => new GameCard(x) { DeckFaction = GwentMap.CardMap[player1.Deck.Leader].Faction, Strength = GwentMap.CardMap[x].Strength }).Mess().ToList();
            PlayersDeck[_Player2Index] = player2.Deck.Deck.Select(x => new GameCard(x) { DeckFaction = GwentMap.CardMap[player2.Deck.Leader].Faction, Strength = GwentMap.CardMap[x].Strength }).Mess().ToList();
        }
        //----------------------------------------------------------------------------------------------
        public Task SetAllInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetAllInfo, GetAllInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetAllInfo, GetAllInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetGameInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetGameInfo, GetGameInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetGameInfo, GetGameInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetCardsInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetCardsInfo, GetCardsInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetCardsInfo, GetCardsInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetPointInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetPointInfo, GetPointInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetPointInfo, GetPointInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetCountInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetCountInfo, GetCountInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetCountInfo, GetCountInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetPassInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetPassInfo, GetPassInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetPassInfo, GetPassInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetWinCountInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetWinCountInfo, GetWinCountInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetWinCountInfo, GetWinCountInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        public Task SetNameInfo()
        {
            var player1Task = Players[_Player1Index].SendAsync(ServerOperationType.SetNameInfo, GetNameInfo(TwoPlayer.Player1));
            var player2Task = Players[_Player2Index].SendAsync(ServerOperationType.SetNameInfo, GetNameInfo(TwoPlayer.Player2));
            return Task.WhenAll(player1Task, player2Task);
        }
        //---------------------------------------------------------
        public GameInfomation GetGameInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                MyRow1Point = PlayersPlace[myPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                MyRow2Point = PlayersPlace[myPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                MyRow3Point = PlayersPlace[myPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus),
                IsMyPlayersPass = IsPlayersPass[myPlayerIndex],
                IsEnemyPlayersPass = IsPlayersPass[enemyPlayerIndex],
                MyWinCount = PlayersWinCount[myPlayerIndex],
                EnemyWinCount = PlayersWinCount[enemyPlayerIndex],
                EnemyName = Players[enemyPlayerIndex].PlayerName,
                MyName = Players[myPlayerIndex].PlayerName,
                MyDeckCount = PlayersDeck[myPlayerIndex].Count(),
                EnemyDeckCount = PlayersDeck[enemyPlayerIndex].Count(),
                MyHandCount = PlayersHandCard[myPlayerIndex].Count() + (IsPlayersLeader[myPlayerIndex] ? 1 : 0),
                EnemyHandCount = PlayersHandCard[enemyPlayerIndex].Count() + (IsPlayersLeader[enemyPlayerIndex] ? 1 : 0),
                MyCemeteryCount = PlayersCemetery[myPlayerIndex].Count(),
                EnemyCemeteryCount = PlayersCemetery[enemyPlayerIndex].Count(),
            };
        }
        public GameInfomation GetCardsInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                IsMyLeader = IsPlayersLeader[myPlayerIndex],
                IsEnemyLeader = IsPlayersLeader[enemyPlayerIndex],
                MyLeader = PlayersLeader[myPlayerIndex],
                EnemyLeader = PlayersLeader[enemyPlayerIndex],
                MyHandCard = PlayersHandCard[myPlayerIndex],
                EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.IsReveal ? x : new GameCard() { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] }),
                MyPlace = PlayersPlace[myPlayerIndex],
                EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] } : item)
                ).ToArray(),
                MyCemetery = PlayersCemetery[myPlayerIndex],
                EnemyCemetery = PlayersCemetery[enemyPlayerIndex],
            };
        }
        public GameInfomation GetPointInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                MyRow1Point = PlayersPlace[myPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                MyRow2Point = PlayersPlace[myPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                MyRow3Point = PlayersPlace[myPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus)
            };
        }
        public GameInfomation GetCountInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {//手牌/ 卡组/ 墓地/
                MyDeckCount = PlayersDeck[myPlayerIndex].Count(),
                EnemyDeckCount = PlayersDeck[enemyPlayerIndex].Count(),
                MyHandCount = PlayersHandCard[myPlayerIndex].Count() + (IsPlayersLeader[myPlayerIndex] ? 1 : 0),
                EnemyHandCount = PlayersHandCard[enemyPlayerIndex].Count() + (IsPlayersLeader[enemyPlayerIndex] ? 1 : 0),
                MyCemeteryCount = PlayersCemetery[myPlayerIndex].Count(),
                EnemyCemeteryCount = PlayersCemetery[enemyPlayerIndex].Count()
            };
        }
        public GameInfomation GetPassInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                IsMyPlayersPass = IsPlayersPass[myPlayerIndex],
                IsEnemyPlayersPass = IsPlayersPass[enemyPlayerIndex]
            };
        }
        public GameInfomation GetWinCountInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                MyWinCount = PlayersWinCount[myPlayerIndex],
                EnemyWinCount = PlayersWinCount[enemyPlayerIndex]
            };
        }
        public GameInfomation GetNameInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                EnemyName = Players[enemyPlayerIndex].PlayerName,
                MyName = Players[myPlayerIndex].PlayerName
            };
        }

        //更新所有信息
        public GameInfomation GetAllInfo(TwoPlayer player)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return new GameInfomation()
            {
                MyRow1Point = PlayersPlace[myPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                MyRow2Point = PlayersPlace[myPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                MyRow3Point = PlayersPlace[myPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Sum(x => x.Strength + x.HealthStatus),
                EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Sum(x => x.Strength + x.HealthStatus),
                IsMyPlayersPass = IsPlayersPass[myPlayerIndex],
                IsEnemyPlayersPass = IsPlayersPass[enemyPlayerIndex],
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
                MyHandCount = PlayersHandCard[myPlayerIndex].Count() + (IsPlayersLeader[myPlayerIndex] ? 1 : 0),
                EnemyHandCount = PlayersHandCard[enemyPlayerIndex].Count() + (IsPlayersLeader[enemyPlayerIndex] ? 1 : 0),
                MyCemeteryCount = PlayersCemetery[myPlayerIndex].Count(),
                EnemyCemeteryCount = PlayersCemetery[enemyPlayerIndex].Count(),
                MyHandCard = PlayersHandCard[myPlayerIndex],
                EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.IsReveal ? x : new GameCard() { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] }),
                MyPlace = PlayersPlace[myPlayerIndex],
                EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
                (
                    x => x.Select(item => item.Conceal ? new GameCard() { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] } : item)
                ).ToArray(),
                MyCemetery = PlayersCemetery[myPlayerIndex],
                EnemyCemetery = PlayersCemetery[enemyPlayerIndex],
            };
        }
        //--------------------------------------
        public Task SendGameResult(TwoPlayer player, GameResultInfomation.GameStatus status,
            int roundCount = 0, int myR1Point = 0, int enemyR1Point = 0,
            int myR2Point = 0, int enemyR2Point = 0, int myR3Point = 0, int enemyR3Point = 0)
        {
            var myPlayerIndex = (player == TwoPlayer.Player1 ? _Player1Index : _Player2Index);
            var enemyPlayerIndex = (player == TwoPlayer.Player1 ? _Player2Index : _Player1Index);
            return Players[myPlayerIndex].SendAsync(ServerOperationType.GameEnd, new GameResultInfomation
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
    }
}