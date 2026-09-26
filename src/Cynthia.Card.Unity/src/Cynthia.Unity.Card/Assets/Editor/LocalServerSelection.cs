using UnityEditor;
using UnityEngine;

public static class LocalServerSelection
{
    private const string LocalMenu = "Tools/Legacy Gwent/Server/Local";
    private const string RemoteMenu = "Tools/Legacy Gwent/Server/Remote";
    private static string Key => "LegacyGwent.UseLocalServer." + Application.dataPath;

    [MenuItem(LocalMenu)]
    private static void Local() => Select(true);

    [MenuItem(RemoteMenu)]
    private static void Remote() => Select(false);

    private static void Select(bool local)
    {
        EditorPrefs.SetBool(Key, local);
        Debug.Log("[LegacyGwent] Server: " + (local ? "Local" : "Remote") + ". Restart Play mode to reconnect.");
    }

    [MenuItem(LocalMenu, true)]
    private static bool ValidateLocal()
    {
        Menu.SetChecked(LocalMenu, EditorPrefs.GetBool(Key, true));
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem(RemoteMenu, true)]
    private static bool ValidateRemote()
    {
        Menu.SetChecked(RemoteMenu, !EditorPrefs.GetBool(Key, true));
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
