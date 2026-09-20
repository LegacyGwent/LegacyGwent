using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

partial class Program
{
    static bool HasProperty(JToken value,string name) => value is JProperty property &&
        string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase) ||
        value.Children().Any(child=>HasProperty(child,name));

    static async Task WireContracts()
    {
        Check(HasProperty(JObject.Parse("{\"wallet\":{\"processedRoundIds\":[\"x\"]}}"),"ProcessedRoundIds"),
            "schema guard detects camelCase internal fields in nested responses");
        Check(HasProperty(JObject.Parse("{\"ProcessedRoundIds\":[]}"),"processedRoundIds"),
            "schema guard detects PascalCase internal fields even when empty");
        using(var client=await WireClient.Connect(url))
        {
            var u=await Login(client,"wire-contract");
            await db.AwardDailyCrown(u.UserName,"private-history",Now);
            foreach(var method in new[]{"GetDailyQuests","GetPremiumCollection"})
            {
                var raw=await client.Invoke<JObject>(method);
                Check(!HasProperty(raw,"ProcessedRoundIds"),method+" excludes persistent history from actual wire JSON");
            }
            Check((await client.Invoke<PremiumCollectionResult>("CraftPremium",new object[]{null})).Status=="unavailable","null card ID is rejected over the wire");
            Check((await client.Invoke<PremiumCollectionResult>("CraftPremium"," ")).Status=="unavailable","blank card ID is rejected over the wire");
            Check((await client.Invoke<PremiumCollectionResult>("SelectPremium","invalid",0)).Status=="unavailable","invalid standard selection is rejected");
            Check((await client.Invoke<PremiumCollectionResult>("SelectPremium","14002",-1)).Status=="invalid_selection","negative version flag is rejected");
            var before=(await Account(u)).MeteoritePowder;
            var available=(await db.GetPremiumCollection(u.UserName)).Costs;
            var unavailable=GwentMap.CardMap.Keys.FirstOrDefault(x=>!available.ContainsKey(x));
            if(unavailable!=null)Check((await client.Invoke<PremiumCollectionResult>("CraftPremium",unavailable)).Status=="unavailable","known card without premium art cannot be crafted");
            Check((await Account(u)).MeteoritePowder==before,"invalid wire requests do not debit wallet");
        }
        using(var unauth=await WireClient.Connect(url))
            Check((await unauth.Invoke<PremiumCollectionResult>("SelectPremium","14002",1)).Status=="unauthenticated","unauthenticated premium selection is rejected");
    }

    static async Task MixedConcurrency()
    {
        var u=await NewUser("different-crafts");await db.GetDailyQuests(u.UserName);
        await Grant(u.Id,80,"funds");
        var costs=(await db.GetPremiumCollection(u.UserName)).Costs;
        var cards=costs.Where(x=>x.Value==100).Take(2).Select(x=>x.Key).ToArray();
        var crafts=await Task.WhenAll(cards.Select(x=>db.CraftPremium(u.UserName,x)));
        var account=await Account(u);
        Check(crafts.Count(x=>x.Success)==1 && account.MeteoritePowder==0 && account.OwnedCards.Count==1,
            "different cards competing for one card's balance unlock exactly one");
        var owned=account.OwnedCards.Single();
        await Task.WhenAll(db.CraftPremium(u.UserName,owned),db.SelectPremium(u.UserName,owned,false));
        Check(!(await Account(u)).SelectedCards.Contains(owned),"duplicate crafting does not undo a concurrent standard selection");
        var shared=await NewUser("multi-service");
        var second=new GwentDatabaseService(host.Services){DailyQuestClock=()=>Now};
        await Task.WhenAll(Enumerable.Range(0,32).Select(i=>(i%2==0?db:second).AwardDailyCrown(shared.UserName,"shared-round",Now)));
        Check((await Account(shared)).DailyQuests.Crowns==1,"independent service instances deduplicate a shared wallet round");
        var other=await NewUser("shared-key-other");await db.AwardDailyCrown(other.UserName,"shared-round",Now);
        Check((await Account(other)).DailyQuests.Crowns==1 && (await Account(shared)).DailyQuests.Crowns==1,
            "identical round keys are independent between accounts");
        await db.AwardDailyCrown(shared.UserName,"shared-round",Now.AddDays(-4));
        Check((await Account(shared)).DailyQuests.Crowns==1,"changing a processed round timestamp cannot earn another crown");
        foreach(int price in new[]{200,400})
        {
            var rich=await NewUser("price-"+price);await Grant(rich.Id,price,"price");
            var card=costs.First(x=>x.Value==price).Key;
            var result=await db.CraftPremium(rich.UserName,card);
            Check(result.Success && result.Collection.MeteoritePowder==0 && result.Collection.OwnedCards.Contains(card),
                "actual crafting deducts exact price "+price);
        }
        var leader=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Leader && costs.ContainsKey(x.Key)).Key;
        var leaderUser=await NewUser("leader-price");await Grant(leaderUser.Id,costs[leader],"leader");
        Check((await db.CraftPremium(leaderUser.UserName,leader)).Collection.MeteoritePowder==0,"leader crafting deducts its configured price");
    }

    static async Task HistoryAndLegacy()
    {
        foreach(string field in new[]{"ProcessedRoundIds","RoundIds"})
            await Group("legacy null "+field,async()=>
            {
                var u=await NewUser("null-"+field);await db.GetDailyQuests(u.UserName);
                await Accounts.UpdateOneAsync(x=>x.Id==u.Id,Builders<PremiumCollection>.Update.Set("DailyQuests."+field,BsonNull.Value));
                var result=await db.AwardDailyCrown(u.UserName,"null-compatible",Now);
                Check(result.Success && result.Wallet.Collection.DailyQuests.Crowns==1,"explicit null "+field+" can receive a new crown");
                var stored=await Account(u);
                Check(stored.DailyQuests.RoundIds!=null && stored.DailyQuests.ProcessedRoundIds!=null,
                    "legacy null "+field+" is repaired in persisted storage");
                await db.AwardDailyCrown(u.UserName,"null-compatible",Now);
                Check((await Account(u)).DailyQuests.Crowns==1,"repaired "+field+" still deduplicates retries");
            });
        foreach(int count in new[]{1000,10000,100000})
        {
            var u=await NewUser("history-"+count);await db.GetDailyQuests(u.UserName);
            var keys=Enumerable.Range(0,count).Select(i=>"0123456789abcdef0123456789abcdef:"+i).ToList();
            await Accounts.UpdateOneAsync(x=>x.Id==u.Id,Builders<PremiumCollection>.Update.Set(x=>x.DailyQuests.ProcessedRoundIds,keys));
            var watch=Stopwatch.StartNew();var state=await db.GetDailyQuests(u.UserName);watch.Stop();long queryMs=watch.ElapsedMilliseconds;
            Check(state.Wallet.Collection.DailyQuests.ProcessedRoundIds.Count==count,"stored history survives query at "+count+" keys");
            watch.Restart();state=await db.AwardDailyCrown(u.UserName,"new-history-round",Now);watch.Stop();
            Check(state.Wallet.Collection.DailyQuests.Crowns==1 && state.Wallet.Collection.DailyQuests.ProcessedRoundIds.Count==count+1,
                "new crown remains correct at "+count+" historical keys",new{queryMs,awardMs=watch.ElapsedMilliseconds});
            var duplicate=await db.AwardDailyCrown(u.UserName,keys[0],Now.AddDays(-1));
            Check(duplicate.Wallet.Collection.DailyQuests.Crowns==1,"oldest retained key remains deduplicated at "+count+" keys");
        }
    }

    static Task FailNextWalletWrite(bool enabled) => db.GetMongoClient().GetDatabase("admin").RunCommandAsync<BsonDocument>(
        new BsonDocument { {"configureFailPoint","failCommand"}, {"mode",enabled?(BsonValue)new BsonDocument("times",1):new BsonString("off")},
            {"data",new BsonDocument{{"failCommands",new BsonArray{"findAndModify"}},{"errorCode",2}}} });

    static async Task FaultBoundaries()
    {
        var u=await NewUser("write-failure");await db.GetDailyQuests(u.UserName);
        Exception error=null;
        await FailNextWalletWrite(true);
        try{await db.AwardDailyCrown(u.UserName,"retry-write",Now);}catch(Exception e){error=e;}
        finally{await FailNextWalletWrite(false);}
        Check(error!=null && (await Account(u)).DailyQuests.Crowns==0 && (await Account(u)).MeteoritePowder==20,
            "injected Mongo write failure leaves wallet and crown unchanged");
        await db.AwardDailyCrown(u.UserName,"retry-write",Now);
        Check((await Account(u)).DailyQuests.Crowns==1,"explicit retry after a failed database write credits one crown");

        var conflicted=await NewUser("conflict-exhaustion");await db.GetDailyQuests(conflicted.UserName);error=null;
        db.DailyQuestClock=()=>{Accounts.UpdateOne(x=>x.Id==conflicted.Id,Builders<PremiumCollection>.Update.Inc(x=>x.Revision,1));return Now;};
        try{await db.AwardDailyCrown(conflicted.UserName,"exhausted",Now);}catch(Exception e){error=e;}
        finally{db.DailyQuestClock=()=>Now;}
        Check(error is InvalidOperationException && (await Account(conflicted)).DailyQuests.Crowns==0,
            "100 forced revision conflicts fail explicitly without granting partial reward");
        await db.AwardDailyCrown(conflicted.UserName,"exhausted",Now);
        Check((await Account(conflicted)).DailyQuests.Crowns==1,"retry after conflict exhaustion remains recoverable");

        var server=host.Services.GetRequiredService<GwentServerService>();
        var notify=await NewUser("notification-failure");
        server.QueueDailyCrown(new User(notify.UserName,"missing-connection"),"notify-once",Now);
        var rewardDeadline=DateTimeOffset.UtcNow.AddSeconds(10);
        while(DateTimeOffset.UtcNow<rewardDeadline && (await Account(notify))?.DailyQuests?.Crowns!=1)await Task.Delay(50);
        Check((await Account(notify)).DailyQuests.Crowns==1,"background reward persists even when its client notification cannot be delivered");
        await db.AwardDailyCrown(notify.UserName,"notify-once",Now);
        Check((await Account(notify)).DailyQuests.Crowns==1,"retry after failed notification cannot duplicate crown");

        using(var c=await WireClient.Connect(url))
        {
            string name="reward-login-fault-"+Guid.NewGuid().ToString("N").Substring(0,8);
            await c.Invoke<bool>("Register",name,"RewardTest2026",name);error=null;await FailNextWalletWrite(true);
            UserInfo logged=null;
            try{logged=await c.Invoke<UserInfo>("Login",name,"RewardTest2026");}catch(Exception e){error=e;}
            finally{await FailNextWalletWrite(false);}
            Check(error==null && logged!=null,"wallet failure cannot reject ordinary authentication");
            PremiumCollectionResult access=null;
            var loginDeadline=DateTimeOffset.UtcNow.AddSeconds(10);
            while(DateTimeOffset.UtcNow<loginDeadline)
            {
                access=await c.Invoke<PremiumCollectionResult>("GetPremiumCollection");
                if(access.Success && access.Collection.DailyQuests?.LoginGranted==true)break;
                await Task.Delay(50);
            }
            Check(access?.Success==true && access.Collection.DailyQuests?.LoginGranted==true,
                "background daily login reward recovers after the injected wallet failure");
        }
    }

    static GwentServerGame SimpleGame()
    {
        var leader=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Leader).Key;
        var card=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Copper).Key;
        return new GwentServerGame(new SinkPlayer{PlayerName="round-one",Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}},
            new SinkPlayer{PlayerName="round-two",Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}});
    }
    static async Task RoundLifecycle()
    {
        var game=SimpleGame();game.IsPlayersPass[0]=game.IsPlayersPass[1]=true;
        Check(!await game.PlayerRound(),"both passed players terminate the round loop");
        game=SimpleGame();game.GameRound=TwoPlayer.Player1;game.IsPlayersPass[1]=true;
        game.PlayersHandCard[0].Clear();game.IsPlayersLeader[0]=false;
        Check(!await game.PlayerRound() && game.IsPlayersPass[0],"empty hand and used leader force pass and terminate when opponent passed");
        foreach(bool surrender in new[]{true,false})
        {
            var u=await NewUser("end-"+surrender);game=SimpleGame();
            game.RoundWon=(w,key,time)=>db.AwardDailyCrown(u.UserName,key,Now).GetAwaiter().GetResult();
            await game.GameEnd(0,new Exception(surrender?"surrender fixture":"disconnect fixture"),surrender);
            Check((await db.GetDailyQuests(u.UserName)).Wallet.Collection.DailyQuests.Crowns==0,
                (surrender?"surrender":"disconnect")+" before any settled round awards no crown");
            game=SimpleGame();game.RoundWon=(w,key,time)=>db.AwardDailyCrown(u.UserName,key,Now).GetAwaiter().GetResult();
            var card=game.PlayersDeck[0][0];card.Status.Conceal=false;card.Status.Strength=10;game.PlayersPlace[0][0].Add(card);
            game.PlayersWinCount[0]=1;await game.BigRoundEnd();
            await game.GameEnd(1,new Exception("fixture end"),surrender);
            Check((await Account(u)).DailyQuests.Crowns==1,(surrender?"surrender":"disconnect")+" preserves a settled crown without adding an unplayed win");
        }
        game=SimpleGame();game.PlayersWinCount[0]=1;
        var winning=game.PlayersDeck[0][0];winning.Status.Conceal=false;winning.Status.Strength=10;game.PlayersPlace[0][0].Add(winning);
        int attempts=0;
        game.RoundWon=(w,key,time)=>{attempts++;throw new Exception("reward queue unavailable");};
        Exception failure=null;try{await game.BigRoundEnd();}catch(Exception e){failure=e;}
        Check(failure==null && game.CurrentRoundCount==1,"reward storage failure must not interrupt normal round advancement",new{error=failure?.Message,round=game.CurrentRoundCount});
        Check(attempts==1 && game.PlayersWinCount[0]==2,"reward enqueue failure is isolated from the winner and round state");

        var user=await NewUser("round-background");await db.GetDailyQuests(user.UserName);
        game=SimpleGame();game.PlayersWinCount[0]=1;
        winning=game.PlayersDeck[0][0];winning.Status.Conceal=false;winning.Status.Strength=10;game.PlayersPlace[0][0].Add(winning);
        var release=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var settled=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        string firstKey=null;DateTimeOffset firstTime=default;
        game.RoundWon=(w,key,time)=>
        {
            firstKey=key;firstTime=time;
            _=Task.Run(async()=>{await release.Task;await db.AwardDailyCrown(user.UserName,key,Now);settled.TrySetResult(true);});
        };
        var roundEnd=game.BigRoundEnd();
        Check(await Task.WhenAny(roundEnd,Task.Delay(3000))==roundEnd && game.CurrentRoundCount==1,
            "round advancement does not wait for reward storage");
        release.TrySetResult(true);await settled.Task;
        Check(firstKey!=null && firstTime!=default && (await Account(user)).DailyQuests.Crowns==1,
            "background settlement keeps the server-generated round identity and credits once");
    }

    static async Task RoomEligibility()
    {
        var server=host.Services.GetRequiredService<GwentServerService>();
        var hub=new FailingHub(false);
        foreach(string mode in new[]{"rank","private-fixture","self","ai#f"})
        {
            var matches=new GwentMatchs(()=>hub,host.Services.GetRequiredService<GwentCardDataService>(),server);
            var leader=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Leader).Key;
            var card=GwentMap.CardMap.First(x=>x.Value.Group==Cynthia.Card.Group.Copper).Key;
            var gate=new TaskCompletionSource<bool>();
            Func<string,ClientPlayer> player=name=>
            {
                var p=new ClientPlayer(new User(name,Guid.NewGuid().ToString("N")){PlayerName=name},()=>hub)
                    {Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}};
                // Pause gameplay traffic after the real room creation and callback binding.
                // This fixture verifies wiring, not an entire played match.
                p.Receive+=_=>gate.Task;return p;
            };
            matches.PlayerJoin(player("fixture-one"),mode);
            if(mode!="ai#f")matches.PlayerJoin(player(mode=="self"?"fixture-one":"fixture-two"),mode);
            var deadline=DateTime.UtcNow.AddSeconds(3);
            while(matches.GwentRooms.FirstOrDefault()?.CurrentGame==null && DateTime.UtcNow<deadline)await Task.Delay(10);
            var room=matches.GwentRooms.FirstOrDefault();
            bool eligible=mode=="rank" || mode=="private-fixture";
            Check(room?.CurrentGame!=null && (room.CurrentGame.RoundWon!=null)==eligible,
                "production room "+mode+" binds crown callback only when eligible");
            if(room?.CurrentGame!=null)
            {
                var completion=(TaskCompletionSource<int>)typeof(GwentServerGame).GetField("_setGameEnd",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(room.CurrentGame);
                completion.TrySetResult(0);await Task.Delay(10);
            }
        }
    }

    sealed class FailingHub : IHubContext<GwentHub>
    {
        public FailingHub(bool fail=true){Clients=new FailingClients(fail);}
        public IHubClients Clients {get;}
        public IGroupManager Groups=>throw new NotSupportedException();
    }
    sealed class FailingClients : IHubClients
    {
        readonly IClientProxy proxy;
        public FailingClients(bool fail){proxy=new FailingProxy(fail);}
        public IClientProxy All=>proxy;
        public IClientProxy AllExcept(IReadOnlyList<string> ids)=>proxy;
        public IClientProxy Client(string id)=>proxy;
        public IClientProxy Clients(IReadOnlyList<string> ids)=>proxy;
        public IClientProxy Group(string id)=>proxy;
        public IClientProxy GroupExcept(string id,IReadOnlyList<string> ids)=>proxy;
        public IClientProxy Groups(IReadOnlyList<string> ids)=>proxy;
        public IClientProxy User(string id)=>proxy;
        public IClientProxy Users(IReadOnlyList<string> ids)=>proxy;
    }
    sealed class FailingProxy:IClientProxy
    {
        readonly bool fail;public FailingProxy(bool fail){this.fail=fail;}
        public Task SendCoreAsync(string method,object[] args,CancellationToken token=default)=>fail?Task.FromException(new Exception("injected notification failure")):Task.CompletedTask;
    }
}
