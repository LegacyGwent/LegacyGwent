using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card.Server
{
    public class GwentServerGame : IGwentServerGame
    {
        public Action<GameResult> GameResultEvent{ get; set; }
        public int[] RedCoin { get; private set; } = new int[3];
        public Pipeline OperactionList { get; private set; } = new Pipeline();
        private readonly GwentCardTypeService _gwentCardTypeService;
        public int _randomSeed;
        public Random RNG { get; private set; }
        public int RowMaxCount { get; set; } = 9;
        public IList<(int PlayerIndex, GameCard CardId)> HistoryList { get; set; } = new List<(int, GameCard)>();
        public GameDeck[] PlayerBaseDeck { get; set; } = new GameDeck[2];
        public Player[] Players { get; set; } = new Player[2]; //玩家数据传输/
        public bool[] IsPlayersLeader { get; set; } = { true, true };//玩家领袖是否可用/
        public IList<GameCard>[] PlayersLeader { get; set; } = new IList<GameCard>[2];//玩家领袖是?/
        public TwoPlayer GameRound { get; set; }//谁的的回合----
        public int RoundCount { get; set; } = 0;//有效比分的回合数
        public int CurrentRoundCount { get; set; } = 0;//当前小局
        public int[] PlayersWinCount { get; set; } = new int[2] { 0, 0 };//玩家胜利场数/
        public int[][] PlayersRoundResult { get; set; } = new int[3][];//三局r1 r2 r3
        public IList<GameCard>[] PlayersDeck { get; set; } = new IList<GameCard>[2];//玩家卡组/
        public IList<GameCard>[] PlayersHandCard { get; set; } = new IList<GameCard>[2];//玩家手牌/
        public IList<GameCard>[][] PlayersPlace { get; set; } = new IList<GameCard>[2][];//玩家场地/
        // public RowStatus[][] GameRowStatus { get; set; } = new RowStatus[2][];//玩家天气
        public GameRow[][] GameRowEffect { get; set; } = new GameRow[2][];//玩家天气效果
        public IList<GameCard>[] PlayersCemetery { get; set; } = new IList<GameCard>[2];//玩家墓地/
        public IList<GameCard>[] PlayersStay { get; set; } = new IList<GameCard>[2];//玩家悬牌
        public Faction[] PlayersFaction { get; set; } = new Faction[2];//玩家们的势力
        public bool[] IsPlayersPass { get; set; } = new bool[2] { false, false };
        public bool[] IsPlayersMulligan { get; set; } = new bool[2] { false, false };
        public int Player1Index { get; } = 0;
        public int Player2Index { get; } = 1;
        public (int? PlayerIndex, int HPoint) WhoHeight
        {
            get
            {
                var player1Row1Point = PlayersPlace[Player1Index][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player1Row2Point = PlayersPlace[Player1Index][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player1Row3Point = PlayersPlace[Player1Index][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player2Row1Point = PlayersPlace[Player2Index][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player2Row2Point = PlayersPlace[Player2Index][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player2Row3Point = PlayersPlace[Player2Index][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
                var player1PlacePoint = (player1Row1Point + player1Row2Point + player1Row3Point);
                var player2PlacePoint = (player2Row1Point + player2Row2Point + player2Row3Point);
                if (player1PlacePoint > player2PlacePoint)
                    return (Player1Index, player1PlacePoint - player2PlacePoint);
                else if (player1PlacePoint < player2PlacePoint)
                    return (Player2Index, player2PlacePoint - player1PlacePoint);
                else
                    return (null, 0);
            }
        }


        private readonly TaskCompletionSource<int> _setGameEnd = new TaskCompletionSource<int>();

        public async Task PlayGame()
        {
            //###游戏开始###
            //双方抽牌10张
            await SendEvent(new OnGameStart());
            await LogicDrawCard(Player1Index, 10);//不会展示动画的,抽牌
            await LogicDrawCard(Player2Index, 10);
            await SetAllInfo();//更新玩家所有数据
            //----------------------------------------------------------------------------------------
            await PlayerBigRound(3, 3);//双方轮流执行回合|第一小局 (传入双方可进行的调度次数)
            await DrawCard(2, 2);//同时抽牌的动画,双方都看到自己先抽牌
            await PlayerBigRound(1, 1);//双方轮流执行回合|第二小局
            if (PlayersWinCount[Player1Index] < 2 && PlayersWinCount[Player2Index] < 2)//如果前两局没有分出结果
            {
                await DrawCard(1, 1);
                await PlayerBigRound(1, 1);//双方轮流执行回合|第三小局
            }
            //-----------------------------------------------------------------------------------------
            await GameOverExecute();//发送游戏结束信息
        }

        public async Task Play()
        {
            await Task.WhenAny(PlayGame(), _setGameEnd.Task);
            await SendOperactionList();
        }

        public async Task GameEnd(int winPlayerIndex, Exception exception)
        {
            if (exception == null)
                // await MessageBox("对方的账号被强制顶下线,比赛结束");
                await MessageBox("对方已断开连接,比赛结束!");
            else
                await MessageBox(exception.Message);


            var redIndex = RedCoin[0];
            var blueIndex = AnotherPlayer(redIndex);
            var result = new GameResult()
            {
                RedPlayerName = Players[redIndex].PlayerName,
                BluePlayerName = Players[blueIndex].PlayerName,
                RedLeaderId = PlayerBaseDeck[redIndex].Leader.CardId,
                BlueLeaderId = PlayerBaseDeck[blueIndex].Leader.CardId,
                RedDeckName = PlayerBaseDeck[redIndex].Name,
                BlueDeckName = PlayerBaseDeck[blueIndex].Name,
                Time = DateTime.UtcNow,
                RedWinCount = PlayersWinCount[redIndex],
                BlueWinCount = PlayersWinCount[blueIndex],
                ValidCount = RoundCount,
                RedPlayerGameResultStatus = redIndex==winPlayerIndex?GameStatus.Win:GameStatus.Lose,
                RedScore = new int[] { PlayersRoundResult[0][redIndex], PlayersRoundResult[1][redIndex], PlayersRoundResult[2][redIndex] },
                BlueScore = new int[] { PlayersRoundResult[0][blueIndex], PlayersRoundResult[1][blueIndex], PlayersRoundResult[2][blueIndex] },
            };
            GameResultEvent(result);

            await Task.WhenAll(SendGameResult(winPlayerIndex, GameStatus.Win),SendGameResult(AnotherPlayer(winPlayerIndex), GameStatus.Lose));
            _setGameEnd.SetResult(winPlayerIndex);
        }

        public async Task BigRoundEnd()//小局结束,进行收场
        {
            await ClientDelay(500);
            var player1Row1Point = PlayersPlace[Player1Index][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player1Row2Point = PlayersPlace[Player1Index][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player1Row3Point = PlayersPlace[Player1Index][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row1Point = PlayersPlace[Player2Index][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row2Point = PlayersPlace[Player2Index][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row3Point = PlayersPlace[Player2Index][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player1PlacePoint = (player1Row1Point + player1Row2Point + player1Row3Point);
            var player2PlacePoint = (player2Row1Point + player2Row2Point + player2Row3Point);
            PlayersRoundResult[CurrentRoundCount][Player1Index] = player1PlacePoint;
            PlayersRoundResult[CurrentRoundCount][Player2Index] = player2PlacePoint;
            if (player1PlacePoint > player2PlacePoint)
            {
                GameRound = TwoPlayer.Player1;
                PlayersWinCount[Player1Index]++;
            }
            else if (player2PlacePoint > player1PlacePoint)
            {
                GameRound = TwoPlayer.Player2;
                PlayersWinCount[Player2Index]++;
            }
            else
            {
                GameRound = AnotherPlayer(RedCoin[CurrentRoundCount]) == Player1Index ? TwoPlayer.Player1 : TwoPlayer.Player2;
                PlayersWinCount[Player1Index]++;
                PlayersWinCount[Player2Index]++;
            }
            RoundCount++;//有效回合的总数
            CurrentRoundCount++;//当前回合
            if (CurrentRoundCount <= 2)
            {
                RedCoin[CurrentRoundCount] = GameRound.ToPlayerIndex(this);
            }
            IsPlayersPass[Player1Index] = false;
            IsPlayersPass[Player2Index] = false;
            await SetWinCountInfo();//设置小皇冠图标
            await SetPassInfo();//重置pass标记
            //首先应该先表示信息
            //-+/*/展示点数
            await Task.WhenAll
            (
                Players[Player1Index].SendAsync(ServerOperationType.BigRoundShowPoint, new BigRoundInfomation()
                {
                    MyPoint = player1PlacePoint,
                    EnemyPoint = player2PlacePoint,
                    MyWinCount = PlayersWinCount[Player1Index],
                    EnemyWinCount = PlayersWinCount[Player2Index],
                    GameStatus = player2PlacePoint == player1PlacePoint ? GameStatus.Draw :
                    (player2PlacePoint > player1PlacePoint ? GameStatus.Lose : GameStatus.Win),
                    Title = player2PlacePoint == player1PlacePoint ? "本局平局" :
                    (player2PlacePoint > player1PlacePoint ? "本局失败!" : "本局胜利!")
                }),
                Players[Player2Index].SendAsync(ServerOperationType.BigRoundShowPoint, new BigRoundInfomation()
                {
                    MyPoint = player2PlacePoint,
                    EnemyPoint = player1PlacePoint,
                    MyWinCount = PlayersWinCount[Player2Index],
                    EnemyWinCount = PlayersWinCount[Player1Index],
                    GameStatus = player2PlacePoint == player1PlacePoint ? GameStatus.Draw :
                    (player2PlacePoint < player1PlacePoint ? GameStatus.Lose : GameStatus.Win),
                    Title = player2PlacePoint == player1PlacePoint ? "本局平局" :
                    (player2PlacePoint < player1PlacePoint ? "本局失败!" : "本局胜利!"),
                })
            );
            await ClientDelay(1500);
            if (PlayersWinCount[Player1Index] >= 2 || PlayersWinCount[Player2Index] >= 2)//如果前两局没有分出结果
            {
                await Task.WhenAll(Players[Player1Index].SendAsync(ServerOperationType.BigRoundShowClose)
                            , Players[Player2Index].SendAsync(ServerOperationType.BigRoundShowClose));
                return;
            }
            //清理天气
            foreach (var row in GameRowEffect.SelectMany(x => x))
            {
                await row.SetStatus<NoneStatus>();
            }
            //-+/*/展示信息
            await Task.WhenAll(Players[Player1Index].SendAsync(ServerOperationType.BigRoundSetMessage, RoundCount <= 1 ? "第 2 小局开始!" : "决胜局开始!")
                            , Players[Player2Index].SendAsync(ServerOperationType.BigRoundSetMessage, RoundCount <= 1 ? "第 2 小局开始!" : "决胜局开始!"));
            await ClientDelay(1500);
            await Task.WhenAll(Players[Player1Index].SendAsync(ServerOperationType.BigRoundShowClose)
                            , Players[Player2Index].SendAsync(ServerOperationType.BigRoundShowClose));
            await ClientDelay(100);
            //888888888888888888888888888888888888888888888888888888888888888888888888
            await SendEvent(new AfterRoundOver(RoundCount, player1PlacePoint, player2PlacePoint, player1PlacePoint == player2PlacePoint ? (int?)null : (player1PlacePoint > player2PlacePoint ? Player1Index : Player2Index)));
            //888888888888888888888888888888888888888888888888888888888888888888888888
            await SendBigRoundEndToCemetery();//将所有牌移到墓地
            //清空所有场上的牌
        }
        //进行一轮回合
        public async Task<bool> PlayerRound()
        {
            //判断这是谁的回合
            var playerIndex = GameRound == TwoPlayer.Player1 ? Player1Index : Player2Index;
            // await Debug($"玩家{playerIndex}的回合开始!");
            //切换回合
            //----------------------------------------------------
            //这里是回合开始的事件
            //888888888888888888888888888888888888888888888888888888888888888888888888
            await SendEvent(new AfterTurnStart(playerIndex));
            //888888888888888888888888888888888888888888888888888888888888888888888888
            //----------------------------------------------------
            //这是硬币动画
            await Players[playerIndex].SendAsync(ServerOperationType.SetCoinInfo, true);
            await Players[AnotherPlayer(playerIndex)].SendAsync(ServerOperationType.SetCoinInfo, false);
            if (!IsPlayersPass[playerIndex])
                await Players[playerIndex].SendAsync(ServerOperationType.RemindYouRoundStart);
            await ClientDelay(500);

            //判断当前是否已经处于pass状态
            if (IsPlayersPass[playerIndex])
            {
                //如果双方都pass...小局结束
                if (IsPlayersPass[AnotherPlayer(playerIndex)] == true)
                    return false;
                return true;
            }
            else if (PlayersHandCard[playerIndex].Count + (IsPlayersLeader[playerIndex] ? 1 : 0) == 0)
            {//如果没有手牌,强制pass
                IsPlayersPass[playerIndex] = true;
                await SetPassInfo();
                //888888888888888888888888888888888888888888888888888888888888888888888888
                await SendEvent(new AfterPlayerPass(playerIndex));
                //888888888888888888888888888888888888888888888888888888888888888888888888
                if (IsPlayersPass[AnotherPlayer(playerIndex)] == true)
                {
                    //如果对方也pass,结束游戏
                    return false;
                }
                return true;
            }
            //让玩家选择拖拽,或者Pass
            await Players[playerIndex].SendAsync(ServerOperationType.GetDragOrPass);
            //获取信息
            var roundInfo = (await ReceiveAsync(playerIndex)).Arguments.ToArray()[0].ToType<RoundInfo>();//接收玩家的选择,提取结果
            if (roundInfo.IsPass)
            {//Pass时候执行
                IsPlayersPass[playerIndex] = true;
                await SetPassInfo();
                //判断对手是否pass
                //888888888888888888888888888888888888888888888888888888888888888888888888
                await SendEvent(new AfterPlayerPass(playerIndex));
                //888888888888888888888888888888888888888888888888888888888888888888888888
                if (IsPlayersPass[AnotherPlayer(playerIndex)] == true)
                {
                    return false;
                }
            }
            else
            {//放置卡牌(单位和法术都是)时执行
             //以上应该不需要改变,至少不是大改动(动画,pass判断之类的)
                await RoundPlayCard(playerIndex, roundInfo);
                await ClientDelay(400);
            }
            return true;
        }

        //一小局的流程
        public async Task PlayerBigRound(int player1Mulligan = 0, int player2Mulligan = 0)
        {
            //双方调度指定次数
            await Task.WhenAll(MulliganCard(Player1Index, player1Mulligan), MulliganCard(Player2Index, player2Mulligan));
            //双方轮流进行游戏
            //1.根据GamRound进行一次流程
            while (await PlayerRound())
            {
                //2.处理回合结束
                await SendEvent(new AfterTurnOver(TwoPlayerToPlayerIndex(GameRound)));
                //3.切换回合
                GameRound = ((GameRound == TwoPlayer.Player1) ? TwoPlayer.Player2 : TwoPlayer.Player1);
            }

            //处理小局结束
            await BigRoundEnd();
        }

        public async Task RoundPlayCard(int playerIndex, RoundInfo cardInfo)//哪一位玩家,打出第几张手牌,打到了第几排,第几列
        {   //获取卡牌,手牌或者领袖,将那个GameCard存起来
            var card = cardInfo.HandCardIndex == -1 ? PlayersLeader[playerIndex][0] : PlayersHandCard[playerIndex][cardInfo.HandCardIndex];
            //如果打出的是领袖,那么设定领袖已经被打出
            if (cardInfo.HandCardIndex == -1)
                IsPlayersLeader[playerIndex] = false;
            //如果是直接丢墓地,触发丢墓地方法
            if (cardInfo.CardLocation.RowPosition == RowPosition.MyCemetery)
                await card.Effect.ToCemetery();
            else
            {   //如果是法术,使用,如果是单位,打出
                if (cardInfo.CardLocation.RowPosition == RowPosition.SpecialPlace)
                    await card.Effect.CardUse();
                else
                    await card.Effect.Play(cardInfo.CardLocation);
            }
        }

        //玩家抽卡
        public async Task<IList<GameCard>> LogicDrawCard(int playerIndex, int count, Func<GameCard, bool> filter = null)
        {
            filter ??= (x => true);//或许应该播放抽卡动画和更新数值
            if (count > PlayersDeck[playerIndex].Where(filter).Count()) count = PlayersDeck[playerIndex].Where(filter).Count();
            var list = new List<GameCard>();
            for (var i = 0; i < count; i++)
            {
                //将卡组顶端的卡牌抽到手牌
                (await LogicCardMove(PlayersDeck[playerIndex].First(filter), PlayersHandCard[playerIndex], 0, autoUpdateDeck: false)).To(list.Add);
            }
            await SetDeckInfo(playerIndex);
            return list;
        }

        //封装的抽卡
        public async Task<(List<GameCard>, List<GameCard>)> DrawCard(int player1Count, int player2Count, Func<GameCard, bool> filter = null)
        {
            //过滤器
            filter ??= (x => true);
            //抽卡限制,不至于抽空卡组
            if (player1Count > PlayersDeck[Player1Index].Where(filter).Count()) player1Count = PlayersDeck[Player1Index].Where(filter).Count();
            if (player2Count > PlayersDeck[Player2Index].Where(filter).Count()) player2Count = PlayersDeck[Player2Index].Where(filter).Count();
            var player1Task = DrawCardAnimation(Player1Index, player1Count, Player2Index, player2Count, filter);
            var player2Task = DrawCardAnimation(Player2Index, player2Count, Player1Index, player1Count, filter);
            await Task.WhenAll(player1Task, player2Task);
            await SetCountInfo();
            return (player1Task.Result, player2Task.Result);
        }
        //只有一个玩家抽卡
        public async Task<List<GameCard>> PlayerDrawCard(int playerIndex, int count = 1, Func<GameCard, bool> filter = null)
        {
            filter ??= (x => true);
            var result = default(List<GameCard>);
            if (playerIndex == Player1Index)
                (result, _) = await DrawCard(count, 0, filter);
            else
                (_, result) = await DrawCard(0, count, filter);
            return result;
        }
        public async Task<List<GameCard>> DrawCardAnimation(int myPlayerIndex, int myPlayerCount, int enemyPlayerIndex, int enemyPlayerCount, Func<GameCard, bool> filter = null)
        {
            filter ??= (x => true);
            var list = new List<GameCard>();
            for (var i = 0; i < myPlayerCount; i++)
            {
                await SendCardMove(myPlayerIndex, new MoveCardInfo()
                {
                    Source = new CardLocation() { RowPosition = RowPosition.MyDeck },
                    Target = new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 },
                    Card = PlayersDeck[myPlayerIndex].First(filter).Status
                });
                //真实抽的卡只有自己的
                var drawcard = (await LogicDrawCard(myPlayerIndex, 1, filter)).Single();
                list.Add(drawcard);
                await ClientDelay(800, myPlayerIndex);
                await SendCardMove(myPlayerIndex, new MoveCardInfo()
                {
                    Source = new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 },
                    Target = new CardLocation() { RowPosition = RowPosition.MyHand, CardIndex = 0 },
                });
                await ClientDelay(300, myPlayerIndex);
                //88888888888888888888888888888888888888888888888888888
                await SendEvent(new AfterPlayerDraw(myPlayerIndex, drawcard, null));
                //88888888888888888888888888888888888888888888888888888
            }
            for (var i = 0; i < enemyPlayerCount; i++)
            {
                await SendCardMove(myPlayerIndex, new MoveCardInfo()
                {
                    Source = new CardLocation() { RowPosition = RowPosition.EnemyDeck },
                    Target = new CardLocation() { RowPosition = RowPosition.EnemyStay, CardIndex = 0 },
                    Card = new CardStatus(PlayersFaction[enemyPlayerIndex])// { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] }
                });
                await ClientDelay(400, myPlayerIndex);
                await SendCardMove(myPlayerIndex, new MoveCardInfo()
                {
                    Source = new CardLocation() { RowPosition = RowPosition.EnemyStay, CardIndex = 0 },
                    Target = new CardLocation() { RowPosition = RowPosition.EnemyHand, CardIndex = 0 },
                });
                await ClientDelay(300, myPlayerIndex);
            }
            return list;
        }

        //封装的调度
        public async Task MulliganCard(int playerIndex, int count)
        {
            if (PlayersDeck[playerIndex].Count <= 0)
                return;
            await Players[playerIndex].SendAsync(ServerOperationType.MulliganStart, PlayersHandCard[playerIndex].Select(x => x.Status), count);
            IsPlayersMulligan[playerIndex] = true;
            await SetMulliganInfo();
            var backList = new List<string>();
            var blackCardPool = new List<GameCard>();
            for (var i = 0; i < count; i++)
            {
                await Players[playerIndex].SendAsync(ServerOperationType.GetMulliganInfo);
                var result = await ReceiveAsync(playerIndex);
                var mulliganCardIndex = result.Arguments.ToArray()[0].ToType<int>();
                if (mulliganCardIndex == -1)
                    break;
                //逻辑处理
                var mulliganCard = PlayersHandCard[playerIndex][mulliganCardIndex];
                mulliganCard.Status.IsReveal = false;
                //先关掉揭示
                //当然调度走揭示单位,要给对手说一声啦
                await Players[AnotherPlayer(playerIndex)].SendAsync
                (
                    ServerOperationType.SetCard,
                    new CardLocation()
                    {
                        RowPosition = RowPosition.EnemyHand,
                        CardIndex = mulliganCardIndex
                    },
                    new CardStatus(mulliganCard.Status.DeckFaction)// { IsCardBack = true, DeckFaction = PlayersHandCard[playerIndex][mulliganCardIndex].Status.DeckFaction }
                );
                //----------------------------------------------------------------------
                //记录本次黑名单的卡牌Id
                var bId = mulliganCard.CardInfo().CardId;
                backList.Add(bId);
                //将手牌中需要调度的牌,移动到卡组最后(因为下一步就会被抽出,所以暂时加入卡组没问题)
                await LogicCardMove(mulliganCard, PlayersDeck[playerIndex], PlayersDeck[playerIndex].Count,autoUpdateDeck:false);
                await SendEvent(new AfterMulliganDraw(mulliganCard));
                
                //将调度走的卡牌加入卡池,并且从手牌移除这张卡
                foreach (var deckCard in PlayersDeck[playerIndex].ToList())
                {
                    if (deckCard.CardInfo().CardId == bId)
                    {
                        blackCardPool.Add(deckCard);
                        PlayersDeck[playerIndex].Remove(deckCard);
                    }
                }
                //如果卡组为空,随机选择黑名单卡池中一张卡进入卡组
                if (PlayersDeck[playerIndex].Count == 0)
                {
                    var rCard = default(GameCard);
                    lock (RNG)
                    {
                        rCard = blackCardPool[RNG.Next(blackCardPool.Count())];
                    }
                    PlayersDeck[playerIndex].Add(rCard);
                    blackCardPool.Remove(rCard);
                }


                //将卡组中第一张牌抽到手牌调度走的位置
                var card = (await LogicCardMove(PlayersDeck[playerIndex][0], PlayersHandCard[playerIndex], mulliganCardIndex,autoUpdateDeck:false));
                await SendEvent(new AfterMulliganDraw(card));
                //----------------------------------------------------------------------
                await Players[playerIndex].SendAsync(ServerOperationType.MulliganData, mulliganCardIndex, card.Status);
                //每次调度
                //立刻推送消息
                await SendOperactionList();
            }
            //++++++++++++++++++++++++++++++++++++++++
            //将黑名单卡池中所有卡牌随机插入到卡组中
            foreach (var card in blackCardPool)
            {
                lock (RNG)
                {
                    PlayersDeck[playerIndex].Insert(RNG.Next(PlayersDeck[playerIndex].Count() + 1), card);
                }
            }
            //++++++++++++++++++++++++++++++++++++++++
            await ClientDelay(500, playerIndex);
            await Players[playerIndex].SendAsync(ServerOperationType.MulliganEnd);
            IsPlayersMulligan[playerIndex] = false;
            await SetMulliganInfo();
            await SetDeckInfo(playerIndex);
            //调度结束立刻推送消息
            await SendOperactionList();
        }
        //----------------------------------------------------------------------------------------------------------------------
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
        //几个从用户那里获得信息的途径
        public Task<IList<int>> GetSelectMenuCards(int playerIndex, IList<CardStatus> selectList, int selectCount = 1, bool isCanOver = false, string title = "选择一张卡牌")//返回点击列表卡牌的顺序
        {
            return GetSelectMenuCards(playerIndex, new MenuSelectCardInfo() { SelectList = selectList, SelectCount = selectCount, IsCanOver = isCanOver, Title = title });
        }
        public async Task<IList<GameCard>> GetSelectMenuCards(int playerIndex, IList<GameCard> selectList, int selectCount = 1, string title = "选择一张卡牌", bool isEnemyBack = true, bool isCanOver = true)//返回点击列表卡牌的顺序
        {
            if (selectList.Count == 0)
                return new List<GameCard>();
            selectCount = selectCount > selectList.Count() ? selectList.Count() : selectCount;
            return
            (
                await GetSelectMenuCards
                (
                    playerIndex,
                    new MenuSelectCardInfo()
                    {
                        SelectList = selectList.Select(card =>
                        {
                            return (!isEnemyBack || card.PlayerIndex == playerIndex) ? card.Status : new CardStatus(card.Status.DeckFaction);// { DeckFaction = card.Status.DeckFaction, IsCardBack = true };
                        }).ToList(),
                        SelectCount = selectCount,
                        IsCanOver = isCanOver,
                        Title = title
                    }
                )
            ).Select(x => selectList[x]).ToList();
        }
        public async Task<IList<int>> GetSelectMenuCards(int playerIndex, MenuSelectCardInfo info)
        {
            if (info.SelectList.Count == 0)
            {
                return new List<int>();
            }
            await Players[playerIndex].SendAsync(ServerOperationType.SelectMenuCards, info);
            return (await ReceiveAsync(playerIndex)).Arguments.ToArray()[0].ToType<IList<int>>();
        }
        public async Task<IList<CardLocation>> GetSelectPlaceCards(int playerIndex, PlaceSelectCardsInfo info)//指示器向边缘扩展格数
        {
            if (info.CanSelect.CardsPartToLocation().Count() == 0)
            {
                return new List<CardLocation>();
            }
            await Players[playerIndex].SendAsync(ServerOperationType.SelectPlaceCards, info);
            return (await ReceiveAsync(playerIndex)).Arguments.ToArray()[0].ToType<IList<CardLocation>>();
        }
        public async Task<RowPosition> GetSelectRow(int playerIndex, GameCard card, IList<RowPosition> rowPart)//选择排
        {
            if (rowPart.Count == 0) return RowPosition.Banish;
            await Players[playerIndex].SendAsync(ServerOperationType.SelectRow, GetCardLocation(playerIndex, card), rowPart);
            return (await ReceiveAsync(playerIndex)).Arguments.ToArray()[0].ToType<RowPosition>();
        }
        public async Task<CardLocation> GetPlayCard(GameCard card, bool isAnother = false)//选择放置一张牌
        {
            var playerIndex = isAnother ? AnotherPlayer(card.PlayerIndex) : card.PlayerIndex;
            await Players[playerIndex].SendAsync(ServerOperationType.PlayCard, GetCardLocation(playerIndex, card));
            return (await ReceiveAsync(playerIndex)).Arguments.ToArray()[0].ToType<CardLocation>();
        }
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
        //------------------------------------------------------------------------------------------------------------------------
        //下面是发送数据包,或者进行一些初始化信息
        //根据当前信息,处理游戏结果

        public async Task<IList<GameCard>> GetSelectPlaceCards(GameCard card, int count = 1, bool isEnemySwitch = false, Func<GameCard, bool> filter = null, SelectModeType SelectMode = SelectModeType.AllRow, CardType selectType = CardType.Unit, int range = 0,bool isHasConceal = false)
        {
            //自定义规则, 是否过滤特殊卡, 过滤自身
            var canSelect = GetGameCardsPart(isEnemySwitch ? AnotherPlayer(card.PlayerIndex) : card.PlayerIndex,
            (
                x => (filter == null ? (true) : filter(x)) &&
                (!x.IsDead) &&//如果不是僵尸
                (!x.Status.IsImmue) &&//如果没有免疫
                (selectType == CardType.Unit ? (GwentMap.CardMap[x.Status.CardId].CardType == CardType.Unit) :
                (selectType == CardType.Special ? (GwentMap.CardMap[x.Status.CardId].CardType == CardType.Special) : true)) &&
                (x != card) &&
                (isHasConceal ? true : !x.Status.Conceal)
            ), isEnemySwitch ? SelectMode.Mirror() : SelectMode);
            if (GameCardsPartCount(canSelect) < count) count = GameCardsPartCount(canSelect);
            if (count <= 0)
                return new List<GameCard>();
            var target = await GetSelectPlaceCards
            (
                isEnemySwitch ? AnotherPlayer(card.PlayerIndex) : card.PlayerIndex,
                new PlaceSelectCardsInfo()
                {
                    CanSelect = canSelect,
                    SelectCard = isEnemySwitch ? GetCardLocation(card.PlayerIndex, card).Mirror() : GetCardLocation(card.PlayerIndex, card),
                    SelectCount = count,
                    Range = range
                }
            );
            var result = (isEnemySwitch ? target.Select(x => x.Mirror()).ToList() : target);
            return result.Select(x => GetCard(card.PlayerIndex, x)).ToList();
        }
        //将某个列表中的元素,移动到另一个列表的某个位置,然后返回被移动的元素     
        public async Task<GameCard> LogicCardMove(GameCard source, IList<GameCard> target, int targetIndex, bool autoUpdateCemetery = true, bool autoUpdateDeck = true)
        {
            var player1SoureRow = (source.PlayerIndex == Player1Index ? source.Status.CardRow : source.Status.CardRow.Mirror());
            var player1TagetRow = ListToRow(Player1Index, target);
            if (!source.Status.CardRow.IsNone())
            {
                var sourceRow = RowToList(source.PlayerIndex, source.Status.CardRow);
                sourceRow.RemoveAt(sourceRow.IndexOf(source));
            }
            if (targetIndex < 0)
            {
                targetIndex = 0;
            }
            if (targetIndex >= target.Count)
            {
                targetIndex = target.Count;
                // await Debug($"指定目标大于等于最长长度,重指定目标为末尾,重定向到的位置是:{target.Count}");
            }
            target.Insert(targetIndex, source);
            source.Status.CardRow = ListToRow(WhoRow(target), target);
            source.PlayerIndex = WhoRow(target);

            if (autoUpdateCemetery && (player1SoureRow.IsInCemetery() || player1TagetRow.IsInCemetery()))
            {
                await SetCemeteryInfo();
            }
            if (autoUpdateDeck && (player1SoureRow.IsInDeck() || player1TagetRow.IsInDeck()))
            {
                await SetDeckInfo();
            }
            return source;
        }
        public async Task GameOverExecute()
        {
            // if (PlayersRoundResult[0][Player1Index] >= PlayersRoundResult[0][Player2Index])
            //     PlayersWinCount[Player1Index]++;
            // if (PlayersRoundResult[0][Player1Index] <= PlayersRoundResult[0][Player2Index])
            //     PlayersWinCount[Player2Index]++;
            // if (PlayersRoundResult[1][Player1Index] >= PlayersRoundResult[1][Player2Index])
            //     PlayersWinCount[Player1Index]++;
            // if (PlayersRoundResult[1][Player1Index] <= PlayersRoundResult[1][Player2Index])
            //     PlayersWinCount[Player2Index]++;
            // if (PlayersRoundResult[2][Player1Index] >= PlayersRoundResult[2][Player2Index])
            //     PlayersWinCount[Player1Index]++;
            // if (PlayersRoundResult[2][Player1Index] <= PlayersRoundResult[2][Player2Index])
            //     PlayersWinCount[Player2Index]++;

            var redIndex = RedCoin[0];
            var blueIndex = AnotherPlayer(redIndex);
            var result = new GameResult()
            {
                RedPlayerName = Players[redIndex].PlayerName,
                BluePlayerName = Players[blueIndex].PlayerName,
                RedLeaderId = PlayerBaseDeck[redIndex].Leader.CardId,
                BlueLeaderId = PlayerBaseDeck[blueIndex].Leader.CardId,
                RedDeckName = PlayerBaseDeck[redIndex].Name,
                BlueDeckName = PlayerBaseDeck[blueIndex].Name,
                Time = DateTime.UtcNow,
                ValidCount = RoundCount,
                RedWinCount = PlayersWinCount[redIndex],
                BlueWinCount = PlayersWinCount[blueIndex],
                RedPlayerGameResultStatus = PlayersWinCount[redIndex]>PlayersWinCount[blueIndex]?GameStatus.Win:(PlayersWinCount[redIndex]==PlayersWinCount[blueIndex]?GameStatus.Draw:GameStatus.Lose),
                RedScore = new int[]{PlayersRoundResult[0][redIndex],PlayersRoundResult[1][redIndex],PlayersRoundResult[2][redIndex]},
                BlueScore = new int[] { PlayersRoundResult[0][blueIndex], PlayersRoundResult[1][blueIndex], PlayersRoundResult[2][blueIndex] },
            };
            GameResultEvent(result);

            await Task.WhenAll(SendGameResult(Player1Index), SendGameResult(Player2Index));
    }
    public IList<GameCard> RowToList(int myPlayerIndex, RowPosition row,bool isHasDead = false,bool isHasConceal = false)
    {
        var enemyPlayerIndex = (AnotherPlayer(myPlayerIndex));
        switch (row)
        {
            case RowPosition.MyHand:
                return PlayersHandCard[myPlayerIndex];
            case RowPosition.EnemyHand:
                return PlayersHandCard[enemyPlayerIndex];
            case RowPosition.MyDeck:
                return PlayersDeck[myPlayerIndex];
            case RowPosition.EnemyDeck:
                return PlayersDeck[enemyPlayerIndex];
            case RowPosition.MyCemetery:
                return PlayersCemetery[myPlayerIndex];
            case RowPosition.EnemyCemetery:
                return PlayersCemetery[enemyPlayerIndex];
            case RowPosition.MyRow1:
                return PlayersPlace[myPlayerIndex][0];
            case RowPosition.EnemyRow1:
                return PlayersPlace[enemyPlayerIndex][0];
            case RowPosition.MyRow2:
                return PlayersPlace[myPlayerIndex][1];
            case RowPosition.EnemyRow2:
                return PlayersPlace[enemyPlayerIndex][1];
            case RowPosition.MyRow3:
                return PlayersPlace[myPlayerIndex][2];
            case RowPosition.EnemyRow3:
                return PlayersPlace[enemyPlayerIndex][2];
            case RowPosition.MyStay:
                return PlayersStay[myPlayerIndex];
            case RowPosition.EnemyStay:
                return PlayersStay[enemyPlayerIndex];
            case RowPosition.MyLeader:
                return PlayersLeader[myPlayerIndex];
            case RowPosition.EnemyLeader:
                return PlayersLeader[enemyPlayerIndex];
            default:
                return null;
        }
    }
    public RowPosition ListToRow(int myPlayerIndex, IList<GameCard> list)
    {//这一行对于这个玩家是哪一行
        var enemyPlayerIndex = AnotherPlayer(myPlayerIndex);
        if (list == PlayersHandCard[myPlayerIndex])
            return RowPosition.MyHand;
        if (list == PlayersHandCard[enemyPlayerIndex])
            return RowPosition.EnemyHand;
        //
        if (list == PlayersDeck[myPlayerIndex])
            return RowPosition.MyDeck;
        if (list == PlayersDeck[enemyPlayerIndex])
            return RowPosition.EnemyDeck;
        //
        if (list == PlayersCemetery[myPlayerIndex])
            return RowPosition.MyCemetery;
        if (list == PlayersCemetery[enemyPlayerIndex])
            return RowPosition.EnemyCemetery;
        //
        if (list == PlayersPlace[myPlayerIndex][0])//||(!list.Any(x=>x.Status.CardRow!=RowPosition.MyRow1)&&!list.Any(x=>x.PlayerIndex!=myPlayerIndex)))
            return RowPosition.MyRow1;
        if (list == PlayersPlace[enemyPlayerIndex][0])//||(!list.Any(x => x.Status.CardRow != RowPosition.MyRow1)&& !list.Any(x => x.PlayerIndex != enemyPlayerIndex)))
            return RowPosition.EnemyRow1;
        //
        if (list == PlayersPlace[myPlayerIndex][1])//|| (!list.Any(x => x.Status.CardRow != RowPosition.MyRow2)&& !list.Any(x => x.PlayerIndex != myPlayerIndex)))
            return RowPosition.MyRow2;
        if (list == PlayersPlace[enemyPlayerIndex][1])//|| (!list.Any(x => x.Status.CardRow != RowPosition.MyRow2)&& !list.Any(x => x.PlayerIndex != enemyPlayerIndex)))
            return RowPosition.EnemyRow2;
        //
        if (list == PlayersPlace[myPlayerIndex][2])//|| (!list.Any(x => x.Status.CardRow != RowPosition.MyRow3)&& !list.Any(x => x.PlayerIndex != myPlayerIndex)))
            return RowPosition.MyRow3;
        if (list == PlayersPlace[enemyPlayerIndex][2])//|| (!list.Any(x => x.Status.CardRow != RowPosition.MyRow3)&& !list.Any(x => x.PlayerIndex != enemyPlayerIndex)))
            return RowPosition.EnemyRow3;
        //
        if (list == PlayersStay[myPlayerIndex])
            return RowPosition.MyStay;
        if (list == PlayersStay[enemyPlayerIndex])
            return RowPosition.EnemyStay;
        //
        //
        if (list == PlayersLeader[myPlayerIndex])
            return RowPosition.MyLeader;
        if (list == PlayersLeader[enemyPlayerIndex])
            return RowPosition.EnemyLeader;
        //
        return RowPosition.SpecialPlace;
    }
    public int WhoRow(IList<GameCard> list)
    {
        if (ListToRow(Player1Index, list).IsMyRow())
            return Player1Index;
        else
            return Player2Index;
    }
    //另一个玩家
    public CardLocation GetCardLocation(int playerIndex, GameCard card)
    {
        var row = (playerIndex == card.PlayerIndex ? card.Status.CardRow : card.Status.CardRow.Mirror());
        var list = RowToList(playerIndex, row);
        return new CardLocation()
        {
            RowPosition = row,
            CardIndex = list.IndexOf(card)
        };
    }
    public CardLocation GetCardLocation(GameCard card) => GetCardLocation(card.PlayerIndex, card);
    public int AnotherPlayer(int playerIndex) => playerIndex == Player1Index ? Player2Index : Player1Index;
    public int GetPlayersPoint(int playerIndex)
    {
        var allcard = GetAllCard(playerIndex);
        return allcard.Where(x => (x.PlayerIndex == playerIndex && x.Status.CardRow.IsOnPlace())).SelectToHealth().Sum(x => x.health);
    }
    public async Task Debug(string msg)
    {
        await Players[Player1Index].SendAsync(ServerOperationType.Debug, msg);
        await Players[Player2Index].SendAsync(ServerOperationType.Debug, msg);
    }
    public async Task MessageBox(string msg)
    {
        await Players[Player1Index].SendAsync(ServerOperationType.MessageBox, msg);
        await Players[Player2Index].SendAsync(ServerOperationType.MessageBox, msg);
    }
    public async Task ClientDelay(int millisecondsDelay, int? playerIndex = null)
    {
        if (playerIndex == null || playerIndex == Player1Index)
            await Players[Player1Index].SendAsync(ServerOperationType.ClientDelay, millisecondsDelay);
        if (playerIndex == null || playerIndex == Player2Index)
            await Players[Player2Index].SendAsync(ServerOperationType.ClientDelay, millisecondsDelay);
        // await ClientDelay(millisecondsDelay);
    }
    public GameCardsPart GetGameCardsPart(int playerIndex, Func<GameCard, bool> filter, SelectModeType selectMode = SelectModeType.All)
    {   //根据游戏与条件,筛选出符合条件的选择对象
        var cardsPart = new GameCardsPart();
        if (selectMode.IsHaveMy())
        {
            if (selectMode.IsHaveHand())
                PlayersHandCard[playerIndex].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.MyHandCards.Add(item.i));
            if (selectMode.IsHaveRow())
            {
                PlayersPlace[playerIndex][0].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.MyRow1Cards.Add(item.i));
                PlayersPlace[playerIndex][1].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.MyRow2Cards.Add(item.i));
                PlayersPlace[playerIndex][2].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.MyRow3Cards.Add(item.i));
            }
        }
        if (selectMode.IsHaveEnemy())
        {
            if (selectMode.IsHaveHand())
                PlayersHandCard[AnotherPlayer(playerIndex)].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.EnemyHandCards.Add(item.i));
            if (selectMode.IsHaveRow())
            {
                PlayersPlace[AnotherPlayer(playerIndex)][0].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.EnemyRow1Cards.Add(item.i));
                PlayersPlace[AnotherPlayer(playerIndex)][1].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.EnemyRow2Cards.Add(item.i));
                PlayersPlace[AnotherPlayer(playerIndex)][2].Select((x, i) => (x, i)).Where(x => filter(x.x)).ForAll(item => cardsPart.EnemyRow3Cards.Add(item.i));
            }
        }
        return cardsPart;
    }
    public int GameCardsPartCount(GameCardsPart part)
    {
        var count = 0;
        if (part.IsSelectEnemyLeader) count++;
        if (part.IsSelectMyLeader) count++;
        count += part.MyHandCards.Count();
        count += part.MyRow1Cards.Count();
        count += part.MyRow2Cards.Count();
        count += part.MyRow3Cards.Count();
        count += part.MyStayCards.Count();
        count += part.EnemyHandCards.Count();
        count += part.EnemyRow1Cards.Count();
        count += part.EnemyRow2Cards.Count();
        count += part.EnemyRow3Cards.Count();
        count += part.EnemyStayCards.Count();
        return count;
    }
    public GameCardsPart MirrorGameCardsPart(GameCardsPart part)
    {
        var cardsPart = new GameCardsPart
        {
            IsSelectMyLeader = part.IsSelectEnemyLeader,
            IsSelectEnemyLeader = part.IsSelectMyLeader
        };
        part.MyHandCards.ForAll(cardsPart.EnemyHandCards.Add);
        part.MyRow1Cards.ForAll(cardsPart.EnemyRow1Cards.Add);
        part.MyRow2Cards.ForAll(cardsPart.EnemyRow2Cards.Add);
        part.MyRow3Cards.ForAll(cardsPart.EnemyRow3Cards.Add);
        part.MyStayCards.ForAll(cardsPart.EnemyStayCards.Add);
        part.EnemyHandCards.ForAll(cardsPart.MyHandCards.Add);
        part.EnemyRow1Cards.ForAll(cardsPart.MyRow1Cards.Add);
        part.EnemyRow2Cards.ForAll(cardsPart.MyRow2Cards.Add);
        part.EnemyRow3Cards.ForAll(cardsPart.MyRow3Cards.Add);
        part.EnemyStayCards.ForAll(cardsPart.MyStayCards.Add);
        return cardsPart;
    }
    public GameCard GetCard(int playerIndex, CardLocation location)
    {
        return RowToList(playerIndex, location.RowPosition)[location.CardIndex];
    }

    public IList<GameCard> GetAllCard(int playerIndex, bool isContainDead = false,bool isHasConceal = false)
    {
        var anotherPlayer = AnotherPlayer(playerIndex);
        return PlayersHandCard[playerIndex]
        .Concat(PlayersHandCard[anotherPlayer])
        .Concat(PlayersLeader[playerIndex])
        .Concat(PlayersLeader[anotherPlayer])
        .Concat(PlayersStay[playerIndex])
        .Concat(PlayersStay[anotherPlayer])
        .Concat(PlayersCemetery[playerIndex])
        .Concat(PlayersCemetery[anotherPlayer])
        .Concat(PlayersDeck[playerIndex])
        .Concat(PlayersDeck[anotherPlayer])
        .Concat(PlayersPlace[playerIndex][0])
        .Concat(PlayersPlace[playerIndex][1])
        .Concat(PlayersPlace[playerIndex][2])
        .Concat(PlayersPlace[anotherPlayer][0])
        .Concat(PlayersPlace[anotherPlayer][1])
        .Concat(PlayersPlace[anotherPlayer][2])
        .Where(x => isContainDead ? true : !x.IsDead)
        .Where(x => isHasConceal ? true : !x.Status.Conceal)
        .ToList();
    }
    //----------------------------------------------------------------------------------------------
    public async Task SetAllInfo()
    {
        var task1 = Players[Player1Index].SendAsync(ServerOperationType.SetAllInfo, GetAllInfo(TwoPlayer.Player1));
        var task2 = Players[Player2Index].SendAsync(ServerOperationType.SetAllInfo, GetAllInfo(TwoPlayer.Player2));
        await Task.WhenAll(task1, task2);
        await SetDeckInfo();
        // await SendOperactionList();
    }
    public async Task SetDeckInfo(int playerIndex)
    {
        await Players[playerIndex].SendAsync(ServerOperationType.SetMyDeck, PlayersDeck[playerIndex].Select(x => new CardStatus(x.Status.CardId)).OrderBy(x => x.CardId).OrderByDescending(x => x.Group).ThenByDescending(x => x.Strength).ToList());
    }
    public async Task SetDeckInfo()
    {
        var player1Task = SetDeckInfo(Player1Index);
        var player2Task = SetDeckInfo(Player2Index);
        await Task.WhenAll(player1Task, player2Task);
    }
    public Task SetCemeteryInfo(int playerIndex)
    {
        var player1Task = Players[playerIndex].SendAsync(ServerOperationType.SetMyCemetery, PlayersCemetery[playerIndex].Select(x => x.Status));
        var player2Task = Players[AnotherPlayer(playerIndex)].SendAsync(ServerOperationType.SetEnemyCemetery, PlayersCemetery[playerIndex].Select(x => x.Status));
        return Task.WhenAll(player1Task, player2Task);
    }
    public async Task SetCemeteryInfo()
    {
        var player1Task = SetCemeteryInfo(Player1Index);
        var player2Task = SetCemeteryInfo(Player2Index);
        await Task.WhenAll(player1Task, player2Task);
    }
    public Task SetGameInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetGameInfo, GetGameInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetGameInfo, GetGameInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetCardsInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetCardsInfo, GetCardsInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetCardsInfo, GetCardsInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetPointInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetPointInfo, GetPointInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetPointInfo, GetPointInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetCountInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetCountInfo, GetCountInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetCountInfo, GetCountInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetPassInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetPassInfo, GetPassInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetPassInfo, GetPassInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetMulliganInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetMulliganInfo, GetMulliganInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetMulliganInfo, GetMulliganInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetWinCountInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetWinCountInfo, GetWinCountInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetWinCountInfo, GetWinCountInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    public Task SetNameInfo()
    {
        var player1Task = Players[Player1Index].SendAsync(ServerOperationType.SetNameInfo, GetNameInfo(TwoPlayer.Player1));
        var player2Task = Players[Player2Index].SendAsync(ServerOperationType.SetNameInfo, GetNameInfo(TwoPlayer.Player2));
        return Task.WhenAll(player1Task, player2Task);
    }
    //---------------------------------------------------------
    public GameInfomation GetGameInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            MyRow1Point = PlayersPlace[myPlayerIndex][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow2Point = PlayersPlace[myPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow3Point = PlayersPlace[myPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            IsMyPlayerPass = IsPlayersPass[myPlayerIndex],
            IsEnemyPlayerPass = IsPlayersPass[enemyPlayerIndex],
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
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            IsMyLeader = IsPlayersLeader[myPlayerIndex],
            IsEnemyLeader = IsPlayersLeader[enemyPlayerIndex],
            MyLeader = PlayersLeader[myPlayerIndex][0].Status,
            EnemyLeader = PlayersLeader[enemyPlayerIndex][0].Status,
            MyHandCard = PlayersHandCard[myPlayerIndex].Select(x => x.Status),
            MyStay = PlayersStay[myPlayerIndex].Select(x => x.Status),
            EnemyStay = PlayersStay[enemyPlayerIndex].Select(x => x.Status),
            EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.Status).Select(x => x.IsReveal ? x : new CardStatus(PlayersFaction[enemyPlayerIndex])),// { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] }),
            MyPlace = PlayersPlace[myPlayerIndex].Select(x => x.Select(c => c.Status)).ToArray(),
            EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
            (
                x => x.Select(c => c.Status).Select(item => item.Conceal ? new CardStatus(PlayersFaction[enemyPlayerIndex]) /* { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] } */: item)
            ).ToArray(),
            MyCemetery = PlayersCemetery[myPlayerIndex].Select(x => x.Status),
            EnemyCemetery = PlayersCemetery[enemyPlayerIndex].Select(x => x.Status),
        };
    }
    public GameInfomation GetPointInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            MyRow1Point = PlayersPlace[myPlayerIndex][0].Where(x=>!x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow2Point = PlayersPlace[myPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow3Point = PlayersPlace[myPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus)
        };
    }
    public GameInfomation GetCountInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
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
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            IsMyPlayerPass = IsPlayersPass[myPlayerIndex],
            IsEnemyPlayerPass = IsPlayersPass[enemyPlayerIndex]
        };
    }
    public GameInfomation GetMulliganInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            IsMyPlayerMulligan = IsPlayersMulligan[myPlayerIndex],
            IsEnemyPlayerMulligan = IsPlayersMulligan[enemyPlayerIndex]
        };
    }
    public GameInfomation GetWinCountInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            MyWinCount = PlayersWinCount[myPlayerIndex],
            EnemyWinCount = PlayersWinCount[enemyPlayerIndex]
        };
    }
    public GameInfomation GetNameInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        return new GameInfomation()
        {
            EnemyName = Players[enemyPlayerIndex].PlayerName,
            MyName = Players[myPlayerIndex].PlayerName
        };
    }

    //更新所有信息
    public GameInfomation GetAllInfo(TwoPlayer player)
    {
        var myPlayerIndex = (player == TwoPlayer.Player1 ? Player1Index : Player2Index);
        var enemyPlayerIndex = (player == TwoPlayer.Player1 ? Player2Index : Player1Index);
        var result = new GameInfomation()
        {
            MyRow1Point = PlayersPlace[myPlayerIndex][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow2Point = PlayersPlace[myPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            MyRow3Point = PlayersPlace[myPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow1Point = PlayersPlace[enemyPlayerIndex][0].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow2Point = PlayersPlace[enemyPlayerIndex][1].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            EnemyRow3Point = PlayersPlace[enemyPlayerIndex][2].Where(x => !x.Status.Conceal).Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus),
            IsMyPlayerPass = IsPlayersPass[myPlayerIndex],
            IsEnemyPlayerPass = IsPlayersPass[enemyPlayerIndex],
            MyWinCount = PlayersWinCount[myPlayerIndex],
            EnemyWinCount = PlayersWinCount[enemyPlayerIndex],
            IsMyLeader = IsPlayersLeader[myPlayerIndex],
            IsEnemyLeader = IsPlayersLeader[enemyPlayerIndex],
            MyLeader = PlayersLeader[myPlayerIndex][0].Status,
            EnemyLeader = PlayersLeader[enemyPlayerIndex][0].Status,
            EnemyName = Players[enemyPlayerIndex].PlayerName,
            MyName = Players[myPlayerIndex].PlayerName,
            MyDeckCount = PlayersDeck[myPlayerIndex].Count(),
            EnemyDeckCount = PlayersDeck[enemyPlayerIndex].Count(),
            MyHandCount = PlayersHandCard[myPlayerIndex].Count() + (IsPlayersLeader[myPlayerIndex] ? 1 : 0),
            EnemyHandCount = PlayersHandCard[enemyPlayerIndex].Count() + (IsPlayersLeader[enemyPlayerIndex] ? 1 : 0),
            MyCemeteryCount = PlayersCemetery[myPlayerIndex].Count(),
            EnemyCemeteryCount = PlayersCemetery[enemyPlayerIndex].Count(),
            MyHandCard = PlayersHandCard[myPlayerIndex].Select(x => x.Status).ToList(),
            EnemyHandCard = PlayersHandCard[enemyPlayerIndex].Select(x => x.Status).Select(x => x.IsReveal ? x : new CardStatus(PlayersFaction[enemyPlayerIndex])/* { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] }*/).ToList(),
            MyPlace = PlayersPlace[myPlayerIndex].Select(x => x.Select(c => c.Status).ToList()).ToArray(),
            EnemyPlace = PlayersPlace[enemyPlayerIndex].Select
            (
                x => x.Select(c => c.Status).Select(item => item.Conceal ? new CardStatus(PlayersFaction[enemyPlayerIndex])/*  { IsCardBack = true, DeckFaction = PlayersFaction[enemyPlayerIndex] } */: item).ToList()
            ).ToArray(),
            MyCemetery = PlayersCemetery[myPlayerIndex].Select(x => x.Status).ToList(),
            EnemyCemetery = PlayersCemetery[enemyPlayerIndex].Select(x => x.Status).ToList(),
            MyStay = PlayersStay[myPlayerIndex].Select(x => x.Status).ToList(),
            EnemyStay = PlayersStay[enemyPlayerIndex].Select(x => x.Status).ToList()
        };
        return result;
    }
    //--------------------------------------
    public Task ShowWeatherApply(int playerIndex, RowPosition row, RowStatus type)
    {
        return Task.WhenAll(Players[playerIndex].SendAsync(ServerOperationType.ShowWeatherApply, row, type),
                            Players[AnotherPlayer(playerIndex)].SendAsync(ServerOperationType.ShowWeatherApply, row.Mirror(), type));
    }
    public Task SendCardOn(int playerIndex, CardLocation location)
    {
        if (!location.RowPosition.IsOnRow()) return Task.CompletedTask;
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.CardOn,
            location
        );
    }
    public Task SendCardDown(int playerIndex, CardLocation location)
    {
        if (!location.RowPosition.IsOnRow()) return Task.CompletedTask;
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.CardDown,
            location
        );
    }
    public Task SendCardMove(int playerIndex, MoveCardInfo info)
    {
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.CardMove,
            info
        );
    }
    public async Task SendSetCard(int playerIndex, GameCard card)//更新某个玩家的一个卡牌
    {
        // await Debug("刷新了卡牌设置");
        // await Debug($"卡牌名称是:{card.Status.Name},生命状态是:{card.Status.HealthStatus}");
        //如果处于敌方场地
        var isBack = (card.Status.CardRow.IsOnPlace() && card.Status.Conceal)||(card.PlayerIndex!=playerIndex&&card.Status.CardRow.IsOnStay()&&card.Status.IsConcealCard);
        if (card.PlayerIndex != playerIndex) isBack |= (card.Status.CardRow.IsInHand() && (!card.Status.IsReveal));
        await Players[playerIndex].SendAsync
        (
            ServerOperationType.SetCard,
            GetCardLocation(playerIndex, card),
            isBack ?
            new CardStatus(card.Status.DeckFaction)
            : card.Status
        );
    }

    public async Task ShowCardMove(CardLocation location, GameCard card, bool refresh = true, bool refreshPoint = false, bool isShowEnemyBack = false, bool autoUpdateCemetery = true, bool autoUpdateDeck = true)
    {
        var isFromHide = card.Status.CardRow.IsInBack();
        var isShowPlayerIndexBack = (!location.RowPosition.IsMyRow() && isShowEnemyBack)||card.Status.Conceal;
        var isShowAnotherPlayerBack = (location.RowPosition.IsMyRow() && isShowEnemyBack)||card.Status.Conceal||card.Status.IsConcealCard;
        await SendCardMove(card.PlayerIndex, new MoveCardInfo()
        {
            Source = GetCardLocation(card.PlayerIndex, card),
            Target = location,
            Card = isShowPlayerIndexBack ? card.Status.CreateBackCard() : (refresh ? ((card.TagetIsShowBack(location, card.PlayerIndex, card.PlayerIndex)) ? card.Status.CreateBackCard() : card.Status) : (isFromHide ? card.Status.CreateBackCard() : null))
        });
        await SendCardMove(AnotherPlayer(card.PlayerIndex), new MoveCardInfo()
        {
            Source = GetCardLocation(AnotherPlayer(card.PlayerIndex), card),
            Target = new CardLocation() { RowPosition = location.RowPosition.Mirror(), CardIndex = location.CardIndex },
            Card = isShowAnotherPlayerBack ? card.Status.CreateBackCard() : (refresh ? (card.TagetIsShowBack(location, card.PlayerIndex, AnotherPlayer(card.PlayerIndex)) ? card.Status.CreateBackCard() : card.Status) : (isFromHide ? card.Status.CreateBackCard() : null))
        });
        //var row = RowToList(card.PlayerIndex, card.Status.CardRow);
        var target = RowToList(card.PlayerIndex, location.RowPosition);
        await LogicCardMove(card, target, location.CardIndex, autoUpdateCemetery, autoUpdateDeck);
        await SetCountInfo();
        if (refreshPoint)
            await SetPointInfo();
    }

    public async Task ShowSetCard(GameCard card)//更新敌我的一个卡牌
    {
        if (!card.Status.CardRow.IsOnRow()) return;
        await Task.WhenAll(SendSetCard(Player1Index, card), SendSetCard(Player2Index, card));
    }
    public async Task ShowCardDown(GameCard card)//落下
    {
        if (!card.Status.CardRow.IsOnRow()) return;
        var task1 = Players[card.PlayerIndex].SendAsync(ServerOperationType.CardDown, GetCardLocation(card.PlayerIndex, card));
        var task2 = Players[AnotherPlayer(card.PlayerIndex)].SendAsync(ServerOperationType.CardDown,
            GetCardLocation(AnotherPlayer(card.PlayerIndex), card));
        await Task.WhenAll(task1, task2);
    }
    public async Task ShowCardOn(GameCard card)//抬起
    {
        if (!card.Status.CardRow.IsOnRow()) return;
        var task1 = Players[card.PlayerIndex].SendAsync(ServerOperationType.CardOn, GetCardLocation(card.PlayerIndex, card));
        var task2 = Players[AnotherPlayer(card.PlayerIndex)].SendAsync(ServerOperationType.CardOn,
            GetCardLocation(AnotherPlayer(card.PlayerIndex), card));
        await Task.WhenAll(task1, task2);
    }
    //
    public Task ShowCardNumberChange(GameCard card, int num, NumberType type = NumberType.Normal)
    {
        return Task.WhenAll
        (
            SendCardNumberChange(Player1Index, card, num, type),
            SendCardNumberChange(Player2Index, card, num, type)
        );
    }
    public Task SendCardNumberChange(int playerIndex, GameCard card, int num, NumberType type = NumberType.Normal)
    {
    
        if (card.IsShowBack(playerIndex)&&!(type==NumberType.Countdown&&playerIndex==card.PlayerIndex)&&!card.Status.CardRow.IsInCemetery()&&!(card.Status.CardRow.IsInDeck()&&card.PlayerIndex==playerIndex))
            return Task.CompletedTask;
        return Players[playerIndex].SendAsync
            (
                ServerOperationType.ShowCardNumberChange,
                GetCardLocation(playerIndex, card),
                num,
                type
            );
    }
    //--
    public Task SendBullet(int playerIndex, GameCard source, GameCard target, BulletType type)
    {
        if (source.IsShowBack(playerIndex) || target.IsShowBack(playerIndex))
            return Task.CompletedTask;
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.ShowBullet,
            GetCardLocation(playerIndex, source),
            GetCardLocation(playerIndex, target),
            type
        );
    }
    public Task ShowBullet(GameCard source, GameCard target, BulletType type)
    {
        return Task.WhenAll
        (
            SendBullet(Player1Index, source, target, type),
            SendBullet(Player2Index, source, target, type)
        );
    }
    //
    public Task SendCardIconEffect(int playerIndex, GameCard card, CardIconEffectType type)
    {
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.ShowCardIconEffect,
            GetCardLocation(playerIndex, card),
            type
        );
    }
    public Task ShowCardIconEffect(GameCard card, CardIconEffectType type)
    {
        return Task.WhenAll
        (
            SendCardIconEffect(Player1Index, card, type),
            SendCardIconEffect(Player2Index, card, type)
        );
    }
    //
    public Task SendCardBreakEffect(int playerIndex, GameCard card, CardBreakEffectType type)
    {
        return Players[playerIndex].SendAsync
        (
            ServerOperationType.ShowCardBreakEffect,
            GetCardLocation(playerIndex, card),
            type
        );
    }
    public Task ShowCardBreakEffect(GameCard card, CardBreakEffectType type)
    {
        return Task.WhenAll
        (
            SendCardBreakEffect(Player1Index, card, type),
            SendCardBreakEffect(Player2Index, card, type)
        );
    }
    //----------------------------------------------------------------------------------------------
    public Task SendGameResult(int playerIndex, GameStatus coerceResult = GameStatus.None)//是否强制指定比赛结果
    {
        var myPlayerIndex = playerIndex;
        var enemyPlayerIndex = AnotherPlayer(playerIndex);
        //---
        int result = 0;//0为平, 1为玩家1胜利, 2为玩家2胜利
        if (PlayersWinCount[myPlayerIndex] == PlayersWinCount[enemyPlayerIndex])
            result = 0;
        if (PlayersWinCount[myPlayerIndex] > PlayersWinCount[enemyPlayerIndex])
            result = 1;
        if (PlayersWinCount[myPlayerIndex] < PlayersWinCount[enemyPlayerIndex])
            result = 2;
        //---
        return Players[myPlayerIndex].SendAsync(ServerOperationType.GameEnd, new GameResultInfomation
        (
            Players[myPlayerIndex].PlayerName,
            Players[enemyPlayerIndex].PlayerName,
            gameStatu:
            (
                coerceResult == GameStatus.None ?
                (result == 0 ? GameStatus.Draw :
                (result == 1 ? GameStatus.Win : GameStatus.Lose)) :
                coerceResult
            ),
            RoundCount,
            PlayersRoundResult[0][myPlayerIndex],
            PlayersRoundResult[0][enemyPlayerIndex],
            PlayersRoundResult[1][myPlayerIndex],
            PlayersRoundResult[1][enemyPlayerIndex],
            PlayersRoundResult[2][myPlayerIndex],
            PlayersRoundResult[2][enemyPlayerIndex]
        ));
    }
    public GwentServerGame(Player player1, Player player2, GwentCardTypeService gwentCardTypeService,Action<GameResult> gameResultEvent)
    {
        GameResultEvent = gameResultEvent;
        _gwentCardTypeService = gwentCardTypeService;
        _randomSeed = (int)DateTime.UtcNow.Ticks;
        RNG = new Random(_randomSeed);
        PlayerBaseDeck[Player1Index] = player1.Deck.ToGameDeck();
        PlayerBaseDeck[Player2Index] = player2.Deck.ToGameDeck();
        //初始化游戏信息
        GameRound = RNG.Next(2) == 1 ? TwoPlayer.Player1 : TwoPlayer.Player2;
        RedCoin[0] = GameRound.ToPlayerIndex(this);
        //随机个先后手
        PlayersRoundResult[0] = new int[2];
        PlayersRoundResult[1] = new int[2];
        PlayersRoundResult[2] = new int[2];
        Players[Player1Index] = player1;
        Players[Player2Index] = player2;
        PlayersPlace[Player1Index] = new List<GameCard>[3];
        PlayersPlace[Player2Index] = new List<GameCard>[3];
        PlayersFaction[Player1Index] = GwentMap.CardMap[player1.Deck.Leader].Faction;
        PlayersFaction[Player2Index] = GwentMap.CardMap[player2.Deck.Leader].Faction;
        //----------------------------------------------------
        PlayersPlace[Player1Index][0] = new List<GameCard>();
        PlayersPlace[Player2Index][0] = new List<GameCard>();
        PlayersPlace[Player1Index][1] = new List<GameCard>();
        PlayersPlace[Player2Index][1] = new List<GameCard>();
        PlayersPlace[Player1Index][2] = new List<GameCard>();
        PlayersPlace[Player2Index][2] = new List<GameCard>();
        //---------------------------------------------------
        GameRowEffect[Player1Index] = new GameRow[3]
        {
            new GameRow(this, PlayersPlace[Player1Index][0], Player1Index, 0.IndexToMyRow()),
            new GameRow(this, PlayersPlace[Player1Index][1], Player1Index, 1.IndexToMyRow()),
            new GameRow(this, PlayersPlace[Player1Index][2], Player1Index, 2.IndexToMyRow())
        };//玩家天气
        GameRowEffect[Player2Index] = new GameRow[3]
        {
            new GameRow(this, PlayersPlace[Player2Index][0], Player2Index, 0.IndexToMyRow()),
            new GameRow(this, PlayersPlace[Player2Index][1], Player2Index, 1.IndexToMyRow()),
            new GameRow(this, PlayersPlace[Player2Index][2], Player2Index, 2.IndexToMyRow())
        };//玩家天气
          //---------------------------------------------------
        PlayersCemetery[Player1Index] = new List<GameCard>();
        PlayersCemetery[Player2Index] = new List<GameCard>();
        PlayersHandCard[Player1Index] = new List<GameCard>();
        PlayersHandCard[Player2Index] = new List<GameCard>();
        PlayersStay[Player1Index] = new List<GameCard>();
        PlayersStay[Player2Index] = new List<GameCard>();
        IsPlayersLeader[Player1Index] = true;
        IsPlayersLeader[Player2Index] = true;
        PlayersLeader[Player1Index] = new List<GameCard>()
        {
            new GameCard(this,Player1Index,
                new CardStatus(
                    player1.Deck.Leader,
                    PlayersFaction[Player1Index],
                    RowPosition.MyLeader
                ),player1.Deck.Leader)
        }.ToList();
        PlayersLeader[Player2Index] = new List<GameCard>
        {
            new GameCard(this,Player2Index,
                new CardStatus(
                    player2.Deck.Leader,
                    PlayersFaction[Player2Index],
                    RowPosition.MyLeader
                ),player2.Deck.Leader)
        }.ToList();
        //将卡组转化成实体,并且打乱牌组
        PlayersDeck[Player1Index] = player1.Deck.Deck.Select(cardId =>
            new GameCard(this, Player1Index,
                new CardStatus(
                    cardId,
                    PlayersFaction[Player1Index],
                    RowPosition.MyDeck
                ), cardId))
        .Mess(RNG).ToList();
        //需要更改,将卡牌效果变成对应Id的卡牌效果
        PlayersDeck[Player2Index] = player2.Deck.Deck.Select(cardId =>
            new GameCard(this, Player2Index,
                new CardStatus(
                    cardId,
                    PlayersFaction[Player2Index],
                    RowPosition.MyDeck
                ), cardId)
        )
        .Mess(RNG).ToList();
    }
    public async Task SendBigRoundEndToCemetery()
    {
        //#############################################
        //#                 需要优化                  
        //#############################################
        foreach (var place in PlayersPlace)
            foreach (var row in place)
                foreach (var card in row.ToList())
                {
                    await card.Effect.RoundEnd();
                    await ClientDelay(10);
                }
        await SetCountInfo();
        await SetPointInfo();
        await SetCemeteryInfo();
    }
    public int TwoPlayerToPlayerIndex(TwoPlayer player)
    {
        return ((player == TwoPlayer.Player1) ? Player1Index : Player2Index);
    }
    public CardLocation GetRandomCanPlayLocation(int playerIndex, bool isAtEnd = false)
    {
        var a = new List<int>();
        if (PlayersPlace[playerIndex][0].Count < RowMaxCount) a.Add(0);
        if (PlayersPlace[playerIndex][1].Count < RowMaxCount) a.Add(1);
        if (PlayersPlace[playerIndex][2].Count < RowMaxCount) a.Add(2);
        if (a.Count == 0) return null;
        var rowIndex = a[RNG.Next(0, a.Count)];
        var count = PlayersPlace[playerIndex][rowIndex].Count;
        if (isAtEnd)
        {
            return new CardLocation(rowIndex.IndexToMyRow(), count);
        }
        return new CardLocation(rowIndex.IndexToMyRow(), RNG.Next(0, count + 1));

    }
    //====================================================================================
    //====================================================================================
    //卡牌事件处理与转发
    public async Task<GameCard> CreateCard(string cardId, int playerIndex, CardLocation position, Action<CardStatus> setting = null)
    {
        //定位到这一排
        var row = RowToList(playerIndex, position.RowPosition);
        if (position.RowPosition.IsOnPlace() && row.Count >= RowMaxCount)
            return null;
        //创造对应的卡
        var creatCard = new GameCard(this, playerIndex, new CardStatus(cardId, PlayersFaction[playerIndex], RowPosition.None), cardId);
        setting?.Invoke(creatCard.Status);
        //将创造的卡以不显示的方式移动到目标位置!
        await LogicCardMove(creatCard, row, position.CardIndex);
        //发送信息,显示创造的卡
        if (position.RowPosition.IsMyRow())
        {
            await Players[playerIndex].SendAsync(ServerOperationType.CreateCard, creatCard.Status, creatCard.GetLocation());
            await Players[AnotherPlayer(playerIndex)].SendAsync(ServerOperationType.CreateCard,
            ((creatCard.IsShowBack(AnotherPlayer(playerIndex))) ? creatCard.Status.CreateBackCard() : creatCard.Status), creatCard.GetLocation().Mirror());
        }
        else
        {
            await Players[playerIndex].SendAsync(ServerOperationType.CreateCard, creatCard.Status, creatCard.GetLocation().Mirror());
            await Players[AnotherPlayer(playerIndex)].SendAsync(ServerOperationType.CreateCard,
            ((creatCard.IsShowBack(AnotherPlayer(playerIndex))) ? creatCard.Status.CreateBackCard() : creatCard.Status), creatCard.GetLocation());
        }
        await AddTask(async () =>
        {
            if (creatCard.Status.CardRow.IsOnPlace())
            {
                await AddTask(async () =>
                {
                    if (creatCard.Status.CardRow.IsOnPlace())
                    {
                        // await ShowCardOn(creatCard);
                        if (position.RowPosition.IsMyRow())
                        {
                            // await AddTask(async () =>
                            // {
                            await creatCard.Effect.CardDown(false,false,true,(false,false));
                            // await AddTask(async () =>
                            // {
                            await creatCard.Effects.RaiseEvent(new CardDownEffect(false, false));
                            // });
                            // });
                        }
                        else
                        {
                            // await AddTask(async () =>
                            // {
                            await creatCard.Effect.CardDown(true,false,true,(false,false));
                            // await AddTask(async () =>
                            // {
                            await creatCard.Effects.RaiseEvent(new CardDownEffect(true, false));
                            // });
                            // });
                        }
                    }
                    //     if (position.RowPosition.IsMyRow())
                    //     {
                    //         await creatCard.Effect.Play(position);
                    //     }
                    //     else
                    //     {
                    //         await creatCard.Effect.Play(position.Mirror(), true);
                    //     }
                });
            }
        });
        return creatCard;
    }
    public async Task<int> CreateAndMoveStay(int playerIndex, string[] cards, int createCount = 1, bool isCanOver = false, string title = "选择生成一张卡")
    {
        var selectList = cards.Select(x => new CardStatus(x)).ToList();
        var result = (await GetSelectMenuCards(playerIndex, selectList, isCanOver: isCanOver, title: title)).Reverse().ToList();
        //先选的先打出
        if (result.Count() <= 0) return 0;
        foreach (var CardIndex in result)
        {
            await CreateCard(selectList[CardIndex].CardId, playerIndex, new CardLocation(RowPosition.MyStay, 0));
        }
        return result.Count();
    }
    public async Task<TEvent> SendEvent<TEvent>(TEvent @event) where TEvent : Event
    {
        //卡牌
        async Task task()
        {
            var list = new List<GameCard>();
            foreach (var card in GetAllCard(Player1Index, true, true).ToList())
            {
                if (card.Status.IsLock) continue;
                await card.Effects.RaiseEvent(@event);
            }
        }
        //天气
        async Task task2()
        {
            foreach (var row in GameRowEffect.SelectMany(x => x))
            {
                await row.Effects.RaiseEvent(@event);
            }
        }
        if (OperactionList.IsRunning)
        {
            await task();
        }
        else
        {
            await AddTask((Func<Task>)task);
        }
        if (OperactionList.IsRunning)
        {
            await task2();
        }
        else
        {
            await AddTask((Func<Task>)task2);
        }
        return @event;
    }
    public async Task<Operation<UserOperationType>> ReceiveAsync(int playerIndex)
    {
        await ((ClientPlayer)Players[Player1Index]).SendOperactionList();
        await ((ClientPlayer)Players[Player2Index]).SendOperactionList();
        return await Players[playerIndex].ReceiveAsync();
    }
    public async Task SendOperactionList()
    {
        await ((ClientPlayer)Players[Player1Index]).SendOperactionList();
        await ((ClientPlayer)Players[Player2Index]).SendOperactionList();
    }

    public CardEffect CreateEffectInstance(string effectId, GameCard targetCard)
    {
        return _gwentCardTypeService.CreateInstance(effectId, targetCard);
    }

    public Task AddTask(params Func<Task>[] task)
    {
        return OperactionList.AddLast(task);
    }
}
}