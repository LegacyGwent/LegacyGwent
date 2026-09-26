using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class RotationRepairDelivery
{
    const string Work="C:/UnityProjects/LegacyGwent/work/DynamicCards/LandsknechtAcceptance-20260912/";
    [Serializable] class Request {public string action;}
    [Serializable] class State {public string utc,project,scene;public bool playing,willChangePlayMode,compiling,updating,sceneDirty;}
    static double lastState;
    static bool busy;
    static RotationRepairDelivery(){EditorApplication.update+=Poll;}
    static void Poll()
    {
        if(busy)return;
        if(EditorApplication.timeSinceStartup-lastState>5)
        {
            lastState=EditorApplication.timeSinceStartup;
            var scene=SceneManager.GetActiveScene();
            File.WriteAllText(Work+"main-editor-state.json",JsonUtility.ToJson(new State{utc=DateTime.UtcNow.ToString("O"),project=Path.GetFullPath("."),scene=scene.path,
                playing=EditorApplication.isPlaying,willChangePlayMode=EditorApplication.isPlayingOrWillChangePlaymode,
                compiling=EditorApplication.isCompiling,updating=EditorApplication.isUpdating,sceneDirty=scene.isDirty},true));
        }
        string requestPath=Work+"delivery.request.json";
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(requestPath))return;
        var request=JsonUtility.FromJson<Request>(File.ReadAllText(requestPath));
        if(request.action=="merge" && EditorApplication.isPlayingOrWillChangePlaymode)return;
        File.Delete(requestPath);busy=true;
        try
        {
            if(request.action=="merge")throw new InvalidOperationException("Use merge_verified_rotation_repairs.py while Unity is closed; this avoids blocking the editor with managed SHA256 checks.");
            else if(request.action=="play")EditorApplication.isPlaying=true;
            else if(request.action=="stop")EditorApplication.isPlaying=false;
            else if(request.action=="build")
            {
                File.WriteAllText(Work+"post-rotation-bundle-build.txt","RUNNING "+DateTime.UtcNow.ToString("O"));
                var bundle=Assets.Script.DynamicCards.Editor.DynamicCardBuild.BuildBundle(BuildTarget.StandaloneWindows64);
                File.WriteAllText(Work+"post-rotation-bundle-build.txt","COMPLETE "+DateTime.UtcNow.ToString("O")+"\n"+bundle);
            }
            else if(request.action=="loginLocalSaved" || request.action=="loginLocalFixture")
            {
                if(!EditorApplication.isPlaying || !EditorPrefs.GetBool("LegacyGwent.UseLocalServer."+Application.dataPath,true))throw new InvalidOperationException("Local test login requires Play mode and the local server");
                var login=UnityEngine.Object.FindObjectOfType<LoginClick>();
                if(request.action=="loginLocalFixture")
                {
                    if(login==null)throw new InvalidOperationException("The local test login scene is not ready");
                    var lines=File.ReadAllLines("C:/UnityProjects/LegacyGwent/scripts/LOCAL-DEVELOPMENT.md");
                    var line=Array.Find(lines,l=>l.IndexOf("`localtest`",StringComparison.OrdinalIgnoreCase)>=0);
                    if(line==null)throw new InvalidOperationException("The documented local fixture was not found");
                    var values=System.Text.RegularExpressions.Regex.Matches(line,"`([^`]+)`");
                    if(values.Count!=2 || !string.Equals(values[0].Groups[1].Value,"localtest",StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Unexpected local fixture format");
                    login.Username.SetTextWithoutNotify(values[0].Groups[1].Value);
                    login.Password.SetTextWithoutNotify(values[1].Groups[1].Value);
                }
                if(login==null || !string.Equals(login.Username.text,"localtest",StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(login.Password.text))throw new InvalidOperationException("The existing local test login is not ready");
                login.Login();
            }
            else throw new InvalidOperationException("Unknown delivery action: "+request.action);
        }
        catch(Exception e){File.WriteAllText(Work+"main-rotation-delivery-failure.txt",e.ToString());Debug.LogException(e);}
        finally{busy=false;}
    }
}
