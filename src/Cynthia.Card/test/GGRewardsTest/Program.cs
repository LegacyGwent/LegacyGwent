using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Integration checks for postgame GG powder rewards. Reward/ledger cases run through the real
// GwentDatabaseService; the authorization cases reuse the real GwentServerService.SendGG path
// with an in-memory connection registry (no SignalR transport and no full network match).
class Program
{
    static int checks;
    static void Check(bool value, string text) { if (!value) throw new Exception("FAIL " + text); Console.WriteLine("PASS " + text); checks++; }
    static IMongoCollection<UserInfo> users;
    static IMongoCollection<PremiumCollection> accounts;
    static GwentDatabaseService db;
    static Func<DateTimeOffset> clock;
    static DateTimeOffset now = DateTimeOffset.Parse("2026-09-13T15:59:00Z");

    static async Task Main()
    {
        clock = () => now;
        // Old-format fixture: no GG fields, so the server defaults (5 / 30) must apply.
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "DailyQuests.json"),
            "{\"LoginPowder\":20,\"Tiers\":[{\"Crowns\":2,\"Powder\":25},{\"Crowns\":4,\"Powder\":35},{\"Crowns\":6,\"Powder\":45}]}");
        var mongo = new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121");
        var provider = new ServiceCollection().AddSingleton<IMongoClient>(mongo).AddSingleton(new InitialPowderOptions(0))
            .AddSingleton<GwentCardDataService>().BuildServiceProvider();
        users = mongo.GetDatabase("gwentdiy").GetCollection<UserInfo>("user");
        accounts = mongo.GetDatabase("gwentdiy").GetCollection<PremiumCollection>("premium_collection");
        db = new GwentDatabaseService(provider) { DailyQuestClock = clock };

        await RewardContract();
        await LedgerAndReplay();
        await WireContract();
        await Authorization();
        Console.WriteLine("COMPLETE checks=" + checks);
    }

    static async Task RewardContract()
    {
        var sender = await NewUser("giver");
        var recipient = await NewUser("taker");
        // Settle both first logins so the GG delta is unambiguous.
        Check((await db.GetDailyQuests(sender.UserName)).Wallet.Collection.MeteoritePowder == 20, "sender login settles once");
        var fresh = await db.GetDailyQuests(recipient.UserName);
        Check(fresh.DailyCap == 155 && fresh.GGPowder == 5 && fresh.GGDailyCap == 30, "old fixture defaults to a GG-inclusive daily cap of 155");
        Check(fresh.Wallet.Collection.MeteoritePowder == 20, "recipient login settles once");

        var first = await db.AwardDailyGG(recipient.UserName, "match-1", now);
        Check(first.Success && first.Processed, "valid GG settles and is newly processed");
        Check((await Account(recipient)).MeteoritePowder == 25, "recipient receives exactly five powder");
        Check((await Account(sender)).MeteoritePowder == 20, "sender wallet is never credited by its own GG");
        var progress = (await Account(recipient)).DailyQuests;
        Check(progress.GGReceived == 1 && progress.GGPowderGranted == 5 && progress.PowderGranted == 25, "progress tracks GG count and powder subset");

        for (int i = 2; i <= 6; i++) await db.AwardDailyGG(recipient.UserName, "match-" + i, now);
        progress = (await Account(recipient)).DailyQuests;
        Check((await Account(recipient)).MeteoritePowder == 50 && progress.GGReceived == 6 && progress.GGPowderGranted == 30, "six valid GGs reach exactly the thirty powder cap");
        var seventh = await db.AwardDailyGG(recipient.UserName, "match-7", now);
        progress = (await Account(recipient)).DailyQuests;
        Check(seventh.Success && seventh.Processed && (await Account(recipient)).MeteoritePowder == 50 && progress.GGReceived == 6, "seventh GG adds no powder or progress but is still deduplicated");
        Check(progress.ProcessedGGIds.Count == 7, "capped GG events stay in the persistent ledger");

        var other = await NewUser("other");
        await db.GetDailyQuests(other.UserName);
        await db.AwardDailyGG(other.UserName, "match-1", now);
        Check((await Account(other)).MeteoritePowder == 25, "GG rewards are isolated per recipient account");
    }

    static async Task LedgerAndReplay()
    {
        var recipient = await NewUser("replay");
        await db.GetDailyQuests(recipient.UserName);

        // Duplicate and concurrent identical match ids settle once.
        var duplicate = await Task.WhenAll(Enumerable.Range(0, 32).Select(_ => db.AwardDailyGG(recipient.UserName, "same-match", now)));
        Check(duplicate.Count(x => x.Processed) == 1 && (await Account(recipient)).MeteoritePowder == 25, "32 concurrent identical GG events pay once");

        // A different finished match against the same opponent earns again.
        await db.AwardDailyGG(recipient.UserName, "next-match", now);
        Check((await Account(recipient)).MeteoritePowder == 30 && (await Account(recipient)).DailyQuests.GGReceived == 2, "a unique later match can earn again");

        // More than six simultaneous unique events cannot exceed the cap.
        var burst = await NewUser("burst");
        await db.GetDailyQuests(burst.UserName);
        await Task.WhenAll(Enumerable.Range(0, 24).Select(i => db.AwardDailyGG(burst.UserName, "burst-" + i, now)));
        var burstProgress = (await Account(burst)).DailyQuests;
        Check((await Account(burst)).MeteoritePowder == 50 && burstProgress.GGReceived == 6 && burstProgress.GGPowderGranted == 30, "24 simultaneous unique GGs stop exactly at the cap");

        // Server recreation retains the ledger and GG progress.
        var restarted = new GwentDatabaseService(new ServiceCollection().AddSingleton<IMongoClient>(new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121")).AddSingleton(new InitialPowderOptions(0)).BuildServiceProvider()) { DailyQuestClock = clock };
        var replay = await restarted.AwardDailyGG(burst.UserName, "burst-0", now);
        Check(replay.Success && !replay.Processed && (await Account(burst)).MeteoritePowder == 50, "recreated server rejects a replayed match id");
        Check((await restarted.GetDailyQuests(burst.UserName)).Wallet.Collection.DailyQuests.GGReceived == 6, "recreated server retains GG progress");

        // China midnight resets the daily GG allowance while keeping the ledger.
        now = DateTimeOffset.Parse("2026-09-13T16:00:01Z");
        var nextDay = await db.GetDailyQuests(burst.UserName);
        Check(nextDay.Wallet.Collection.DailyQuests.GGReceived == 0 && nextDay.Wallet.Collection.DailyQuests.GGPowderGranted == 0 && nextDay.Wallet.Collection.DailyQuests.Day == "2026-09-14", "China midnight resets GG progress without touching the balance");
        await db.AwardDailyGG(burst.UserName, "after-midnight", now);
        Check((await Account(burst)).MeteoritePowder == 75 && (await Account(burst)).DailyQuests.GGReceived == 1, "next China day can earn GG powder again");
        var cappedReplay = await db.AwardDailyGG(burst.UserName, "burst-7", now);
        Check(cappedReplay.Success && !cappedReplay.Processed && (await Account(burst)).MeteoritePowder == 75 && (await Account(burst)).DailyQuests.GGReceived == 1, "yesterday's capped GG replay cannot pay on the new day");

        // Legacy wallets without the GG ledger upgrade without losing credited GG progress.
        var legacy = await NewUser("legacy");
        await db.GetDailyQuests(legacy.UserName);
        await db.AwardDailyGG(legacy.UserName, "legacy-1", now);
        await accounts.UpdateOneAsync(x => x.Id == legacy.Id, Builders<PremiumCollection>.Update.Unset("DailyQuests.ProcessedGGIds"));
        await db.AwardDailyCrown(legacy.UserName, "legacy-crown:0", now);
        var upgraded = await db.GetDailyQuests(legacy.UserName);
        Check(upgraded.Wallet.Collection.DailyQuests.GGReceived == 1 && upgraded.Wallet.Collection.DailyQuests.GGPowderGranted == 5 && upgraded.Wallet.Collection.MeteoritePowder == 25, "legacy upgrade preserves GG counts and rewards");
        Check(upgraded.Wallet.Collection.DailyQuests.ProcessedGGIds != null && upgraded.Wallet.Collection.DailyQuests.ProcessedGGIds.Count == 0, "legacy upgrade initializes the ledger; unset historical GG ids cannot be reconstructed from counts");

        // Same-day crown settlement must clone, not drop, GG fields.
        var clone = await db.AwardDailyCrown(legacy.UserName, "legacy-crown:1", now);
        Check(clone.Wallet.Collection.DailyQuests.GGReceived == 1 && clone.Wallet.Collection.DailyQuests.GGPowderGranted == 5, "same-day cloning preserves GG fields");

        now = DateTimeOffset.Parse("2026-09-13T15:59:00Z");
    }

    static async Task WireContract()
    {
        var recipient = await NewUser("wire");
        await db.GetDailyQuests(recipient.UserName);
        await db.AwardDailyGG(recipient.UserName, "wire-1", now);
        var result = await db.GetDailyQuests(recipient.UserName);
        Check(result.GGPowder == 5 && result.GGDailyCap == 30 && result.DailyCap == 155, "daily result publishes the GG contract and the GG-inclusive cap");

        var newtonsoft = JsonConvert.SerializeObject(result);
        Check(!newtonsoft.Contains("ProcessedGGIds") && !newtonsoft.Contains("ProcessedRoundIds"), "Newtonsoft output hides both persistent ledgers");
        Check(newtonsoft.Contains("GGReceived") && newtonsoft.Contains("GGPowderGranted"), "Newtonsoft output exposes the GG progress fields");

        var options = new JsonSerializerOptions { Converters = { new DailyQuestProgressJsonConverter() } };
        var stj = System.Text.Json.JsonSerializer.Serialize(result.Wallet.Collection.DailyQuests, options);
        Check(!stj.Contains("ProcessedGGIds") && !stj.Contains("ProcessedRoundIds"), "System.Text.Json converter hides both persistent ledgers");
        Check(stj.Contains("GGReceived") && JObject.Parse(stj)["GGPowderGranted"] != null, "System.Text.Json converter exposes the GG progress fields");
    }

    static async Task Authorization()
    {
        // The real GwentLocalizationService loads Locales relative to the process working directory.
        Directory.SetCurrentDirectory(FindServerDirectory());
        var hub = new StubHubContext();
        var service = new GwentServerService(hub, db, ServiceProvider(), new StubEnvironment(),
            new GwentCardDataService(), new GwentLocalizationService(),
            new RewardSettlementService(db, hub, NullLogger<RewardSettlementService>.Instance),
            new PremiumDeckSelectionService(db, NullLogger<PremiumDeckSelectionService>.Instance),
            new InitialPowderGrantService(db, NullLogger<InitialPowderGrantService>.Instance));
        var online = (IDictionary<string, User>)typeof(GwentServerService)
            .GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service);

        var sender = await NewUser("auth-sender");
        var recipient = await NewUser("auth-recipient");
        var bystander = await NewUser("auth-bystander");
        await db.GetDailyQuests(recipient.UserName);
        await db.GetDailyQuests(bystander.UserName);
        online["conn-sender"] = new User(sender.UserName, "conn-sender") { PlayerName = sender.PlayerName };
        online["conn-recipient"] = new User(recipient.UserName, "conn-recipient") { PlayerName = recipient.PlayerName };

        Check(!await service.SendGG("unknown-connection", sender.PlayerName, recipient.PlayerName), "unauthenticated connection cannot send GG");
        Check(!await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "GG without a finished match is rejected");
        Check((await Account(recipient)).MeteoritePowder == 20, "rejected GG leaves the wallet untouched");

        service.RecordFinishedMatch("auth-match-1", online["conn-sender"], online["conn-recipient"]);
        Check(!await service.SendGG("conn-sender", "ForgedName", recipient.PlayerName), "forged sender display name is rejected");
        Check((await Account(recipient)).MeteoritePowder == 20, "forged sender grants nothing");
        Check(!await service.SendGG("conn-sender", sender.PlayerName, bystander.PlayerName), "a non-participant recipient is rejected");
        Check((await Account(bystander)).MeteoritePowder == 20, "non-participant wallet is untouched");
        Check(!await service.SendGG("conn-sender", sender.PlayerName, "NotARealPlayer"), "an arbitrary recipient name is rejected");
        Check(!await service.SendGG("conn-sender", sender.PlayerName, sender.PlayerName), "self GG is rejected");

        Check(await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "authenticated postgame GG to the real opponent succeeds");
        Check((await Account(recipient)).MeteoritePowder == 25, "authorized GG credits the recipient five powder");
        Check(await GGCounter(recipient) == 1, "social GG count increments once for the first valid GG");
        Check(await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "duplicate send is idempotent");
        Check((await Account(recipient)).MeteoritePowder == 25, "duplicate send cannot pay twice");
        Check(await GGCounter(recipient) == 1, "duplicate send does not repeat the social count");

        // Disconnected recipients still keep the reward; the social message is simply skipped.
        online.Remove("conn-recipient");
        service.RecordFinishedMatch("auth-match-2", online["conn-sender"], new User(recipient.UserName, "conn-recipient") { PlayerName = recipient.PlayerName });
        Check(await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "disconnected recipient still receives the reward");
        Check((await Account(recipient)).MeteoritePowder == 30, "offline GG persisted to the wallet");
        Check(await GGCounter(recipient) == 2, "offline valid GG still increments the social count");

        // AI / self matches never record a receipt, so the same call stays rejected.
        var ai = await NewUser("auth-ai");
        online["conn-ai"] = new User(ai.UserName, "conn-ai") { PlayerName = ai.PlayerName };
        Check(!await service.SendGG("conn-ai", ai.PlayerName, recipient.PlayerName), "AI or unfinished game has no receipt and cannot pay");

        // Cap through the real service path: further valid GGs stay socially accepted but not rewarded.
        online["conn-recipient"] = new User(recipient.UserName, "conn-recipient") { PlayerName = recipient.PlayerName };
        for (int i = 3; i <= 6; i++)
        {
            service.RecordFinishedMatch("auth-match-" + i, online["conn-sender"], online["conn-recipient"]);
            Check(await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "service accepts valid GG " + i);
        }
        Check((await Account(recipient)).MeteoritePowder == 50 && (await Account(recipient)).DailyQuests.GGReceived == 6, "service path enforces the daily GG cap");
        service.RecordFinishedMatch("auth-match-7", online["conn-sender"], online["conn-recipient"]);
        Check(await service.SendGG("conn-sender", sender.PlayerName, recipient.PlayerName), "capped GG is still socially valid");
        Check((await Account(recipient)).MeteoritePowder == 50, "capped GG grants no more powder through the service");
        Check(await GGCounter(recipient) == 7, "validated capped GGs still increment the social count once each");
    }

    static IServiceProvider ServiceProvider()
    {
        var mongo = new MongoClient(Environment.GetEnvironmentVariable("REWARD_TEST_MONGO_URI") ?? "mongodb://127.0.0.1:28121");
        return new ServiceCollection().AddSingleton<IMongoClient>(mongo).AddSingleton(new InitialPowderOptions(0))
            .AddSingleton<GwentCardDataService>().BuildServiceProvider();
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

    static async Task<UserInfo> NewUser(string prefix)
    {
        var user = new UserInfo { UserName = "gg-" + prefix + "-" + Guid.NewGuid().ToString("N"), PlayerName = "GG " + prefix + " " + Guid.NewGuid().ToString("N").Substring(0, 6), Decks = new List<DeckModel>() };
        await users.InsertOneAsync(user);
        return user;
    }

    static Task<PremiumCollection> Account(UserInfo user) => accounts.Find(x => x.Id == user.Id).FirstAsync();

    static async Task<int> GGCounter(UserInfo user) => (await users.Find(x => x.Id == user.Id).FirstAsync()).GGsReceived;

    sealed class StubHubContext : IHubContext<GwentHub>
    {
        public IHubClients Clients { get; } = new StubClients();
        public IGroupManager Groups { get; } = new StubGroups();
    }
    sealed class StubClients : IHubClients
    {
        public IClientProxy All => new StubProxy();
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => new StubProxy();
        public IClientProxy Client(string connectionId) => new StubProxy();
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => new StubProxy();
        public IClientProxy Group(string groupName) => new StubProxy();
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => new StubProxy();
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new StubProxy();
        public IClientProxy User(string userId) => new StubProxy();
        public IClientProxy Users(IReadOnlyList<string> userIds) => new StubProxy();
    }
    sealed class StubProxy : IClientProxy
    {
        public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    sealed class StubGroups : IGroupManager
    {
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
    sealed class StubEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "GGRewardsTest";
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}
