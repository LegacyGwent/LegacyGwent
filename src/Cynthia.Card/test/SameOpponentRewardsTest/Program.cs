using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Cynthia.Card.Server.Services.GwentGameService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;

// Real Mongo integration checks for the same-human-opponent daily reward rule.
// Every fixture is prefixed with a fresh id; this test never deletes collections or documents.
class Program
{
    static int checks;
    static DateTimeOffset now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    static IMongoCollection<UserInfo> users;
    static IMongoCollection<PremiumCollection> accounts;
    static IMongoCollection<GameResult> results;
    static GwentDatabaseService db;
    static GwentServerService server;
    static readonly string Prefix = "same-opponent-" + Guid.NewGuid().ToString("N");
    static readonly Dictionary<string, string> currentMatches = new Dictionary<string, string>();

    static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception("FAIL " + name);
        checks++; Console.WriteLine("PASS " + name);
    }

    static async Task Main()
    {
        // The real GwentLocalizationService loads Locales relative to the process working directory.
        Directory.SetCurrentDirectory(FindServerDirectory());
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "DailyQuests.json"),
            "{\"LoginPowder\":20,\"Tiers\":[{\"Crowns\":2,\"Powder\":25},{\"Crowns\":4,\"Powder\":35},{\"Crowns\":6,\"Powder\":45}],\"GGPowder\":5,\"GGDailyCap\":30}");
        var mongo = new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121");
        var provider = new ServiceCollection().AddSingleton<IMongoClient>(mongo).AddSingleton(new InitialPowderOptions(0))
            .AddSingleton<GwentCardDataService>().BuildServiceProvider();
        var database = mongo.GetDatabase("gwentdiy");
        users = database.GetCollection<UserInfo>("user");
        accounts = database.GetCollection<PremiumCollection>("premium_collection");
        results = database.GetCollection<GameResult>("gameresults");
        db = new GwentDatabaseService(provider) { DailyQuestClock = () => now };
        server = MakeServer();
        var fixture = await NewUser("fixture"); await Login(fixture);
        var collectionsBefore = (await database.ListCollectionNames().ToListAsync()).OrderBy(x => x).ToArray();

        await FirstMatchPaysBothSides();
        await SecondMatchIsBlockedInBothDirections();
        await DifferentOpponentAndOldDayAreAllowed();
        await PendingOnlyAndLateFirstResultAreSafe();
        await SurrenderDisconnectAndColourSwapStillCount();
        await ConcurrencyAndRestartStayDeduplicated();
        await MidnightAndLoginStayIndependent();
        await ExactMidnightAndIncompleteHistory();
        await LateFirstGgIgnoresLaterResults();
        await CasRetryReevaluatesChinaDay();
        await LegacyApiRemainsSafe();
        var collectionsAfter = (await database.ListCollectionNames().ToListAsync()).OrderBy(x => x).ToArray();
        Check(collectionsBefore.SequenceEqual(collectionsAfter), "same-opponent rule creates no Mongo collection");
        Console.WriteLine("COMPLETE checks=" + checks + " prefix=" + Prefix);
    }

    static async Task FirstMatchPaysBothSides()
    {
        var pair = await NewPair("first");
        await Login(pair.A); await Login(pair.B);
        Record("first-match", pair.A, pair.B, now);
        await AddResult("first-match", pair.A.PlayerName, pair.B.PlayerName, now, false, false);
        Check((await Crown(pair.A, "first-r1", pair.B)).Wallet.Collection.DailyQuests.Crowns == 1, "first match awards first winning round crown");
        Check((await Crown(pair.A, "first-r2", pair.B)).Wallet.Collection.DailyQuests.Crowns == 2, "first match awards second winning round crown");
        Check(await Send(pair, true), "first match GG from A is accepted");
        Check(await Send(pair, false), "first match GG from B is accepted");
        Check((await Account(pair.A)).MeteoritePowder == 50, "first match A receives login plus two crowns and GG");
        Check((await Account(pair.B)).MeteoritePowder == 25, "first match B receives login and GG");
    }

    static async Task SecondMatchIsBlockedInBothDirections()
    {
        var pair = await NewPair("blocked");
        await Login(pair.A); await Login(pair.B);
        await AddResult("blocked-history", pair.A.PlayerName, pair.B.PlayerName, now.AddMinutes(-2), false, false);
        Record("blocked-match", pair.A, pair.B, now);
        var aBefore = await Account(pair.A); var bBefore = await Account(pair.B);
        var ar = await Crown(pair.A, "blocked-r1", pair.B); var br = await Crown(pair.B, "blocked-r1", pair.A);
        Check(ar.Wallet.Collection.DailyQuests.Crowns == aBefore.DailyQuests.Crowns && br.Wallet.Collection.DailyQuests.Crowns == bBefore.DailyQuests.Crowns, "second match blocks crowns for both directions");
        var blockedFirst = await db.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName,
            Prefix + "blocked-match", now);
        Check(blockedFirst.Success && blockedFirst.Processed && (await Account(pair.B)).MeteoritePowder == bBefore.MeteoritePowder,
            "first blocked GG is recorded once without powder");
        Check(await Send(pair, true) && await Send(pair, false), "second match keeps GG social acceptance in both directions while blocking powder");
        Check((await Account(pair.A)).MeteoritePowder == aBefore.MeteoritePowder && (await Account(pair.B)).MeteoritePowder == bBefore.MeteoritePowder, "blocked second match leaves both wallets unchanged");
        now = DateTimeOffset.Parse("2026-09-21T16:00:00Z");
        var retry = await Crown(pair.A, "blocked-r1", pair.B);
        var blockedReplay = await db.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName,
            Prefix + "blocked-match", now);
        Check(retry.Wallet.Collection.DailyQuests.Crowns == 0 && blockedReplay.Success && !blockedReplay.Processed &&
            (await Account(pair.B)).MeteoritePowder == 40 && blockedReplay.Result.Wallet.Collection.DailyQuests.GGReceived == 0,
            "blocked round and GG replay cannot pay after China midnight");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task DifferentOpponentAndOldDayAreAllowed()
    {
        var a = await NewUser("different-a"); var b = await NewUser("different-b"); var c = await NewUser("different-c");
        await Login(a); await Login(b); await Login(c);
        await AddResult("yesterday", a.PlayerName, b.PlayerName, now.AddDays(-1), false, false);
        Record("different-match", a, c, now);
        Check((await Crown(a, "different-r1", c)).Wallet.Collection.DailyQuests.Crowns == 1, "different opponent remains eligible");
        Check(await Send(new Pair(a, c), true), "different opponent GG remains eligible");
        var next = await NewUser("new-day"); await Login(next);
        now = DateTimeOffset.Parse("2026-09-21T16:00:01Z");
        Record("new-day-match", a, b, now);
        Check((await Crown(a, "new-day-r1", b)).Wallet.Collection.DailyQuests.Crowns == 1, "same opponent is eligible after China midnight");
        Check(await Send(new Pair(a, b), true), "same opponent GG is eligible after China midnight");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task PendingOnlyAndLateFirstResultAreSafe()
    {
        var pair = await NewPair("late"); await Login(pair.A); await Login(pair.B);
        Record("late-match", pair.A, pair.B, now);
        Check((await Crown(pair.A, "late-r1", pair.B)).Wallet.Collection.DailyQuests.Crowns == 1, "current match receipt alone does not suppress first crown");
        Check(await Send(pair, true), "current match receipt alone does not suppress first GG");
        // A second in-memory receipt may be registered before the first game result arrives.
        Record("late-second", pair.A, pair.B, now);
        Check((await Crown(pair.A, "late-r2", pair.B)).Wallet.Collection.DailyQuests.Crowns == 2, "late first result is not confused with current second receipt");
        await AddResult("late-match", pair.A.PlayerName, pair.B.PlayerName, now.AddMinutes(-1), false, false);
        Check(await Send(pair, true), "after the first result exists the later same-opponent GG remains socially accepted without powder");
    }

    static async Task SurrenderDisconnectAndColourSwapStillCount()
    {
        var pair = await NewPair("edge"); await Login(pair.A); await Login(pair.B);
        await AddResult("surrender", pair.B.PlayerName, pair.A.PlayerName, now, true, true);
        Record("edge-match", pair.A, pair.B, now);
        Check((await Crown(pair.A, "edge-r1", pair.B)).Wallet.Collection.DailyQuests.Crowns == 0, "surrendered prior match blocks later crown");
        Check(await Send(pair, true), "disconnected or surrendered prior match keeps GG social acceptance without powder");
        var other = await NewPair("edge-other"); await Login(other.A); await Login(other.B);
        await AddResult("ranked-history", other.A.PlayerName, other.B.PlayerName, now, false, true);
        Record("ranked-match", other.B, other.A, now); // red/blue swapped in the later receipt
        Check((await Crown(other.B, "ranked-r1", other.A)).Wallet.Collection.DailyQuests.Crowns == 0, "ranked history blocks after red-blue swap");
    }

    static async Task ConcurrencyAndRestartStayDeduplicated()
    {
        var pair = await NewPair("concurrent"); await Login(pair.A); await Login(pair.B);
        Record("concurrent-match", pair.A, pair.B, now);
        var crowns = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Crown(pair.A, "concurrent-r1", pair.B)));
        Check((await Account(pair.A)).DailyQuests.Crowns == 1 && crowns.Length == 16, "concurrent duplicate crowns settle once");
        var ggs = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Send(pair, true)));
        Check(ggs.All(x => x) && (await Account(pair.B)).MeteoritePowder == 25, "concurrent duplicate GGs settle once");
        await AddResult("concurrent-match", pair.A.PlayerName, pair.B.PlayerName, now.AddMinutes(-1), false, false);
        var restarted = new GwentDatabaseService(new ServiceCollection().AddSingleton<IMongoClient>(new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121")).AddSingleton(new InitialPowderOptions(0)).BuildServiceProvider()) { DailyQuestClock = () => now };
        Check((await restarted.GetDailyQuests(pair.B.UserName)).Wallet.Collection.DailyQuests.GGReceived == 1, "recreated database service retains blocked and paid ledger state");
        var restartCrown = await restarted.AwardDailyCrown(pair.A.UserName, pair.A.PlayerName, pair.B.PlayerName,
            Prefix + "concurrent-second", "concurrent-second:0", now);
        Check(restartCrown.Wallet.Collection.DailyQuests.Crowns == 1, "recreated database service still blocks a second same-opponent crown");
        var restartGg = await restarted.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName,
            Prefix + "concurrent-second", now);
        Check(restartGg.Success && restartGg.Processed && (await Account(pair.B)).MeteoritePowder == 25,
            "recreated database service records a blocked GG without powder");
        now = DateTimeOffset.Parse("2026-09-21T16:00:01Z");
        var crossDayReplay = await restarted.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName,
            Prefix + "concurrent-second", now);
        Check(crossDayReplay.Success && !crossDayReplay.Processed && (await Account(pair.B)).MeteoritePowder == 45 &&
            crossDayReplay.Result.Wallet.Collection.DailyQuests.GGReceived == 0 &&
            crossDayReplay.Result.Wallet.Collection.DailyQuests.GGPowderGranted == 0,
            "persisted blocked GG cannot replay for powder after China midnight");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task MidnightAndLoginStayIndependent()
    {
        var u = await NewUser("login");
        var before = await Login(u); Check(before.MeteoritePowder == 20, "login reward remains available without a match");
        now = DateTimeOffset.Parse("2026-09-21T15:59:59Z"); await Login(u);
        now = DateTimeOffset.Parse("2026-09-21T16:00:00Z");
        var next = await Login(u);
        Check(next.MeteoritePowder == 40 && next.DailyQuests.LoginGranted, "China midnight resets login independently of opponent blocking");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task ExactMidnightAndIncompleteHistory()
    {
        var pair = await NewPair("exact-midnight"); await Login(pair.A); await Login(pair.B);
        now = DateTimeOffset.Parse("2026-09-21T16:00:00Z");
        var history = await AddResult("midnight-history", pair.B.PlayerName, pair.A.PlayerName, now, false, false);
        await results.UpdateOneAsync(x => x.Id == history.Id,
            Builders<GameResult>.Update.Set(x => x.RedWinCount, 0).Set(x => x.BlueWinCount, 0).Set(x => x.ValidCount, 0));
        Record("midnight-second", pair.A, pair.B, now);
        var crown = await Crown(pair.A, "midnight-second:0", pair.B);
        Check(crown.Wallet.Collection.DailyQuests.Crowns == 0, "incomplete disconnect history at exact midnight blocks even same-millisecond second event");
        await Send(pair, true);
        var wallet = await Account(pair.B);
        Check(wallet.MeteoritePowder == 40 && wallet.DailyQuests.GGReceived == 0,
            "midnight block preserves new-day login but pays no GG even for non-effective unranked history");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task LateFirstGgIgnoresLaterResults()
    {
        var pair = await NewPair("late-first-gg"); await Login(pair.A); await Login(pair.B);
        var firstFinished = now;
        await AddResult("late-gg-first", pair.A.PlayerName, pair.B.PlayerName, firstFinished, false, false);
        await AddResult("late-gg-second", pair.B.PlayerName, pair.A.PlayerName, firstFinished.AddMinutes(1), false, false);
        now = firstFinished.AddMinutes(2);
        var first = await db.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName, Prefix + "late-gg-first", firstFinished);
        Check(first.Processed && first.Result.Wallet.Collection.MeteoritePowder == 25,
            "late first GG ignores its own saved result and later same-opponent result");
        var second = await db.AwardDailyGG(pair.B.UserName, pair.B.PlayerName, pair.A.PlayerName, Prefix + "late-gg-second", firstFinished.AddMinutes(1));
        Check(second.Processed && second.Result.Wallet.Collection.MeteoritePowder == 25 && second.Result.Wallet.Collection.DailyQuests.GGReceived == 1,
            "second match excludes itself but still sees first result and awards nothing");
        now = DateTimeOffset.Parse("2026-09-21T03:59:00Z");
    }

    static async Task CasRetryReevaluatesChinaDay()
    {
        var pair = await NewPair("cas-midnight"); await Login(pair.A); await Login(pair.B);
        var beforeMidnight = DateTimeOffset.Parse("2026-09-21T15:59:59.900Z");
        var afterMidnight = DateTimeOffset.Parse("2026-09-21T16:00:00.100Z");
        await AddResult("cas-history", pair.A.PlayerName, pair.B.PlayerName, beforeMidnight.AddMinutes(-1), false, false);
        int samples = 0;
        db.DailyQuestClock = () =>
        {
            if (Interlocked.Increment(ref samples) == 1)
            {
                // Force the actual Mongo revision compare/exchange to lose its first attempt.
                accounts.UpdateOne(x => x.Id == pair.A.Id, Builders<PremiumCollection>.Update.Inc(x => x.Revision, 1));
                return beforeMidnight;
            }
            return afterMidnight;
        };
        try
        {
            var result = await db.AwardDailyCrown(pair.A.UserName, pair.A.PlayerName, pair.B.PlayerName,
                Prefix + "cas-second", Prefix + "cas-second:0", beforeMidnight);
            Check(samples >= 2 && result.Wallet.Collection.DailyQuests.Day == "2026-09-22",
                "forced Mongo CAS conflict retries with the new China date");
            Check(result.Wallet.Collection.DailyQuests.Crowns == 1 && result.Wallet.Collection.MeteoritePowder == 40,
                "CAS retry does not reuse yesterday's blocked verdict or duplicate login credit");
        }
        finally { db.DailyQuestClock = () => now; }
    }

    static async Task LegacyApiRemainsSafe()
    {
        var u = await NewUser("legacy-api"); await Login(u);
        var state = await db.AwardDailyCrown(u.UserName, Prefix + "legacy-round", now);
        Check(state.Success && state.Wallet.Collection.DailyQuests.Crowns == 1, "legacy three-argument crown API remains safe and functional");
    }

    static async Task<PremiumCollection> Login(UserInfo u) => (await db.GetDailyQuests(u.UserName)).Wallet.Collection;
    static Task<PremiumCollection> Account(UserInfo u) => accounts.Find(x => x.Id == u.Id).FirstAsync();
    static async Task<UserInfo> NewUser(string tag)
    {
        var u = new UserInfo { UserName = Prefix + tag + "-" + Guid.NewGuid().ToString("N"), PlayerName = Prefix + " player " + Guid.NewGuid().ToString("N").Substring(0, 6), Decks = new List<DeckModel>() };
        await users.InsertOneAsync(u); return u;
    }
    sealed class Pair { public UserInfo A; public UserInfo B; public Pair(UserInfo a, UserInfo b) { A=a; B=b; } }
    static async Task<Pair> NewPair(string tag) => new Pair(await NewUser(tag + "-a"), await NewUser(tag + "-b"));

    static void Record(string id, UserInfo a, UserInfo b, DateTimeOffset at)
    {
        var ua = new User(a.UserName, "conn-" + a.Id) { PlayerName = a.PlayerName };
        var ub = new User(b.UserName, "conn-" + b.Id) { PlayerName = b.PlayerName };
        var matchId = Prefix + id;
        currentMatches[a.UserName + "|" + b.UserName] = matchId;
        currentMatches[b.UserName + "|" + a.UserName] = matchId;
        AddOnline(ua); AddOnline(ub); server.RecordFinishedMatch(matchId, ua, ub, at);
    }
    static void AddOnline(User u) => ((IDictionary<string, User>)typeof(GwentServerService).GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(server))[u.ConnectionId] = u;
    static async Task<bool> Send(Pair p, bool fromA)
    {
        var sender = fromA ? p.A : p.B; var receiver = fromA ? p.B : p.A;
        return await server.SendGG("conn-" + sender.Id, sender.PlayerName, receiver.PlayerName);
    }

    static async Task<GameResult> AddResult(string id, string red, string blue, DateTimeOffset at, bool surrender, bool ranked)
    {
        var result = new GameResult { Id = Prefix + id, Time = at.UtcDateTime, RedPlayerName = red, BluePlayerName = blue, RedPlayerGameResultStatus = GameStatus.Win, RedWinCount = 2, BlueWinCount = 0, ValidCount = 3, RedScore = new[] { 1, 1, 0 }, BlueScore = new[] { 0, 0, 0 }, isSurrender = surrender, isRanked = ranked };
        await results.InsertOneAsync(result); return result;
    }

    static async Task<DailyQuestResult> Crown(UserInfo user, string round, UserInfo opponent)
    {
        string matchId;
        if (!currentMatches.TryGetValue(user.UserName + "|" + opponent.UserName, out matchId))
            matchId = Prefix + "match-for-" + round;
        return await db.AwardDailyCrown(user.UserName, user.PlayerName, opponent.PlayerName,
            matchId, round, now);
    }

    // Locate src/Cynthia.Card/src/Cynthia.Card.Server by walking up from the test output so the
    // suite never depends on ignored work artifacts or a manually preset working directory.
    static string FindServerDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "Cynthia.Card", "src", "Cynthia.Card.Server");
            if (File.Exists(Path.Combine(candidate, "Locales", "config.json"))) return candidate;
            dir = dir.Parent;
        }
        throw new Exception("Cynthia.Card.Server directory not found above " + AppContext.BaseDirectory);
    }

    static GwentServerService MakeServer()
    {
        var hub = new Hub();
        return new GwentServerService(hub, db, dbProvider(), new Env(), new GwentCardDataService(), new GwentLocalizationService(),
            new RewardSettlementService(db, hub, NullLogger<RewardSettlementService>.Instance),
            new PremiumDeckSelectionService(db, NullLogger<PremiumDeckSelectionService>.Instance),
            new InitialPowderGrantService(db, NullLogger<InitialPowderGrantService>.Instance));
    }
    static IServiceProvider dbProvider() => new ServiceCollection().AddSingleton<IMongoClient>(new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121")).AddSingleton(new InitialPowderOptions(0)).AddSingleton<GwentCardDataService>().BuildServiceProvider();
    sealed class Hub : IHubContext<GwentHub> { public IHubClients Clients { get; } = new HubClients(); public IGroupManager Groups { get; } = new Groups(); }
    sealed class HubClients : IHubClients { public IClientProxy All=>new Proxy(); public IClientProxy AllExcept(IReadOnlyList<string>x)=>new Proxy(); public IClientProxy Client(string x)=>new Proxy(); public IClientProxy Clients(IReadOnlyList<string>x)=>new Proxy(); public IClientProxy Group(string x)=>new Proxy(); public IClientProxy Groups(IReadOnlyList<string>x)=>new Proxy(); public IClientProxy GroupExcept(string x,IReadOnlyList<string>y)=>new Proxy(); public IClientProxy User(string x)=>new Proxy(); public IClientProxy Users(IReadOnlyList<string>x)=>new Proxy(); }
    sealed class Proxy : IClientProxy { public Task SendCoreAsync(string m, object[] a, CancellationToken c=default) => Task.CompletedTask; }
    sealed class Groups : IGroupManager { public Task AddToGroupAsync(string c,string g,CancellationToken t=default)=>Task.CompletedTask; public Task RemoveFromGroupAsync(string c,string g,CancellationToken t=default)=>Task.CompletedTask; }
    sealed class Env : IWebHostEnvironment { public string EnvironmentName {get;set;}="Test"; public string ApplicationName {get;set;}="SameOpponentRewardsTest"; public string ContentRootPath {get;set;}=Directory.GetCurrentDirectory(); public IFileProvider ContentRootFileProvider {get;set;} public string WebRootPath {get;set;} public IFileProvider WebRootFileProvider {get;set;} }
}
