using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using UnityEngine;
using Alsein.Extensions.LifetimeAnnotations;
using System.Threading;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GwentClientGameService
    {
        private LocalPlayer _player;
        private long _id;
        //--------------------------------
        public GameCodeService GameCodeService { get; set; }
        public GlobalUIService GlobalUIService { get; set; }
        //--------------------------------
        public GwentClientGameService(GameCodeService codeService, GlobalUIService globalUIService)
        {
            _id = DateTime.UtcNow.Ticks;
            GameCodeService = codeService;
            GlobalUIService = globalUIService;
        }

        public async Task Play(LocalPlayer player)
        {
            Debug.Log($"游戏开始,Id:{_id}");
            _player = player;
            // while (await StartHandle(await _player.ReceiveAsync())) ;
            // if (_mustOver) return;
            // Debug.Log("预处理完毕");
            while (await ResponseOperations(await _player.ReceiveAsync())) ;
        }
        //-----------------------------------------------------------------------
        //响应指令
        private async Task<bool> ResponseOperations(IList<Operation<ServerOperationType>> operations)
        {
            // Debug.Log($"收到了一个集合指令,其中包含{operations.Count}个指令,Id:{_id}");
            foreach (var operaction in operations)
            {
                if (!await ResponseOperation(operaction))
                    return false;
            }
            return true;
        }
        private async Task<bool> ResponseOperation(Operation<ServerOperationType> operation)
        {
            // Debug.Log($"执行了指令{operation.OperationType},线程Id:{Thread.CurrentThread.ManagedThreadId}");
            var arguments = operation.Arguments.ToArray();
            switch (operation.OperationType)
            {
                //----------------------------------------------------------------------------------
                //新指令
                case ServerOperationType.ClientDelay:
                    var dTime = arguments[0].ToType<int>();
                    // Debug.Log($"延迟触发,延迟时常:{dTime}");
                    await Task.Delay(dTime);
                    break;
                case ServerOperationType.SelectMenuCards:
                    GameCodeService.SelectMenuCards(arguments[0].ToType<MenuSelectCardInfo>(), _player);
                    break;
                case ServerOperationType.SelectPlaceCards:
                    GameCodeService.SelectPlaceCards(arguments[0].ToType<PlaceSelectCardsInfo>(), _player);
                    break;
                case ServerOperationType.SelectRow:
                    GameCodeService.SelectRow(arguments[0].ToType<CardLocation>(), arguments[1].ToType<IList<RowPosition>>(), _player);
                    break;
                case ServerOperationType.PlayCard:
                    GameCodeService.PlayCard(arguments[0].ToType<CardLocation>(), _player);
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
                    _ = GlobalUIService.YNMessageBox("收到了一个来自服务器的消息", arguments[0].ToType<string>());
                    break;
                case ServerOperationType.GetDragOrPass:
                    GameCodeService.GetPlayerDrag(_player);
                    break;
                case ServerOperationType.RoundEnd://回合结束
                    GameCodeService.RoundEnd();
                    break;
                case ServerOperationType.CardsToCemetery:
                    GameCodeService.ShowCardsToCemetery(arguments[0].ToType<GameCardsPart>());
                    break;
                case ServerOperationType.GameEnd://游戏结束,以及游戏结束信息
                    GameCodeService.ShowGameResult(arguments[0].ToType<GameResultInfomation>());
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
                case ServerOperationType.GetMulliganInfo:
                    GameCodeService.GetMulliganInfo(_player);
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
                    GameCodeService.BigRoundSetMessage(arguments[0].ToType<string>());
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
                    GameCodeService.SetAllInfo(arguments[0].ToType<GameInfomation>());
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