using System;
using System.IO;
using System.Linq;
using Autofac;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Script.DynamicCards.Editor
{
    [InitializeOnLoad]
    public static class DynamicCardVerification
    {
        private static int step;
        private static double start;
        private static Image art;
        private static RectTransform frame;
        private static Camera camera;
        private static RenderTexture target;
        private static DynamicCardQuality oldSetting;
        private static Vector4 initialViewMaterial;
        private static string Output { get { return Path.GetFullPath("../../../../work/DynamicCards"); } }
        static DynamicCardVerification() { EditorApplication.playModeStateChanged += State; }
        public static void Run()
        {
            var source = EditorSceneManager.OpenScene("Assets/Resources/Scenes/RightClick.unity", OpenSceneMode.Single);
            var logic = Object.FindObjectOfType<righclickLogic>();
            if (logic == null) throw new Exception("Original right-click preview is missing.");
            var card = DynamicCardView.FindCardRoot(logic.CardImg.transform, logic.CardBorder.transform);
            string artPath = AnimationUtility.CalculateTransformPath(logic.CardImg.transform, card);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            var clone = Object.Instantiate(card.gameObject);
            SceneManager.MoveGameObjectToScene(clone, scene);
            clone.name = "VerifiedCard";
            var canvasObject = new GameObject("VerificationCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            SceneManager.MoveGameObjectToScene(canvasObject, scene);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            var rect = (RectTransform)clone.transform;rect.SetParent(canvas.transform, false);
            rect.anchorMin = rect.anchorMax = new Vector2(.5f,.5f);rect.anchoredPosition = Vector2.zero;rect.localScale = Vector3.one;
            var image = rect.Find(artPath).GetComponent<Image>();image.name = "VerifiedArt";
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Addressables/Cards/202887.png");
            foreach (var script in clone.GetComponentsInChildren<MonoBehaviour>(true))
                if (!(script is UnityEngine.EventSystems.UIBehaviour)) script.enabled = false;
            EditorSceneManager.CloseScene(source, true);
            SceneManager.SetActiveScene(scene);
            SessionState.SetBool("DynamicCardVerify", true);
            EditorApplication.isPlaying = true;
        }
        public static void Settings()
        {
            var source = EditorSceneManager.OpenScene("Assets/Resources/Scenes/Game.unity", OpenSceneMode.Single);
            var original = Resources.FindObjectsOfTypeAll<InitSetting>().First(s => s.gameObject.scene == source);
            var originalPanel = original.QualityPanel.transform.parent.parent;
            string selectorPath = AnimationUtility.CalculateTransformPath(original.QualityPanel.transform,originalPanel);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
            var panel = Object.Instantiate(originalPanel.gameObject);SceneManager.MoveGameObjectToScene(panel,scene);panel.SetActive(true);
            EditorSceneManager.CloseScene(source,true);SceneManager.SetActiveScene(scene);
            var bootstrap = new GameObject("Local verification services").AddComponent<Bootstrapper>();
            DependencyResolver.Container = bootstrap.AutoRegisterService(new ContainerBuilder()).Build();
            var previous = DynamicCardSettings.Quality;int quality = PlayerPrefs.GetInt("quality",2);
            DynamicCardSettingRow.Install(panel.transform.Find(selectorPath).gameObject);
            var row = panel.transform.Find("DynamicCardsOption");if(row==null)throw new Exception("Settings row not installed.");
            var selector = row.GetComponentInChildren<ChoseValue>(true);
            if(selector.onValueChanged.GetPersistentEventCount()!=0)throw new Exception("Cloned quality listener leaked into animated-card settings.");
            selector.Index = 1;if(!DynamicCardSettings.Enabled || PlayerPrefs.GetInt("quality",2)!=quality)throw new Exception("Setting changed the wrong preference.");
            selector.Index = 0;if(DynamicCardSettings.Enabled)throw new Exception("Setting did not turn off.");
            DynamicCardSettings.Quality = previous;
            var canvas = new GameObject("Settings canvas",typeof(Canvas)).GetComponent<Canvas>();canvas.renderMode = RenderMode.ScreenSpaceCamera;
            panel.transform.SetParent(canvas.transform,false);((RectTransform)panel.transform).anchoredPosition = Vector2.zero;
            camera = new GameObject("Settings camera").AddComponent<Camera>();camera.backgroundColor = new Color(.035f,.045f,.065f);camera.clearFlags = CameraClearFlags.SolidColor;camera.orthographic = true;
            target = new RenderTexture(1280,900,24);target.Create();camera.targetTexture = target;canvas.worldCamera = camera;canvas.planeDistance = 100;
            Canvas.ForceUpdateCanvases();Capture("unity_settings.png");
            Debug.Log("DYNAMIC_SETTINGS_PASS row toggle isolated-preference");
        }
        private static void State(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("DynamicCardVerify", false)) return;
            SessionState.SetBool("DynamicCardVerify", false);
            oldSetting = DynamicCardSettings.Quality;
            art = GameObject.Find("VerifiedArt").GetComponent<Image>();
            frame = GameObject.Find("VerifiedCard").GetComponent<RectTransform>();
            camera = new GameObject("VerificationCamera").AddComponent<Camera>();
            camera.gameObject.AddComponent<AudioListener>();
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.035f,.045f,.065f);camera.orthographic = true;camera.orthographicSize = 450;
            target = new RenderTexture(1280,900,24);target.Create();camera.targetTexture = target;
            var canvas = GameObject.Find("VerificationCanvas").GetComponent<Canvas>();canvas.worldCamera = camera;canvas.planeDistance = 100;
            DynamicCardSettings.Enabled = false;
            DynamicCardView.Bind(art, "202887", false, true, frame);
            step = 0; start = EditorApplication.timeSinceStartup;
            EditorApplication.update += Tick;
        }
        private static void Tick()
        {
            try
            {
                var elapsed = EditorApplication.timeSinceStartup - start;
                if (elapsed > 45) throw new Exception("Animated card verification timed out at step " + step);
                if (step == 0 && elapsed > .5)
                {
                    if (art.GetComponentInChildren<RawImage>() != null) throw new Exception("Disabled mode created dynamic art.");
                    if(frame.GetComponent<DynamicCardDragHandle>().enabled)throw new Exception("Static mode intercepted the original drag handling.");
                    Capture("unity_static.png");DynamicCardSettings.Enabled = true;step = 1;
                }
                else if (step == 1 && art.GetComponentInChildren<RawImage>() != null)
                { Capture("unity_intro.png");start = EditorApplication.timeSinceStartup;step = 2; }
                else if (step == 2 && elapsed > 4.5)
                {
                    Capture("unity_loop.png");
                    if(ExecuteEvents.GetEventHandler<IDragHandler>(art.gameObject)!=frame.gameObject)throw new Exception("Card image drag did not bubble to the whole-card frame.");
                    ExecuteEvents.Execute(frame.gameObject,new PointerEventData(null){position=Vector2.zero},ExecuteEvents.beginDragHandler);
                    ExecuteEvents.Execute(frame.gameObject,new PointerEventData(null){position=new Vector2(100,260)},ExecuteEvents.dragHandler);step = 3;
                }
                else if (step == 3 && elapsed > 5.2)
                {
                    Capture("unity_drag.png");
                    float pitch = Mathf.DeltaAngle(0, frame.localEulerAngles.x);
                    if (Mathf.Abs(pitch) > 3.1f || Mathf.Abs(pitch) < 2.5f) throw new Exception("Vertical limit is not half the original: " + pitch);
                    ExecuteEvents.Execute(frame.gameObject,new PointerEventData(null),ExecuteEvents.endDragHandler);step = 4;
                }
                else if (step == 4 && elapsed > 6)
                {
                    if (Quaternion.Angle(frame.localRotation, Quaternion.identity) > .1f) throw new Exception("Card did not return to center.");
                    DynamicCardSettings.Enabled = false;step = 5;
                }
                else if (step == 5 && elapsed > 6.5)
                {
                    if (art.GetComponentInChildren<RawImage>() != null) throw new Exception("Turning off did not release dynamic art.");
                    DynamicCardSettings.Enabled = true;DynamicCardView.Bind(art, "202887", true, true, frame);step = 6;
                }
                else if (step == 6 && elapsed > 7)
                {
                    if (art.GetComponentInChildren<RawImage>() != null) throw new Exception("Concealed card revealed dynamic art.");
                    DynamicCardView.Bind(art, "missing-art-for-verification", false, true, frame);step = 7;
                }
                else if (step == 7 && elapsed > 7.5)
                {
                    if (art.GetComponentInChildren<RawImage>() != null) throw new Exception("Unmatched art did not fall back.");
                    DynamicCardView.Bind(art, "11210300", false, true, frame);step = 8;
                }
                else if (step == 8 && art.GetComponentInChildren<RawImage>() != null)
                {
                    CheckGeralt(false);Capture("unity_geralt_before_cut.png");start = EditorApplication.timeSinceStartup;step = 9;
                }
                else if (step == 9 && elapsed > 1.6)
                {
                    CheckGeralt(true);Capture("unity_geralt_after_cut.png");
                    DynamicCardView.Bind(art,"6010200",false,true,frame);start=EditorApplication.timeSinceStartup;step=10;
                }
                else if(step==10 && elapsed>.4 && art.GetComponentInChildren<RawImage>()!=null)
                {
                    initialViewMaterial=ViewMaterial();
                    var view=art.GetComponent<DynamicCardView>();view.OnBeginDrag(new PointerEventData(null){position=Vector2.zero});
                    view.OnDrag(new PointerEventData(null){position=new Vector2(400,200)});step=11;
                }
                else if(step==11 && elapsed>1.2)
                {
                    if((ViewMaterial()-initialViewMaterial).sqrMagnitude<.0001f)throw new Exception("View-dependent material did not respond to drag.");
                    Capture("unity_reflection_drag.png");art.GetComponent<DynamicCardView>().OnEndDrag(new PointerEventData(null));step=12;
                }
                else if(step==12 && elapsed>2)
                {
                    if((ViewMaterial()-initialViewMaterial).sqrMagnitude>.0001f && elapsed<5)return;
                    if((ViewMaterial()-initialViewMaterial).sqrMagnitude>.0001f)throw new Exception("View-dependent material did not return to center. initial="+initialViewMaterial+" actual="+ViewMaterial()+" frame="+frame.localEulerAngles);
                    Debug.Log("DYNAMIC_RUNTIME_PASS static-toggle intro-loop drag-half-limit return hidden unmatched cleanup geralt-cut-timing view-materials");Finish(0);
                }
            }
            catch (Exception exception) { Debug.LogException(exception);Finish(1); }
        }
        private static void CheckGeralt(bool cut)
        {
            var view = art.GetComponent<DynamicCardView>();
            var model = (GameObject)typeof(DynamicCardView).GetField("model",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(view);
            var before = model.transform.Find("10090100/Pivot/GeraltSwordmasterMesh/Pose01");
            var after = model.transform.Find("10090100/Pivot/GeraltSwordmasterMesh/Pose02");
            if(before==null || after==null || before.gameObject.activeSelf==cut || after.gameObject.activeSelf!=cut)
                throw new Exception("Geralt cut groups are out of phase: cut="+cut);
        }
        private static Vector4 ViewMaterial()
        {
            var view=art.GetComponent<DynamicCardView>();var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
            var model=(GameObject)typeof(DynamicCardView).GetField("model",flags).GetValue(view);
            var data=(DynamicCardEntry)typeof(DynamicCardView).GetField("entry",flags).GetValue(view);
            var anchor=model.transform.Find("Camera-attached effects");if(anchor==null || anchor.childCount==0)throw new Exception("Camera-attached effect was not isolated from the tilt pivot.");
            var motion=data.viewMotions.First(m=>m.kind==0);var renderer=DynamicCardPaths.Find(model.transform,motion.path).GetComponent<Renderer>();
            var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block,motion.materialIndex);return block.GetVector(motion.property+"_ST");
        }
        private static void Capture(string name)
        {
            camera.Render();var previous = RenderTexture.active;RenderTexture.active = target;
            var image = new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();
            Directory.CreateDirectory(Output);File.WriteAllBytes(Path.Combine(Output,name),image.EncodeToPNG());Object.Destroy(image);RenderTexture.active = previous;
        }
        private static void Finish(int code)
        { EditorApplication.update -= Tick;DynamicCardSettings.Quality = oldSetting;EditorApplication.Exit(code); }
    }
}
