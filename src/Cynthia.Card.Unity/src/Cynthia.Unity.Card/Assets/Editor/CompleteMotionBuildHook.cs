using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Assets.Script.DynamicCards.Editor;

[InitializeOnLoad]
public static class CompleteMotionBuildHook
{
    const string Work = "C:/UnityProjects/LegacyGwent/work/DynamicCards/CompleteMotionGoal-20260911/";
    static CompleteMotionBuildHook() { EditorApplication.update += Poll; }
    static void Poll()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(Work + "build-bundles.request")) return;
        File.Delete(Work + "build-bundles.request");
        File.WriteAllText(Work + "main-bundle-build.txt", "RUNNING " + DateTime.UtcNow.ToString("O"));
        try
        {
            var result = DynamicCardBuild.BuildBundle(BuildTarget.StandaloneWindows64);
            File.WriteAllText(Work + "main-bundle-build.txt", "COMPLETE " + DateTime.UtcNow.ToString("O") + "\n" + result);
        }
        catch (Exception e)
        {
            File.WriteAllText(Work + "main-bundle-build.txt", "FAILED " + DateTime.UtcNow.ToString("O") + "\n" + e);
            Debug.LogException(e);
        }
    }
}
