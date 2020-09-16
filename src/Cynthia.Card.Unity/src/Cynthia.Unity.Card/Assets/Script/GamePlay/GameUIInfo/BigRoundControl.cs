using System.Collections;
using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;

public class BigRoundControl : MonoBehaviour
{
    public Text Title;
    public Text Message;
    public Text MyPoint;
    public Text EnemyPoint;
    public Image TitleBg;
    public Image BigRoundBg;
    public GameObject BigRound;
    public GameObject MessageShow;
    public GameObject PointShow;
    public GameObject MyWinCountLeft;
    public GameObject MyWinCountRight;
    public GameObject EnemyWinCountLeft;
    public GameObject EnemyWinCountRight;

    private LocalizationService _translator => DependencyResolver.Container.Resolve<LocalizationService>();

    public void Test()
    {
        //ShowPoint(false, "输掉啦!", 1, 99999, 0, 2);
    }
    public void ShowPoint(BigRoundInfomation data)
    {
        if (data.GameStatus == GameStatus.Win)
        {
            BigRoundBg.color = new Color32(10, 10, 24, 220);
            TitleBg.color = new Color32(0, 130, 255, 255);
            Title.text = _translator.GetText("IngameMenu_RoundWonTitle");

            var roundCount = data.EnemyWinCount + data.MyWinCount;
            Message.text = _translator.GetText(roundCount == 1 ? "IngameMenu_SecondRoundTitle" : "IngameMenu_LastRoundTitle");
        }
        else if (data.GameStatus == GameStatus.Lose)
        {
            BigRoundBg.color = new Color32(24, 10, 10, 220);
            TitleBg.color = new Color32(255, 0, 0, 255);
            Title.text = _translator.GetText("IngameMenu_RoundLostTitle");

            var roundCount = data.EnemyWinCount + data.MyWinCount;
            Message.text = _translator.GetText(roundCount == 1 ? "IngameMenu_SecondRoundTitle" : "IngameMenu_LastRoundTitle");
        }
        else
        {
            BigRoundBg.color = new Color32(10, 10, 10, 220);
            TitleBg.color = new Color32(200, 130, 80, 255);
            Title.text = _translator.GetText("IngameMenu_RoundDrawTitle");
            Message.text = _translator.GetText("IngameMenu_LastRoundTitle");
        }

        SetPoint(data);
        BigRound.SetActive(true);
    }

    public void SetPoint(BigRoundInfomation data)
    {
        MyPoint.text = data.MyPoint.ToString();
        EnemyPoint.text = data.EnemyPoint.ToString();
        MyPoint.color = ClientGlobalInfo.NormalColor;
        EnemyPoint.color = ClientGlobalInfo.NormalColor;
        if (data.MyPoint > data.EnemyPoint)
        {
            MyPoint.color = ClientGlobalInfo.WinColor;
        }
        else if (data.EnemyPoint > data.MyPoint)
        {
            EnemyPoint.color = ClientGlobalInfo.WinColor;
        }
        MyWinCountLeft.SetActive(false);
        MyWinCountRight.SetActive(false);
        EnemyWinCountLeft.SetActive(false);
        EnemyWinCountRight.SetActive(false);
        if (data.MyWinCount > 0)
            MyWinCountLeft.SetActive(true);
        if (data.MyWinCount > 1)
            MyWinCountRight.SetActive(true);
        if (data.EnemyWinCount > 0)
            EnemyWinCountLeft.SetActive(true);
        if (data.EnemyWinCount > 1)
            EnemyWinCountRight.SetActive(true);
        PointShow.SetActive(true);
        MessageShow.SetActive(false);
    }
    public void CloseBigRound()
    {
        BigRound.SetActive(false);
    }

    public void DisplayMessage()
    {
        PointShow.SetActive(false);
        MessageShow.SetActive(true);
    }
}
