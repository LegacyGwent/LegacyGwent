// Assets/Editor/BuildVersionWriter.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Text.RegularExpressions;

public class BuildVersionWriter : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string clientVersion = PlayerSettings.bundleVersion;
        if (!Regex.IsMatch(clientVersion ?? string.Empty, @"^\d+\.\d+\.\d+$"))
        {
            throw new BuildFailedException(
                $"Client bundle version must use major.minor.patch, but was '{clientVersion}'.");
        }

        string outputDir = Path.GetDirectoryName(report.summary.outputPath);
        string versionFile = Path.Combine(outputDir, "version.txt");
        File.WriteAllText(versionFile, clientVersion);
        Debug.Log("BuildVersionWriter: version.txt written -> " + clientVersion);
    }
}
