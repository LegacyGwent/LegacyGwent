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
        }
        catch(Exception e){Check(false,"client suite completed without exception: "+e);}
        var result=new{passed=failures==0,finished=true,failed=failures,count=checks.Count,checks};
        if(args.Length>0)File.WriteAllText(args[0],JsonConvert.SerializeObject(result,Formatting.Indented));
        Console.WriteLine("COMPLETE checks="+checks.Count+" failed="+failures);Environment.ExitCode=failures==0?0:1;
    }
}
