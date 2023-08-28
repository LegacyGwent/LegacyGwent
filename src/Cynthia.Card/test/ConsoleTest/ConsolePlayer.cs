﻿using Alsein.Extensions.IO;
using Alsein.Extensions;
using Cynthia.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;

namespace ConsoleTest
{
    public class ConsolePlayer : Player
    {
        #region 底层指令维护, 不用动

        public ClientGameData Data { get; set; } = new ClientGameData();

        public ConsolePlayer(DeckModel deck, string playerName = "玩家A") : base()
        {
            Receive += async x =>
            {
                await HandleServerOperation((Operation<ServerOperationType>)x.Result);
            };
            Deck = deck;
            PlayerName = playerName;
        }

        public ConsolePlayer(string deckCode, string playerName = "玩家A") : this(deckCode.DeCompressToDeck(), playerName)
        {
        }

        public virtual async Task HandleServerOperation(Operation<ServerOperationType> operation)
        {
            await ResponseOperation(operation);
        }

        private async Task<bool> ResponseOperation(Operation<ServerOperationType> operation)
        {
            await Task.CompletedTask;
            var arguments = operation.Arguments.ToArray();
            switch (operation.OperationType)
            {
                case ServerOperationType.ClientDelay:
                    var dTime = arguments[0].ToType<int>();
                    Delay(dTime);
                    break;
                case ServerOperationType.SelectMenuCards:
                    SelectMenuCards(arguments[0].ToType<MenuSelectCardInfo>(), (o) => SendAsync(o));
                    break;
                case ServerOperationType.SelectPlaceCards:
                    SelectPlaceCards(arguments[0].ToType<PlaceSelectCardsInfo>(), (o) => SendAsync(o));
                    break;
                case ServerOperationType.SelectRow:
                    SelectRow(arguments[0].ToType<CardLocation>(), arguments[1].ToType<IList<RowPosition>>(), (o) => SendAsync(o));
                    break;
                case ServerOperationType.PlayCard:
                    PlayCard(arguments[0].ToType<CardLocation>(), (o) => SendAsync(o));
                    break;
                //-------------------------
                //小动画
                case ServerOperationType.ShowWeatherApply:
                    ShowWeatherApply(arguments[0].ToType<RowPosition>(), arguments[1].ToType<RowStatus>());
                    break;
                case ServerOperationType.ShowCardNumberChange:
                    ShowCardNumberChange(arguments[0].ToType<CardLocation>(), arguments[1].ToType<int>(), arguments[2].ToType<NumberType>());
                    break;
                case ServerOperationType.ShowCardIconEffect:
                    ShowCardIconEffect(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardIconEffectType>());
                    break;
                case ServerOperationType.ShowCardBreakEffect:
                    ShowCardBreakEffect(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardBreakEffectType>());
                    break;
                case ServerOperationType.ShowBullet:
                    ShowBullet(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardLocation>(), arguments[2].ToType<BulletType>());
                    break;
                //-------------------------
                case ServerOperationType.SetCard:
                    SetCard(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardStatus>());
                    break;
                case ServerOperationType.CreateCard:
                    CreateCard(arguments[0].ToType<CardStatus>(), arguments[1].ToType<CardLocation>());
                    break;
                //----------------------------------------------------------------------------------
                case ServerOperationType.Debug:
                    Log(arguments[0].ToType<string>());
                    break;
                case ServerOperationType.MessageBox:
                    YNMessageBox("收到了一个来自服务器的消息", arguments[0].ToType<string>());
                    break;
                case ServerOperationType.GetDragOrPass:
                    GetPlayerDrag((o) => SendAsync(o));
                    break;
                case ServerOperationType.RoundEnd://回合结束
                    RoundEnd();
                    break;
                case ServerOperationType.CardsToCemetery:
                    ShowCardsToCemetery(arguments[0].ToType<GameCardsPart>());
                    break;
                case ServerOperationType.GameEnd://游戏结束,以及游戏结束信息
                    ShowGameResult(arguments[0].ToType<GameResultInfomation>());
                    return false;
                case ServerOperationType.CardMove:
                    CardMove(arguments[0].ToType<MoveCardInfo>());
                    break;
                case ServerOperationType.CardOn:
                    CardOn(arguments[0].ToType<CardLocation>());
                    break;
                case ServerOperationType.CardDown:
                    CardDown(arguments[0].ToType<CardLocation>());
                    break;
                //------------------------------------------------------------------------
                //和调度有关的一切
                case ServerOperationType.MulliganStart:
                    MulliganStart(arguments[0].ToType<IList<CardStatus>>(), arguments[1].ToType<int>());
                    break;
                case ServerOperationType.MulliganData:
                    MulliganData(arguments[0].ToType<int>(), arguments[1].ToType<CardStatus>());
                    break;
                case ServerOperationType.GetMulliganInfo:
                    GetMulliganInfo((o) => SendAsync(o));
                    break;
                case ServerOperationType.MulliganEnd:
                    MulliganEnd();
                    break;
                case ServerOperationType.SetMulliganInfo:
                    //
                    SetMulliganInfo(arguments[0].ToType<GameInfomation>());
                    break;
                //----------------
                //显示你的回合到了
                case ServerOperationType.RemindYouRoundStart:
                    RoundStartShow();
                    break;
                //-----------------------------------------------------------------------
                //小局结束显示结果信息
                case ServerOperationType.BigRoundShowPoint:
                    BigRoundShowPoint(arguments[0].ToType<BigRoundInfomation>());
                    break;
                case ServerOperationType.BigRoundSetMessage:
                    BigRoundSetMessage(arguments[0].ToType<string>());
                    break;
                case ServerOperationType.BigRoundShowClose:
                    BigRoundShowClose();
                    break;
                //------------------------------------------------------------------------
                //SET数值和墓地
                case ServerOperationType.SetCoinInfo:
                    SetCoinInfo(arguments[0].ToType<bool>());
                    break;
                case ServerOperationType.SetMyCemetery:
                    SetMyCemeteryInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetEnemyCemetery:
                    SetEnemyCemeteryInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetMyDeck:
                    SetMyDeckInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetAllInfo:
                    SetAllInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetCardsInfo:
                    SetCardsInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetGameInfo:
                    SetGameInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetNameInfo:
                    SetNameInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetCountInfo:
                    SetCountInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetPointInfo:
                    SetPointInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetPassInfo:
                    SetPassInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetWinCountInfo:
                    SetWinCountInfo(arguments[0].ToType<GameInfomation>());
                    break;
                default:
                    break;
            }
            return true;
        }

        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }

        #endregion

        #region 辅助方法

        public string CardDes(CardLocation location, bool onlyStatus = false)
        {
            if(location.RowPosition.IsInBack())
            {
                return onlyStatus ? "" : LocationDes(location);
            }
            var cardStatusDes = CardDes(Data.GetCard(location));
            if (onlyStatus)
            {
                return cardStatusDes;
            }
            return $"{LocationDes(location)}: {cardStatusDes}";
        }

        public string LocationDes(CardLocation location) => $"{RowDes(location.RowPosition)}{location.CardIndex}号位";

        public string CardDes(CardStatus cardStatus)
        {
            if (cardStatus.IsCardBack)
            {
                return "[反面]";
            }
            var strength = cardStatus.IsSpying ? "" : $"[点数{cardStatus.Strength + cardStatus.HealthStatus}]";
            var arrmor = $"{(cardStatus.Armor != 0 ? $"[护甲{cardStatus.Armor}]" : "")}";
            var countdown = $"{(cardStatus.Countdown != 0 ? $"[倒数{cardStatus.Countdown}]" : "")}";
            return $"{cardStatus.Name}{strength}{arrmor}{countdown}";
        }

        public string RowDes(RowPosition row) => row switch
        {
            RowPosition.MyRow1 => "我方前排",
            RowPosition.MyRow2 => "我方中排",
            RowPosition.MyRow3 => "我方后排",
            RowPosition.EnemyRow1 => "敌方前排",
            RowPosition.EnemyRow2 => "敌方中排",
            RowPosition.EnemyRow3 => "敌方后排",
            RowPosition.MyHand => "我方手牌",
            RowPosition.EnemyHand => "敌方手牌",
            RowPosition.MyStay => "我方临时区",
            RowPosition.EnemyStay => "敌方临时区",
            RowPosition.MyDeck => "我方卡组",
            RowPosition.EnemyDeck => "敌方卡组",
            RowPosition.MyCemetery => "我方墓地",
            RowPosition.EnemyCemetery => "敌方墓地",
            RowPosition.SpecialPlace => "特殊牌区",
            RowPosition.MyLeader => "我方领袖",
            RowPosition.EnemyLeader => "敌方领袖",
            RowPosition.Banish => "放逐区",
            RowPosition.None => "无效区",
            _ => "异常",
        };

        public RoundInfo GetRandomPlay()
        {
            var stayPlayCardIndex = HandCanPlay().Mess().First();
            var card = default(CardStatus);
            if (stayPlayCardIndex == -1)
            {
                card = Data.GetCard(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyLeader });
                Data.IsMyLeader = false;
            }
            else
                card = Data.GetCard(new CardLocation() { CardIndex = stayPlayCardIndex, RowPosition = RowPosition.MyHand });
            var cardCanUse = CardCanPlay(card.CardId.CardInfo().CardUseInfo);
            var round = new RoundInfo()
            {
                IsPass = false,
                HandCardIndex = stayPlayCardIndex,
                CardLocation = (cardCanUse.Count() == 0 ? new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyCemetery } :
                (
                    cardCanUse.GroupBy(x => x.RowPosition).Mess().First().Mess().First()
                ))
            };
            return round;
        }

        // Pass指令
        public RoundInfo GetPassPlay() => new RoundInfo()
        {
            IsPass = true
        };

        public IEnumerable<CardLocation> CardCanPlay(CardUseInfo range)//当前卡牌可以放置的所有位置(不包含弃牌)
        {
            var result = new List<CardLocation>();
            switch (range)
            {
                case CardUseInfo.AnyPlace:
                    result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                    break;
                case CardUseInfo.MyPlace:
                    result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                    break;
                case CardUseInfo.EnemyPlace:
                    result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                    break;
                case CardUseInfo.MyRow:
                    if (Data.MyPlace[0].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[0].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = x }));
                    if (Data.MyPlace[1].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[1].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow2, CardIndex = x }));
                    if (Data.MyPlace[2].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[2].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow3, CardIndex = x }));
                    break;
                case CardUseInfo.EnemyRow:
                    if (Data.EnemyPlace[0].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[0].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow1, CardIndex = x }));
                    if (Data.EnemyPlace[1].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[1].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow2, CardIndex = x }));
                    if (Data.EnemyPlace[2].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[2].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow3, CardIndex = x }));
                    break;
                case CardUseInfo.AnyRow:
                    if (Data.MyPlace[0].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[0].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = x }));
                    if (Data.MyPlace[1].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[1].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow2, CardIndex = x }));
                    if (Data.MyPlace[2].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.MyPlace[2].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow3, CardIndex = x }));
                    if (Data.EnemyPlace[0].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[0].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow1, CardIndex = x }));
                    if (Data.EnemyPlace[1].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[1].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow2, CardIndex = x }));
                    if (Data.EnemyPlace[2].Count < GwentGlobalSetting.RowMaxCount)
                        result.AddRange(0.To(Data.EnemyPlace[2].Count)
                        .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow3, CardIndex = x }));
                    break;
                case CardUseInfo.ReSet:
                default:
                    break;
            }
            return result;
        }

        public IEnumerable<int> HandCanPlay(string inclinationId = "")//当前可以打出的手牌
        {
            if (Data.MyHandCard.Any(x => x.CardId == inclinationId) || (Data.IsMyLeader && Data.MyLeader.First().CardId == inclinationId))
            {
                var result = new List<int>();
                for (var i = 0; i < Data.MyHandCard.Count; i++)
                {
                    if (Data.MyHandCard[i].CardId == inclinationId)
                    {
                        result.Add(i);
                    }
                }
                if (Data.IsMyLeader && Data.MyLeader.First().CardId == inclinationId)
                {
                    if (Data.MyHandCard.Count > 0)
                        return result.Append(-1);//
                    else
                        return (-1).To(-1);
                }
                return result;
            }
            else
            {
                var result = 0.To(Data.MyHandCard.Count - 1);//0-数量-1
                if (Data.IsMyLeader)
                {
                    if (Data.MyHandCard.Count > 0)
                        return result.Append(-1);//
                    else
                        return (-1).To(-1);
                }
                return result;
            }
        }

        public string GameStatusDes(GameStatus gameStatus) => gameStatus switch
        {
            GameStatus.Win => "胜利",
            GameStatus.Lose => "失败",
            GameStatus.Draw => "平局",
            _ => "异常"
        };

        #endregion

        public virtual void Delay(int num)
        {
            // 控制客户端延迟, 从而流畅展示动画, 对于控制台而言没什么意义
            // Thread.Sleep(num);
        }

        public virtual void Log(string num)
        {
            // 对应服务端Debug信息, 不做展示
        }

        public virtual void YNMessageBox(string title, string message)
        {
            Console.WriteLine($"{title}: {message}");
        }
        //--------------------
        public virtual void BigRoundShowPoint(BigRoundInfomation data)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"[小局结束] 本小局{GameStatusDes(data.GameStatus)} ({data.MyPoint}:{data.EnemyPoint}), 当前比分({data.MyWinCount}:{data.EnemyWinCount})");
        }

        public virtual void BigRoundSetMessage(string message)
        {
            Console.WriteLine($"{message}");
        }
        public virtual void BigRoundShowClose()
        {
            // 无需实现
        }
        //--------------------
        // (选择卡牌,选择场地卡牌,选择行和更新卡牌信息)

        public virtual void SetCard(CardLocation location, CardStatus card)
        {
            Data.SetCard(location, card);
        }
        public virtual void CreateCard(CardStatus card, CardLocation location)
        {
            Data.CreateCard(card, location);
        }
        //--------------------
        public virtual void MulliganStart(IList<CardStatus> cards, int total)//调度界面
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("开始调度");
        }
        //调度结束
        public virtual void MulliganEnd()
        {
            Console.WriteLine("\n调度结束");
        }
        //更新信息(需要更改),动画之类的
        public virtual void MulliganData(int index, CardStatus card)
        {
            Console.WriteLine($"第{index + 1}张手牌被调度走, 换来了: {CardDes(card)}");
        }
        //-------------------------------------------------------------------------------------------
        //更新数据的方法们
        public virtual void SetMulliganInfo(GameInfomation gameInfomation)
        {
            Data.SetMulliganInfo(gameInfomation);
        }
        public virtual void SetAllInfo(GameInfomation gameInfomation)//更新全部数据
        {
            Data.SetAllInfo(gameInfomation);
        }
        public virtual void SetMyCemeteryInfo(IList<CardStatus> myCemetery)
        {
            Data.SetMyCemeteryInfo(myCemetery);
        }
        public virtual void SetEnemyCemeteryInfo(IList<CardStatus> enemyCemetery)
        {
            Data.SetEnemyCemeteryInfo(enemyCemetery);
        }
        public virtual void SetMyDeckInfo(IList<CardStatus> myDeck)
        {
            Data.SetMyDeckInfo(myDeck);
        }
        //--
        public virtual void SetGameInfo(GameInfomation gameInfomation)//更新数值+胜场数据
        {
            Data.SetGameInfo(gameInfomation);
        }
        public virtual void SetCardsInfo(GameInfomation gameInfomation)//更新卡牌类型数据
        {
            Data.SetCardsInfo(gameInfomation);
        }
        public virtual void SetCoinInfo(bool isBlueCoin)
        {
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"啪嗒嗒, 硬币跳动, {(isBlueCoin ? "你的" : "敌方")}回合开始!\n");
        }
        public virtual void SetPointInfo(GameInfomation gameInfomation)
        {
            Data.SetPointInfo(gameInfomation);
        }
        public virtual void SetCountInfo(GameInfomation gameInfomation)
        {
            Data.SetCountInfo(gameInfomation);
        }
        public virtual void SetPassInfo(GameInfomation gameInfomation)
        {
            if(gameInfomation.IsEnemyPlayerPass && !Data.IsEnemyPlayerPass)
            {
                Console.WriteLine("敌方选择Pass");
            }
            if (gameInfomation.IsMyPlayerPass && !Data.IsMyPlayerPass)
            {
                Console.WriteLine("我方选择Pass");
            }
            Data.SetPassInfo(gameInfomation);
        }
        public virtual void SetWinCountInfo(GameInfomation gameInfomation)
        {
            // 设置小王冠(小局比分), 不用处理
            Data.SetWinCountInfo(gameInfomation);
        }
        public virtual void SetNameInfo(GameInfomation gameInfomation)
        {
            // 服务器没调用过该方法
            Data.SetNameInfo(gameInfomation);
        }
        //-------------------------------------------------------------------------------------------
        public virtual void CardMove(MoveCardInfo info)//卡牌移动
        {
            var change = info.Card != null ? $"翻面为: {CardDes(info.Card)} 并" : "";
            Console.WriteLine($"【卡牌移动】{CardDes(info.Source)}{change}移动至 {LocationDes(info.Target)}");
            Data.CardMove(info);
        }
        public virtual void CardOn(CardLocation location)//卡牌抬起
        {
            Console.WriteLine($"【卡牌抬起】{CardDes(location)}");
        }
        public virtual void CardDown(CardLocation location)//卡牌落下
        {
            Console.WriteLine($"【卡牌落下】{CardDes(location)}");
        }
        //----------------------------------
        public virtual void ShowWeatherApply(RowPosition row, RowStatus type)
        {
            Data.ShowWeatherApply(row, type);
        }
        public virtual void ShowCardNumberChange(CardLocation location, int num, NumberType type)
        {
        }
        public virtual void ShowBullet(CardLocation source, CardLocation taget, BulletType type)
        {
        }
        public virtual void ShowCardIconEffect(CardLocation location, CardIconEffectType type)
        {
        }
        public virtual void ShowCardBreakEffect(CardLocation location, CardBreakEffectType type)
        {
            Data.ShowCardBreakEffect(location, type);
        }
        //-------------------------------------------------------------------------------------------
        public virtual void ShowCardsToCemetery(GameCardsPart cards)
        {
            Data.ShowCardsToCemetery(cards);
        }
        public virtual void ShowGameResult(GameResultInfomation gameResult)//设定并展示游戏结束画面
        {
            Console.WriteLine("===================================");
            Console.WriteLine($"游戏{GameStatusDes(gameResult.GameStatu)}");
            Console.WriteLine($"{gameResult.MyName} vs {gameResult.EnemyName}");
            if (gameResult.RoundCount >= 1)
            {
                Console.WriteLine($"R1: {gameResult.MyR1Point} : {gameResult.EnemyR1Point}");
            }
            if(gameResult.RoundCount >= 2)
            {
                Console.WriteLine($"R2: {gameResult.MyR2Point} : {gameResult.EnemyR2Point}");
            }
            if(gameResult.RoundCount >= 3)
            {
                Console.WriteLine($"R3: {gameResult.MyR3Point} : {gameResult.EnemyR3Point}");
            }
            Console.WriteLine("===================================");
        }

        public virtual void RoundEnd()
        {
            // 暂时不需实现, 在硬币变动处已实现
        }

        //----------------------------------
        //回合开始动画
        public virtual void RoundStartShow()
        {
            // 暂时不需实现, 在硬币变动处已实现
        }

        #region 服务端需要玩家做出操作

        // 需要玩家打出一张卡 (或弃牌, Pass)
        public void GetPlayerDrag(Action<Operation<UserOperationType>> send)
        {
            Console.WriteLine("[1]. 随机出牌");
            Console.WriteLine("[_]. Pass");

            var operate = Console.ReadLine().Trim() switch
            {
                "1" => GetRandomPlay(),
                _ => GetPassPlay()
            };

            send(Operation.Create(UserOperationType.RoundOperate, operate));
        }

        // 需要玩家在菜单中进行选择
        public void SelectMenuCards(MenuSelectCardInfo info, Action<Operation<UserOperationType>> send)
        {
            Console.WriteLine($"{info.Title}");
            Console.ReadLine();
            //先后手固定选0
            if (info.Title == "请选择你认为后手价值的点数")
            {
                var result = new List<int>()
                    {
                        info.SelectList.Indexed().First(x=>x.Value.Name=="0").Key
                    };
                send(
                    Operation.Create(UserOperationType.SelectMenuCardsInfo,
                    result
                ));
            }
            else
            {
                send(Operation.Create(UserOperationType.SelectMenuCardsInfo, 0.To(info.SelectList.Count - 1).Mess().Take(info.SelectCount).ToList()));
            }
        }

        // 需要玩家选择若干张卡
        public void SelectPlaceCards(PlaceSelectCardsInfo info, Action<Operation<UserOperationType>> send)
        {
            Console.WriteLine($"请为卡牌选择目标: {CardDes(info.SelectCard)}");
            Console.ReadLine();
            send(Operation.Create(UserOperationType.SelectPlaceCardsInfo, info.CanSelect.CardsPartToLocation().Mess().Take(info.SelectCount).ToList()));
        }

        // 需要玩家选择一行
        public void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, Action<Operation<UserOperationType>> send)
        {
            Console.WriteLine($"请为卡牌选择一行 [{CardDes(selectCard)}]");
            Console.ReadLine();
            send(Operation.Create(UserOperationType.SelectRowInfo, rowPart.Mess().First()));
        }

        // 需要玩家选择卡牌打出的位置
        public void PlayCard(CardLocation location, Action<Operation<UserOperationType>> send)
        {
            Console.WriteLine($"请为卡牌选择打出位置: {CardDes(location)}");
            Console.ReadLine();
            var cardStatus = Data.GetCard(location);
            var cardCanPlay = CardCanPlay(cardStatus.CardId.CardInfo().CardUseInfo);
        }

        // 需要玩家调度
        public void GetMulliganInfo(Action<Operation<UserOperationType>> send)
        {
            var handCount = Data.MyHandCard.Count();
            Console.WriteLine($"\n请进行调度");
            Console.WriteLine($"[1~{handCount}]. 调度卡牌");
            Console.WriteLine("[_]. 不再调度");
            var res = Console.ReadLine();
            var mulliganIndex = int.TryParse(res, out var index) ? index : -1;
            mulliganIndex = (mulliganIndex < 1 || mulliganIndex > handCount) ? -1 : mulliganIndex - 1;

            send(Operation.Create(UserOperationType.MulliganInfo, mulliganIndex));//-1表示不进行调度
        }

        #endregion
    }
}
