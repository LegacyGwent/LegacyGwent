using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.IO;

namespace Cynthia.Card.AI
{
    public abstract class AIPlayer : Player
    {
        public ClientGameData Data { get; set; } = new ClientGameData();

        public AIPlayer() : base()
        {
            Receive += async x =>
            {
                await HandleServerOperation((Operation<ServerOperationType>)x.Result);
            };
            SetDeckAndName();
        }

        public abstract void SetDeckAndName();

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

        public (bool, RoundInfo) TryGetRandomPlay(string cardId)
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

        public RoundInfo GetPassPlay()
        {
            return new RoundInfo()
            {
                IsPass = true
            };
        }

        public Task SendAsync(Operation<UserOperationType> operation) => _downstream.SendAsync(operation);
        public Task SendAsync(UserOperationType type, params object[] objs) => _downstream.SendAsync(Operation.Create(type, objs));
        public new Task<Operation<ServerOperationType>> ReceiveAsync() => _downstream.ReceiveAsync<Operation<ServerOperationType>>();
        public new event Func<TubeReceiveEventArgs, Task> Receive { add => _downstream.Receive += value; remove => _downstream.Receive -= value; }



        public virtual void Delay(int num)
        {
        }

        public virtual void Log(string num)
        {
        }

        public virtual void YNMessageBox(string title, string message)
        {
        }
        //--------------------
        public virtual void BigRoundShowPoint(BigRoundInfomation data)
        {
        }
        public virtual void BigRoundSetMessage(string message)
        {
        }
        public virtual void BigRoundShowClose()
        {
        }
        //--------------------
        //新纪元系列(选择卡牌,选择场地卡牌,选择行和更新卡牌信息)

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
        }
        //调度结束
        public virtual void MulliganEnd()
        {
        }
        //更新信息(需要更改),动画之类的
        public virtual void MulliganData(int index, CardStatus card)
        {
        }

        //获取调度信息
        public abstract void GetMulliganInfo(Action<Operation<UserOperationType>> send);
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
            Data.SetPassInfo(gameInfomation);
        }
        public virtual void SetWinCountInfo(GameInfomation gameInfomation)
        {
            Data.SetWinCountInfo(gameInfomation);
        }
        public virtual void SetNameInfo(GameInfomation gameInfomation)
        {
            Data.SetNameInfo(gameInfomation);
        }
        //-------------------------------------------------------------------------------------------
        public virtual void CardMove(MoveCardInfo info)//卡牌移动
        {
            Data.CardMove(info);
        }
        public virtual void CardOn(CardLocation location)//卡牌抬起
        {
        }
        public virtual void CardDown(CardLocation location)//卡牌落下
        {
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
        }

        public virtual void RoundEnd()
        {
        }

        //----------------------------------
        //回合开始动画
        public virtual void RoundStartShow()
        {
        }

        public abstract void GetPlayerDrag(Action<Operation<UserOperationType>> send);//玩家的回合到了

        public abstract void SelectMenuCards(MenuSelectCardInfo info, Action<Operation<UserOperationType>> send);

        public abstract void SelectPlaceCards(PlaceSelectCardsInfo info, Action<Operation<UserOperationType>> send);

        public abstract void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, Action<Operation<UserOperationType>> send);

        public abstract void PlayCard(CardLocation location, Action<Operation<UserOperationType>> send);

        public IEnumerable<int> HandCanPlay()//当前可以打出的手牌
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
    }
}
