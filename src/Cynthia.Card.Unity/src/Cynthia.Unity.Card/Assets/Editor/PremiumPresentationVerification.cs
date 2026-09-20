using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Assets.Script.DynamicCards;
using Cynthia.Card;
using Cynthia.Card.Client;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PremiumPresentationVerification
{
    private static string Work => Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../work/PremiumPresentation"));
    private static bool running;
    static PremiumPresentationVerification() { EditorApplication.update += Poll; }
    private static T Field<T>(object instance, string name) => (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
    private static async void Poll()
    {
        string request = Path.Combine(Work, "check.request");
        if (running || EditorApplication.isCompiling || !EditorApplication.isPlaying || !File.Exists(request)) return;
        string mode;
        try { mode = File.ReadAllText(request).Trim(); File.Delete(request); } catch (IOException) { return; }
        running = true;
        var checks = new List<string>();
        try
        {
            var client = DependencyResolver.Container.Resolve<GwentClientService>();
            if (!EditorPrefs.GetBool("LegacyGwent.UseLocalServer." + Application.dataPath, true) || client.User?.UserName != "premium-ui-test")
                throw new Exception("Prepare the local premium-ui-test fixture first.");
            if (mode == "native")
            {
                var view = UnityEngine.Object.FindObjectOfType<righclickLogic>().CardImg.GetComponent<DynamicCardView>();
                var samples = new List<object>();
                Directory.CreateDirectory(Path.Combine(Work,"native"));
                float started = Time.unscaledTime;
                for (int i = 0; i < 720; i++)
                {
                    var angle = Field<Vector2>(view,"current");
                    samples.Add(new {time=Time.unscaledTime-started,held=Field<bool>(view,"dragging"),x=angle.x,y=angle.y});
                    if (i % 6 == 0) ScreenCapture.CaptureScreenshot(Path.Combine(Work,"native",(i/6).ToString("D3")+".png"));
                    await Task.Delay(16);
                }
                File.WriteAllText(Path.Combine(Work,"native-input.json"),JsonConvert.SerializeObject(samples,Formatting.Indented));
                return;
            }
            if (SceneManager.GetSceneByName("RightClick").isLoaded)
            { UnityEngine.Object.FindObjectOfType<righclickLogic>().Closerightclick(); await Task.Delay(650); }
            await PremiumCollectionClient.Refresh();
            string id = PremiumCollectionClient.Account.OwnedCards.FirstOrDefault(x => GwentMap.CardMap[x].CardType != CardType.Special);
            if (id == null)
            {
                id = PremiumCollectionClient.Costs.Keys.First(x => PremiumCollectionClient.Costs[x] == 100 && GwentMap.CardMap[x].CardType != CardType.Special && GwentMap.CardMap[x].LinkedCards.Count != 0);
                var craft = await PremiumCollectionClient.Craft(id);
                if (!craft.Success) throw new Exception("Fixture needs 100 local powder to unlock a unit for the motion check: " + craft.Status);
            }
            var editor = Resources.FindObjectsOfTypeAll<EditorInfo>().First(x => x.gameObject.scene.IsValid());
            editor.MainUI.SetActive(false); editor.EditorUI.SetActive(true); editor.OpenEditor(false); await Task.Delay(400);
            editor.SetPremiumFilter(mode == "contracts" ? 2 : 1);
            var type = typeof(ArtCard).Assembly.GetType("Assets.Script.Localization.LocalizationService");
            string name = (string)type.GetMethod("GetCardName").Invoke(DependencyResolver.Container.Resolve(type), new object[] { id });
            editor.ShowSearchChange(name);
            editor.SelectSwitchUICard(new CardStatus(id) { IsPremium = true });
            await Task.Delay(3200);
            var hover = editor.ShowArtCard.CardImg.GetComponent<DynamicCardView>();
            Check(Field<GameObject>(hover,"model") != null, "hover loaded real 3D premium", checks);
            var pivot = Field<Transform>(hover, "pivot");
            var visual = Field<RectTransform>(Field<DynamicCardPresentation>(hover, "presentation"), "visual");
            var rotation1 = visual.localRotation; var inner1 = pivot.localRotation;
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "hover-a.png"));
            await Task.Delay(1400);
            Check(Quaternion.Angle(rotation1, visual.localRotation) > .1f, "hover frame continuously rotates", checks);
            Check(Quaternion.Angle(inner1, pivot.localRotation) > .1f, "hover internal 3D viewpoint moves with frame", checks);
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "hover-b.png"));
            if (mode != "hover")
            {
                editor.OpenCardDetails(new CardStatus(id) { IsPremium = true }); await Task.Delay(2800);
                var page = UnityEngine.Object.FindObjectOfType<righclickLogic>();
                if (page == null) throw new Exception("Detail page did not open");
                var view = page.CardImg.GetComponent<DynamicCardView>();
                Check(Field<GameObject>(view, "model") != null, "details loaded real 3D premium", checks);
                ScreenCapture.CaptureScreenshot(Path.Combine(Work, "details.png"));
                DumpPage(page);
                if (mode == "contracts")
                {
                    var point = RectTransformUtility.WorldToScreenPoint(null, page.CardBorder.rectTransform.TransformPoint(page.CardBorder.rectTransform.rect.center));
                    var data = new PointerEventData(EventSystem.current) { position = point, button = PointerEventData.InputButton.Left };
                    var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(data, hits);
                    var receiver = hits.Select(x => ExecuteEvents.GetEventHandler<IPointerDownHandler>(x.gameObject)).FirstOrDefault(x => x != null);
                    Check(receiver != null && receiver.GetComponent<DynamicCardDragHandle>() != null, "visible card raycast reaches the drag handle", checks);
                    foreach (var direction in new[] { Vector2.right, Vector2.up, Vector2.left, Vector2.down })
                    {
                        // Same EventSystem handlers as a mouse press; sample while held, then release.
                        data.position = point; ExecuteEvents.Execute(receiver, data, ExecuteEvents.pointerDownHandler);
                        data.position = point + direction * Screen.height * .4f; ExecuteEvents.Execute(receiver, data, ExecuteEvents.dragHandler);
                        await Task.Delay(250);
                        var normalized = Field<Vector2>(view, "current");
                        Check(Vector2.Dot(normalized, new Vector2(-direction.x, direction.y)) > .8f, "drag " + direction + " depresses the pushed edge", checks);
                        var frame = Field<RectTransform>(Field<DynamicCardPresentation>(view,"presentation"),"visual");
                        float angle = direction.x == 0 ? Mathf.DeltaAngle(0,frame.localEulerAngles.x) : Mathf.DeltaAngle(0,frame.localEulerAngles.y);
                        Check(direction.x == 0 ? angle * direction.y > 1 : angle * direction.x < -5, "outer frame follows the drag with original angle limits", checks);
                        ScreenCapture.CaptureScreenshot(Path.Combine(Work, "drag-" + (direction == Vector2.right ? "right" : direction == Vector2.left ? "left" : direction == Vector2.up ? "up" : "down") + ".png"));
                        ExecuteEvents.Execute(receiver,data,ExecuteEvents.pointerUpHandler);
                        await Task.Delay(600);
                        Check(Field<Vector2>(view,"current").magnitude < .01f, "release returns smoothly to neutral within 0.6 s", checks);
                    }
                    ScreenCapture.CaptureScreenshot(Path.Combine(Work,"returned.png"));
                    var buttons = page.CardImg.canvas.GetComponentsInChildren<UnityEngine.UI.Button>(true);
                    buttons.First(x=>x.name=="PreviousCard").onClick.Invoke(); await Task.Delay(500);
                    Check(Field<GameObject>(view,"model")==null,"navigation to ordinary version removes dynamic artwork",checks);
                    Check(!Field<bool>(view,"dragging") && !Field<DynamicCardPresentation>(view,"presentation").GetComponentInChildren<DynamicCardDragHandle>().enabled,"ordinary version has no active drag",checks);
                    ScreenCapture.CaptureScreenshot(Path.Combine(Work,"details-ordinary.png"));
                    buttons.First(x=>x!=null && x.name=="NextCard").onClick.Invoke(); await Task.Delay(1800);
                    Check(Field<GameObject>(view,"model")!=null,"navigation back to premium reloads dynamic artwork",checks);
                    if (page.CardInfo.LinkedCards.Count > 0)
                    {
                        page.ScrollContent.GetComponentInChildren<LinkedCard>().UpdateOnButtonClick(); await Task.Delay(400);
                        Check(page.DisplayID!=id,"connected card opens its detail",checks);
                        page.BackButton(); await Task.Delay(1800);
                        Check(page.DisplayID==id && Field<GameObject>(view,"model")!=null,"history back restores premium detail",checks);
                    }
                    page.Closerightclick(); await Task.Delay(650);
                    Check(!SceneManager.GetSceneByName("RightClick").isLoaded && !GameEvent.RighClickActive,"fade close releases the modal scene and input gate",checks);
                    editor.OpenCardDetails(new CardStatus(id){IsPremium=true}); await Task.Delay(2000);
                }
            }
            File.WriteAllText(Path.Combine(Work,"result.json"),JsonConvert.SerializeObject(new { mode, passed=true,card=id,checks },Formatting.Indented));
        }
        catch (Exception e) { File.WriteAllText(Path.Combine(Work,"result.json"),JsonConvert.SerializeObject(new {mode,passed=false,error=e.ToString(),checks},Formatting.Indented)); Debug.LogException(e); }
        finally { running=false; }
    }
    private static void Check(bool condition,string label,List<string> checks) { if(!condition)throw new Exception(label);checks.Add(label); }
    private static void DumpPage(righclickLogic page)
    {
        File.WriteAllText(Path.Combine(Work,"page-layout.json"),JsonConvert.SerializeObject(page.CardImg.canvas.GetComponentsInChildren<UnityEngine.UI.Text>()
            .Where(x=>x.enabled).Select(x=>new {name=x.name,text=x.text,width=x.rectTransform.rect.width,height=x.rectTransform.rect.height,x=x.rectTransform.position.x,y=x.rectTransform.position.y,font=x.font.name,fontSize=x.fontSize}),Formatting.Indented));
    }
}
