using UnityEngine;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;
#endif

// A separate persistent object keeps the notification alive across the GamePlay scene load.
internal sealed class MatchTaskbarNotification : MonoBehaviour
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private static MatchTaskbarNotification instance;
    private IntPtr flashingWindow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.runInBackground = true;
        if (instance != null) return;
        var owner = new GameObject("MatchTaskbarNotification");
        DontDestroyOnLoad(owner);
        instance = owner.AddComponent<MatchTaskbarNotification>();
    }
#endif

    internal static void NotifyMatchFound()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        try
        {
            if (instance == null) Initialize();
            instance.StopFlashing();
            if (Application.isFocused || WindowsTaskbarFlash.IsPlayerForeground()) return;
            var window = WindowsTaskbarFlash.FindPlayerWindow();
            if (window == IntPtr.Zero) return;
            instance.flashingWindow = window;
            WindowsTaskbarFlash.Start(window);
        }
        catch (Exception exception)
        {
            // Optional desktop feedback must never prevent entry into the matched game.
            Debug.LogWarning("[LegacyGwent] Match taskbar notification unavailable: " + exception.GetType().Name);
        }
#endif
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private void OnApplicationFocus(bool focused)
    {
        if (focused) StopFlashing();
    }

    private void Update()
    {
        if (flashingWindow != IntPtr.Zero && WindowsTaskbarFlash.IsPlayerForeground()) StopFlashing();
    }

    private void OnDisable() { StopFlashing(); }
    private void OnApplicationQuit() { StopFlashing(); }

    private void StopFlashing()
    {
        if (flashingWindow == IntPtr.Zero) return;
        var window = flashingWindow;
        flashingWindow = IntPtr.Zero;
        WindowsTaskbarFlash.Stop(window);
    }
#endif
}

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
// Kept internal so native behavior can be exercised by a standalone verification harness.
internal static class WindowsTaskbarFlash
{
    private const uint FlashStop = 0;
    private const uint FlashTray = 2;
    private const uint FlashTimerNoForeground = 12;
    private const uint WindowOwner = 4;

    [StructLayout(LayoutKind.Sequential)]
    private struct FlashInfo
    {
        public uint Size;
        public IntPtr Window;
        public uint Flags;
        public uint Count;
        public uint Timeout;
    }

    private delegate bool EnumWindowsCallback(IntPtr window, IntPtr parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter);
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr window, uint command);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr window, StringBuilder className, int capacity);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FlashWindowEx(ref FlashInfo info);

    internal static bool IsPlayerForeground()
    {
        uint processId;
        GetWindowThreadProcessId(GetForegroundWindow(), out processId);
        return processId == GetCurrentProcessId();
    }

    internal static IntPtr FindPlayerWindow()
    {
        var result = IntPtr.Zero;
        EnumWindows((window, parameter) =>
        {
            if (!IsPlayerWindow(window) || !IsWindowVisible(window) || GetWindow(window, WindowOwner) != IntPtr.Zero)
                return true;
            result = window;
            return false;
        }, IntPtr.Zero);
        return result;
    }

    private static bool IsPlayerWindow(IntPtr window)
    {
        uint processId;
        GetWindowThreadProcessId(window, out processId);
        if (processId != GetCurrentProcessId()) return false;
        var className = new StringBuilder(64);
        GetClassName(window, className, className.Capacity);
        return className.ToString() == "UnityWndClass";
    }

    internal static void Start(IntPtr window)
    {
        if (!IsPlayerWindow(window) || IsPlayerForeground()) return;
        Flash(window, FlashTray | FlashTimerNoForeground);
    }

    internal static void Stop(IntPtr window)
    {
        // Handles may be destroyed/reused during a display-mode change; never flash another process.
        if (IsPlayerWindow(window)) Flash(window, FlashStop);
    }

    private static void Flash(IntPtr window, uint flags)
    {
        var info = new FlashInfo
        {
            Size = (uint)Marshal.SizeOf(typeof(FlashInfo)),
            Window = window,
            Flags = flags,
            Count = uint.MaxValue,
            Timeout = 0
        };
        // The return value describes the previous caption state, not success/failure.
        FlashWindowEx(ref info);
    }
}
#endif
