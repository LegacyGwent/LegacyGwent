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
using UnityEngine.AddressableAssets;

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
    public Text MyTitle;
    public Text EnemyTitle;
    //-----------------------------------
    public GameObject MyCrownLeft;//
    public GameObject MyCrownRight;//
    public GameObject EnemyCrownLeft;//
    public GameObject EnemyCrownRight;//
    public GameObject MyLandObject;
    public GameObject EnemyLandObject;
    //-----------------------------------
    public Image MyAvatar;
    public Image EnemyAvatar;
    public Image MyBorder;
    public Image EnemyBorder;
    public string Myavatar;
    public string Enemyname;
    //----------------------------------
    public GameObject MyPass;
    public GameObject EnemyPass;
    public Text MyShowMessage;//
    public Text EnemyShowMessage;//
    public RopeController ropeController;

    private LocalizationService _translator;
    private GwentClientService server;
    private int myoldland;
    private int enemyoldland;
    //----------------------------------    
    public GameObject MyCrossIcon;
    public GameObject MyGraveyardIcon;
    public GameObject EnemyCrossIcon;
    public GameObject EnemyGraveyardIcon;
    public GameObject MyCardsIcon;
    public GameObject MyDeckIcon;
    public GameObject EnemyCardsIcon;
    public GameObject EnemyDeckIcon;


    //---------------------------------- 
    private int myAllPoint;
    private int enemyAllPoint;
    //----------------------------------   
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); } // lists all title cosmetics
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; } // stores the color of the title cosmetics

    private void Awake()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
    }

    public void SetPointInfo(GameInfomation gameInfomation, int myland = 0, int enemyland = 0)
    {
        MyAllPoint.color = ClientGlobalInfo.NormalColor;
        EnemyAllPoint.color = ClientGlobalInfo.NormalColor;
        MyRow1Point.text = gameInfomation.MyRow1Point.ToString();
        MyRow2Point.text = gameInfomation.MyRow2Point.ToString();
        MyRow3Point.text = gameInfomation.MyRow3Point.ToString();
        EnemyRow1Point.text = gameInfomation.EnemyRow1Point.ToString();
        EnemyRow2Point.text = gameInfomation.EnemyRow2Point.ToString();
        EnemyRow3Point.text = gameInfomation.EnemyRow3Point.ToString();
        myAllPoint = (gameInfomation.MyRow1Point + gameInfomation.MyRow2Point + gameInfomation.MyRow3Point);
        enemyAllPoint = (gameInfomation.EnemyRow1Point + gameInfomation.EnemyRow2Point + gameInfomation.EnemyRow3Point);
        MyAllPoint.text = myAllPoint.ToString();
        EnemyAllPoint.text = enemyAllPoint.ToString();
        if (myAllPoint > enemyAllPoint)
            MyAllPoint.color = ClientGlobalInfo.WinColor;
        else if (myAllPoint < enemyAllPoint)
            EnemyAllPoint.color = ClientGlobalInfo.WinColor;

        //if (myland == 0)
        //    MyLandObject.SetActive(false);
        //if (enemyland == 0)
        //    EnemyLandObject.SetActive(false);

        SetMyLand(myland);
        SetEnemyLand(enemyland);
            
    }
    public void SetCountInfo(GameInfomation gameInfomation)
    {
        MyHandCount.text = gameInfomation.MyHandCount.ToString();
        EnemyHandCount.text = gameInfomation.EnemyHandCount.ToString();


        int myDeck = gameInfomation.MyDeckCount;
        if (myDeck == 0)
        {
            MyDeckCount.gameObject.SetActive(false);
            MyCardsIcon.SetActive(false);
            MyDeckIcon.SetActive(false);
        }
        else
        {
            MyDeckCount.gameObject.SetActive(true);
            MyDeckCount.text = myDeck.ToString();
            MyCardsIcon.SetActive(true);
            MyDeckIcon.SetActive(true);
        }


        int EnemyDeck = gameInfomation.EnemyDeckCount;
        if (EnemyDeck == 0)
        {
            EnemyDeckCount.gameObject.SetActive(false);
            EnemyCardsIcon.SetActive(false);
            EnemyDeckIcon.SetActive(false);
        }
        else
        {
            EnemyDeckCount.gameObject.SetActive(true);
            EnemyDeckCount.text = EnemyDeck.ToString();
            EnemyCardsIcon.SetActive(true);
            EnemyDeckIcon.SetActive(true);
        }

        int myCemetery = gameInfomation.MyCemeteryCount;
        if (myCemetery == 0)
        {
            MyCemeteryCount.gameObject.SetActive(false);
            MyCrossIcon.SetActive(false);
            MyGraveyardIcon.SetActive(false);
        }
        else
        {
            MyCemeteryCount.text = myCemetery.ToString();
            MyCemeteryCount.gameObject.SetActive(true);
            MyCrossIcon.SetActive(true);
            MyGraveyardIcon.SetActive(true);
        }

        int enemyCemetery = gameInfomation.EnemyCemeteryCount;
        if (enemyCemetery == 0)
        {
            EnemyCemeteryCount.gameObject.SetActive(false);
            EnemyCrossIcon.SetActive(false);
            EnemyGraveyardIcon.SetActive(false);
        }
        else
        {
            EnemyCemeteryCount.text = enemyCemetery.ToString();
            EnemyCemeteryCount.gameObject.SetActive(true);
            EnemyCrossIcon.SetActive(true);
            EnemyGraveyardIcon.SetActive(true);
        }

        //MyDeckCount.text = gameInfomation.MyDeckCount.ToString();
        //EnemyDeckCount.text = gameInfomation.EnemyDeckCount.ToString();
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
    public async void SetNameInfo(GameInfomation gameInfomation)
    {
        var Enemy = gameInfomation.EnemyName;
        List<string> botnames = new List<string> { "ai0_name", "ai1_name", "ai2_name", "ai3_name", "ai4_name", "ai5_name"};
        if (botnames.Contains(Enemy))
        {
            EnemyName.text = _translator.GetText(Enemy);
        }
        else
        {
            EnemyName.text = Enemy;
        }
        MyName.text = gameInfomation.MyName;
        MyTitle.text = _translator.GetText(gameInfomation.MyTitle + "Name");
        Myavatar = gameInfomation.MyAvatar;
        Enemyname = gameInfomation.EnemyName;
        EnemyTitle.text = _translator.GetText(gameInfomation.EnemyTitle + "Name");
        string mycolor = _titles.Where(x => x.ID == gameInfomation.MyTitle).Single().TitleColor;
        MyTitle.color= mycolormap[mycolor];
        string enemycolor = _titles.Where(x => x.ID == gameInfomation.EnemyTitle).Single().TitleColor;
        EnemyTitle.color= mycolormap[enemycolor];
        var op = Addressables.LoadAssetAsync<Sprite>(gameInfomation.MyAvatar);
        Sprite go = op.WaitForCompletion();
        MyAvatar.sprite = go;
        op = Addressables.LoadAssetAsync<Sprite>(gameInfomation.EnemyAvatar);
        go = op.WaitForCompletion();
        Debug.Log(gameInfomation.EnemyAvatar);
        EnemyAvatar.sprite = go;
        op = Addressables.LoadAssetAsync<Sprite>(gameInfomation.MyBorder);
        go = op.WaitForCompletion();
        MyBorder.sprite = go;
        op = Addressables.LoadAssetAsync<Sprite>(gameInfomation.EnemyBorder);
        go = op.WaitForCompletion();
        EnemyBorder.sprite = go;

    }
    public void SetMMRInfo(int myMMR, int enemyMMR)
    {
        MyMMR.text = Convert.ToString(myMMR);
        EnemyMMR.text = Convert.ToString(enemyMMR);
    }
    public void SetMyLand(int land)
    {
        string mode = SettingPanel.GetCoinDisplayMode(); //move to awake later
        Debug.Log("Coin mode currently is: " + mode);

        if (mode == "CoinAdded")
        {
            MyLand.text  =  (land+myAllPoint).ToString();
        }
        else
        {
            MyLand.text  = "+ " + land.ToString();
        }

        Debug.Log("my land is"+ land.ToString());

        if (land == 0)
            MyLandObject.SetActive(false);
        else
            MyLandObject.SetActive(true);
    }
    public void SetEnemyLand(int land)
    {
        string mode = SettingPanel.GetCoinDisplayMode(); //move to awake later
        Debug.Log("Coin mode currently is: " + mode);

        if (mode == "CoinAdded")
        {
            EnemyLand.text  = (land+enemyAllPoint).ToString();
        }
        else
        {
            EnemyLand.text  = "+ " + land.ToString();
        }

        Debug.Log("enemy land is "+ land.ToString());

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
        //Set the name, avatar, border and title of both players
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
