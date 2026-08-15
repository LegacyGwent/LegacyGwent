using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Command-line player build entry points that always package Addressables first.
/// Invoke with -executeMethod DiyAiPlayerBuilder.BuildWindows64 and optionally
/// -aitestOutputPath &lt;absolute player path&gt;. After a proven Addressables build,
/// BuildWindows64PlayerOnly can validate an incremental code/UI-only change.
/// </summary>
public static class DiyAiPlayerBuilder
{
    private const string OutputArgument = "-aitestOutputPath";

    public static void BuildWindows64()
    {
        BuildWindows64Core(true);
    }

    public static void BuildWindows64PlayerOnly()
    {
        BuildWindows64Core(false);
    }

    private static void BuildWindows64Core(bool rebuildAddressables)
    {
        if (rebuildAddressables)
            BuildAddressables();
        else
            VerifyExistingAddressables();

        var outputPath = ReadArgument(OutputArgument);
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            outputPath = Path.GetFullPath(
                Path.Combine(Application.dataPath, "../../../.tmp/builds/AITest-Windows/DiyGwent-AITest.exe"));
        }

        var outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrWhiteSpace(outputDirectory))
            throw new InvalidOperationException($"Invalid build output path: {outputPath}");

        Directory.CreateDirectory(outputDirectory);
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        if (scenes.Length == 0)
            throw new InvalidOperationException("No enabled scenes were found in EditorBuildSettings.");

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        });

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Windows player build failed: {report.summary.result}, " +
                $"errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}");
        }

        Debug.Log(
            $"DiyAiPlayerBuilder: Windows build succeeded at {outputPath}; " +
            $"size={report.summary.totalSize} bytes.");
    }

    private static void VerifyExistingAddressables()
    {
        var settingsPath = Path.Combine(
            Path.GetDirectoryName(Application.dataPath),
            "Library", "com.unity.addressables", "aa", "Windows", "settings.json");
        if (!File.Exists(settingsPath))
            throw new InvalidOperationException(
                $"Player-only build requires existing Addressables runtime data: {settingsPath}");
        Debug.Log($"DiyAiPlayerBuilder: reusing existing Addressables from {settingsPath}");
    }

    private static void BuildAddressables()
    {
        AddressableAssetSettings.BuildPlayerContent(out var result);
        if (!string.IsNullOrWhiteSpace(result.Error))
            throw new InvalidOperationException($"Addressables build failed: {result.Error}");

        Debug.Log($"DiyAiPlayerBuilder: Addressables build succeeded; output={result.OutputPath}");
    }

    private static string ReadArgument(string name)
    {
        var args = Environment.GetCommandLineArgs();
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        }

        return null;
    }
}
