using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;

public class GameResultControl : MonoBehaviour
{
    //胜利?失败?
    public Text TitleResult;

    //显示敌我玩家名
    public Text MyName;
    public Text EnemyName;

    //三个回合的结果数字
    public Text Round1MyPoint;
    public Text Round1EnemyPoint;
    public Text Round2MyPoint;
    public Text Round2EnemyPoint;
    public Text Round3MyPoint;
    public Text Round3EnemyPoint;

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

    public void ShowGameResult(GameResultInfomation gameResult)
    {
        MyName.text = gameResult.MyName;
        EnemyName.text = gameResult.EnemyName;
        if (gameResult.RoundCount < 3)
            Round3.SetActive(false);
        if (gameResult.RoundCount < 2)
            Round2.SetActive(false);
        if (gameResult.RoundCount < 1)
            Round1.SetActive(false);
        if (gameResult.GameStatu == GameStatus.Win)
        {
            BackgroundMain.color = ConstInfo.WinBgColor;
            BackgroundLeft.sprite = GameWinBgLeft;
            BackgroundRight.sprite = GameWinBgRight;
            TitleResult.text = "胜利";
            TitleResult.color = ConstInfo.MyColor;
        }
        if (gameResult.GameStatu == GameStatus.Lose)
        {
            BackgroundMain.color = ConstInfo.LoseBgColor;
            BackgroundLeft.sprite = GameLoseBgLeft;
            BackgroundRight.sprite = GameLoseBgRight;
            TitleResult.text = "失败";
            TitleResult.color = ConstInfo.EnemyColor;
        }
        if (gameResult.GameStatu == GameStatus.Draw)
        {
            BackgroundMain.color = ConstInfo.DrawBgColor;
            BackgroundLeft.sprite = GameDrawBgLeft;
            BackgroundRight.sprite = GameDrawBgRight;
            TitleResult.text = "平局";
            TitleResult.color = ConstInfo.NormalColor;
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
                Round1MyPoint.color = ConstInfo.WinColor;
                Round1EnemyPoint.color = ConstInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR1Point < gameResult.EnemyR1Point)
            {
                Round1ResultIcon.sprite = RoundLoseIcon;
                Round1MyPoint.color = ConstInfo.NormalColor;
                Round1EnemyPoint.color = ConstInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR1Point == gameResult.EnemyR1Point)
            {
                Round1ResultIcon.sprite = RoundDrawIcon;
                Round1MyPoint.color = ConstInfo.NormalColor;
                Round1EnemyPoint.color = ConstInfo.NormalColor;
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
                Round2MyPoint.color = ConstInfo.WinColor;
                Round2EnemyPoint.color = ConstInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR2Point < gameResult.EnemyR2Point)
            {
                Round2ResultIcon.sprite = RoundLoseIcon;
                Round2MyPoint.color = ConstInfo.NormalColor;
                Round2EnemyPoint.color = ConstInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR2Point == gameResult.EnemyR2Point)
            {
                Round2ResultIcon.sprite = RoundDrawIcon;
                Round2MyPoint.color = ConstInfo.NormalColor;
                Round2EnemyPoint.color = ConstInfo.NormalColor;
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
                Round3MyPoint.color = ConstInfo.WinColor;
                Round3EnemyPoint.color = ConstInfo.NormalColor;
                myWinCount++;
            }
            if (gameResult.MyR3Point < gameResult.EnemyR3Point)
            {
                Round3ResultIcon.sprite = RoundLoseIcon;
                Round3MyPoint.color = ConstInfo.NormalColor;
                Round3EnemyPoint.color = ConstInfo.WinColor;
                enemyWinCount++;
            }
            if (gameResult.MyR3Point == gameResult.EnemyR3Point)
            {
                Round3ResultIcon.sprite = RoundDrawIcon;
                Round3MyPoint.color = ConstInfo.NormalColor;
                Round3EnemyPoint.color = ConstInfo.NormalColor;
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
        gameObject.SetActive(true);
    }
}
