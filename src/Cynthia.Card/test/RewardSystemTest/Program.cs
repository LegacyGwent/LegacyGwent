using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SuiteStartup
{
    public static long InitialAmount;
    public static bool RunInitialBackfill;
    readonly Startup production;
    public SuiteStartup(IWebHostEnvironment env){production=new Startup(env);}
    public void ConfigureServices(IServiceCollection services)
    {
        production.ConfigureServices(services);
        services.RemoveAll<IMongoClient>();
        services.AddSingleton<IMongoClient>(new MongoClient(
            Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121"));
        services.RemoveAll<InitialPowderOptions>();
        services.AddSingleton(new InitialPowderOptions(InitialAmount));
        // Keep only the reward workers under test; omit seasons and the midnight timer.
        foreach(var d in services.Where(x=>x.ServiceType==typeof(IHostedService)).ToArray())services.Remove(d);
        services.AddSingleton<IHostedService>(provider=>provider.GetRequiredService<RewardSettlementService>());
        services.AddSingleton<IHostedService>(provider=>provider.GetRequiredService<PremiumDeckSelectionService>());
        if(RunInitialBackfill)
            services.AddSingleton<IHostedService>(provider=>provider.GetRequiredService<InitialPowderGrantService>());
    }
    public void Configure(IApplicationBuilder app,IWebHostEnvironment env,ILogger<Startup> logger)=>production.Configure(app,env,logger);
}
partial class Program
{
    static string root,work,url,lan;
    static long ticks;
    static DateTimeOffset Now {get=>new DateTimeOffset(Interlocked.Read(ref ticks),TimeSpan.Zero);set=>Interlocked.Exchange(ref ticks,value.UtcTicks);}
    static IWebHost host;
    static GwentDatabaseService db;
    static readonly List<object> results=new List<object>();
    static int failed;
    static bool finished;
    static bool runMongoRestart;
    static IMongoCollection<UserInfo> Users=>db.GetMongoClient().GetDatabase("gwentdiy").GetCollection<UserInfo>("user");
    static IMongoCollection<PremiumCollection> Accounts=>db.GetMongoClient().GetDatabase("gwentdiy").GetCollection<PremiumCollection>("premium_collection");
    static void Check(bool ok,string label,object actual=null)
    {
        if(!ok)failed++;
        results.Add(new{passed=ok,label,actual});Console.WriteLine((ok?"PASS ":"FAIL ")+label);
        Save();
    }
    static void Save()=>File.WriteAllText(Path.Combine(work,"results.json"),JsonConvert.SerializeObject(new{passed=finished && failed==0,finished,failed,count=results.Count,lanAddress=url,checks=results},Formatting.Indented));
    static async Task Group(string name,Func<Task> run){Console.WriteLine("SCENARIO "+name);try{await run();}catch(Exception e){Check(false,name+" completed without exception",e.ToString());}}
    static async Task Main(string[] args)
    {
        if(args.Length>0 && args[0]=="--config-probe"){await ConfigProbe(args[1]);return;}
        root=Path.GetFullPath(args[0]);work=Path.GetFullPath(args[1]);lan=args[2];
        runMongoRestart=args.Contains("mongo-restart");
        if(!System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().SelectMany(x=>x.GetIPProperties().UnicastAddresses).Any(x=>x.Address.ToString()==lan))
            throw new Exception("LAN address must belong to this test machine");
        Directory.CreateDirectory(work);url="ws://"+lan+":5016/hub/gwent";
        Directory.SetCurrentDirectory(Path.Combine(root,"src/Cynthia.Card/src/Cynthia.Card.Server"));
        Now=DateTimeOffset.Parse("2026-09-13T15:59:59Z");
        try
        {
            await StartHost();
            await Group("LAN login / reset / cap",NetworkDaily);
            await Group("per-copy inventory, crafting and deck versions",PremiumCopyCases);
            await Group("calendar boundaries and offline gaps",Calendar);
            await Group("processing-day attribution and cross-day retries",ProcessingDay);
            await Group("strict network schema and input boundaries",WireContracts);
            await Group("multi-service and mixed-wallet concurrency",MixedConcurrency);
            await Group("legacy null fields and long deduplication history",HistoryAndLegacy);
            if(args.Contains("failpoints"))await Group("database failures and notification isolation",FaultBoundaries);
            await Group("round lifecycle and surrender rewards",RoundLifecycle);
            await Group("production room reward eligibility",RoomEligibility);
            await Group("configuration validation in fresh processes",ConfigurationCases);
            await Group("concurrency and deduplication",Concurrent);
            await Group("administrator account grants",Admin);
            await Group("craft / reward / admin transaction composition",Composition);
            await Group("actual round settlement",RoundSettlement);
            await Group("server and database restart persistence",Restart);
            if(args.Contains("full"))await Group("all standalone reward regression programs",ExternalRegressions);
            if(args.Contains("ui"))await Group("Unity client cross-day and wallet UI",ClientUi);
            await Group("one-time initial powder grants",InitialPowderCases);
            finished=true;
        }
        catch(Exception e){Check(false,"suite host initialization",e.ToString());}
        finally {if(host!=null){await host.StopAsync();host.Dispose();}Save();}
        Console.WriteLine("COMPLETE checks="+results.Count+" failed="+failed);
        Environment.ExitCode=failed==0?0:1;
    }
    static async Task StartHost()
    {
        host=WebHost.CreateDefaultBuilder().UseStartup<SuiteStartup>().UseUrls("http://"+lan+":5016")
            .ConfigureLogging(x=>x.ClearProviders()).Build();
        db=host.Services.GetRequiredService<GwentDatabaseService>();db.DailyQuestClock=()=>Now;
        Command.MongodbConnect(db);await host.StartAsync();
    }
    static async Task<UserInfo> NewUser(string prefix)
    {
        var u=new UserInfo{UserName="reward-"+prefix+"-"+Guid.NewGuid().ToString("N").Substring(0,10),PlayerName="Reward "+Guid.NewGuid().ToString("N"),PassWord="RewardTest2026",Decks=new List<DeckModel>()};
        await Users.InsertOneAsync(u);return u;
    }
    static async Task<UserInfo> Login(WireClient c,string label)
    {
        string name="reward-"+label+"-"+Guid.NewGuid().ToString("N").Substring(0,8);
        if(!await c.Invoke<bool>("Register",name,"RewardTest2026",name))throw new Exception("Register failed");
        return await c.Invoke<UserInfo>("Login",name,"RewardTest2026");
    }
    static Task<PremiumCollection> Account(UserInfo u)=>Accounts.Find(x=>x.Id==u.Id).FirstAsync();
    static async Task<bool> Reject(WireClient c,string name,params object[] args)
    {try{await c.Invoke<object>(name,args);return false;}catch(InvalidOperationException){return true;}}
    static async Task NetworkDaily()
    {
        Now=DateTimeOffset.Parse("2026-09-13T15:59:59Z");
        using(var a=await WireClient.Connect(url))
        using(var b=await WireClient.Connect(url))
        using(var unauth=await WireClient.Connect(url))
        {
            Check((await unauth.Invoke<DailyQuestResult>("GetDailyQuests")).Status=="unauthenticated","LAN unauthenticated daily access denied");
            Check((await unauth.Invoke<PremiumCollectionResult>("CraftPremium","14002")).Status=="unauthenticated","LAN unauthenticated spending denied");
            var user=await Login(a,"lan-a");var other=await Login(b,"lan-b");
            var loginDeadline=DateTimeOffset.UtcNow.AddSeconds(10);
            while(DateTimeOffset.UtcNow<loginDeadline && (await Accounts.Find(x=>x.Id==user.Id).FirstOrDefaultAsync())?.MeteoritePowder!=20)await Task.Delay(25);
            Check((await Account(user)).MeteoritePowder==20,"LAN successful login queues the server-only daily reward");
            var wrong=await unauth.Invoke<UserInfo>("Login",user.UserName,"incorrect");
            Check(wrong==null && (await unauth.Invoke<DailyQuestResult>("GetDailyQuests")).Status=="unauthenticated","LAN failed login grants no access or powder");
            var state=await a.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.Id==user.Id && state.Wallet.Collection.DailyQuests.Day=="2026-09-13","LAN identity and China server day");
            Check((DateTimeOffset.Parse(state.ResetUtc)-DateTimeOffset.Parse(state.ServerUtc)).TotalSeconds==1,"LAN countdown is one second before China midnight");
            var refresh=await Task.WhenAll(Enumerable.Range(0,32).Select(_=>a.Invoke<DailyQuestResult>("GetDailyQuests")));
            Check(refresh.All(x=>x.Wallet.Collection.MeteoritePowder==20),"32 LAN refreshes do not repeat login reward");
            await a.Invoke<UserInfo>("Login",user.UserName,"RewardTest2026");
            Check((await Account(user)).MeteoritePowder==20,"same-connection relogin does not repeat reward");
            Now=Now.AddSeconds(1);
            state=await a.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.MeteoritePowder==40 && state.Wallet.Collection.DailyQuests.PowderGranted==20,"online client refresh across China midnight grants exactly next login");
            Check(state.Wallet.Collection.DailyQuests.Crowns==0 && (DateTimeOffset.Parse(state.ResetUtc)-DateTimeOffset.Parse(state.ServerUtc)).TotalHours==24,"midnight resets crowns and countdown to 24 hours");
            long[] expected={40,65,65,100,100,145};
            for(int i=0;i<6;i++)
            {
                await db.AwardDailyCrown(user.UserName,"lan-round:"+i,Now);
                state=await a.Invoke<DailyQuestResult>("GetDailyQuests");
                Check(state.Wallet.Collection.MeteoritePowder==expected[i] && state.Wallet.Collection.DailyQuests.Crowns==i+1,"LAN reflects crown tier "+(i+1));
            }
            await Task.WhenAll(Enumerable.Range(0,80).Select(i=>db.AwardDailyCrown(user.UserName,"extra:"+i,Now)));
            state=await a.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.MeteoritePowder==145 && state.Wallet.Collection.DailyQuests.PowderGranted==125 && state.Wallet.Collection.DailyQuests.Crowns==6,"80 excess settlements cannot exceed daily 125 powder / 6 crowns");
            Check((await Account(other)).MeteoritePowder==20,"other connected account receives only its own login reward");
            Check((await b.Invoke<DailyQuestResult>("GetDailyQuests")).Wallet.Collection.MeteoritePowder==40,"other online account gets its own next-day login once");
            Check(await Reject(a,"GetDailyQuests",other.Id,"2099-01-01"),"LAN cannot specify another account or server date");
            Check(await Reject(a,"AwardDailyCrown",user.UserName,"forged",Now),"LAN has no public crown-award method");
            Check(await Reject(a,"GrantMeteoritePowder",other.Id,100000),"LAN has no administrator reward method");
            Check(await Reject(a,"CraftPremium","14002",0),"LAN cannot override the server crafting price");
            Check((await Account(user)).MeteoritePowder==145,"forged reward requests leave wallet unchanged");
        }
    }
    static async Task Calendar()
    {
        foreach(string boundary in new[]{"2026-10-01T00:00:00+08:00","2027-01-01T00:00:00+08:00","2028-02-29T00:00:00+08:00","2028-03-01T00:00:00+08:00"})
        {
            var u=await NewUser("calendar");var zero=DateTimeOffset.Parse(boundary);Now=zero.AddTicks(-1);
            await db.GetDailyQuests(u.UserName);Now=zero;
            var state=await db.GetDailyQuests(u.UserName);
            Check(state.Wallet.Collection.MeteoritePowder==40 && state.Wallet.Collection.DailyQuests.Day==zero.ToString("yyyy-MM-dd"),"calendar transition "+boundary);
            await db.GetDailyQuests(u.UserName);
            Check((await Account(u)).MeteoritePowder==40,"calendar repeated refresh remains once "+boundary);
        }
        var user=await NewUser("offline");Now=DateTimeOffset.Parse("2026-09-01T04:00:00Z");
        await db.GetDailyQuests(user.UserName);Now=Now.AddDays(10);
        var next=await db.GetDailyQuests(user.UserName);
        Check(next.Wallet.Collection.MeteoritePowder==40,"ten offline days do not accumulate unclaimed daily bonuses");
        for(int day=0;day<7;day++){Now=Now.AddDays(1);await db.GetDailyQuests(user.UserName);}
        Check((await Account(user)).MeteoritePowder==180,"seven consecutive later login days each pay exactly twenty");
        var before=JsonConvert.SerializeObject(await Account(user));Now=Now.AddDays(-2);
        Check((await db.GetDailyQuests(user.UserName)).Status=="clock_behind" && JsonConvert.SerializeObject(await Account(user))==before,"clock rollback neither resets progress nor rewrites wallet");
        Now=Now.AddDays(2);
        Check((await db.GetDailyQuests(user.UserName)).Wallet.Collection.MeteoritePowder==180,"clock recovery does not double-pay latest day");
        var late=await NewUser("late");Now=DateTimeOffset.Parse("2026-09-14T16:00:00Z");
        await db.GetDailyQuests(late.UserName);
        await db.AwardDailyCrown(late.UserName,"late",Now.AddTicks(-1));
        Check((await Account(late)).DailyQuests.Crowns==1,"past-day settlement credits the processing day");
        await db.AwardDailyCrown(late.UserName,"future",Now.AddTicks(1));
        Check((await Account(late)).DailyQuests.Crowns==1,"future settlement cannot credit current-day crowns");
        foreach(string key in new[]{null,""," ",new string('x',101)})
        {bool rejected=false;try{await db.AwardDailyCrown(late.UserName,key,Now);}catch(ArgumentException){rejected=true;}Check(rejected,"invalid settlement key rejected: "+(key==null?"null":key.Length.ToString()));}
        await db.AwardDailyCrown(late.UserName,new string('x',100),Now);
        Check((await Account(late)).DailyQuests.Crowns==2,"maximum valid 100-character round key accepted");
    }
    static async Task ProcessingDay()
    {
        Now=DateTimeOffset.Parse("2026-09-18T15:59:59Z");
        var u=await NewUser("processing-day");var settled=Now;
        await db.AwardDailyCrown(u.UserName,"credited-yesterday",settled);
        Now=Now.AddSeconds(1);
        var state=await db.AwardDailyCrown(u.UserName,"credited-yesterday",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==0 && state.Wallet.Collection.MeteoritePowder==40,
            "credited round retry across midnight grants only the new login, no crown");
        state=await db.AwardDailyCrown(u.UserName,"delayed-first",settled);
        Check(state.Wallet.Collection.DailyQuests.Day=="2026-09-19" && state.Wallet.Collection.DailyQuests.Crowns==1,
            "first processing of a delayed round attributes it to today's progress");
        await Task.WhenAll(Enumerable.Range(0,64).Select(_=>db.AwardDailyCrown(u.UserName,"delayed-second",settled)));
        var account=await Account(u);
        Check(account.DailyQuests.Crowns==2 && account.MeteoritePowder==65,
            "concurrent delayed round retries grant one crown and today's two-crown tier once");
        Check(account.DailyQuests.ProcessedRoundIds.Count==3,
            "cross-day processed keys are persisted independently of today's credited keys");
        Check(!JsonConvert.SerializeObject(await db.GetDailyQuests(u.UserName)).Contains("ProcessedRoundIds"),
            "persistent deduplication history is omitted from client JSON");
        Now=Now.AddDays(3);
        state=await db.AwardDailyCrown(u.UserName,"delayed-second",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==0 && state.Wallet.Collection.MeteoritePowder==85,
            "processed round remains deduplicated after several days");
        state=await db.AwardDailyCrown(u.UserName,"first-seen-days-late",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==1,
            "first-seen older settlement still counts on its processing day");

        var capped=await NewUser("processing-cap");settled=Now;
        for(int i=0;i<7;i++)await db.AwardDailyCrown(capped.UserName,"cap:"+i,settled);
        account=await Account(capped);
        Check(account.DailyQuests.Crowns==6 && account.DailyQuests.RoundIds.Count==6 && account.DailyQuests.ProcessedRoundIds.Count==7,
            "capped settlements are recorded without increasing crowns or daily rewards");
        Now=Now.AddDays(1);
        state=await db.AwardDailyCrown(capped.UserName,"cap:6",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==0 && state.Wallet.Collection.MeteoritePowder==145,
            "yesterday's capped settlement cannot become a rewarded crown on retry");

        var legacy=await NewUser("processing-legacy");settled=Now;
        await db.AwardDailyCrown(legacy.UserName,"legacy:0",settled);
        await Accounts.UpdateOneAsync(x=>x.Id==legacy.Id,Builders<PremiumCollection>.Update.Unset("DailyQuests.ProcessedRoundIds"));
        Now=Now.AddDays(1);
        state=await db.AwardDailyCrown(legacy.UserName,"legacy:0",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==0 && state.Wallet.Collection.DailyQuests.ProcessedRoundIds.Contains("legacy:0"),
            "legacy wallet imports existing credited keys before its first new-day reset");

        // Force a wallet revision conflict between the first clock sample and its CAS update.
        // The retry samples the next day and must not retain yesterday's progress or reward tier.
        Now=DateTimeOffset.Parse("2026-09-25T15:59:59Z");
        var racing=await NewUser("processing-midnight-cas");
        await db.AwardDailyCrown(racing.UserName,"before-midnight",Now);
        settled=Now;int samples=0;
        db.DailyQuestClock=()=>
        {
            if(Interlocked.Increment(ref samples)==1)
            {
                Accounts.UpdateOne(x=>x.Id==racing.Id,Builders<PremiumCollection>.Update.Inc(x=>x.Revision,1));
                Now=Now.AddSeconds(1);return settled;
            }
            return Now;
        };
        try { state=await db.AwardDailyCrown(racing.UserName,"cas-midnight",settled); }
        finally { db.DailyQuestClock=()=>Now; }
        Check(samples>=2 && state.Wallet.Collection.DailyQuests.Day=="2026-09-26" && state.Wallet.Collection.DailyQuests.Crowns==1 && state.Wallet.Collection.MeteoritePowder==40,
            "CAS retry crossing midnight uses the successful processing attempt's day");
        state=await db.AwardDailyCrown(racing.UserName,"cas-midnight",settled);
        Check(state.Wallet.Collection.DailyQuests.Crowns==1 && state.Wallet.Collection.MeteoritePowder==40,
            "retry after midnight CAS success does not duplicate its crown");
    }
    static async Task Concurrent()
    {
        Now=DateTimeOffset.Parse("2026-09-20T08:00:00Z");var u=await NewUser("race");
        await Task.WhenAll(Enumerable.Range(0,128).Select(_=>db.GetDailyQuests(u.UserName)));
        Check((await Account(u)).MeteoritePowder==20,"128 simultaneous first logins create one wallet and one reward");
        Check(await Accounts.CountDocumentsAsync(x=>x.Id==u.Id)==1,"concurrent initialization produces one account document");
        await Task.WhenAll(Enumerable.Range(0,128).Select(_=>db.AwardDailyCrown(u.UserName,"same:0",Now)));
        Check((await Account(u)).DailyQuests.Crowns==1 && (await Account(u)).MeteoritePowder==20,"128 repeated same-round callbacks count one crown");
        await Task.WhenAll(Enumerable.Range(1,128).Select(i=>db.AwardDailyCrown(u.UserName,"unique:"+i,Now)));
        var account=await Account(u);
        Check(account.MeteoritePowder==125 && account.DailyQuests.Crowns==6 && account.DailyQuests.PowderGranted==125,"128 competing unique wins stop exactly at daily cap");
        Check(account.DailyQuests.RoundIds.Distinct().Count()==6 && account.DailyQuests.RoundIds.Count==6 &&
            account.DailyQuests.ProcessedRoundIds.Count==129 && account.DailyQuests.ProcessedRoundIds.Distinct().Count()==129,
            "daily credited keys stay bounded while all processed round keys remain unique");
        long revision=account.Revision;
        await Task.WhenAll(Enumerable.Range(0,64).Select(_=>db.GetDailyQuests(u.UserName)));
        Check((await Account(u)).Revision==revision,"read-only repeated task refreshes do not write new revisions");
        Now=Now.AddDays(1);
        await Task.WhenAll(Enumerable.Range(0,64).Select(i=>i%2==0?db.GetDailyQuests(u.UserName):db.AwardDailyCrown(u.UserName,"next-day:0",Now)));
        account=await Account(u);
        Check(account.MeteoritePowder==145 && account.DailyQuests.Crowns==1,"concurrent day reset, login and duplicated crown settle once");
    }
    static async Task<(int code,string text)> Grant(string player,long amount,string reward,string reason="Reward suite")
    {
        var start=new ProcessStartInfo("powershell.exe"){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
        var mongoUri=Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121";
        mongoUri=mongoUri.TrimEnd('/');
        if(!mongoUri.EndsWith("/gwentdiy",StringComparison.OrdinalIgnoreCase))mongoUri+="/gwentdiy";
        foreach(string a in new[]{"-NoProfile","-NonInteractive","-ExecutionPolicy","Bypass","-File",Path.Combine(root,"scripts/Grant-MeteoritePowder.ps1"),"-PlayerId",player,"-Amount",amount.ToString(),"-RewardId",reward,"-Reason",reason,"-MongoUri",mongoUri})start.ArgumentList.Add(a);
        using(var p=Process.Start(start))
        {
            var output=p.StandardOutput.ReadToEndAsync();var error=p.StandardError.ReadToEndAsync();
            if(!await Task.Run(()=>p.WaitForExit(30000))){p.Kill();throw new TimeoutException("admin grant process");}
            var text=(await output)+(await error);
            Directory.CreateDirectory(Path.Combine(work,"admin-processes"));
            File.WriteAllText(Path.Combine(work,"admin-processes",Guid.NewGuid().ToString("N")+".json"),JsonConvert.SerializeObject(new{player,amount,reward,reason,exitCode=p.ExitCode,output=text},Formatting.Indented));
            return(p.ExitCode,text);
        }
    }
    static async Task Admin()
    {
        Now=DateTimeOffset.Parse("2026-09-21T08:00:00Z");var a=await NewUser("admin-a");var b=await NewUser("admin-b");
        var first=await Grant(a.Id,250,"offline");
        Check(first.code==0 && (await Account(a)).MeteoritePowder==250,"administrator can grant exact powder to offline account by player ID",new{process=first,account=await Account(a)});
        Check(await Accounts.CountDocumentsAsync(x=>x.Id==b.Id)==0,"offline grant does not initialize or credit another account");
        Check((await db.GetDailyQuests(a.UserName)).Wallet.Collection.MeteoritePowder==270,"first login retains offline grant and adds twenty");
        var duplicate=await Grant(a.Id,250,"offline");
        Check(duplicate.code==0 && (await Account(a)).MeteoritePowder==270 && (await Account(a)).Rewards.Count==1,"retry same reward ID does not duplicate powder or ledger");
        await Grant(a.Id,999,"offline","different payload");
        Check((await Account(a)).MeteoritePowder==270 && (await Account(a)).Rewards.Single().Amount==250,"same reward ID with altered amount cannot regrant");
        await Grant(b.Id,300,"offline");
        Check((await Account(b)).MeteoritePowder==300,"same reward ID is independent per target account");
        var jobs=await Task.WhenAll(Enumerable.Range(0,10).Select(_=>Grant(a.Id,17,"concurrent")));
        Check(jobs.All(x=>x.code==0) && (await Account(a)).MeteoritePowder==287 && (await Account(a)).Rewards.Count(x=>x.RewardId=="concurrent")==1,"ten real concurrent administrator processes grant one duplicate reward");
        jobs=await Task.WhenAll(Enumerable.Range(0,8).Select(i=>Grant(a.Id,3,"distinct-"+i)));
        Check(jobs.All(x=>x.code==0) && (await Account(a)).MeteoritePowder==311,"eight distinct concurrent grants all accumulate without loss");
        string reason="本地测试：特殊字符 \"quote\" \\ slash\n第二行";
        await Grant(a.Id,1,"unicode",reason);
        Check((await Account(a)).Rewards.Single(x=>x.RewardId=="unicode").Reason==reason,"administrator reason preserves Unicode, quotes and newlines safely");
        var unknown=await Grant("not-a-player-id",10,"unknown");
        Check(unknown.code!=0 && await Accounts.CountDocumentsAsync(x=>x.Id=="not-a-player-id")==0,"unknown player ID fails without creating an orphan wallet");
        var name=await Grant(a.UserName,10,"username");
        Check(name.code!=0 && (await Account(a)).MeteoritePowder==312,"username is not confused with player ID");
        foreach(long value in new long[]{0,-1,1000000001})
        {var bad=await Grant(a.Id,value,"bad-"+value);Check(bad.code!=0 && (await Account(a)).MeteoritePowder==312,"invalid administrator amount rejected: "+value);}
        var empty=await Grant(a.Id,10," ");Check(empty.code!=0,"blank reward ID rejected");
        empty=await Grant(a.Id,10,"blank-reason"," ");Check(empty.code!=0,"blank administrator reason rejected");
        Check((await Account(b)).MeteoritePowder==300,"all administrator edge cases leave unrelated account untouched");
        for(int i=0;i<6;i++)await db.AwardDailyCrown(a.UserName,"admin-cap:"+i,Now);
        await Grant(a.Id,500,"after-cap");
        var final=await Account(a);
        Check(final.MeteoritePowder==917 && final.DailyQuests.PowderGranted==125,"administrator rewards may exceed daily cap without resetting or consuming daily quota");
        await db.AwardDailyCrown(a.UserName,"post-admin-extra",Now);
        Check((await Account(a)).MeteoritePowder==917,"administrator grant does not reopen exhausted daily rewards");
        var boundary=await NewUser("amount-boundary");
        var minimum=await Grant(boundary.Id,1,"minimum");
        Check(minimum.code==0 && (await Account(boundary)).MeteoritePowder==1,"minimum administrator amount grants exactly one powder");
        var maximum=await Grant(boundary.Id,1000000000,"maximum");
        Check(maximum.code==0 && (await Account(boundary)).MeteoritePowder==1000000001,"maximum allowed administrator amount grants in full without truncation");
    }
    static async Task Composition()
    {
        Now=DateTimeOffset.Parse("2026-09-22T08:00:00Z");var u=await NewUser("compose");
        await db.GetDailyQuests(u.UserName);await Grant(u.Id,500,"seed");
        string card=(await db.GetPremiumCollection(u.UserName)).Costs.First(x=>x.Value==100).Key;
        var crafts=Enumerable.Range(0,24).Select(_=>db.CraftPremium(u.UserName,card)).ToArray();
        var crowns=Enumerable.Range(0,32).Select(i=>db.AwardDailyCrown(u.UserName,"compose:"+i,Now)).ToArray();
        var grants=Enumerable.Range(0,8).Select(i=>Grant(u.Id,5,"compose-admin:"+i)).ToArray();
        await Task.WhenAll(crafts.Cast<Task>().Concat(crowns).Concat(grants));
        var account=await Account(u);
        Check(crafts.Count(x=>x.Result.Success)==1,"24 competing craft requests charge exactly once");
        Check(account.MeteoritePowder==520-100+105+40,"concurrent craft, daily tiers and eight administrator grants conserve exact balance");
        Check(account.DailyQuests.PowderGranted==125 && account.OwnedCards.Contains(card),"concurrent transactions preserve cap and permanent ownership together");
        await Users.ReplaceOneAsync(x=>x.Id==u.Id,u);
        Check((await Account(u)).MeteoritePowder==565 && (await Account(u)).DailyQuests.Crowns==6,"legacy user replacement cannot overwrite independent wallet");
        await db.CraftPremium(u.UserName,card);await db.AwardDailyCrown(u.UserName,"after-spending",Now);
        Check((await Account(u)).MeteoritePowder==565,"spending and repeated crafting do not reopen capped daily rewards");
        var broke=await NewUser("exact-cost");await db.GetDailyQuests(broke.UserName);
        await Grant(broke.Id,80,"exact");var crafted=await db.CraftPremium(broke.UserName,card);
        Check(crafted.Success && crafted.Collection.MeteoritePowder==0,"exact-cost crafting reaches zero without underflow");
        var other=(await db.GetPremiumCollection(broke.UserName)).Costs.First(x=>x.Value==100 && x.Key!=card).Key;
        Check((await db.CraftPremium(broke.UserName,other)).Status=="insufficient_powder" && (await Account(broke)).MeteoritePowder==0,"insufficient powder leaves balance and ownership unchanged");
    }
    static async Task RoundSettlement()
    {
        var a=await NewUser("round-a");var b=await NewUser("round-b");db.DailyQuestClock=()=>DateTimeOffset.UtcNow;
        try
        {
            var leader=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Leader).Key;
            var card=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Copper).Key;
            for(int scenario=0;scenario<3;scenario++)
            {
                var game=new GwentServerGame(new SinkPlayer{Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}},new SinkPlayer{Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}});
                var ca=game.PlayersDeck[0][0];ca.Status.Strength=scenario==1?0:10;ca.Status.Conceal=false;game.PlayersPlace[0][0].Add(ca);
                var cb=game.PlayersDeck[1][0];cb.Status.Strength=scenario==0?0:10;cb.Status.Conceal=false;game.PlayersPlace[1][0].Add(cb);
                game.PlayersWinCount[0]=game.PlayersWinCount[1]=1;
                int calls=0;game.RoundWon=(w,k,t)=>{calls++;db.AwardDailyCrown(w==0?a.UserName:b.UserName,k,t).GetAwaiter().GetResult();};
                var end=game.BigRoundEnd();if(await Task.WhenAny(end,Task.Delay(8000))!=end)throw new TimeoutException("BigRoundEnd");await end;
                Check(calls==(scenario==2?0:1),"real BigRoundEnd callback count for winner/tie scenario "+scenario);
            }
            Check((await Account(a)).DailyQuests.Crowns==1 && (await Account(b)).DailyQuests.Crowns==1,"actual score settlement credits only each round winner, never the tied round");
        }
        finally{db.DailyQuestClock=()=>Now;}
    }
    static async Task Restart()
    {
        Now=DateTimeOffset.Parse("2026-09-23T08:00:00Z");
        string name;UserInfo user;
        using(var c=await WireClient.Connect(url)){user=await Login(c,"restart");name=user.UserName;}
        await Grant(user.Id,333,"restart-admin");for(int i=0;i<4;i++)await db.AwardDailyCrown(name,"persist:"+i,Now);
        var before=await Account(user);
        await host.StopAsync();host.Dispose();host=null;await StartHost();
        using(var c=await WireClient.Connect(url))
        {
            await c.Invoke<UserInfo>("Login",name,"RewardTest2026");
            var state=await c.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.MeteoritePowder==413 && state.Wallet.Collection.DailyQuests.Crowns==4,"restarted ASP.NET host retains exact wallet and crown tiers over LAN");
            await db.AwardDailyCrown(name,"persist:3",Now);await Grant(user.Id,333,"restart-admin");
            Check((await Account(user)).MeteoritePowder==413,"round and administrator retry remain idempotent after host restart");
        }
        if(!runMongoRestart)return;
        File.WriteAllText(Path.Combine(work,"mongo-restart.request"),"restart isolated reward-test MongoDB");
        var deadline=DateTime.UtcNow.AddSeconds(50);while(!File.Exists(Path.Combine(work,"mongo-restarted"))){if(DateTime.UtcNow>deadline)throw new TimeoutException("Mongo restart");await Task.Delay(500);}
        var after=await Account(user);
        Check(after.MeteoritePowder==before.MeteoritePowder && after.DailyQuests.Crowns==4 && after.Rewards.Count==1,"MongoDB process restart recovers acknowledged wallet, daily progress and admin ledger from disk");
        Now=Now.AddDays(1);
        using(var c=await WireClient.Connect(url))
        {
            await c.Invoke<UserInfo>("Login",name,"RewardTest2026");
            var state=await c.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.MeteoritePowder==433 && state.Wallet.Collection.DailyQuests.PowderGranted==20 && state.Wallet.Collection.DailyQuests.Crowns==0,"day reset after database restart preserves wallet and pays new login once");
            await db.AwardDailyCrown(name,"persist:3",Now.AddDays(-1));
            state=await c.Invoke<DailyQuestResult>("GetDailyQuests");
            Check(state.Wallet.Collection.MeteoritePowder==433 && state.Wallet.Collection.DailyQuests.Crowns==0,
                "cross-day round retry after MongoDB restart remains deduplicated");
            var raw=await c.Invoke<JObject>("GetDailyQuests");
            Check(!HasProperty(raw,"ProcessedRoundIds"),"network response does not expose persistent processed-round history");
        }
    }
    static async Task<JObject> UiResult(string label)
    {
        var deadline=DateTime.UtcNow.AddSeconds(110);var path=Path.Combine(work,"ui-"+label+".json");
        while(!File.Exists(path))
        {
            if(File.Exists(Path.Combine(work,"ui-error.txt")))throw new Exception(File.ReadAllText(Path.Combine(work,"ui-error.txt")));
            if(DateTime.UtcNow>deadline)throw new TimeoutException("Unity UI "+label);await Task.Delay(300);
        }
        return JObject.Parse(File.ReadAllText(path));
    }
    static async Task ClientUi()
    {
        var u=await NewUser("ui");var other=await NewUser("ui-other");Now=DateTimeOffset.Parse("2026-09-25T15:59:59Z");
        File.WriteAllText(Path.Combine(root,"work/RewardSystem/ui.request"),JsonConvert.SerializeObject(new{work,endpoint="http://"+lan+":5016/hub/gwent",username=u.UserName}));
        var initial=await UiResult("initial");
        var initialization=JObject.Parse(File.ReadAllText(Path.Combine(work,"ui-login-initialization.json")));
        Check((bool)initialization["initializedOnLogin"],"Unity daily task entry is initialized on login before opening deck builder",initialization);
        Check((bool)initialization["singlePanel"],"Unity opening deck builder keeps exactly one task panel and entry",initialization);
        Check((bool)initial["passed"] && (long)initial["account"]["MeteoritePowder"]==10,"Unity real login shows first ten powder in task UI");
        Now=Now.AddSeconds(1);
        File.WriteAllText(Path.Combine(work,"ui-phase-0.json"),JsonConvert.SerializeObject(new{label="automatic-midnight",balance=20,day="2026-09-26"}));
        var midnight=await UiResult("automatic-midnight");
        Check((bool)midnight["passed"] && (long)midnight["account"]["MeteoritePowder"]==20 && (int)midnight["account"]["DailyQuests"]["Crowns"]==0,"Unity minute ticker automatically refreshes across midnight without reopening task page");
        Check((double)midnight["remaining"]>86300,"Unity countdown resets to approximately 24 hours using server time");
        for(int i=0;i<16;i++)await db.AwardDailyCrown(u.UserName,"ui-cap:"+i,Now);
        File.WriteAllText(Path.Combine(work,"ui-phase-1.json"),JsonConvert.SerializeObject(new{label="cap",balance=80}));
        var cap=await UiResult("cap");
        Check((long)cap["account"]["MeteoritePowder"]==80 && (int)cap["account"]["DailyQuests"]["PowderGranted"]==70 && (int)cap["account"]["DailyQuests"]["Crowns"]==6,"Unity displays capped six crowns and daily seventy powder after sixteen wins");
        Check(cap["text"].Any(x=>x.ToString().Contains("70 / 70")),"Unity task page visible text shows the exhausted daily cap");
        await Grant(u.Id,125,"ui-admin");
        File.WriteAllText(Path.Combine(work,"ui-phase-2.json"),JsonConvert.SerializeObject(new{label="grant",balance=205}));
        var grant=await UiResult("grant");
        Check((long)grant["account"]["MeteoritePowder"]==205 && (int)grant["account"]["DailyQuests"]["PowderGranted"]==70,"Unity refresh shows administrator powder without reopening daily quota");
        File.WriteAllText(Path.Combine(work,"ui-phase-3.json"),JsonConvert.SerializeObject(new{label="switch",balance=10,username=other.UserName}));
        var switched=await UiResult("switch");
        Check((string)switched["account"]["Id"]==other.Id && (long)switched["account"]["MeteoritePowder"]==10 && (int)switched["account"]["DailyQuests"]["Crowns"]==0,"Unity account switch clears previous player's powder and crown cache");
        Check((await Account(u)).MeteoritePowder==205,"Unity account switch leaves the first player's wallet intact");
        await Task.Delay(1000);
    }
    private sealed class SinkPlayer:Player{public SinkPlayer(){_downstream.Receive+=_=>Task.CompletedTask;}}
}
