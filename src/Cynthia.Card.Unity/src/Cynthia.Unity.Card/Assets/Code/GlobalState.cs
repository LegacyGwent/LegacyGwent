using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalState
{
    public static bool IsToMatch = false;

    public static bool IsLoadGlobal = false;
    public static int DefaultDeckIndex
    {
        get => _defaultDeckIndex;
        set
        {
            _defaultDeckIndex = value;
            PlayerPrefs.SetInt("DefaultDeckIndex", value);
        }
    }
    private static int _defaultDeckIndex = 0;
    public static readonly Color WinColor = new Color(255f / 255f, 220f / 255f, 0f / 255f, 255f / 255f);//黄色胜利文字
    public static readonly Color EnemyColor = new Color(212f / 255f, 0f / 255f, 4f / 255f, 255f / 255f);//敌方文字 红
    public static readonly Color MyColor = new Color(0f / 255f, 143f / 255f, 203f / 255f, 255f / 255f);//我方文字 蓝
    public static readonly Color NormalColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);//白色

    public static readonly Color LoseBgColor = new Color(24f / 255f, 10f / 255f, 10f / 255f, 240f / 255f);
    public static readonly Color WinBgColor = new Color(10f / 255f, 10f / 255f, 24f / 255f, 240f / 255f);
    public static readonly Color DrawBgColor = new Color(24f / 255f, 24f / 255f, 24f / 255f, 240f / 255f);

    public static readonly Color GreenColor = new Color(50f / 255f, 255f / 255f, 50f / 255f, 255f / 255f);//增益文字
    public static readonly Color RedColor = new Color(255f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);//减益文字

    public static readonly Color BlueColor = new Color(50f / 255f, 120f / 255f, 255f / 255f, 255f / 255f);//倒计时
}