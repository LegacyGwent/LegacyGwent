// Assets/Editor/BuildVersionWriter.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildVersionWriter : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string outputDir = Path.GetDirectoryName(report.summary.outputPath);
        string versionFile = Path.Combine(outputDir, "version.txt");
        File.WriteAllText(versionFile, "2.1.9");
        Debug.Log("BuildVersionWriter: version.txt written -> " + Application.version);
    }
}
