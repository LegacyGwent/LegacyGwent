using System.Collections;
using System.Collections.Generic;
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

    public void Test()
    {
        //ShowPoint(false, "输掉啦!", 1, 99999, 0, 2);
    }
    public void ShowMessage(BigRoundInfomation data)
    {
        //bool isWin,string title, string message
        BigRound.SetActive(true);
        Title.text = data.Title;
        if (data.GameStatus == GameStatus.Win)
        {
            BigRoundBg.color = new Color32(10, 10, 24, 220);
            TitleBg.color = new Color32(0, 130, 255, 255);
        }
        else if (data.GameStatus == GameStatus.Lose)
        {
            BigRoundBg.color = new Color32(24, 10, 10, 220);
            TitleBg.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            BigRoundBg.color = new Color32(10, 10, 10, 220);
            TitleBg.color = new Color32(200, 130, 80, 255);
        }
        SetMessage(data.Message);
        BigRound.SetActive(true);
    }
    public void ShowPoint(BigRoundInfomation data)
    {//bool isWin,string title, int myPoint, int enemyPoint, int myWinCount, int enemyWinCount
        BigRound.SetActive(true);
        Title.text = data.Title;
        if (data.GameStatus == GameStatus.Win)
        {
            BigRoundBg.color = new Color32(10, 10, 24, 220);
            TitleBg.color = new Color32(0, 130, 255, 255);
        }
        else if (data.GameStatus == GameStatus.Lose)
        {
            BigRoundBg.color = new Color32(24, 10, 10, 220);
            TitleBg.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            BigRoundBg.color = new Color32(10, 10, 10, 220);
            TitleBg.color = new Color32(200, 130, 80, 255);
        }
        SetPoint(data);
        BigRound.SetActive(true);
    }
    public void SetMessage(string message)
    {
        Message.text = message;
        PointShow.SetActive(false);
        MessageShow.SetActive(true);
    }
    public void SetPoint(BigRoundInfomation data)
    {
        MyPoint.text = data.MyPoint.ToString();
        EnemyPoint.text = data.EnemyPoint.ToString();
        MyPoint.color = GlobalState.NormalColor;
        EnemyPoint.color = GlobalState.NormalColor;
        if (data.MyPoint > data.EnemyPoint)
        {
            MyPoint.color = GlobalState.WinColor;
        }
        else if (data.EnemyPoint > data.MyPoint)
        {
            EnemyPoint.color = GlobalState.WinColor;
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
}
