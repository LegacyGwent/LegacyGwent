using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Assets.Script.DynamicCards;
using Assets.Script.Localization;
using Assets.Script.ResourceManagement;
using Cynthia.Card;
using Cynthia.Card.Common.Models.Localization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Offline verification: no login, account writes, or saved scene changes.
[InitializeOnLoad]
public static class LocalizationVerification
{
    private static readonly string Work = Path.GetFullPath(Path.Combine(Application.dataPath,"../../../../../work/Localization"));
    private static readonly BindingFlags Fields = BindingFlags.Static|BindingFlags.NonPublic;
    static LocalizationVerification()
    {
        EditorApplication.update += Poll;
        EditorApplication.playModeStateChanged += state =>
        {
            if (!SessionState.GetBool("LocalizationVerification.Play",false)) return;
            if(state==PlayModeStateChange.EnteredPlayMode)
                EditorApplication.delayCall += () => { Run(); EditorApplication.isPlaying=false; };
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                SessionState.SetBool("LocalizationVerification.Play",false);
                var setup=JsonConvert.DeserializeObject<SceneSetup[]>(SessionState.GetString("LocalizationVerification.Scenes","[]"));
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
        };
    }
    private static void Poll()
    {
        string request=Path.Combine(Work,"verify.request");
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(request))return;
        string mode=File.ReadAllText(request).Trim();File.Delete(request);
        if(mode=="play") RunPlay(); else Run();
    }
    [MenuItem("Tools/Localization/Verify in Play mode")]
    public static void RunPlay()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode before starting the isolated localization check.");
        for(int i=0;i<SceneManager.sceneCount;i++)
            if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save open scenes before running this check.");
        SessionState.SetString("LocalizationVerification.Scenes",JsonConvert.SerializeObject(EditorSceneManager.GetSceneManagerSetup()));
        SessionState.SetBool("LocalizationVerification.Play",true);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        EditorApplication.isPlaying=true;
    }
    [MenuItem("Tools/Localization/Verify all languages")]
    public static void Run()
    {
        Directory.CreateDirectory(Work);
        var checks=new List<string>();
        var warnings=new List<string>();
        var savedContainer=DependencyResolver.Container;
        var scene=default(Scene);
        IContainer testContainer=null;
        GameObject page=null;
        var bindings=new List<LocalizedLabel>();
        DailyQuestPanel panel=null;
        PremiumCollectionPanel premium=null;
        var savedState=Get(typeof(DailyQuestClient),"<State>k__BackingField");
        var savedAccount=Get(typeof(PremiumCollectionClient),"<Account>k__BackingField");
        var savedPending=Get(typeof(DailyQuestClient),"pending");
        var savedError=Get(typeof(DailyQuestClient),"errorKey");
        var savedSample=Get(typeof(DailyQuestClient),"sampledAt");
        var savedSeconds=Get(typeof(DailyQuestClient),"secondsToReset");
        var savedQueued=Get(typeof(DailyQuestClient),"refreshQueued");
        var savedReady=Get(typeof(PremiumCollectionClient),"<Ready>k__BackingField");
        var savedCosts=Get(typeof(PremiumCollectionClient),"<Costs>k__BackingField");
        try
        {
            if(EditorApplication.isPlaying && !SessionState.GetBool("LocalizationVerification.Play",false))throw new InvalidOperationException("Use the isolated Play-mode verification command.");
            var type=typeof(LocalizedLabel).Assembly.GetType("Assets.Script.Localization.LocalizationService");
            var builder=new ContainerBuilder();builder.RegisterType(type).SingleInstance();
            testContainer=builder.Build();DependencyResolver.Container=testContainer;
            var service=testContainer.Resolve(type);
            var locale=type.GetField("TextLocalization").GetValue(service);
            var choose=locale.GetType().GetMethod("ChooseLanguage");
            var config=JsonConvert.DeserializeObject<List<ConfigEntry>>(Resources.Load<TextAsset>("Locales/config").text);
            var all=config.Select(c=>JsonConvert.DeserializeObject<GameLocale>(Resources.Load<TextAsset>("Locales/"+c.Filename).text)).ToList();
            var union=all.SelectMany(x=>x.MenuLocales.Keys).Distinct().ToArray();
            var cardUnion=all.SelectMany(x=>x.CardLocales.Keys).Distinct().ToArray();
            scene=EditorApplication.isPlaying ? SceneManager.CreateScene("LocalizationVerification") : EditorSceneManager.NewPreviewScene();
            var host=new GameObject("Localization verification");SceneManager.MoveGameObjectToScene(host,scene);
            host.SetActive(false);
            var editor=host.AddComponent<EditorInfo>();
            var main=new GameObject("Test main UI",typeof(RectTransform));SceneManager.MoveGameObjectToScene(main,scene);
            editor.MainUI=main;
            panel=main.AddComponent<DailyQuestPanel>();panel.Initialize(editor);
            Set(typeof(DailyQuestClient),"pending",new TaskCompletionSource<bool>().Task);
            var state=new DailyQuestResult{Status="ok",LoginPowder=20,DailyCap=125,Tiers=new List<DailyQuestTier>{
                new DailyQuestTier{Crowns=2,Powder=25},new DailyQuestTier{Crowns=4,Powder=35},new DailyQuestTier{Crowns=6,Powder=45}}};
            var account=new PremiumCollection{MeteoritePowder=123456,DailyQuests=new DailyQuestProgress{LoginGranted=true,Crowns=3,PowderGranted=45}};
            Set(typeof(DailyQuestClient),"<State>k__BackingField",state);
            Set(typeof(PremiumCollectionClient),"<Account>k__BackingField",account);
            Set(typeof(DailyQuestClient),"errorKey",null);
            Set(typeof(DailyQuestClient),"sampledAt",Time.realtimeSinceStartup);
            Set(typeof(DailyQuestClient),"secondsToReset",45000d);
            panel.Open();page=GameObject.Find("DailyQuestPage");SceneManager.MoveGameObjectToScene(page,scene);
            // Edit-mode harness explicitly enters the runtime binding lifecycle.
            bindings.AddRange(page.GetComponentsInChildren<LocalizedLabel>());
            if(!EditorApplication.isPlaying)
                foreach(var binding in bindings)typeof(LocalizedLabel).GetMethod("OnEnable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(binding,null);
            var cameraObject=new GameObject("Verification camera",typeof(Camera));SceneManager.MoveGameObjectToScene(cameraObject,scene);
            var camera=cameraObject.GetComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.nearClipPlane=.1f;camera.farClipPlane=100;
            camera.scene=scene;
            var canvas=page.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            var premiumUi=new GameObject("Premium verification",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler));SceneManager.MoveGameObjectToScene(premiumUi,scene);
            var premiumCanvas=premiumUi.GetComponent<Canvas>();premiumCanvas.renderMode=RenderMode.ScreenSpaceCamera;premiumCanvas.worldCamera=camera;premiumCanvas.planeDistance=2;
            var premiumScaler=premiumUi.GetComponent<CanvasScaler>();premiumScaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;premiumScaler.referenceResolution=new Vector2(1600,900);
            editor.EditorUI=premiumUi;editor.ShowSearch=Search(premiumUi.transform,"ShowSearch",new Vector2(-500,200));editor.EditorSearch=Search(premiumUi.transform,"DeckSearch",new Vector2(-500,50));
            typeof(EditorInfo).GetField("<EditorStatus>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(editor,EditorStatus.ShowCards);
            premium=main.AddComponent<PremiumCollectionPanel>();premium.Initialize(editor);
            typeof(PremiumCollectionPanel).GetField("detailsOwner",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(premium,host.AddComponent<righclickLogic>());
            Set(typeof(PremiumCollectionClient),"<Ready>k__BackingField",true);
            Set(typeof(PremiumCollectionClient),"<Costs>k__BackingField",new Dictionary<string,int>{{"12001",400}});
            premium.Preview(new CardStatus("12001"){IsPremium=true});
            var premiumRefresh=typeof(PremiumCollectionPanel).GetMethod("RefreshLabels",BindingFlags.Instance|BindingFlags.NonPublic);
            bindings.AddRange(premiumUi.GetComponentsInChildren<LocalizedLabel>(true));
            if(!EditorApplication.isPlaying)
            {
                typeof(PremiumCollectionPanel).GetMethod("OnEnable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(premium,null);
                foreach(var binding in premiumUi.GetComponentsInChildren<LocalizedLabel>(true))typeof(LocalizedLabel).GetMethod("OnEnable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(binding,null);
            }
            var paint=typeof(DailyQuestPanel).GetMethod("Paint",BindingFlags.Instance|BindingFlags.NonPublic);
            for(int i=0;i<config.Count;i++)
            {
                choose.Invoke(locale,new object[]{i});var pack=all[i];string lang=config[i].Filename;
                Require(union.All(pack.MenuLocales.ContainsKey),lang+" menu key parity",checks);
                Require(cardUnion.All(pack.CardLocales.ContainsKey),lang+" card key parity",checks);
                foreach(var item in pack.MenuLocales)
                    string.Format(item.Value,new object[]{1,2,3,4,5,6,7,8});
                checks.Add(lang+" all menu formats parse");
                Require(page.GetComponentsInChildren<Text>().Any(t=>t.text==pack.MenuLocales["DailyQuest_Title"]),lang+" open page switches language",checks);
                Require(page.GetComponentsInChildren<Text>().Any(t=>t.text.Contains(pack.MenuLocales["DailyQuest_Granted"])),lang+" reward status switches language",checks);
                Require(premiumUi.GetComponentsInChildren<Text>().Any(t=>t.text==pack.MenuLocales["Premium_Standard"]),lang+" premium filters switch language",checks);
                Require(premiumUi.GetComponentsInChildren<Text>().Any(t=>t.text==LocalizedLabel.Get("Premium_Craft",400)),lang+" crafting button switches language",checks);
                typeof(PremiumCollectionPanel).GetField("error",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(premium,"Premium_InsufficientPowder");premiumRefresh.Invoke(premium,null);
                Require(premiumUi.GetComponentsInChildren<Text>().Any(t=>t.text==pack.MenuLocales["Premium_InsufficientPowder"]),lang+" crafting errors localized",checks);
                typeof(PremiumCollectionPanel).GetField("error",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(premium,null);premiumRefresh.Invoke(premium,null);
                Canvas.ForceUpdateCanvases();
                foreach(var label in page.GetComponentsInChildren<Text>())
                {
                    label.font.RequestCharactersInTexture(label.text,label.fontSize,label.fontStyle);
                    var missing=label.text.Where(c=>!char.IsWhiteSpace(c)&&!label.font.HasCharacter(c)).Distinct().ToArray();
                    if(missing.Length>0)warnings.Add(lang+" font "+label.name+": "+new string(missing));
                }
                Render(camera,lang);
                page.SetActive(false);Render(camera,"premium-"+lang);page.SetActive(true);
                // Offline/loading/error/completed paths use the same selected language.
                Set(typeof(DailyQuestClient),"<State>k__BackingField",null);
                Set(typeof(DailyQuestClient),"errorKey","DailyQuest_SyncError");paint.Invoke(panel,null);
                Require(page.GetComponentsInChildren<Text>().Any(t=>t.text==pack.MenuLocales["DailyQuest_SyncError"]),lang+" sync error localized",checks);
                Set(typeof(DailyQuestClient),"<State>k__BackingField",state);Set(typeof(DailyQuestClient),"errorKey",null);
                account.DailyQuests.Crowns=6;paint.Invoke(panel,null);
                Require(page.GetComponentsInChildren<Text>().Any(t=>t.text==pack.MenuLocales["DailyQuest_Complete"]),lang+" completed state localized",checks);
                account.DailyQuests.Crowns=3;paint.Invoke(panel,null);
            }
            Require(warnings.Count==0,"daily quest fonts cover all four languages",checks);
            // Emulate an older downloaded pack without touching the user's real cache.
            string cache=Path.Combine(Work,"legacy-cache");Directory.CreateDirectory(cache);
            File.WriteAllText(Path.Combine(cache,"config.json"),JsonConvert.SerializeObject(config));
            for(int i=0;i<config.Count;i++)
            {
                var old=JsonConvert.DeserializeObject<GameLocale>(JsonConvert.SerializeObject(all[i]));
                foreach(var key in old.MenuLocales.Keys.Where(k=>k.StartsWith("DailyQuest_")).ToArray())old.MenuLocales.Remove(key);
                old.CardLocales.Remove("70018");
                old.CardLocales["12001"].Info="SERVER_OVERRIDE";
                File.WriteAllText(Path.Combine(cache,config[i].Filename+".json"),JsonConvert.SerializeObject(old));
            }
            var handler=new TextLocalizationFileHandler("");
            typeof(TextLocalizationFileHandler).GetField("_directoryPath",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(handler,cache);
            locale.GetType().GetProperty("ResourceHandler").SetValue(locale,handler);
            for(int i=0;i<config.Count;i++)
            {
                choose.Invoke(locale,new object[]{i});string lang=config[i].Filename;
                Require(LocalizedLabel.Get("DailyQuest_Title")==all[i].MenuLocales["DailyQuest_Title"],lang+" old cache uses same-language bundled UI",checks);
                var getInfo=locale.GetType().GetMethod("GetCardInfo");
                Require((string)getInfo.Invoke(locale,new object[]{"70018"})==all[i].CardLocales["70018"].Info,lang+" old cache uses bundled missing card",checks);
                Require((string)getInfo.Invoke(locale,new object[]{"12001"})=="SERVER_OVERRIDE",lang+" server card description preserved",checks);
            }
            File.WriteAllText(Path.Combine(Work,EditorApplication.isPlaying?"play-results.json":"unity-results.json"),JsonConvert.SerializeObject(new{passed=true,playMode=EditorApplication.isPlaying,checks,warnings},Formatting.Indented));
        }
        catch(Exception e)
        {
            File.WriteAllText(Path.Combine(Work,EditorApplication.isPlaying?"play-results.json":"unity-results.json"),JsonConvert.SerializeObject(new{passed=false,error=e.ToString(),checks,warnings},Formatting.Indented));Debug.LogException(e);
        }
        finally
        {
            if(!EditorApplication.isPlaying)
                foreach(var binding in bindings)if(binding!=null)typeof(LocalizedLabel).GetMethod("OnDisable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(binding,null);
            if(panel!=null)typeof(DailyQuestPanel).GetField("page",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(panel,null);
            if(panel!=null && !EditorApplication.isPlaying)typeof(DailyQuestPanel).GetMethod("OnDestroy",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(panel,null);
            if(premium!=null && !EditorApplication.isPlaying)typeof(PremiumCollectionPanel).GetMethod("OnDisable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(premium,null);
            if(scene.IsValid())
            {
                if(EditorApplication.isPlaying){foreach(var root in scene.GetRootGameObjects())Object.DestroyImmediate(root);SceneManager.UnloadSceneAsync(scene);}
                else EditorSceneManager.ClosePreviewScene(scene);
            }
            Set(typeof(DailyQuestClient),"<State>k__BackingField",savedState);Set(typeof(PremiumCollectionClient),"<Account>k__BackingField",savedAccount);
            Set(typeof(DailyQuestClient),"pending",savedPending);Set(typeof(DailyQuestClient),"errorKey",savedError);
            Set(typeof(DailyQuestClient),"sampledAt",savedSample);Set(typeof(DailyQuestClient),"secondsToReset",savedSeconds);
            Set(typeof(DailyQuestClient),"refreshQueued",savedQueued);
            Set(typeof(PremiumCollectionClient),"<Ready>k__BackingField",savedReady);Set(typeof(PremiumCollectionClient),"<Costs>k__BackingField",savedCosts);
            DependencyResolver.Container=savedContainer;testContainer?.Dispose();
        }
    }
    private static object Get(Type type,string name)=>type.GetField(name,Fields).GetValue(null);
    private static InputField Search(Transform parent,string name,Vector2 position)
    {
        var root=new GameObject(name,typeof(RectTransform),typeof(InputField));root.transform.SetParent(parent,false);
        var rect=root.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(600,50);rect.anchoredPosition=position;
        var label=new GameObject("Text",typeof(RectTransform),typeof(Text));label.transform.SetParent(root.transform,false);
        var text=label.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");
        var input=root.GetComponent<InputField>();input.textComponent=text;return input;
    }
    private static void Set(Type type,string name,object value)=>type.GetField(name,Fields).SetValue(null,value);
    private static void Require(bool value,string message,List<string> checks){if(!value)throw new Exception(message);checks.Add(message);}
    private static void Render(Camera camera,string language)
    {
        var previous=RenderTexture.active;
        var texture=new RenderTexture(1600,900,24);var output=new Texture2D(1600,900,TextureFormat.RGB24,false);
        try
        {
            camera.targetTexture=texture;camera.Render();RenderTexture.active=texture;
            output.ReadPixels(new Rect(0,0,1600,900),0,0);output.Apply();File.WriteAllBytes(Path.Combine(Work,"daily-"+language+".png"),output.EncodeToPNG());
        }
        finally{camera.targetTexture=null;RenderTexture.active=previous;Object.DestroyImmediate(texture);Object.DestroyImmediate(output);}
    }
}
