using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

class Program
{
    static int checks;
    static void Check(bool value,string text){if(!value)throw new Exception(text);Console.WriteLine("PASS "+text);checks++;}
    static async Task Main(string[] args)
    {
        var mongo=new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121");
        // Daily earning contracts are independent of the one-time launch grant.
        var provider=new ServiceCollection().AddSingleton<IMongoClient>(mongo).AddSingleton(new InitialPowderOptions(0)).BuildServiceProvider();
        var users=mongo.GetDatabase("gwentdiy").GetCollection<UserInfo>("user");
        var accounts=mongo.GetDatabase("gwentdiy").GetCollection<PremiumCollection>("premium_collection");
        var now=DateTimeOffset.Parse("2026-09-13T15:59:00Z");
        var db=new GwentDatabaseService(provider){DailyQuestClock=()=>now};
        if(args.Length==2 && args[0]=="ui")
        {
            db.DailyQuestClock=()=>DateTimeOffset.UtcNow;
            int target=int.Parse(args[1]);if(target<0 || target>6)throw new ArgumentException("UI crown target must be 0..6");
            var stamp=DateTimeOffset.UtcNow;
            for(int i=0;i<target;i++)await db.AwardDailyCrown("premium-ui-test","daily-ui:"+stamp.ToOffset(TimeSpan.FromHours(8)).ToString("yyyyMMdd")+":"+i,stamp);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(await db.GetDailyQuests("premium-ui-test")));return;
        }
        var user=new UserInfo{UserName="daily-contract-"+Guid.NewGuid().ToString("N"),PlayerName="Daily Test",Decks=new List<DeckModel>()};
        await users.InsertOneAsync(user);
        Check(!(await db.GetDailyQuests("missing-"+Guid.NewGuid())).Success,"unknown account rejected");
        var logins=await Task.WhenAll(Enumerable.Range(0,24).Select(_=>db.GetDailyQuests(user.UserName)));
        var state=(await db.GetDailyQuests(user.UserName));
        Check(state.Wallet.Collection.MeteoritePowder==20 && state.Wallet.Collection.DailyQuests.LoginGranted,"24 concurrent login requests grant once");
        Check(state.ResetUtc.StartsWith("2026-09-13T16:00") && state.Wallet.Collection.DailyQuests.Day=="2026-09-13","reset uses China midnight independently of host timezone");
        Check(state.DailyCap==125 && state.Tiers.Select(x=>x.Powder).SequenceEqual(new[]{25,35,45}),"approved incremental rewards and daily cap");
        await Task.WhenAll(Enumerable.Range(0,20).Select(_=>db.AwardDailyCrown(user.UserName,"match-a:0",now)));
        state=await db.GetDailyQuests(user.UserName);
        Check(state.Wallet.Collection.DailyQuests.Crowns==1 && state.Wallet.Collection.MeteoritePowder==20,"duplicate round callbacks count once");
        state=await db.AwardDailyCrown(user.UserName,"match-a:1",now);
        Check(state.Wallet.Collection.MeteoritePowder==45,"two crowns grant configured first tier");
        await db.AwardDailyCrown(user.UserName,"match-b:0",now);
        state=await db.AwardDailyCrown(user.UserName,"match-b:1",now);
        Check(state.Wallet.Collection.MeteoritePowder==80,"four crowns grant configured second tier");
        await db.AwardDailyCrown(user.UserName,"match-c:0",now);
        state=await db.AwardDailyCrown(user.UserName,"match-c:1",now);
        Check(state.Wallet.Collection.MeteoritePowder==125,"six crowns grant configured third tier");
        await Task.WhenAll(Enumerable.Range(0,30).Select(i=>db.AwardDailyCrown(user.UserName,"excess:"+i,now)));
        state=await db.GetDailyQuests(user.UserName);
        Check(state.Wallet.Collection.MeteoritePowder==125 && state.Wallet.Collection.DailyQuests.RoundIds.Count==6,"excess games stop rewards and keep bounded dedup storage");
        now=now.AddMinutes(1);
        await Task.WhenAll(Enumerable.Range(0,24).Select(_=>db.GetDailyQuests(user.UserName)));
        state=await db.GetDailyQuests(user.UserName);
        Check(state.Wallet.Collection.MeteoritePowder==145 && state.Wallet.Collection.DailyQuests.Crowns==0,"midnight resets progress and pays next login once");
        state=await db.AwardDailyCrown(user.UserName,"late-prior-day",now.AddSeconds(-1));
        Check(state.Wallet.Collection.DailyQuests.Crowns==1,"late prior-day result counts on its processing day");
        state=await db.AwardDailyCrown(user.UserName,"match-a:0",now.AddMinutes(-1));
        Check(state.Wallet.Collection.DailyQuests.Crowns==1,"credited prior-day round cannot count again after reset");
        state=await db.AwardDailyCrown(user.UserName,"excess:0",now.AddMinutes(-1));
        Check(state.Wallet.Collection.DailyQuests.Crowns==1,"previously capped round cannot count after reset");
        state=await db.AwardDailyCrown(user.UserName,"future",now.AddSeconds(1));
        Check(state.Wallet.Collection.DailyQuests.Crowns==1,"future settlement time rejected");
        var restarted=new GwentDatabaseService(provider){DailyQuestClock=()=>now};
        Check((await restarted.GetDailyQuests(user.UserName)).Wallet.Collection.MeteoritePowder==145,"server restart retains login dedup and wallet");
        await accounts.UpdateOneAsync(x=>x.Id==user.Id,Builders<PremiumCollection>.Update.Inc(x=>x.MeteoritePowder,100).Inc(x=>x.Revision,1));
        string card=state.Wallet.Costs.First(x=>x.Value==100).Key;
        await Task.WhenAll(db.CraftPremium(user.UserName,card),db.AwardDailyCrown(user.UserName,"new:0",now),db.AwardDailyCrown(user.UserName,"new:1",now));
        state=await db.GetDailyQuests(user.UserName);
        Check(state.Wallet.Collection.MeteoritePowder==170 && state.Wallet.Collection.OwnedCards.Contains(card),"craft and crown rewards commit without lost wallet updates");
        await users.ReplaceOneAsync(x=>x.Id==user.Id,user);
        Check((await db.GetDailyQuests(user.UserName)).Wallet.Collection.MeteoritePowder==170,"legacy user replacement preserves daily progress");
        now=now.AddDays(-1);
        Check((await db.GetDailyQuests(user.UserName)).Status=="clock_behind","clock rollback cannot regrant a previous day");
        now=now.AddDays(1);
        var other=new UserInfo{UserName="daily-other-"+Guid.NewGuid().ToString("N"),PlayerName="Daily Other",Decks=new List<DeckModel>()};await users.InsertOneAsync(other);
        Check((await db.GetDailyQuests(other.UserName)).Wallet.Collection.MeteoritePowder==20,"account rewards are isolated");
        // Exercise the actual BigRoundEnd path with server-side scores, including draws.
        var leader=GwentMap.CardMap.First(x=>x.Value.Group==Group.Leader).Key;
        for(int scenario=0;scenario<3;scenario++)
        {
            var game=new GwentServerGame(new SinkPlayer{Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}},new SinkPlayer{Deck=new DeckModel{Leader=leader,Deck=new List<string>{card}}});
            var a=game.PlayersDeck[0][0];a.Status.Strength=scenario==1?0:10;a.Status.Conceal=false;game.PlayersPlace[0][0].Add(a);
            var b=game.PlayersDeck[1][0];b.Status.Strength=scenario==0?0:10;b.Status.Conceal=false;game.PlayersPlace[1][0].Add(b);
            game.PlayersWinCount[0]=game.PlayersWinCount[1]=1;
            int winner=-1;string key=null;
            game.RoundWon=(w,k,t)=>{winner=w;key=k;};
            var end=game.BigRoundEnd();if(await Task.WhenAny(end,Task.Delay(5000))!=end)throw new Exception("round settlement timed out");await end;
            Check(winner==(scenario==2?-1:scenario) && (winner<0 || key.EndsWith(":0")),"real round settlement scenario "+scenario);
        }
        Console.WriteLine("COMPLETE checks="+checks);
    }
    private sealed class SinkPlayer:Player{public SinkPlayer(){_downstream.Receive+=_=>Task.CompletedTask;}}
}
