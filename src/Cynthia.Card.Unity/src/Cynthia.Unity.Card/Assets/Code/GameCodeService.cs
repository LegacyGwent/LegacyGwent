using System;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.Audio;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GameCodeService
    {
        private GameObject _code;
        public GameCodeService()
        {
            // Debug.Log("尝试获取了Code!");
            _code = GameObject.Find("Code");
            // Debug.Log(_code == null ? "没能成功获取" : "成功获取到了");
        }
        //-------------------------------------------------------------------------------------------
        public void SelectUICard(int index)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.SelectCard(index);
        }
        public void ClickUICard(int index)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.ClickCard(index);
        }
        //--------------------
        public void BigRoundShowPoint(BigRoundInfomation data)
        {
            _code.GetComponent<GameCode>().BigRoundControl.ShowPoint(data);
        }
        public void BigRoundSetMessage()
        {
            _code.GetComponent<GameCode>().BigRoundControl.DisplayMessage();
        }
        public void BigRoundShowClose()
        {
            _code.GetComponent<GameCode>().BigRoundControl.CloseBigRound();
        }
        //--------------------
        //新纪元系列(选择卡牌,选择场地卡牌,选择行和更新卡牌信息)
        public void SelectMenuCards(MenuSelectCardInfo info, LocalPlayer player)
        {
            _ = _code.GetComponent<GameCode>().GameCardShowControl.SelectMenuCards(info, player);
        }
        public void SelectPlaceCards(PlaceSelectCardsInfo info, LocalPlayer player)
        {
            _ = _code.GetComponent<GameCode>().GameEvent.SelectPlaceCards(info, player);
        }
        public void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, LocalPlayer player)
        {
            _ = _code.GetComponent<GameCode>().GameEvent.SelectRow(selectCard, rowPart, player);
        }
        public void PlayCard(CardLocation location, LocalPlayer player)
        {
            _ = _code.GetComponent<GameCode>().GameEvent.GetPlayCard(location, player);
        }
        public void SetCard(CardLocation location, CardStatus card)
        {
            _code.GetComponent<GameCode>().GameEvent.SetCard(location, card);
        }
        public void CreateCard(CardStatus card, CardLocation location)
        {
            _code.GetComponent<GameCode>().GameEvent.CreateCard(card, location);
        }
        //--------------------
        public void MulliganStart(IList<CardStatus> cards, int total)//调度界面
        {
            _code.GetComponent<GameCode>().GameCardShowControl.MulliganStart(cards, total);
        }
        //调度结束
        public void MulliganEnd()
        {
            // start timer for first round in a big round
            _code.GetComponent<GameCode>().GameUIControl.ropeController.StartRopeTimer();
            _code.GetComponent<GameCode>().GameCardShowControl.OperationEnd();
        }
        //更新信息(需要更改),动画之类的
        public void MulliganData(int index, CardStatus card)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.MulliganData(index, card);
        }
        //获取调度信息
        public void GetMulliganInfo(LocalPlayer player)
        {
            _ = _code.GetComponent<GameCode>().GameCardShowControl.GetMulliganInfo(player);
        }
        //----------------------------------
        //回合开始动画
        public void RoundStartShow()
        {
            _code.GetComponent<GameCode>().MyRoundShow.Play("RoundShow");
        }
        //-------------------------------------------------------------------------------------------
        //更新数据的方法们
        public void SetMulliganInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetMulliganInfo(gameInfomation);
        }
        public void SetAllInfo(GameInfomation gameInfomation)//更新全部数据
        {
            _code.GetComponent<GameCode>().GameUIControl.SetGameInfo(gameInfomation);
            _code.GetComponent<GameCode>().GameCardsControl.SetCardsInfo(gameInfomation);
        }
        public void SetMyCemeteryInfo(IList<CardStatus> myCemetery)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.MyCemetery = myCemetery;
        }
        public void SetEnemyCemeteryInfo(IList<CardStatus> enemyCemetery)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.EnemyCemetery = enemyCemetery;
        }
        public void SetMyDeckInfo(IList<CardStatus> myDeck)
        {
            _code.GetComponent<GameCode>().GameCardShowControl.MyDeck = myDeck;
        }
        //--
        public void SetGameInfo(GameInfomation gameInfomation)//更新数值+胜场数据
        {
            _code.GetComponent<GameCode>().GameUIControl.SetGameInfo(gameInfomation);
        }
        public void SetCardsInfo(GameInfomation gameInfomation)//更新卡牌类型数据
        {
            _code.GetComponent<GameCode>().GameCardsControl.SetCardsInfo(gameInfomation);
        }
        //
        public void SetCoinInfo(bool isBlueCoin)
        {
            _code.GetComponent<GameCode>().GameEvent.SetCoinInfo(isBlueCoin);
        }
        public void SetMyLand(int coinpoint)
        {
            Debug.Log("my coin points are "  + coinpoint.ToString());
            _code.GetComponent<GameCode>().GameUIControl.SetMyLandInfo(coinpoint);

        }
        public void SetEnemyLand(int coinpoint)
        {
            Debug.Log("enemy coin points are "  + coinpoint.ToString());
            _code.GetComponent<GameCode>().GameUIControl.SetEnemyLandInfo(coinpoint);
            
        }
        public void SetPointInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetPointInfo(gameInfomation);
        }
        public void SetCountInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetCountInfo(gameInfomation);
        }
        public void SetPassInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetPassInfo(gameInfomation);
        }
        public void SetWinCountInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetWinCountInfo(gameInfomation);
        }
        public void SetNameInfo(GameInfomation gameInfomation)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetNameInfo(gameInfomation);
        }
        public void SetMMRInfo(int myMMR, int EnemyMMR)
        {
            _code.GetComponent<GameCode>().GameUIControl.SetMMRInfo(myMMR, EnemyMMR);
        }
        //-------------------------------------------------------------------------------------------
        public void CardMove(MoveCardInfo info)//卡牌移动
        {
            _code.GetComponent<GameCode>().GameEvent.CardMove(info);
        }
        public void CardOn(CardLocation location)//卡牌抬起
        {
            _code.GetComponent<GameCode>().GameEvent.CardOn(location);
        }
        public void CardDown(CardLocation location)//卡牌落下
        {
            _code.GetComponent<GameCode>().GameEvent.CardDown(location);
        }
        //----------------------------------
        public void ShowWeatherApply(RowPosition row, RowStatus type)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowWeatherApply(row, type);
        }
        public void ShowCardNumberChange(CardLocation location, int num, NumberType type)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowNumber(location, num, type);
        }
        public void ShowBullet(CardLocation source, CardLocation taget, BulletType type)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowBullet(source, taget, type);
        }
        public void ShowCardIconEffect(CardLocation location, CardIconEffectType type)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowCardIconEffect(location, type);
        }
        public void ShowCardBreakEffect(CardLocation location, CardBreakEffectType type)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowCardBreakEffect(location, type);
        }
        //-------------------------------------------------------------------------------------------
        public void LeaveGame()//立刻离开游戏,进入主菜单
        {
            _code.GetComponent<GameCode>().LeaveGame();
        }
        public void ShowCardsToCemetery(GameCardsPart cards)
        {
            _code.GetComponent<GameCode>().GameEvent.ShowCardsToCemetery(cards);
        }
        public void ShowGameResult(GameResultInfomation gameResult)//设定并展示游戏结束画面
        {
            _code.GetComponent<GameCode>().GameResultControl.ShowGameResult(gameResult);
        }
        public void ShowMMRResult(int oldMMR, int newMMR)//设定并展示MMR
        {
            _code.GetComponent<GameCode>().GameResultControl.ShowMMRResult(oldMMR, newMMR);
        }
        public void GetPlayerDrag(LocalPlayer player)//玩家的回合到了
        {
            _ = _code.GetComponent<GameCode>().GameEvent.GetPlayerDrag(player);
        }
        public void RoundEnd()
        {
            // start timing for my and opponent's round
            _code.GetComponent<GameCode>().GameUIControl.ropeController.StartRopeTimer();
            _code.GetComponent<GameCode>().GameEvent.RoundEnd();
        }
        public void GameStart()
        {
            // start timing for deciding red coin
            _code.GetComponent<GameCode>().GameUIControl.ropeController.StartRopeTimer();
            _code.GetComponent<GameCode>().GameUIControl.SetDecideCoinInfo(true);
        }
        /*
        public void MyCardEffectEnd()//结束卡牌效果
        {
            _code.GetComponent<GameCode>().GameEvent.MyCardEffectEnd();
        }
        public void EnemyDrag(RoundInfo enemyRoundInfo,CardStatus cardInfo)
        {
            _code.GetComponent<GameCode>().GameEvent.EnemyDrag(enemyRoundInfo,cardInfo);
        }
        public void EnemyCardEffectEnd()//结束卡牌效果
        {
            _code.GetComponent<GameCode>().GameEvent.EnemyCardEffectEnd();
        }
        public void SetCardTo(RowPosition rowIndex,int cardIndex,RowPosition tagetRowIndex,int tagetCardIndex)
        {
            _code.GetComponent<GameCode>().GameEvent.SetCardTo(rowIndex, cardIndex, tagetRowIndex, tagetCardIndex);
        }
        public void GetCardFrom(RowPosition getPosition,RowPosition tagetPosition,int tagetCardIndex,CardStatus cardInfo)
        {
            _code.GetComponent<GameCode>().GameEvent.GetCardFrom(getPosition, tagetPosition, tagetCardIndex, cardInfo);
        }*/
        //-------------------------------------------------
        public Transform GetGameScale()
        {
            return _code.GetComponent<GameCode>().GameScale;
        }
    }
}
