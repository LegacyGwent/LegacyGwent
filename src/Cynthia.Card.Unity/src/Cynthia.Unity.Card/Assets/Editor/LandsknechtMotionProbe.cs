using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.DynamicCards;

[InitializeOnLoad]
public static class LandsknechtMotionProbe
{
    public const string Work="C:/UnityProjects/LegacyGwent/work/DynamicCards/LandsknechtAcceptance-20260912/";
    static LandsknechtMotionProbe(){EditorApplication.update+=Poll;}
    static void Poll()
    {
        if(!EditorApplication.isPlaying || EditorApplication.isCompiling || !File.Exists(Work+"capture.request"))return;
        var editor=UnityEngine.Object.FindObjectOfType<EditorInfo>();
        if(editor==null || editor.EditorStatus!=EditorStatus.ShowCards)return;
        var run=File.ReadAllText(Work+"capture.request").Trim();File.Delete(Work+"capture.request");
        editor.SelectSwitchUICard(new Cynthia.Card.CardStatus("70104"));
        new GameObject("Landsknecht motion evidence").AddComponent<LandsknechtMotionRecorder>().Begin(editor,run);
    }
}
public class LandsknechtMotionRecorder:MonoBehaviour
{
    [Serializable] class Pose {public string bone;public Vector3 position;public Quaternion rotation;}
    [Serializable] class Frame {public float age,time;public int frame;public Pose[] bones;public Vector3[] spear;}
    [Serializable] class Result {public string run,bundleRoot,sourceMesh;public bool complete;public string[] spearBones;public List<Frame> frames=new List<Frame>();}
    static object Field(object o,string name){return o.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(o);}
    public void Begin(EditorInfo editor,string run){StartCoroutine(Capture(editor,run));}
    IEnumerator Capture(EditorInfo editor,string run)
    {
        string dir=LandsknechtMotionProbe.Work+run+"/";Directory.CreateDirectory(dir);
        DynamicCardView view=null;GameObject model=null;float requested=Time.realtimeSinceStartup;
        while(model==null)
        {
            view=editor.ShowArtCard.CardImg.GetComponent<DynamicCardView>();
            if(view!=null)
            {
                var loaded=Field(view,"entry") as DynamicCardEntry;
                var surface=Field(view,"surface") as RawImage;
                if(loaded!=null && loaded.artIds.Contains("202156") && surface!=null && surface.enabled)
                    model=Field(view,"model") as GameObject;
            }
            if(!DynamicCardSettings.Enabled || Time.realtimeSinceStartup-requested>35)
            {
                File.WriteAllText(dir+"capture-status.txt",DynamicCardSettings.Enabled?"FAILED: actual preview did not load":"STOPPED: animated cards are disabled");
                Destroy(gameObject);yield break;
            }
            if(model==null)yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        model=Field(view,"model") as GameObject;
        if(model==null){File.WriteAllText(dir+"capture-status.txt","STOPPED: actual preview was released");Destroy(gameObject);yield break;}
        var skin=model.GetComponentsInChildren<SkinnedMeshRenderer>(true).Single(s=>s.name=="_3_man");
        var raw=(RawImage)Field(view,"surface");
        var bones=skin.bones;var mesh=new Mesh();
        var result=new Result{run=run,bundleRoot=(string)Field(DynamicCardLibrary.Instance,"bundleRoot"),sourceMesh=skin.sharedMesh.name,spearBones=bones.Select(b=>b==null?"<null>":b.name).ToArray()};
        skin.BakeMesh(mesh);
        // GPU-only imported meshes do not expose original weights at runtime.
        // Their source binding evidence is already audited separately; BakeMesh remains readable.
        File.WriteAllText(dir+"mesh.json",JsonUtility.ToJson(new MeshEvidence{vertices=mesh.vertices,sourceMeshReadable=skin.sharedMesh.isReadable,
            weights=skin.sharedMesh.isReadable?skin.sharedMesh.boneWeights:null,bindposes=skin.sharedMesh.isReadable?skin.sharedMesh.bindposes:null},true));
        float start=Time.realtimeSinceStartup,next=0;
        while(Time.realtimeSinceStartup-start<17)
        {
            yield return new WaitForEndOfFrame();
            float time=Time.realtimeSinceStartup-start;if(time<next)continue;next=time+1f/30;
            skin.BakeMesh(mesh);
            result.frames.Add(new Frame{age=(float)Field(view,"age"),time=time,frame=Time.frameCount,spear=mesh.vertices,bones=bones.Where(b=>b!=null).Select(b=>new Pose{bone=b.name,position=b.localPosition,rotation=b.localRotation}).ToArray()});
            var rt=(RenderTexture)raw.texture;var prev=RenderTexture.active;RenderTexture.active=rt;
            var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);tex.Apply();RenderTexture.active=prev;
            File.WriteAllBytes(dir+"frame-"+result.frames.Count.ToString("D4")+".jpg",tex.EncodeToJPG(85));Destroy(tex);
        }
        result.complete=true;File.WriteAllText(dir+"poses.json",JsonUtility.ToJson(result));Destroy(mesh);Destroy(gameObject);
    }
    [Serializable] class MeshEvidence{public bool sourceMeshReadable;public Vector3[] vertices;public BoneWeight[] weights;public Matrix4x4[] bindposes;}
}
