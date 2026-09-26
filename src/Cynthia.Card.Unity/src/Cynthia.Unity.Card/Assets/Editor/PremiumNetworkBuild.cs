using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Local two-process test build. Does not change the release output or build preferences.
[InitializeOnLoad]
public static class PremiumNetworkBuild
{
    const string Work="C:/UnityProjects/LegacyGwent/work/PremiumNetwork/";
    static PremiumNetworkBuild(){EditorApplication.update+=Poll;}
    static void Poll()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(Work+"build.request"))return;
        File.Delete(Work+"build.request");File.WriteAllText(Work+"build-result.txt","RUNNING");
        string previous=Environment.GetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS");
        try
        {
            Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS","1");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{
                scenes=EditorBuildSettings.scenes.Where(x=>x.enabled).Select(x=>x.path).ToArray(),
                locationPathName=Work+"Client/PremiumNetwork.exe",target=BuildTarget.StandaloneWindows64,
                options=BuildOptions.Development | (File.Exists(Work+"Client/PremiumNetwork.exe")?BuildOptions.BuildScriptsOnly:BuildOptions.None)});
            File.WriteAllText(Work+"build-result.txt",report.summary.result+" errors="+report.summary.totalErrors+" bytes="+report.summary.totalSize);
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Network test player build failed");
        }
        catch(Exception e){File.WriteAllText(Work+"build-result.txt",e.ToString());Debug.LogException(e);}
        finally{Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS",previous);}
    }
}
