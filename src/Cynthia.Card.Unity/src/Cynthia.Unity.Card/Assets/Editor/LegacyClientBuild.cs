using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Script.DynamicCards.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

// GameCI and local batch builds use exactly the same variant/platform preparation.
public static class LegacyClientBuild
{
    public static void Build()
    {
        var args = Arguments();
        string variant = Required(args, "clientVariant");
        if (variant != "standard" && variant != "premium") throw new BuildFailedException("Invalid clientVariant");
        var target = (BuildTarget)Enum.Parse(typeof(BuildTarget), Required(args, "buildTarget"));
        if (EditorUserBuildSettings.activeBuildTarget != target)
            throw new BuildFailedException("Launch Unity with the matching -buildTarget before running this method.");
        string old = Environment.GetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS");
        try
        {
            Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS", variant == "premium" ? "1" : "0");
            PlayerSettings.bundleVersion = Required(args, "buildVersion");
            DynamicCardBuild.ConfigurePlatform(target);
            if (target == BuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = int.Parse(Required(args, "androidVersionCode"));
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = Required(args, "androidKeystoreName");
                PlayerSettings.Android.keystorePass = Required(args, "androidKeystorePass");
                PlayerSettings.Android.keyaliasName = Required(args, "androidKeyaliasName");
                PlayerSettings.Android.keyaliasPass = Required(args, "androidKeyaliasPass");
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                EditorUserBuildSettings.buildAppBundle = false;
            }
            DynamicCardBuild.PrepareForBuild();
            AddressablesPlayerBuildResult addressables;
            AddressableAssetSettings.BuildPlayerContent(out addressables);
            if (!string.IsNullOrEmpty(addressables.Error)) throw new BuildFailedException("Addressables build failed: " + addressables.Error);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                target = target, locationPathName = Required(args, "customBuildPath"), options = BuildOptions.None,
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray()
            });
            if (report.summary.result != BuildResult.Succeeded) throw new BuildFailedException("Player build failed: " + report.summary.result);
        }
        finally
        {
            DynamicCardBuild.RestoreStage();
            Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS", old);
            // Never persist signing passwords in editor state beyond the build.
            PlayerSettings.Android.keystorePass = ""; PlayerSettings.Android.keyaliasPass = "";
        }
    }

    private static Dictionary<string, string> Arguments()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i + 1 < args.Length; i++)
            if (args[i].StartsWith("-") && !args[i + 1].StartsWith("-")) values[args[i].TrimStart('-')] = args[++i];
        return values;
    }

    private static string Required(Dictionary<string, string> args, string key)
    {
        string value;
        if (!args.TryGetValue(key, out value) || string.IsNullOrEmpty(value))
            throw new BuildFailedException("Missing build parameter: " + key);
        return value;
    }
}
