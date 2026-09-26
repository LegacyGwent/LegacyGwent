using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

partial class Program
{
    static GwentDatabaseService InitialService(long amount) => new GwentDatabaseService(
        new ServiceCollection().AddSingleton(db.GetMongoClient()).AddSingleton(new InitialPowderOptions(amount)).BuildServiceProvider())
        { DailyQuestClock = () => Now };
    static int InitialGrants(PremiumCollection account) => account.Rewards.Count(x => x.RewardId == GwentDatabaseService.InitialPowderRewardId);

    static async Task InitialBackfillCommandCounts()
    {
        // The dedicated client observes only this service, excluding unrelated hosted workers.
        long writes = 0, reads = 0;
        var settings = db.GetMongoClient().Settings.Clone();
        var configure = settings.ClusterConfigurator;
        settings.ClusterConfigurator = builder =>
        {
            configure?.Invoke(builder);
            builder.Subscribe<CommandStartedEvent>(command =>
            {
                if (command.DatabaseNamespace.DatabaseName != "gwentdiy") return;
                if (command.CommandName == "update" || command.CommandName == "findAndModify") Interlocked.Increment(ref writes);
                if (command.CommandName == "find" || command.CommandName == "getMore") Interlocked.Increment(ref reads);
            });
        };
        using var provider = new ServiceCollection().AddSingleton<IMongoClient>(new MongoClient(settings))
            .AddSingleton(new InitialPowderOptions(1700)).BuildServiceProvider();
        var service = new GwentDatabaseService(provider);
        var prefix = "initial-batch-" + Guid.NewGuid().ToString("N");
        var batch = Enumerable.Range(0, 520).Select(index => new UserInfo
            { UserName = prefix + index, PlayerName = prefix + index }).ToList();
        await Users.InsertManyAsync(batch);
        var ids = batch.Select(user => user.Id).ToList();
        try
        {
            Check(await service.BackfillInitialPowder() == batch.Count,
                "initial backfill grants a cohort spanning multiple cursor batches");
            Interlocked.Exchange(ref writes, 0); Interlocked.Exchange(ref reads, 0);
            Check(await service.BackfillInitialPowder() == 0, "successful second batched sweep grants nothing twice");
            long sweepWrites = Interlocked.Read(ref writes), sweepReads = Interlocked.Read(ref reads);
            long totalUsers = await Users.CountDocumentsAsync(Builders<UserInfo>.Filter.Empty);
            Check(sweepWrites == 0 && sweepReads <= 3 * ((totalUsers + 511) / 512) + 4,
                "second sweep performs bounded batch reads and zero per-user updates",
                new { sweepWrites, sweepReads, totalUsers });
            Interlocked.Exchange(ref writes, 0);
            var wallet = (await service.GetPremiumCollection(batch[0].UserName)).Collection;
            Check(Interlocked.Read(ref writes) == 0 && wallet.MeteoritePowder == 1700 && InitialGrants(wallet) == 1,
                "reading a claimed wallet avoids initial-grant writes");

            var later = await NewUser("initial-batch-new"); ids.Add(later.Id);
            Check(await service.BackfillInitialPowder() == 1 && (await Account(later)).MeteoritePowder == 1700,
                "a new account still receives the configured grant after a converged sweep");

            var repair = batch[1];
            await Accounts.UpdateOneAsync(account => account.Id == repair.Id,
                Builders<PremiumCollection>.Update.Set(account => account.Rewards, null));
            Check(await service.BackfillInitialPowder() == 0 && (await Account(repair)).Rewards != null &&
                    (await Account(repair)).MeteoritePowder == 1700,
                "batched filtering retains claimed null-ledger repair without paying again");
            var receiptOnly = batch[2];
            await Accounts.UpdateOneAsync(account => account.Id == receiptOnly.Id,
                Builders<PremiumCollection>.Update.Set(account => account.InitialPowderGranted, false));
            Check(await service.BackfillInitialPowder() == 0 && (await Account(receiptOnly)).InitialPowderGranted &&
                    (await Account(receiptOnly)).MeteoritePowder == 1700,
                "receipt-only claims still promote the flag without another payment");
        }
        finally
        {
            await Users.DeleteManyAsync(Builders<UserInfo>.Filter.In(user => user.Id, ids));
            await Accounts.DeleteManyAsync(Builders<PremiumCollection>.Filter.In(account => account.Id, ids));
        }
    }

    static async Task InitialPowderCases()
    {
        var configured = InitialPowderOptions.Load();
        Check(configured.Amount == 3000, "shipped initial powder configuration is 3000");
        var path = Path.Combine(work, "initial-config.json");
        foreach (var json in new[] { "{", "null", "{}", "{\"Amount\":null}", "{\"Amount\":-1}", "{\"Amount\":1000000001}", "{\"Amount\":5.5}", "{\"Amount\":\"5000\"}" })
        {
            File.WriteAllText(path, json); bool rejected = false;
            try { InitialPowderOptions.Load(path); } catch { rejected = true; }
            Check(rejected, "invalid initial powder configuration rejected: " + json);
        }
        File.Delete(path);
        bool missing = false; try { InitialPowderOptions.Load(path); } catch (FileNotFoundException) { missing = true; }
        Check(missing, "missing initial configuration fails explicitly");
        foreach (var value in new long[] { 0, 1200, 1000000000 })
        {
            File.WriteAllText(path, "{\"Enabled\":" + (value == 0 ? "false" : "true") + ",\"Amount\":" + value + "}");
            var loaded = InitialPowderOptions.Load(path);
            Check(loaded.Amount == value && loaded.Enabled == (value != 0), "valid configurable amount " + value);
        }

        var service = InitialService(5000);
        var other = InitialService(5000);
        var old = await NewUser("initial-no-wallet");
        await Task.WhenAll(Enumerable.Range(0, 40).Select(i => (i % 2 == 0 ? service : other).GetPremiumCollection(old.UserName)));
        var wallet = await Account(old);
        Check(wallet.MeteoritePowder == 5000 && wallet.Revision == 1 && InitialGrants(wallet) == 1,
            "40 concurrent requests across services grant an old account without a wallet exactly once");
        Check(wallet.Rewards.Single().Amount == 5000 && DateTimeOffset.TryParse(wallet.Rewards.Single().GrantedUtc, out _),
            "initial reward records actual amount and UTC audit time");
        var card = (await service.GetPremiumCollection(old.UserName)).Costs.First(x => x.Value == 100).Key;
        await service.CraftPremium(old.UserName, card);
        var spent = (await other.GetPremiumCollection(old.UserName)).Collection;
        Check(spent.MeteoritePowder == 4900 && InitialGrants(spent) == 1, "spent initial powder is never refilled on login or reload");

        var rich = await NewUser("initial-existing-wallet");
        var existing = new PremiumCollection { Id = rich.Id, MeteoritePowder = 7000, Revision = 9,
            DailyQuests = new DailyQuestProgress { Day = "2026-09-14", Crowns = 3, LoginGranted = true, PowderGranted = 20 } };
        existing.OwnedCards.Add(card); existing.SelectedCards.Add(card);
        existing.Rewards.Add(new PowderReward { RewardId = "unrelated-admin", Amount = 7000 });
        await Accounts.InsertOneAsync(existing);
        wallet = (await service.GetPremiumCollection(rich.UserName)).Collection;
        Check(wallet.MeteoritePowder == 12000 && wallet.Revision == 11 && wallet.InventoryVersion == 1, "existing balances above 5000 receive an additive grant and one inventory migration");
        Check(wallet.OwnedCards.Contains(card) && wallet.SelectedCards.Contains(card) && wallet.DailyQuests.Crowns == 3 && wallet.Rewards.Count == 2,
            "initial grant preserves ownership, selection, daily progress and other rewards");

        var raw = db.GetMongoClient().GetDatabase("gwentdiy").GetCollection<BsonDocument>("premium_collection");
        foreach (bool explicitNull in new[] { false, true })
        {
            var legacy = await NewUser("initial-legacy-ledger");
            await db.GetPremiumCollection(legacy.UserName);
            await raw.UpdateOneAsync(new BsonDocument("_id", legacy.Id), explicitNull
                ? new BsonDocument("$set", new BsonDocument("Rewards", BsonNull.Value))
                : new BsonDocument("$unset", new BsonDocument("Rewards", "")));
            wallet = (await service.GetPremiumCollection(legacy.UserName)).Collection;
            Check(wallet.MeteoritePowder == 5000 && InitialGrants(wallet) == 1,
                "legacy " + (explicitNull ? "null" : "missing") + " ledger is safely initialized");
        }
        var paused = await NewUser("initial-paused");
        wallet = (await InitialService(0).GetPremiumCollection(paused.UserName)).Collection;
        Check(wallet.MeteoritePowder == 0 && InitialGrants(wallet) == 0, "zero pauses granting without consuming eligibility");
        Check((await service.GetPremiumCollection(paused.UserName)).Collection.MeteoritePowder == 5000, "resume grants previously paused account");
        var tuned = InitialService(1200);
        Check((await tuned.GetPremiumCollection(old.UserName)).Collection.MeteoritePowder == 4900, "lower configured amount does not remove existing funds");
        Check((await InitialService(9000).GetPremiumCollection(old.UserName)).Collection.MeteoritePowder == 4900, "higher configured amount does not grant twice");
        var later = await NewUser("initial-tuned");
        Check((await tuned.GetPremiumCollection(later.UserName)).Collection.MeteoritePowder == 1200, "unclaimed accounts receive the newly configured amount");

        var mixed = await NewUser("initial-mixed");
        var operations = Enumerable.Range(0, 20).Select(i => i % 2 == 0
            ? (Task)service.CraftPremium(mixed.UserName, card)
            : other.AwardDailyCrown(mixed.UserName, "initial-mixed-round", Now));
        await Task.WhenAll(operations);
        wallet = await Account(mixed);
        Check(wallet.MeteoritePowder == 4920 && wallet.DailyQuests.Crowns == 1 && wallet.OwnedCards.Count == 1 && InitialGrants(wallet) == 1,
            "initial grant, daily login, duplicate crowns and crafting compose atomically");
        Check((await service.GetPremiumCollection("missing-initial-identity")).Status == "unauthenticated", "unknown identity cannot claim initial powder");

        var overflow = await NewUser("initial-overflow");
        await Accounts.InsertOneAsync(new PremiumCollection { Id = overflow.Id, MeteoritePowder = 9007199254740991 });
        bool overflowRejected = false;
        try { await service.GetPremiumCollection(overflow.UserName); } catch (InvalidDataException) { overflowRejected = true; }
        wallet = await Account(overflow);
        Check(overflowRejected && wallet.MeteoritePowder == 9007199254740991 && InitialGrants(wallet) == 0,
            "overflow rejects the entire grant without consuming eligibility");
        var offline = await NewUser("initial-offline");
        int errors = 0; bool failedPass = false;
        try { await service.BackfillInitialPowder(onError: (id, e) => errors++); } catch (InvalidOperationException) { failedPass = true; }
        Check(failedPass && errors == 1 && (await Account(offline)).MeteoritePowder == 5000,
            "one corrupt wallet does not prevent other offline accounts from receiving backfill");
        await Accounts.UpdateOneAsync(x => x.Id == overflow.Id, Builders<PremiumCollection>.Update.Set(x => x.MeteoritePowder, 0).Inc(x => x.Revision, 1));
        Check(await service.BackfillInitialPowder() == 1, "retry resumes only the previously failed account");
        Check(await other.BackfillInitialPowder() == 0, "repeated whole-server backfill does not duplicate grants");
        await InitialBackfillCommandCounts();
        using (var cancelled = new CancellationTokenSource())
        {
            cancelled.Cancel(); bool stopped = false;
            try { await service.BackfillInitialPowder(cancelled.Token); } catch (OperationCanceledException) { stopped = true; }
            Check(stopped, "shutdown cancels backfill cleanly");
        }

        // Exercise the production hosted service and actual SignalR registration/login with the grant enabled.
        var startupOnly = await NewUser("initial-hosted-offline");
        await host.StopAsync(); host.Dispose(); host = null;
        SuiteStartup.InitialAmount = 5000; SuiteStartup.RunInitialBackfill = true;
        await StartHost();
        var deadline = DateTimeOffset.UtcNow.AddSeconds(15);
        while (DateTimeOffset.UtcNow < deadline)
        {
            var current = await Accounts.Find(x => x.Id == startupOnly.Id).FirstOrDefaultAsync();
            if (current != null && InitialGrants(current) == 1) break;
            await Task.Delay(100);
        }
        Check((await Account(startupOnly)).MeteoritePowder == 5000, "production background service grants an offline account on server startup");
        using (var client = await WireClient.Connect(url))
        {
            var name = "initial-register-" + Guid.NewGuid().ToString("N");
            Check(await client.Invoke<bool>("Register", name, "RewardTest2026", name), "network registration succeeds with initial powder enabled");
            var registered = await Users.Find(x => x.UserName == name).FirstAsync();
            deadline = DateTimeOffset.UtcNow.AddSeconds(15);
            while (DateTimeOffset.UtcNow < deadline && await Accounts.CountDocumentsAsync(x => x.Id == registered.Id) == 0)
                await Task.Delay(100);
            Check((await Account(registered)).MeteoritePowder == 5000, "new account receives powder before first login");
            await client.Invoke<UserInfo>("Login", name, "RewardTest2026");
            var response = await client.Invoke<PremiumCollectionResult>("GetPremiumCollection");
            Check(response.Collection.MeteoritePowder == 5020 && InitialGrants(response.Collection) == 1,
                "real login and client wallet response contain 5000 initial plus 20 daily powder");
        }
        await host.StopAsync(); host.Dispose(); host = null;
        await StartHost();
        wallet = await Account(old);
        Check(wallet.MeteoritePowder == 4900 && InitialGrants(wallet) == 1, "actual host restart preserves spent wallet and claim identity");
    }
}
