using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ClientGlobalInfo
{
    public static bool IsToMatch = false;
    public static bool IsPreviousRankMatch = false;

    public static bool IsLoadGlobal = false;

    public static string ViewingRoomId { get; set; } = "";

    public static string DefaultDeckId
    {
        get => _defaultDeckId;
        set
        {
            _defaultDeckId = value;
            PlayerPrefs.SetString("DefaultDeckId", value);
        }
    }
    public static readonly Version Version = new Version(0, 1, 0, 2);
    private static string _defaultDeckId = PlayerPrefs.GetString("DefaultDeckId", "");
    public static readonly Color WinColor = new Color(255f / 255f, 220f / 255f, 0f / 255f, 255f / 255f);//黄色胜利文字
    public static readonly Color EnemyColor = new Color(212f / 255f, 0f / 255f, 4f / 255f, 255f / 255f);//敌方文字 红
    public static readonly Color MyColor = new Color(0f / 255f, 143f / 255f, 203f / 255f, 255f / 255f);//我方文字 蓝
    public static readonly Color NormalColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);//白色

    public static readonly Color ErrorColor = new Color(224 / 255f, 90 / 255f, 90 / 255f, 255 / 255f);//卡组错误

    public static readonly Color LoseBgColor = new Color(24f / 255f, 10f / 255f, 10f / 255f, 240f / 255f);
    public static readonly Color WinBgColor = new Color(10f / 255f, 10f / 255f, 24f / 255f, 240f / 255f);
    public static readonly Color DrawBgColor = new Color(24f / 255f, 24f / 255f, 24f / 255f, 240f / 255f);

    public static readonly Color GreenColor = new Color(50f / 255f, 255f / 255f, 50f / 255f, 255f / 255f);//增益文字
    public static readonly Color RedColor = new Color(255f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);//减益文字

    public static readonly Color BlueColor = new Color(50f / 255f, 120f / 255f, 255f / 255f, 255f / 255f);//倒计时

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

    [DllImport("User32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    public static void OpenWindow(string lpClassName, string lpWindowName)
    {
        IntPtr hwnd = FindWindow(lpClassName, lpWindowName);

        if (hwnd == IntPtr.Zero)
        {
            return;
        }

        IntPtr activeWndHwnd = GetForegroundWindow();

        if (hwnd != activeWndHwnd)
        {
            ShowWindowAsync(hwnd, 1);
            SetForegroundWindow(hwnd);
        }
    }
#endif
}
