using System.Collections;
using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;

public class GameResultControl : MonoBehaviour
{
    //胜利?失败?
    [SerializeField] GameObject popup_prefab;
    public Text TitleResult;

    //显示敌我玩家名
    public Text MyName;
    public Text EnemyName;
    public string enemyname;
    public string myname;
    private float timer=4;
    private float interval=6;
    //三个回合的结果数字
    public Text Round1MyPoint;
    public Text Round1EnemyPoint;
    public Text Round2MyPoint;
    public Text Round2EnemyPoint;
    public Text Round3MyPoint;
    public Text Round3EnemyPoint;
    public Text MMRChangeText;

    //三个回合的结果图标
    public Image Round1ResultIcon;
    public Image Round2ResultIcon;
    public Image Round3ResultIcon;

    //三个回合的结果是否显示
    public GameObject Round1;
    public GameObject Round2;
    public GameObject Round3;

    //胜利图标
    public GameObject MyWinIconLeft;
    public GameObject MyWinIconRight;
    public GameObject EnemyWinIconLeft;
    public GameObject EnemyWinIconRight;
    // public GameObject GGObject;
    public GameObject GGButton;
    //背景相关,主背景色,左背景,右背景
    public Image BackgroundMain;
    public Image BackgroundLeft;
    public Image BackgroundRight;
    //----------------------------------------------------------以上为控制控件
    //----------------------------------------------------------以下为需求素材
    //回合点数分割线图标
    public Sprite RoundWinIcon;
    public Sprite RoundLoseIcon;
    public Sprite RoundDrawIcon;
    //输,赢,平背景左右
    public Sprite GameWinBgLeft;
    public Sprite GameWinBgRight;
    public Sprite GameLoseBgLeft;
    public Sprite GameLoseBgRight;
    public Sprite GameDrawBgLeft;
    public Sprite GameDrawBgRight;

    private LocalizationService _translator => DependencyResolver.Container.Resolve<LocalizationService>();
    public void ShowMMRResult(int oldMMR, int newMMR)
    {
        MMRChangeText.text = $"MMR: {oldMMR} → {newMMR}";
    }
    public void ShowGameResult(GameResultInfomation gameResult)
    {
        MyName.text = gameResult.MyName;
        EnemyName.text = gameResult.EnemyName;
        enemyname = gameResult.EnemyName;
        myname = gameResult.MyName;
        if (gameResult.RoundCount < 3)
            Round3.SetActive(false);
        if (gameResult.RoundCount < 2)
            Round2.SetActive(false);
        if (gameResult.RoundCount < 1)
            Round1.SetActive(false);
        if (gameResult.GameStatu == GameStatus.Win)
        {
            BackgroundMain.color = ClientGlobalInfo.WinBgColor;
            BackgroundLeft.sprite = GameWinBgLeft;
            BackgroundRight.sprite = GameWinBgRight;
            TitleResult.text = _translator.GetText("IngameMenu_VictoryTitle");
            TitleResult.color = ClientGlobalInfo.MyColor;
        }
        if (gameResult.GameStatu == GameStatus.Lose)
        {
            BackgroundMain.color = ClientGlobalInfo.LoseBgColor;
            BackgroundLeft.sprite = GameLoseBgLeft;
            BackgroundRight.sprite = GameLoseBgRight;
            TitleResult.text = _translator.GetText("IngameMenu_DefeatTitle");
            TitleResult.color = ClientGlobalInfo.EnemyColor;
        }
        if (gameResult.GameStatu == GameStatus.Draw)
        {
            BackgroundMain.color = ClientGlobalInfo.DrawBgColor;
            BackgroundLeft.sprite = GameDrawBgLeft;
            BackgroundRight.sprite = GameDrawBgRight;
            TitleResult.text = _translator.GetText("IngameMenu_DrawTitle");
            TitleResult.color = ClientGlobalInfo.NormalColor;
        }
        var myWinCount = 0;
        var enemyWinCount = 0;
        if (gameResult.RoundCount >= 1)
        {
            Round1MyPoint.text = gameResult.MyR1Point.ToString();
            Round1EnemyPoint.text = gameResult.EnemyR1Point.ToString();
            if (gameResult.MyR1Point > gameResult.EnemyR1Point)
            {
                Round1ResultIcon.sprite = RoundWinIcon;
                Round1MyPoint.color = ClientGlobalInfo.WinColor;
                Round1EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR1Point < gameResult.EnemyR1Point)
            {
                Round1ResultIcon.sprite = RoundLoseIcon;
                Round1MyPoint.color = ClientGlobalInfo.NormalColor;
                Round1EnemyPoint.color = ClientGlobalInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR1Point == gameResult.EnemyR1Point)
            {
                Round1ResultIcon.sprite = RoundDrawIcon;
                Round1MyPoint.color = ClientGlobalInfo.NormalColor;
                Round1EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
                enemyWinCount++;
            }
        }
        if (gameResult.RoundCount >= 2)
        {
            Round2MyPoint.text = gameResult.MyR2Point.ToString();
            Round2EnemyPoint.text = gameResult.EnemyR2Point.ToString();
            if (gameResult.MyR2Point > gameResult.EnemyR2Point)
            {
                Round2ResultIcon.sprite = RoundWinIcon;
                Round2MyPoint.color = ClientGlobalInfo.WinColor;
                Round2EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR2Point < gameResult.EnemyR2Point)
            {
                Round2ResultIcon.sprite = RoundLoseIcon;
                Round2MyPoint.color = ClientGlobalInfo.NormalColor;
                Round2EnemyPoint.color = ClientGlobalInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR2Point == gameResult.EnemyR2Point)
            {
                Round2ResultIcon.sprite = RoundDrawIcon;
                Round2MyPoint.color = ClientGlobalInfo.NormalColor;
                Round2EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
                enemyWinCount++;
            }
        }
        if (gameResult.RoundCount >= 3)
        {
            Round3MyPoint.text = gameResult.MyR3Point.ToString();
            Round3EnemyPoint.text = gameResult.EnemyR3Point.ToString();
            if (gameResult.MyR3Point > gameResult.EnemyR3Point)
            {
                Round3ResultIcon.sprite = RoundWinIcon;
                Round3MyPoint.color = ClientGlobalInfo.WinColor;
                Round3EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR3Point < gameResult.EnemyR3Point)
            {
                Round3ResultIcon.sprite = RoundLoseIcon;
                Round3MyPoint.color = ClientGlobalInfo.NormalColor;
                Round3EnemyPoint.color = ClientGlobalInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR3Point == gameResult.EnemyR3Point)
            {
                Round3ResultIcon.sprite = RoundDrawIcon;
                Round3MyPoint.color = ClientGlobalInfo.NormalColor;
                Round3EnemyPoint.color = ClientGlobalInfo.NormalColor;
                myWinCount++;
                enemyWinCount++;
            }
        }
        if (myWinCount != 0)
        {
            MyWinIconLeft.SetActive(true);
            if (myWinCount >= 2)
                MyWinIconRight.SetActive(true);
        }
        if (enemyWinCount != 0)
        {
            EnemyWinIconLeft.SetActive(true);
            if (enemyWinCount >= 2)
                EnemyWinIconRight.SetActive(true);
        }
        // UpdateGGBool();
        gameObject.SetActive(true);

    }
    public async void SendGG()
    {
        await DependencyResolver.Container.Resolve<GwentClientService>().SendGG(myname, enemyname);
        GGButton.SetActive(false);
    }
    void Update()
    {

        if (timer<interval)
        {
            timer=timer+Time.deltaTime;
        }
        else
        {
            DisplayGGObject();
        }
    }
    private async void DisplayGGObject()
    {
        string sender = await DependencyResolver.Container.Resolve<GwentGGService>().DisplayGG();
        GGSender.ggsender = sender;
        bool isdisplaygg = sender.Length >=1;
        if (isdisplaygg)
        {
            AudioManager.Instance.PlayAudio("GG", AudioType.Effect, AudioPlayMode.PlayOneShoot);
            GameObject popupobject = Instantiate(popup_prefab);
        }
    }
}
