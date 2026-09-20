using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Assets.Script.DynamicCards;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class InPlaceCraftVerification
{
    static readonly string Work = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../work/InPlaceCraft"));
    static bool running;
    static readonly List<string> checks = new List<string>();
    static readonly List<object> frames = new List<object>();
    static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    static void Check(bool ok, string text) { if (!ok) throw new Exception(text); checks.Add(text); File.WriteAllLines(Path.Combine(Work,"progress.txt"),checks); }
    static async Task Until(Func<bool> condition, string label, int seconds = 60)
    { var end=DateTime.UtcNow.AddSeconds(seconds); while(!condition()){if(DateTime.UtcNow>end)throw new TimeoutException(label);await Task.Delay(30);} }
    static Vector2 Center(RectTransform rect) { var corners=new Vector3[4];rect.GetWorldCorners(corners);return (corners[0]+corners[2])*.5f; }
    static InPlaceCraftVerification() { EditorApplication.update += Poll; }
    static async void Poll()
    {
        var request=Path.Combine(Work,"check.request");
        if(running || !EditorApplication.isPlaying || EditorApplication.isCompiling || !File.Exists(request))return;
        File.Delete(request);running=true;checks.Clear();frames.Clear();
        bool background=Application.runInBackground,enabled=DynamicCardSettings.Enabled;
        try
        {
            Application.runInBackground=true;DynamicCardSettings.Enabled=true;
            if(!EditorPrefs.GetBool("LegacyGwent.UseLocalServer."+Application.dataPath,true))throw new Exception("Local server required");
            await Until(()=>DependencyResolver.Container!=null,"bootstrap");
            var client=DependencyResolver.Container.Resolve<GwentClientService>();
            if(client.User!=null && client.User.UserName!="inplace-craft-test")throw new Exception("An unrelated user is logged in");
            if(client.HubConnection.State==HubConnectionState.Disconnected)await client.HubConnection.StartAsync();
            await client.Register("inplace-craft-test","InPlaceCraft2026","In Place Craft Test");
            if(client.User==null)await client.Login("inplace-craft-test","InPlaceCraft2026");
            Check(client.User!=null,"dedicated local fixture login");
            await client.HubConnection.InvokeAsync<bool>("ClearNewlyUnlockedTrinkets",client.User.UserName);
            SceneManager.LoadScene("Game");
            EditorInfo editor=null;
            await Until(()=>(editor=Resources.FindObjectsOfTypeAll<EditorInfo>().FirstOrDefault(x=>x.gameObject.scene==SceneManager.GetActiveScene()))!=null,"Game scene");
            await PremiumCollectionClient.Refresh();
            editor.MainUI.SetActive(false);editor.EditorUI.SetActive(true);editor.OpenEditor(false);
            await Task.Delay(700);
            Check(editor.PremiumFilter==2,"collection defaults to paired ordinary and premium cards");
            var deck=client.User.Decks.First(x=>x.Id!="blacklist");
            var faction=GwentMap.CardMap[deck.Leader].Faction;
            var ids=PremiumCollectionClient.Costs.Where(x=>x.Value==100 && !PremiumCollectionClient.Owns(x.Key) &&
                GwentMap.CardMap[x.Key].Group==Group.Copper && GwentMap.CardMap[x.Key].Faction==faction).Take(3).Select(x=>x.Key).ToArray();
            Check(ids.Length==3,"three available uncrafted bronze card types");
            editor.SetShowCardInfo(ids.Select(x=>new CardStatus(x)).ToList());
            await Task.Delay(100);
            var cards=editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
            await Until(()=>cards.All(x=>x.GetComponent<CanvasGroup>().alpha>0),"static art ready");
            Check(cards.Length==6 && Enumerable.Range(0,3).All(i=>cards[i*2].CurrentCore.CardId==ids[i] &&
                cards[i*2].CurrentCore.IsPremium==false && cards[i*2+1].CurrentCore.CardId==ids[i] && cards[i*2+1].CurrentCore.IsPremium==true),
                "one ordinary card followed immediately by one premium counterpart, without copy counters");
            var locked=cards[1];var normal=cards[0];
            Check(Center(locked.CardImg.rectTransform).x>Center(normal.CardImg.rectTransform).x &&
                Mathf.Abs(Center(locked.CardImg.rectTransform).y-Center(normal.CardImg.rectTransform).y)<2,"premium counterpart is on the right in the same row");
            Check(locked.CardImg.material.shader.name=="LegacyGwent/UI/LockedPremium" && locked.CardBorder.material.shader.name=="LegacyGwent/UI/LockedPremium",
                "locked artwork and border use a real grayscale shader");
            Check(normal.CardImg.GetComponent<PremiumCardAppearance>()==null && Field<GameObject>(locked.CardImg.GetComponent<DynamicCardView>(),"model")==null,
                "ordinary card stays colored while unowned premium has no animated model");
            var battle=SceneManager.CreateScene("GamePlay");
            PremiumCardAppearance.Apply(locked.CardImg,locked.CurrentCore,locked.CardBorder,locked.FactionIcon);
            Check(!locked.CardImg.GetComponent<PremiumCardAppearance>().IsLocked,"battle premium appearance never depends on the observer owning that card");
            var unload=SceneManager.UnloadSceneAsync(battle);await Until(()=>unload.isDone,"temporary battle scene cleanup");
            PremiumCardAppearance.Apply(locked.CardImg,locked.CurrentCore,locked.CardBorder,locked.FactionIcon);
            Check(locked.CardImg.GetComponent<PremiumCardAppearance>().IsLocked,"collection restores its own locked-card grayscale after returning from battle");
            locked.GetComponent<EditorUIShowCard>().OnPointerClick(new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left});
            Check(editor.PremiumPanel.CurrentCardId==ids[0] && editor.ShowArtCard.CurrentCore.IsPremium==true,"clicking locked premium selects it for crafting in the collection");
            await Task.Delay(300);
            var preview=editor.ShowArtCard;
            Check(preview.CardImg.material.shader.name=="LegacyGwent/UI/LockedPremium","selected locked portrait is also grayscale");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work,"before.png"));await Task.Delay(200);
            long before=PremiumCollectionClient.Account.MeteoritePowder;
            var denied=await PremiumCollectionClient.Craft("__unavailable_card__");
            Check(denied.Status=="unavailable" && PremiumCollectionClient.Account.MeteoritePowder==before && locked.CardImg.GetComponent<PremiumCardAppearance>().IsLocked,
                "rejected crafting preserves the gray card and balance without an animation");
            int instance=locked.GetInstanceID();var position=Center(locked.CardImg.rectTransform);float scroll=editor.ShowCardScroll.value;
            editor.OpenCardDetails(new CardStatus(ids[0]){IsPremium=true});
            await Until(()=>UnityEngine.Object.FindObjectOfType<righclickLogic>()!=null,"premium details opened for crafting");
            await Task.Delay(500);
            editor.PremiumPanel.CraftClicked();editor.PremiumPanel.CraftClicked();
            await Until(()=>PremiumCollectionClient.Owns(ids[0]),"server craft");
            Check(Resources.FindObjectsOfTypeAll<RectMask2D>().Any(x=>x.name=="PremiumCraftInPlace" && x.gameObject.activeInHierarchy),
                "craft glow is clipped to its card rather than spilling into the page");
            bool lostCard=false,overlay=false,grayDuringCharge=false,modelSeen=false;float drift=0;
            var start=Time.realtimeSinceStartup;
            while(editor.PremiumPanel.Busy || Time.realtimeSinceStartup-start<8)
            {
                if(Time.realtimeSinceStartup-start>70)throw new TimeoutException("in-place reveal");
                if(locked==null || locked.GetInstanceID()!=instance){lostCard=true;break;}
                drift=Mathf.Max(drift,Vector2.Distance(position,Center(locked.CardImg.rectTransform)));
                if(locked.GetComponent<CanvasGroup>().alpha<=0)lostCard=true;
                overlay|=GameObject.Find("PremiumTransmutation")!=null;
                bool animated=Field<GameObject>(locked.CardImg.GetComponent<DynamicCardView>(),"model")!=null;
                bool gray=locked.CardImg.GetComponent<PremiumCardAppearance>().IsLocked;
                grayDuringCharge|=gray;modelSeen|=animated;
                frames.Add(new{time=Time.realtimeSinceStartup-start,animated,gray,alpha=locked.GetComponent<CanvasGroup>().alpha,drift});
                if(frames.Count==15)ScreenCapture.CaptureScreenshot(Path.Combine(Work,"charge.png"));
                await Task.Delay(50);
            }
            await Until(()=>Field<GameObject>(locked.CardImg.GetComponent<DynamicCardView>(),"model")!=null && !locked.CardImg.GetComponent<PremiumCardAppearance>().IsLocked,"premium first frame");
            Check(!lostCard && locked.GetInstanceID()==instance,"craft retains the same visible card instance throughout loading and reveal");
            Check(!overlay && drift<2 && editor.ShowCardScroll.value==scroll,"no central craft page, position change or scroll reset");
            Check(grayDuringCharge && modelSeen,"in-place charge transitions from grayscale to a live model");
            Check(PremiumCollectionClient.Account.MeteoritePowder==before-100 && PremiumCollectionClient.Account.OwnedCards.Count(x=>x==ids[0])==1 && PremiumCollectionClient.Selected(ids[0]),
                "double click charges once and unlocks the entire card type");
            Check(Field<GameObject>(normal.CardImg.GetComponent<DynamicCardView>(),"model")==null,"ordinary counterpart remains static after crafting");
            Check(Field<GameObject>(preview.CardImg.GetComponent<DynamicCardView>(),"model")!=null && !preview.CardImg.GetComponent<PremiumCardAppearance>().IsLocked,"existing side portrait reveals premium in place");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work,"after.png"));await Task.Delay(200);
            var repeat=await PremiumCollectionClient.Craft(ids[0]);
            Check(repeat.Status=="already_owned" && PremiumCollectionClient.Account.MeteoritePowder==before-100,"a second craft attempt cannot charge for another bronze copy");
            editor.ShowDeckEditorClick(deck.Id);editor.SetPremiumFilter(2);editor.SetEditorCardInfo(new[]{new CardStatus(ids[0])});
            await Task.Delay(400);
            var premium=editor.EditorCardsContext.GetComponentsInChildren<EditorUICoreCard>().Single(x=>x.cardShowInfo.CurrentCore.IsPremium==true);
            Check(!premium.CountIcon.activeSelf,"premium tile does not display an X3 ownership/crafting counter");
            editor.ClickEditorUICoreCard(new CardStatus(ids[0]){IsPremium=false});await Task.Delay(350);
            Check(PremiumCollectionClient.Selected(ids[0]),"adding ordinary comparison tile does not turn off the crafted premium selection");
            File.WriteAllText(Path.Combine(Work,"result.json"),JsonConvert.SerializeObject(new{passed=true,checks,card=ids[0],before,after=PremiumCollectionClient.Account.MeteoritePowder,frames},Formatting.Indented));
        }
        catch(Exception e){File.WriteAllText(Path.Combine(Work,"result.json"),JsonConvert.SerializeObject(new{passed=false,checks,error=e.ToString(),frames},Formatting.Indented));Debug.LogException(e);}
        finally{Application.runInBackground=background;DynamicCardSettings.Enabled=enabled;running=false;}
    }
}
