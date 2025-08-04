using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GwentClientGameService
    {
        private LocalPlayer _player;
        private long _id;
        private int myMMR;
        //--------------------------------
        public GameCodeService GameCodeService { get; set; }
        public GlobalUIService GlobalUIService { get; set; }

        public HubConnection _hubConnection { get; set; }
        public GwentClientService _server { get; set; }

        // public TaskCompletionSource<bool> _disconnectTaskSource { get; set; }

        // public Task<bool> ConnectTask { get => _disconnectTaskSource.Task; }


        //--------------------------------
        public GwentClientGameService(GameCodeService codeService, GlobalUIService globalUIService)
        {
            Debug.Log("创造游戏");
            // _disconnectTaskSource = new TaskCompletionSource<bool>();
            _hubConnection = DependencyResolver.Container.ResolveNamed<HubConnection>("game");
            _server = DependencyResolver.Container.Resolve<GwentClientService>();
            // _hubConnection.Closed += async e =>
            // {
            //     await Task.CompletedTask;
            //     _disconnectTaskSource.SetException(e);
            // };
            _id = DateTime.UtcNow.Ticks;
            GameCodeService = codeService;
            GlobalUIService = globalUIService;
        }

        public async Task Play(LocalPlayer player)
        {
            Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + $" 游戏开始,Id:{_id}");
            _player = player;
            // var game = Task.Run(async () =>
            // {
            Debug.Log("运行开始");
            while (
                await ResponseOperations(
                    await _player.ReceiveAsync(), ClientGlobalInfo.ViewingRoomId != ""
                )
            ) ;
            // });
            // await Task.WhenAny(game, ConnectTask);
        }
        //-----------------------------------------------------------------------
        //响应指令
        private async Task<bool> ResponseOperations(IList<Operation<ServerOperationType>> operations, bool isViewer = false)
        {
            Debug.Log($"收到了一个集合指令,其中包含{operations.Count}个指令,Id:{_id}");
            foreach (var operation in operations)
            {
                Debug.Log($"包含指令{operation.OperationType}");
            }
            foreach (var operation in operations)
            {
                // Debug.Log($"执行了指令{operaction.OperationType},线程Id:{Thread.CurrentThread.ManagedThreadId}");
                if (!await ResponseOperation(operation, isViewer))
                {
                    return false;
                }
            }
            Debug.Log($"处理完毕");
            return true;
        }

        private async Task<bool> ResponseOperation(Operation<ServerOperationType> operation, bool isViewer = false)
        {
            Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + $" 开始处理指令{operation.OperationType}");
            var arguments = operation.Arguments.ToArray();
            if (!isViewer)
            {
                switch (operation.OperationType)
                {
                    case ServerOperationType.SelectMenuCards:
                        GameCodeService.SelectMenuCards(arguments[0].ToType<MenuSelectCardInfo>(), _player);
                        return true;
                    case ServerOperationType.SelectPlaceCards:
                        GameCodeService.SelectPlaceCards(arguments[0].ToType<PlaceSelectCardsInfo>(), _player);
                        return true;
                    case ServerOperationType.SelectRow:
                        GameCodeService.SelectRow(arguments[0].ToType<CardLocation>(), arguments[1].ToType<IList<RowPosition>>(), _player);
                        return true;
                    case ServerOperationType.PlayCard:
                        GameCodeService.PlayCard(arguments[0].ToType<CardLocation>(), _player);
                        return true;
                    case ServerOperationType.GetMulliganInfo:
                        GameCodeService.GetMulliganInfo(_player);
                        return true;
                    case ServerOperationType.GetDragOrPass:
                        GameCodeService.GetPlayerDrag(_player);
                        return true;
                }
            }
            switch (operation.OperationType)
            {
                case ServerOperationType.GameStart:
                    GameCodeService.GameStart();
                    break;
                //----------------------------------------------------------------------------------
                //新指令
                case ServerOperationType.ClientDelay:
                    var dTime = arguments[0].ToType<int>();
                    Debug.Log($"延迟触发,延迟时常:{dTime}");
                    await Task.WhenAny(Task.Delay(dTime));
                    break;
                //-------------------------
                //小动画
                case ServerOperationType.ShowWeatherApply:
                    GameCodeService.ShowWeatherApply(arguments[0].ToType<RowPosition>(), arguments[1].ToType<RowStatus>());
                    break;
                case ServerOperationType.ShowCardNumberChange:
                    GameCodeService.ShowCardNumberChange(arguments[0].ToType<CardLocation>(), arguments[1].ToType<int>(), arguments[2].ToType<NumberType>());
                    break;
                case ServerOperationType.ShowCardIconEffect:
                    GameCodeService.ShowCardIconEffect(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardIconEffectType>());
                    break;
                case ServerOperationType.ShowCardBreakEffect:
                    GameCodeService.ShowCardBreakEffect(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardBreakEffectType>());
                    break;
                case ServerOperationType.ShowBullet:
                    GameCodeService.ShowBullet(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardLocation>(), arguments[2].ToType<BulletType>());
                    break;
                //-------------------------
                case ServerOperationType.SetCard:
                    GameCodeService.SetCard(arguments[0].ToType<CardLocation>(), arguments[1].ToType<CardStatus>());
                    break;
                case ServerOperationType.CreateCard:
                    GameCodeService.CreateCard(arguments[0].ToType<CardStatus>(), arguments[1].ToType<CardLocation>());
                    break;
                //----------------------------------------------------------------------------------
                case ServerOperationType.Debug:
                    Debug.Log(arguments[0].ToType<string>());
                    break;
                case ServerOperationType.MessageBox:
                    _ = GlobalUIService.YNMessageBox("PopupWindow_ReceivedMessageTitle", arguments[0].ToType<string>(), "PopupWindow_OkButton", isOnlyYes: true);
                    break;
                case ServerOperationType.RoundEnd://回合结束
                    GameCodeService.RoundEnd();
                    break;
                case ServerOperationType.CardsToCemetery:
                    GameCodeService.ShowCardsToCemetery(arguments[0].ToType<GameCardsPart>());
                    break;
                case ServerOperationType.GameEnd://游戏结束,以及游戏结束信息l
                    var info = arguments[0].ToType<GameResultInfomation>();
                    GameCodeService.ShowGameResult(info);
                    var newMMR = await _server.GetPalyernameMMR(info.MyName);
                    GameCodeService.ShowMMRResult(myMMR, newMMR);
                    return false;
                case ServerOperationType.CardMove:
                    GameCodeService.CardMove(arguments[0].ToType<MoveCardInfo>());
                    break;
                case ServerOperationType.CardOn:
                    GameCodeService.CardOn(arguments[0].ToType<CardLocation>());
                    break;
                case ServerOperationType.CardDown:
                    GameCodeService.CardDown(arguments[0].ToType<CardLocation>());
                    break;
                //------------------------------------------------------------------------
                //和调度有关的一切
                case ServerOperationType.MulliganStart:
                    GameCodeService.MulliganStart(arguments[0].ToType<IList<CardStatus>>(), arguments[1].ToType<int>());
                    break;
                case ServerOperationType.MulliganData:
                    GameCodeService.MulliganData(arguments[0].ToType<int>(), arguments[1].ToType<CardStatus>());
                    break;
                case ServerOperationType.MulliganEnd:
                    GameCodeService.MulliganEnd();
                    break;
                case ServerOperationType.SetMulliganInfo:
                    //
                    GameCodeService.SetMulliganInfo(arguments[0].ToType<GameInfomation>());
                    break;
                //----------------
                //显示你的回合到了
                case ServerOperationType.RemindYouRoundStart:
                    GameCodeService.RoundStartShow();
                    break;
                //-----------------------------------------------------------------------
                //小局结束显示结果信息
                case ServerOperationType.BigRoundShowPoint:
                    GameCodeService.BigRoundShowPoint(arguments[0].ToType<BigRoundInfomation>());
                    break;
                case ServerOperationType.BigRoundSetMessage:
                    GameCodeService.BigRoundSetMessage();
                    break;
                case ServerOperationType.BigRoundShowClose:
                    GameCodeService.BigRoundShowClose();
                    break;
                //------------------------------------------------------------------------
                //SET数值和墓地
                case ServerOperationType.SetCoinInfo:
                    GameCodeService.SetCoinInfo(arguments[0].ToType<bool>());
                    break;
                case ServerOperationType.SetMyCemetery:
                    GameCodeService.SetMyCemeteryInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetEnemyCemetery:
                    GameCodeService.SetEnemyCemeteryInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetMyDeck:
                    GameCodeService.SetMyDeckInfo(arguments[0].ToType<List<CardStatus>>());
                    break;
                case ServerOperationType.SetAllInfo:
                    var gameInfo = arguments[0].ToType<GameInfomation>();
                    GameCodeService.SetAllInfo(gameInfo);
                    myMMR = await _server.GetPalyernameMMR(gameInfo.MyName);
                    var enemyMMR = await _server.GetPalyernameMMR(gameInfo.EnemyName);
                    GameCodeService.SetMMRInfo(myMMR, enemyMMR);
                    break;
                case ServerOperationType.SetCardsInfo:
                    GameCodeService.SetCardsInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetGameInfo:
                    GameCodeService.SetGameInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetNameInfo:
                    GameCodeService.SetNameInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetCountInfo:
                    GameCodeService.SetCountInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetPointInfo:
                    GameCodeService.SetPointInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetPassInfo:
                    GameCodeService.SetPassInfo(arguments[0].ToType<GameInfomation>());
                    break;
                case ServerOperationType.SetWinCountInfo:
                    GameCodeService.SetWinCountInfo(arguments[0].ToType<GameInfomation>());
                    break;
                    /*旧时代指令集
                case ServerOperationType.EnemyCardDrag:
                    GameCodeService.EnemyDrag(arguments[0].ToType<RoundInfo>(),arguments[1].ToType<CardStatus>());
                    break;
                case ServerOperationType.MyCardEffectEnd:
                    GameCodeService.MyCardEffectEnd();
                    break;
                case ServerOperationType.EnemyCardEffectEnd://卡牌效果落下
                    GameCodeService.EnemyCardEffectEnd();
                    break;
                case ServerOperationType.SetCardTo:
                    GameCodeService.SetCardTo
                    (
                        arguments[0].ToType<RowPosition>(), 
                        arguments[1].ToType<int>(),
                        arguments[2].ToType<RowPosition>(),
                        arguments[3].ToType<int>()
                    );
                    break;
                case ServerOperationType.GetCardFrom:
                    GameCodeService.GetCardFrom
                    (
                        arguments[0].ToType<RowPosition>(),
                        arguments[1].ToType<RowPosition>(),
                        arguments[2].ToType<int>(),
                        arguments[3].ToType<CardStatus>()
                    );
                    break;*/
            }
            return true;
        }
    }
}
