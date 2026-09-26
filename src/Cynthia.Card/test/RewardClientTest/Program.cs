using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Assets.Script.DynamicCards;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

class Program
{
    static int failures;
    static readonly List<object> checks=new List<object>();
    static void Check(bool ok,string label){checks.Add(new{passed=ok,label});if(!ok)failures++;Console.WriteLine((ok?"PASS ":"FAIL ")+label);}
    static GwentClientService Fresh()
    {
        var c=new GwentClientService{User=new UserInfo{UserName="client-test"}};
        DependencyResolver.Container.Client=c;UnityEngine.Time.realtimeSinceStartup=0;PremiumCollectionClient.Reset();return c;
    }
    static PremiumCollectionResult Wallet(UserInfo u,long revision,int crowns=0,long powder=10)=>new PremiumCollectionResult
    {Status="ok",Collection=new PremiumCollection{Id=u.Id,Revision=revision,MeteoritePowder=powder,DailyQuests=new DailyQuestProgress{Day="2026-09-14",LoginGranted=true,Crowns=crowns,PowderGranted=10}},Costs=new Dictionary<string,int>{{"card",100}}};
    static DailyQuestResult Daily(UserInfo u,long revision=1,int crowns=0)=>new DailyQuestResult
    {Status="ok",ServerUtc="2026-09-14T15:59:00Z",ResetUtc="2026-09-14T16:00:00Z",Wallet=Wallet(u,revision,crowns)};
    static TaskCompletionSource<object> Pending()=>new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
    static async Task Main(string[] args)
    {
        try
        {
            void Content(string value)
            {
                UnityEngine.Resources.Content = value == null ? null : new UnityEngine.TextAsset { text = value };
                typeof(ClientContent).GetField("premium", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, null);
            }
            Content(null);
            Check(!ClientContent.HasPremiumContent, "missing build marker defaults to standard capabilities");
            Content("{\"schema\":1,\"variant\":\"standard\"}");
            UnityEngine.PlayerPrefs.SetInt("DynamicCards.Quality",3);
            DynamicCardSettings.Quality = DynamicCardQuality.Low;
            Check(DynamicCardSettings.Quality == DynamicCardQuality.Off && UnityEngine.PlayerPrefs.GetInt("DynamicCards.Quality",0)==3,
                "standard package disables animation without overwriting premium quality preferences");
            var sourceDeck = new DeckModel { Name="saved", Leader="leader", Deck=new List<string>{"card"},
                PremiumCards=new Dictionary<string,int>{{"card",1}}, PremiumLeader=true };
            var wireDeck=ClientContent.ForServer(sourceDeck);
            Check(wireDeck.PremiumCards==null && wireDeck.PremiumLeader==null && wireDeck.Id==sourceDeck.Id && wireDeck.Deck[0]=="card",
                "standard deck request omits appearance fields and preserves gameplay data");
            Check(sourceDeck.PremiumLeader==true && sourceDeck.PremiumCards["card"]==1 && !ReferenceEquals(wireDeck.Deck,sourceDeck.Deck),
                "standard serialization does not mutate the saved premium deck");
            Content("{\"schema\":2,\"variant\":\"premium\"}");
            Check(!ClientContent.HasPremiumContent,"unsupported capability schema fails closed");
            Content("{\"schema\":1,\"variant\":\"premium\"}");
            Check(ClientContent.HasPremiumContent && ReferenceEquals(ClientContent.ForServer(sourceDeck),sourceDeck),
                "premium package keeps explicit deck appearance fields");
            Check(DynamicCardSettings.Quality==DynamicCardQuality.High,"premium package restores existing quality choice");
            UnityEngine.Application.isMobilePlatform=true; UnityEngine.SystemInfo.graphicsShaderLevel=20;
            Check(!ClientContent.CanAnimate && DynamicCardSettings.Quality==DynamicCardQuality.Off,"unsupported mobile graphics uses static fallback");
            UnityEngine.SystemInfo.graphicsShaderLevel=35;
            Check(ClientContent.CanAnimate,"ES3 capable premium player may animate");
            UnityEngine.Application.isMobilePlatform=false;
            var c=Fresh();PremiumCollectionClient.Accept(Wallet(c.User,3,2,20),c.User.Id);
            PremiumCollectionClient.Accept(Wallet(c.User,2,0,10),c.User.Id);
            Check(PremiumCollectionClient.Account.Revision==3 && PremiumCollectionClient.Account.MeteoritePowder==20,"older wallet response cannot roll back new balance or crowns");
            var foreign=new UserInfo();PremiumCollectionClient.Accept(Wallet(foreign,100),foreign.Id);
            Check(PremiumCollectionClient.Account.Id==c.User.Id,"foreign account response cannot replace active wallet");

            c=Fresh();var pending=Pending();c.HubConnection.Respond=(m,a)=>pending.Task;
            var refresh=DailyQuestClient.Refresh(true);var also=DailyQuestClient.Refresh();
            Check(c.HubConnection.Calls==1,"simultaneous task refreshes share one in-flight request");
            pending.SetResult(Daily(c.User));await Task.WhenAll(refresh,also);
            Check(DailyQuestClient.State.Success && PremiumCollectionClient.Ready,"successful task response updates task and wallet state");
            UnityEngine.Time.realtimeSinceStartup=10;
            Check(Math.Abs(DailyQuestClient.RemainingSeconds-50)<.01,"countdown follows monotonic elapsed time");
            UnityEngine.Time.realtimeSinceStartup=100;
            Check(DailyQuestClient.RemainingSeconds==0,"expired countdown is clamped to zero");

            c=Fresh();pending=Pending();c.HubConnection.Respond=(m,a)=>pending.Task;
            var firstUser=c.User;refresh=DailyQuestClient.Refresh(true);
            c.User=new UserInfo();PremiumCollectionClient.Reset();
            pending.SetResult(Daily(firstUser));await refresh;
            Check(DailyQuestClient.State==null && !PremiumCollectionClient.Ready,"late A response after switching to B is discarded");

            c=Fresh();pending=Pending();c.HubConnection.Respond=(m,a)=>pending.Task;
            firstUser=c.User;refresh=DailyQuestClient.Refresh(true);
            PremiumCollectionClient.Reset();c.User=new UserInfo();PremiumCollectionClient.Reset();c.User=firstUser;
            pending.SetResult(Daily(firstUser));await refresh;
            Check(DailyQuestClient.State==null,"A-to-B-to-A switch still rejects prior session response");

            c=Fresh();pending=Pending();c.HubConnection.Respond=(m,a)=>pending.Task;
            var craft=PremiumCollectionClient.Craft("card");firstUser=c.User;
            c.User=new UserInfo();PremiumCollectionClient.Reset();pending.SetResult(Wallet(firstUser,2));
            Check((await craft).Status=="session_changed" && !PremiumCollectionClient.Ready,"late craft response cannot unlock cards for another session");

            c=Fresh();c.HubConnection.Respond=(m,a)=>Task.FromException<object>(new Exception("transport failed"));
            await DailyQuestClient.Refresh(true);
            Check(DailyQuestClient.Error!=null,"transport failure exposes synchronization error");
            c.HubConnection.Respond=(m,a)=>Task.FromResult<object>(Daily(c.User));await DailyQuestClient.Refresh(true);
            Check(DailyQuestClient.Error==null && DailyQuestClient.State.Success,"refresh recovers after transport failure");

            c=Fresh();c.HubConnection.State=HubConnectionState.Disconnected;c.HubConnection.Respond=(m,a)=>Task.FromResult<object>(Daily(c.User));
            await DailyQuestClient.Refresh(true);Check(c.HubConnection.Calls==0,"disconnected refresh does not issue transport request");
            c.HubConnection.State=HubConnectionState.Connected;await DailyQuestClient.Refresh(true);
            Check(DailyQuestClient.State!=null,"reconnection can synchronize after skipped disconnected refresh");
            await DailyQuestClient.Refresh();Check(c.HubConnection.Calls==1,"ordinary refresh respects five-second throttle");

            c=Fresh();pending=Pending();c.HubConnection.Respond=(m,a)=>c.HubConnection.Calls==1?pending.Task:Task.FromResult<object>(Daily(c.User,2,1));
            refresh=DailyQuestClient.Refresh(true);var notification=DailyQuestClient.Refresh(true);
            pending.SetResult(Daily(c.User,1,0));await Task.WhenAll(refresh,notification);
            Check(c.HubConnection.Calls>=2 && PremiumCollectionClient.Account.DailyQuests.Crowns==1,
                "award notification during an in-flight old snapshot must schedule a fresh fetch");

            c=Fresh();pending=Pending();c.HubConnection.Respond=(m,a)=>c.HubConnection.Calls==1?pending.Task:Task.FromResult<object>(Daily(c.User,2,1));
            refresh=DailyQuestClient.Refresh(true);
            var burst=new List<Task>{refresh};
            for(int i=0;i<20;i++)burst.Add(DailyQuestClient.Refresh(true));
            pending.SetResult(Daily(c.User,1));await Task.WhenAll(burst);
            Check(c.HubConnection.Calls==2,"burst of in-flight reward notifications coalesces into one fresh request");

            c=Fresh();pending=Pending();var oldUser=c.User;c.HubConnection.Respond=(m,a)=>pending.Task;
            refresh=DailyQuestClient.Refresh(true);notification=DailyQuestClient.Refresh(true);
            c.User=new UserInfo();PremiumCollectionClient.Reset();
            c.HubConnection.Respond=(m,a)=>Task.FromResult<object>(Daily(c.User,3,2));
            await DailyQuestClient.Refresh(true);pending.SetResult(Daily(oldUser,1));await Task.WhenAll(refresh,notification);
            Check(c.HubConnection.Calls==2 && PremiumCollectionClient.Account.Id==c.User.Id && PremiumCollectionClient.Account.DailyQuests.Crowns==2,
                "session reset discards queued old-account refresh without clearing new session state");

            c=Fresh();c.HubConnection.Respond=(m,a)=>Task.FromResult<object>(Daily(c.User));
            await DailyQuestClient.Refresh();UnityEngine.Time.realtimeSinceStartup=6;await DailyQuestClient.Refresh();
            Check(c.HubConnection.Calls==2,"synchronous request completion releases pending state for later refresh");

            c=Fresh();var gg=Wallet(c.User,3,0,15);
            gg.Collection.DailyQuests.GGReceived=1;gg.Collection.DailyQuests.GGPowderGranted=5;
            PremiumCollectionClient.Accept(gg,c.User.Id);PremiumCollectionClient.Accept(Wallet(c.User,2),c.User.Id);
            Check(PremiumCollectionClient.Account.DailyQuests.GGReceived==1 && PremiumCollectionClient.Account.MeteoritePowder==15,
                "older wallet snapshot cannot undo received GG progress or powder");
            c=Fresh();pending=Pending();var rewarded=Daily(c.User,2);
            rewarded.GGPowder=5;rewarded.GGDailyCap=30;rewarded.DailyCap=155;
            rewarded.Wallet.Collection.DailyQuests.GGReceived=1;rewarded.Wallet.Collection.DailyQuests.GGPowderGranted=5;
            rewarded.Wallet.Collection.MeteoritePowder=15;
            c.HubConnection.Respond=(m,a)=>c.HubConnection.Calls==1?pending.Task:Task.FromResult<object>(rewarded);
            refresh=DailyQuestClient.Refresh(true);notification=DailyQuestClient.Refresh(true);
            pending.SetResult(Daily(c.User,1));await Task.WhenAll(refresh,notification);
            Check(c.HubConnection.Calls==2 && PremiumCollectionClient.Account.DailyQuests.GGReceived==1 &&
                PremiumCollectionClient.Account.MeteoritePowder==15 && DailyQuestClient.State.GGDailyCap==30,
                "GG notification during stale fetch refreshes both task progress and wallet");
            PremiumCollectionClient.Reset();
            Check(DailyQuestClient.State==null && PremiumCollectionClient.Account==null,"logout clears GG and wallet state");

        }
        catch(Exception e){Check(false,"client suite completed without exception: "+e);}
        var result=new{passed=failures==0,finished=true,failed=failures,count=checks.Count,checks};
        if(args.Length>0)File.WriteAllText(args[0],JsonConvert.SerializeObject(result,Formatting.Indented));
        Console.WriteLine("COMPLETE checks="+checks.Count+" failed="+failures);Environment.ExitCode=failures==0?0:1;
    }
}
