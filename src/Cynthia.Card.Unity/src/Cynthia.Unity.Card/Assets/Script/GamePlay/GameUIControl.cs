using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;
using System.Linq;
using System;
using Assets.Script.Localization;
using Autofac;

public class GameUIControl : MonoBehaviour
{
    public Text MyHandCount;//
    public Text MyCemeteryCount;//
    public Text MyDeckCount;//
    public Text EnemyHandCount;//
    public Text EnemyCemeteryCount;//
    public Text EnemyDeckCount;//
    //-----------------------------------
    public Text MyRow1Point;
    public Text MyRow2Point;
    public Text MyRow3Point;
    public Text MyLand;
    public Text EnemyLand;
    public Text MyAllPoint;
    public Text EnemyRow1Point;
    public Text EnemyRow2Point;
    public Text EnemyRow3Point;
    public Text EnemyAllPoint;
    //-----------------------------------
    public Text MyName;//
    public Text EnemyName;//
    public Text MyMMR;
    public Text EnemyMMR;
    //-----------------------------------
    public GameObject MyCrownLeft;//
    public GameObject MyCrownRight;//
    public GameObject EnemyCrownLeft;//
    public GameObject EnemyCrownRight;//
    public GameObject MyLandObject;
    public GameObject EnemyLandObject;
    //----------------------------------
    public GameObject MyPass;
    public GameObject EnemyPass;
    public Text MyShowMessage;//
    public Text EnemyShowMessage;//
    public RopeController ropeController;

    private LocalizationService _translator;
    private GwentClientService server;

    private void Awake()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    public void SetPointInfo(GameInfomation gameInfomation)
    {
        MyAllPoint.color = ClientGlobalInfo.NormalColor;
        EnemyAllPoint.color = ClientGlobalInfo.NormalColor;
        MyRow1Point.text = gameInfomation.MyRow1Point.ToString();
        MyRow2Point.text = gameInfomation.MyRow2Point.ToString();
        MyRow3Point.text = gameInfomation.MyRow3Point.ToString();
        EnemyRow1Point.text = gameInfomation.EnemyRow1Point.ToString();
        EnemyRow2Point.text = gameInfomation.EnemyRow2Point.ToString();
        EnemyRow3Point.text = gameInfomation.EnemyRow3Point.ToString();
        var myAllPoint = (gameInfomation.MyRow1Point + gameInfomation.MyRow2Point + gameInfomation.MyRow3Point);
        var enemyAllPoint = (gameInfomation.EnemyRow1Point + gameInfomation.EnemyRow2Point + gameInfomation.EnemyRow3Point);
        MyAllPoint.text = myAllPoint.ToString();
        EnemyAllPoint.text = enemyAllPoint.ToString();
        if (myAllPoint > enemyAllPoint)
            MyAllPoint.color = ClientGlobalInfo.WinColor;
        else if (myAllPoint < enemyAllPoint)
            EnemyAllPoint.color = ClientGlobalInfo.WinColor;
    }
    public void SetCountInfo(GameInfomation gameInfomation)
    {
        MyHandCount.text = gameInfomation.MyHandCount.ToString();
        EnemyHandCount.text = gameInfomation.EnemyHandCount.ToString();
        MyCemeteryCount.text = gameInfomation.MyCemeteryCount.ToString();
        EnemyCemeteryCount.text = gameInfomation.EnemyCemeteryCount.ToString();
        MyDeckCount.text = gameInfomation.MyDeckCount.ToString();
        EnemyDeckCount.text = gameInfomation.EnemyDeckCount.ToString();
    }
    public void SetPassInfo(GameInfomation gameInfomation)
    {
        if (gameInfomation.IsMyPlayerPass)
            MyShowMessage.text = _translator.GetText("IngameMenu_Passed");
        if (gameInfomation.IsEnemyPlayerPass)
            EnemyShowMessage.text = _translator.GetText("IngameMenu_Passed");
        MyPass.SetActive(gameInfomation.IsMyPlayerPass);
        EnemyPass.SetActive(gameInfomation.IsEnemyPlayerPass);
    }
    public void SetMulliganInfo(GameInfomation gameInfomation)
    {
        //if (gameInfomation.IsMyPlayerPass)
        //MyShowMessage.text = "放弃跟牌";
        if (gameInfomation.IsEnemyPlayerMulligan)
            EnemyShowMessage.text = _translator.GetText("IngameMenu_EnemyRedrawing");
        //MyPass.SetActive(gameInfomation.IsMyPlayerMulligan);
        EnemyPass.SetActive(gameInfomation.IsEnemyPlayerMulligan);
    }
    public void SetDecideCoinInfo(bool isEnemyPlayerDecidingCoin)
    {
        if (isEnemyPlayerDecidingCoin)
            EnemyShowMessage.text = _translator.GetText("IngameMenu_EnemyDecidingCoin");
        EnemyPass.SetActive(isEnemyPlayerDecidingCoin);
    }
    public void SetWinCountInfo(GameInfomation gameInfomation)
    {
        if (gameInfomation.MyWinCount == 0)
        {
            MyCrownLeft.SetActive(false);
            MyCrownRight.SetActive(false);
        }
        if (gameInfomation.MyWinCount == 1)
        {
            MyCrownLeft.SetActive(true);
            MyCrownRight.SetActive(false);
        }
        if (gameInfomation.MyWinCount == 2)
        {
            MyCrownLeft.SetActive(true);
            MyCrownRight.SetActive(true);
        }
        if (gameInfomation.EnemyWinCount == 0)
        {
            EnemyCrownLeft.SetActive(false);
            EnemyCrownRight.SetActive(false);
        }
        if (gameInfomation.EnemyWinCount == 1)
        {
            EnemyCrownLeft.SetActive(true);
            EnemyCrownRight.SetActive(false);
        }
        if (gameInfomation.EnemyWinCount == 2)
        {
            EnemyCrownLeft.SetActive(true);
            EnemyCrownRight.SetActive(true);
        }
    }
    public void SetNameInfo(GameInfomation gameInfomation)
    {
        EnemyName.text = gameInfomation.EnemyName;
        MyName.text = gameInfomation.MyName;
    }
    public void SetMMRInfo(int myMMR, int enemyMMR)
    {
        MyMMR.text = Convert.ToString(myMMR);
        EnemyMMR.text = Convert.ToString(enemyMMR);
    }
    public void SetMyLandInfo(int land)
    {
        MyLand.text  = "+ " + land.ToString();
        Debug.Log("my land is"+ land.ToString());
        if (land == 0)
            MyLandObject.SetActive(false);
        else
            MyLandObject.SetActive(true);
    }
    public void SetEnemyLandInfo(int land)
    {
        EnemyLand.text  = "+ " + land.ToString();
        Debug.Log("enemy land is"+ land.ToString());
        if (land == 0)
            EnemyLandObject.SetActive(false);
        else
            EnemyLandObject.SetActive(true);
    }
    //全部的信息
    public void SetGameInfo(GameInfomation gameInfomation)
    {
        //****关于卡牌相关在另一边写了
        //-----------------------------------
        //pass
        SetPassInfo(gameInfomation);
        //各种点数
        SetPointInfo(gameInfomation);
        //------------------------------------
        //各种数量
        SetCountInfo(gameInfomation);
        //------------------------------------
        //名称
        SetNameInfo(gameInfomation);
        //-------------------------------------
        //皇冠图标
        SetWinCountInfo(gameInfomation);
        //-------------------------------------
        //调度
        SetMulliganInfo(gameInfomation);
        //-------------------------------------
    }
}
