using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

// File-based control for local test runs only. Never included in a player build.
[InitializeOnLoad]
public static class RewardSuiteEditorControl
{
    static readonly string Root=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../../../work/RewardFullSuite"));
    static double next;
    static RewardSuiteEditorControl(){Directory.CreateDirectory(Root);EditorApplication.update+=Poll;}
    static void Poll()
    {
        if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+1;
        var scene=SceneManager.GetActiveScene();
        File.WriteAllText(Path.Combine(Root,"editor-status.json"),JsonConvert.SerializeObject(new{
            utc=DateTime.UtcNow,project=Application.dataPath,playing=EditorApplication.isPlaying,
            compiling=EditorApplication.isCompiling,updating=EditorApplication.isUpdating,scene=scene.path,dirty=scene.isDirty}));
        var request=Path.Combine(Root,"editor-command.txt");
        if(!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating)return;
        string command=File.ReadAllText(request).Trim();File.Delete(request);
        try
        {
            switch(command)
            {
                case "refresh":AssetDatabase.Refresh();break;
                case "play":EditorApplication.isPlaying=true;break;
                case "stop":EditorApplication.isPlaying=false;break;
                default:throw new InvalidOperationException("Unsupported reward-test command: "+command);
            }
            File.WriteAllText(Path.Combine(Root,"editor-command-result.json"),JsonConvert.SerializeObject(new{command,accepted=true,utc=DateTime.UtcNow}));
        }
        catch(Exception e){File.WriteAllText(Path.Combine(Root,"editor-command-result.json"),JsonConvert.SerializeObject(new{command,accepted=false,error=e.ToString()}));}
    }
}
