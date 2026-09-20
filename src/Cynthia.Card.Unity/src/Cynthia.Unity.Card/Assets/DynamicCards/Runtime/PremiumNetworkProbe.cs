#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Alsein.Extensions.IO;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // Opt-in development-player fixture. Uses the real account, matchmaking and battle UI paths.
    // It cannot run in a release build and never accepts a remote server or arbitrary account.
    public sealed class PremiumNetworkProbe : MonoBehaviour
    {
        const string Work="C:/UnityProjects/LegacyGwent/work/PremiumNetwork/";
        string side, folder; int round;
        GwentClientService client;
        Task pendingInput;
        readonly List<object> samples=new List<object>();
        static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Launch()
        {
            var args=Environment.GetCommandLineArgs();
            string side=args.Contains("-premium-network-a")?"a":args.Contains("-premium-network-b")?"b":null;
            if(side==null)return;
            var go=new GameObject("PremiumNetworkProbe");DontDestroyOnLoad(go);
            var probe=go.AddComponent<PremiumNetworkProbe>();probe.side=side;
            probe.round=args.Contains("-premium-network-round=2")?2:1;
            string run=args.FirstOrDefault(x=>x.StartsWith("-premium-network-run=",StringComparison.Ordinal));
            string output=Work;
            if(run!=null)
            {
                string id=run.Substring("-premium-network-run=".Length);
                if(!System.Text.RegularExpressions.Regex.IsMatch(id,"^[A-Za-z0-9_-]+$"))
                    throw new ArgumentException("Invalid premium-network run id");
                output=Work+"runs/"+id+"/";
            }
            probe.folder=output+"round"+probe.round+"-"+side+"/";
            Directory.CreateDirectory(probe.folder);probe.StartCoroutine(probe.Begin());
        }
        IEnumerator Begin()
        {
            Application.runInBackground=true;Application.targetFrameRate=30;
            DynamicCardSettings.Enabled=true;
            // Allow the ordinary login-page startup/update work to finish before leaving its scene.
            yield return new WaitForSecondsRealtime(12);
            var setup=Setup();yield return new WaitUntil(()=>setup.IsCompleted);
            if(setup.IsFaulted){Fail(setup.Exception);yield break;}
            var match=Match();yield return new WaitUntil(()=>match.IsCompleted);
            if(match.IsFaulted){Fail(match.Exception);yield break;}
            float started=Time.realtimeSinceStartup;int index=0;
            while(Time.realtimeSinceStartup-started<100 && SceneManager.GetActiveScene().name=="GamePlay")
            {
                yield return new WaitForEndOfFrame();
                try
                {
                    var game=FindObjectOfType<GameEvent>();
                    if(game!=null && Time.realtimeSinceStartup-started>18)Drive(game);
                    var cards=FindObjectsOfType<CardShowInfo>().Where(x=>x.CurrentCore!=null).Select(x=>CardSample(x)).ToArray();
                    var opening=FindObjectsOfType<MyCards>().Select(x=>new {path=PathOf(x.transform),view=ViewSample(x.CardImg)}).ToArray();
                    samples.Add(new{t=Time.realtimeSinceStartup-started,frame=Time.frameCount,scene=SceneManager.GetActiveScene().name,operation=game==null?"none":game.NowOperationType.ToString(),cards,opening});
                    File.WriteAllText(folder+"samples.json",JsonConvert.SerializeObject(samples,Formatting.Indented));
                    if(index%4==0)ScreenCapture.CaptureScreenshot(folder+"frame-"+index.ToString("D3")+".png");
                    File.WriteAllText(folder+"status.json",JsonConvert.SerializeObject(new{state="battle",samples=samples.Count,elapsed=Time.realtimeSinceStartup-started}));
                    index++;
                }
                catch(Exception e){Fail(e);yield break;}
                yield return new WaitForSecondsRealtime(.5f);
            }
            File.WriteAllText(folder+"complete.json",JsonConvert.SerializeObject(new{completed=true,samples=samples.Count,scene=SceneManager.GetActiveScene().name}));
        }
        async Task Setup()
        {
            client=DependencyResolver.Container.Resolve<GwentClientService>();
            if(client.User!=null)throw new Exception("Fixture requires a fresh independent client");
            if(client.HubConnection.State!=HubConnectionState.Connected)throw new Exception("Local hub is not connected");
            string username="premium-network-"+side;
            await client.Register(username,"PremiumNetwork2026","Network "+side.ToUpperInvariant());
            var user=await client.Login(username,"PremiumNetwork2026");
            if(user==null)throw new Exception("Local fixture login failed");
            File.WriteAllText(folder+"account.json",JsonConvert.SerializeObject(new{id=user.Id,username,round,side},Formatting.Indented));
            while(!File.Exists(folder+"funded"))await Task.Delay(500);
            await PremiumCollectionClient.Refresh();
            var deck=user.Decks.First(x=>x.IsBasicDeck() && PremiumCollectionClient.Costs.ContainsKey(x.Leader));
            var cards=deck.Deck.Distinct().Where(x=>PremiumCollectionClient.Costs.ContainsKey(x)).OrderBy(x=>x).ToList();
            cards.Add(deck.Leader);
            foreach(string id in cards)
            {
                if(!PremiumCollectionClient.Owns(id))
                {
                    var result=await client.HubConnection.InvokeAsync<PremiumCollectionResult>("CraftPremium",id);
                    if(!result.Success)throw new Exception("Craft "+id+": "+result.Status);
                    PremiumCollectionClient.Accept(result,user.Id);
                }
                bool selected=id==deck.Leader ? (side=="a")== (round==1) : (cards.IndexOf(id)%2==0)==(side=="a");
                var selection=await client.HubConnection.InvokeAsync<PremiumCollectionResult>("SelectPremium",id,selected?1:0);
                if(!selection.Success)throw new Exception("Selection "+id+": "+selection.Status);
                PremiumCollectionClient.Accept(selection,user.Id);
            }
            File.WriteAllText(folder+"prepared.json",JsonConvert.SerializeObject(new{userId=user.Id,side,round,deck,account=PremiumCollectionClient.Account},Formatting.Indented));
            await client.ClearNewlyUnlockedTrinkets(user.UserName);
            SceneManager.LoadScene("Game");await Task.Delay(2000);
            while(!File.Exists(folder+"match"))await Task.Delay(500);
            client.Player.Deck=deck;
        }
        async Task Match()
        {
            var deck=client.Player.Deck;
            if(!await client.NewMatchOfPassword(deck.Id,"premium-network-local-round"+round,0))throw new Exception("Match request refused");
            client.ClientState=ClientState.Match;
            if(!await client.MatchResult())throw new Exception("Match cancelled");
            client.IsAutoPlay=false;
            SceneManager.LoadScene("GamePlay");client.ClientState=ClientState.Play;
            await Task.Delay(100);
        }
        void Drive(GameEvent game)
        {
            if(pendingInput!=null && !pendingInput.IsCompleted)return;
            if(pendingInput!=null && pendingInput.IsFaulted)throw pendingInput.Exception;
            var menu=game.GameCardShowControl;
            string type=typeof(GameCardShowControl).GetField("_nowUseMenuType",Private).GetValue(menu).ToString();
            if(type=="Mulligan"){menu.MulliganEndButtonClick();return;}
            if(type=="Select")
            {
                for(int i=0;i<menu.UseCardList.Count && menu.NowSelect.Count<menu.NowSelectTotal;i++)
                    if(!menu.NowSelect.Contains(i))menu.ClickCard(i);
                return;
            }
            // Feed the same UI input channel used by drag/drop. The normal client and server
            // still validate, serialize, broadcast and render every action.
            var sender=(ITubeInlet)typeof(GameEvent).GetField("sender",Private).GetValue(game);
            switch(game.NowOperationType)
            {
                case GameOperationType.GetPassOrGrag:
                    var indices=game.HandCanPlay().Where(x=>x>=0).ToArray();
                    if(indices.Length==0){pendingInput=sender.SendAsync(new RoundInfo{IsPass=true});break;}
                    int index=indices[0];
                    var card=game.GetCard(new CardLocation{RowPosition=RowPosition.MyHand,CardIndex=index});
                    var location=game.CardCanPlay(card.CardUseInfo).FirstOrDefault()??new CardLocation{RowPosition=RowPosition.MyCemetery,CardIndex=0};
                    pendingInput=sender.SendAsync(new RoundInfo{IsPass=false,HandCardIndex=index,CardLocation=location});break;
                case GameOperationType.SelectRow:
                    pendingInput=sender.SendAsync(game.CanSelectRow.First());break;
                case GameOperationType.SelectCards:
                    var info=game.SelectPlaceCardsInfo;
                    pendingInput=sender.SendAsync<IList<CardLocation>>(info.CanSelect.CardsPartToLocation().Take(info.SelectCount).ToList());break;
                case GameOperationType.PlayCard:
                    pendingInput=sender.SendAsync(game.CardCanPlay(GwentMap.CardMap[game.CurrentPlayCard.CardId].CardUseInfo).First());break;
            }
        }
        object CardSample(CardShowInfo card)
        {
            var status=card.CurrentCore;
            return new{instance=card.GetInstanceID(),path=PathOf(card.transform),card=status.CardId,premium=status.IsPremium,back=status.IsCardBack,
                view=ViewSample(card.CardImg)};
        }
        object ViewSample(Image art)
        {
            if(art==null)return null;
            var view=art.GetComponent<DynamicCardView>();
            var model=view==null?null:(GameObject)typeof(DynamicCardView).GetField("model",Private).GetValue(view);
            var texture=view==null?null:(RenderTexture)typeof(DynamicCardView).GetField("texture",Private).GetValue(view);
            var surface=view==null?null:(RawImage)typeof(DynamicCardView).GetField("surface",Private).GetValue(view);
            string hash=null;
            if(texture!=null && texture.IsCreated() && model!=null && model.activeInHierarchy && surface!=null && surface.isActiveAndEnabled)
            {
                var old=RenderTexture.active;var small=RenderTexture.GetTemporary(64,64,0);
                Graphics.Blit(texture,small);RenderTexture.active=small;
                var pixels=new Texture2D(64,64,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,64,64),0,0);pixels.Apply();
                var bytes=pixels.GetRawTextureData<byte>();uint sum=2166136261;
                for(int i=0;i<bytes.Length;i++)sum=(sum^bytes[i])*16777619;
                hash=sum.ToString("x8");Destroy(pixels);RenderTexture.active=old;RenderTexture.ReleaseTemporary(small);
            }
            return new{active=art.isActiveAndEnabled,sprite=art.sprite==null?null:art.sprite.name,view=view!=null,
                allowed=view!=null && (bool)typeof(DynamicCardView).GetField("premiumAllowed",Private).GetValue(view),
                model=model!=null && model.activeInHierarchy,surface=surface!=null && surface.isActiveAndEnabled,
                age=view==null?0:(float)typeof(DynamicCardView).GetField("age",Private).GetValue(view),hash};
        }
        static string PathOf(Transform t){return t.parent==null?t.name:PathOf(t.parent)+"/"+t.name;}
        void Fail(Exception e){File.WriteAllText(folder+"error.txt",e.ToString());Debug.LogException(e);}
    }
}
#endif
