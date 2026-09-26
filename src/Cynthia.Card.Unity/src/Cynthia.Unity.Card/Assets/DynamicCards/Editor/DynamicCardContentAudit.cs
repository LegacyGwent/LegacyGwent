using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Script.DynamicCards.Editor
{
    public static class DynamicCardContentAudit
    {
        [Serializable] private class Result { public int cards, particles; public List<string> issues = new List<string>(); }
        [Serializable] private class SourceOptionalRenderers { public string[] sourceNullMaterials; }
        public static void Run()
        {
            string selected=Environment.GetEnvironmentVariable("DYNAMIC_CARD_AUDIT_IDS");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var catalog = JsonUtility.FromJson<DynamicCardCatalog>(File.ReadAllText(DynamicCardLibrary.CatalogAsset));
            string output = Path.GetFullPath("../../../../work/DynamicCards/RenderAudit");Directory.CreateDirectory(output);
            var result = new Result();
            var camera = new GameObject("Audit camera").AddComponent<Camera>();camera.enabled = false;
            camera.clearFlags = CameraClearFlags.SolidColor;camera.backgroundColor = Color.clear;camera.allowHDR = false;
            var rt = new RenderTexture(384,384,24);rt.Create();camera.targetTexture = rt;
            foreach (var entry in catalog.cards)
            {
                if(!string.IsNullOrEmpty(selected) && !selected.Split(',').Contains(entry.id))continue;
                GameObject model = null;
                try
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.prefab);
                    if (prefab == null) throw new Exception("prefab missing");
                    model = Object.Instantiate(prefab);model.SetActive(true);
                    var original=JsonUtility.FromJson<SourceOptionalRenderers>(File.ReadAllText(Path.GetDirectoryName(entry.prefab)+"/conversion.json"));
                    foreach (Transform child in model.transform) if(child.localPosition.sqrMagnitude>1000000)child.localPosition=Vector3.zero;
                    var renderers = model.GetComponentsInChildren<Renderer>(true);
                    if (renderers.Length==0)throw new Exception("no renderers");
                    foreach (var renderer in renderers)
                    {
                        if(!renderer.enabled || !renderer.gameObject.activeInHierarchy)continue;
                        var materials=renderer.sharedMaterials;var particleRenderer=renderer as ParticleSystemRenderer;
                        for(int slot=0;slot<materials.Length;slot++)
                        {
                            if(particleRenderer!=null && (particleRenderer.renderMode==ParticleSystemRenderMode.None || (slot>0 && !particleRenderer.GetComponent<ParticleSystem>().trails.enabled)))continue;
                            var material=materials[slot];
                            if(material==null && original.sourceNullMaterials!=null && original.sourceNullMaterials.Contains(renderer.name))continue;
                            if(material==null || material.shader==null || !material.shader.name.StartsWith("DynamicCards/"))result.issues.Add(entry.id+": material "+renderer.name+" slot "+slot);
                        }
                    }
                    foreach(var pair in entry.transformPairs ?? new DynamicCardTransformPair[0])
                        if(!string.IsNullOrEmpty(pair.target) && (DynamicCardPaths.Find(model.transform,pair.source)==null || DynamicCardPaths.Find(model.transform,pair.target)==null))result.issues.Add(entry.id+": transform pair "+pair.source+" -> "+pair.target);
                    foreach(var evt in entry.particleEvents ?? new DynamicCardParticleEvent[0])
                        if(DynamicCardPaths.Find(model.transform,evt.path)==null)result.issues.Add(entry.id+": particle event "+evt.path);
                    foreach(var animator in model.GetComponentsInChildren<Animator>(true))
                    { animator.Rebind();animator.Update(0);animator.Update(entry.introDuration+1); }
                    foreach(Transform child in model.transform)if(child.localPosition.sqrMagnitude>1000000)child.localPosition=Vector3.zero;
                    foreach(var vertexAnimation in model.GetComponentsInChildren<DynamicCardVertexAnimation>(true)){vertexAnimation.Initialize();vertexAnimation.Advance(1);}
                    var pivot=DynamicCardPaths.Find(model.transform,entry.pivot);
                    if(pivot!=null)pivot.localRotation*=Quaternion.Euler((entry.xStart+entry.xEnd)*.5f,(entry.yStart+entry.yEnd)*.5f,0);
                    camera.transform.position=new Vector3(0,0,entry.cameraDistance);
                    var controllers=model.AddComponent<DynamicCardSourceControllers>();controllers.Initialize(entry,camera.transform);
                    foreach(var particle in model.GetComponentsInChildren<ParticleSystem>(true))
                    { result.particles++;if(particle.main.playOnAwake)particle.Simulate(1,false,true); }
                    var effects=model.AddComponent<DynamicCardEffects>();effects.Initialize(entry);effects.Tick(entry.introDuration+1);
                    controllers.Tick(entry.introDuration+1,1f/30,(entry.xStart+entry.xEnd)*.5f,(entry.yStart+entry.yEnd)*.5f);
                    camera.transform.position=new Vector3(0,0,entry.cameraDistance);camera.fieldOfView=entry.fieldOfView;
                    camera.nearClipPlane=entry.nearClip;camera.farClipPlane=entry.farClip;
                    camera.Render();RenderTexture.active=rt;
                    var image=new Texture2D(384,384,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,384,384),0,0);image.Apply();
                    if(image.GetPixels32().Count(c=>c.a>50 && c.r+c.g+c.b>20)<1000)result.issues.Add(entry.id+": nearly empty render");
                    if(image.GetPixels32().Count(c=>c.r>245 && c.b>245 && c.g<10)>384*384/3)result.issues.Add(entry.id+": error shader render");
                    File.WriteAllBytes(output+"/"+entry.id+".png",image.EncodeToPNG());Object.DestroyImmediate(image);RenderTexture.active=null;
                    result.cards++;
                }
                catch(Exception e) { result.issues.Add(entry.id+": "+e.Message); }
                finally { if(model!=null)Object.DestroyImmediate(model); }
                if(result.cards%25==0) { EditorUtility.UnloadUnusedAssetsImmediate(); Debug.Log("DYNAMIC_AUDIT_PROGRESS "+result.cards); }
            }
            camera.targetTexture=null;rt.Release();Object.DestroyImmediate(rt);Object.DestroyImmediate(camera.gameObject);
            File.WriteAllText(output+(string.IsNullOrEmpty(selected)?"/audit.json":"/selected.json"),JsonUtility.ToJson(result,true));
            Debug.Log("DYNAMIC_AUDIT_DONE cards="+result.cards+" particles="+result.particles+" issues="+result.issues.Count);
        }
        public static void Selected()
        {
            var ids=new List<string>("10030100,10090100,14050100,15010100,15350100,15620100,15660100".Split(','));
            var previous=JsonUtility.FromJson<Result>(File.ReadAllText(Path.GetFullPath("../../../../work/DynamicCards/RenderAudit/audit.json")));
            ids.AddRange(previous.issues.Select(i=>i.Substring(0,8)));
            Environment.SetEnvironmentVariable("DYNAMIC_CARD_AUDIT_IDS",string.Join(",",ids.Distinct()));
            Run();
        }
        public static void Bundle()
        {
            string path=DynamicCardBuild.BuildBundle(BuildTarget.StandaloneWindows64);
            var bundle=AssetBundle.LoadFromFile(path);if(bundle==null)throw new Exception("Bundle failed to open");
            int count=bundle.GetAllAssetNames().Count(p=>p.EndsWith("/card.prefab"));
            int expected=JsonUtility.FromJson<DynamicCardCatalog>(File.ReadAllText(DynamicCardLibrary.CatalogAsset)).cards.Length;
            if(count!=expected)throw new Exception("Bundle card count: "+count);
            bundle.Unload(true);Debug.Log("DYNAMIC_FULL_BUNDLE_PASS cards="+count+" bytes="+new FileInfo(path).Length);
        }
    }
}
