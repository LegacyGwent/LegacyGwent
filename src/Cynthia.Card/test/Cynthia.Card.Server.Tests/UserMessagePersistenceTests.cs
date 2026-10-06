using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    // Explicit opt-in; use only a disposable local mongod. No external/standard databases.
    public sealed class SeasonMongoFactAttribute : FactAttribute
    {
        public SeasonMongoFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("LEGACYGWENT_SEASON_TEST_MONGO") != "mongodb://127.0.0.1:28163")
                Skip = "Requires disposable MongoDB on 127.0.0.1:28163 and LEGACYGWENT_SEASON_TEST_MONGO opt-in.";
        }
    }

    public class UserMessagePersistenceTests : IDisposable
    {
        private readonly IMongoDatabase _mongo;
        private readonly IMongoCollection<UserInfo> _users;
        private readonly ServiceProvider _provider;
        private readonly GwentDatabaseService _database;
        private readonly string _prefix = "season-test-" + Guid.NewGuid().ToString("N");

        public UserMessagePersistenceTests()
        {
            var client = new MongoClient("mongodb://127.0.0.1:28163/?serverSelectionTimeoutMS=3000");
            _mongo = client.GetDatabase("gwentdiy");
            _users = _mongo.GetCollection<UserInfo>("user");
            _provider = new ServiceCollection().AddSingleton<IMongoClient>(client).BuildServiceProvider();
            _database = new GwentDatabaseService(_provider);
        }

        [SeasonMongoFact]
        public async Task OldAccountBacklogUsesOneIdentityAndDrainsWithoutRankOrRewards()
        {
            var user = NewUser("login", "display");
            user.UserMessages = new List<string> { Code(7), Code(11), Code(20) };
            var other = NewUser("other-login", "login"); // collision with first account's login
            other.UserMessages = new List<string> { Code(7, "other-season") };
            await _users.InsertManyAsync(new[] { user, other });
            Assert.Equal(user.UserMessages, _database.QueryUserMessages(user.UserName));
            foreach (var message in user.UserMessages)
                Assert.True(await _database.RemoveUserMessage(user.UserName, UserMessage.ReCreateMessage(message).MessageId));
            Assert.Empty(_database.QueryUserMessages(user.UserName));
            Assert.Equal(other.UserMessages, _database.QueryUserMessages(other.UserName));
            Assert.True(await _database.RemoveUserMessage(user.UserName, 20)); // lost response/retry
        }

        [SeasonMongoFact]
        public async Task ConcurrentOfflineSendersPreserveLegacyMessagesAndNeverReuseDrainedIds()
        {
            var user = NewUser("login", "display");
            user.UserMessages = new List<string> { Code(90), Code(120) };
            var other = NewUser("display", "other-display"); // old save wrongly addressed this account
            await _users.InsertManyAsync(new[] { user, other });
            var sends = new List<Task<bool>>();
            for (var i = 0; i < 16; i++) sends.Add(_database.SaveUserMessage(user.PlayerName, Message("new-" + i)));
            var remove = _database.RemoveUserMessage(user.UserName, 90);
            Assert.All(await Task.WhenAll(sends), Assert.True);
            Assert.True(await remove);
            var remaining = _database.QueryUserMessages(user.UserName);
            Assert.Equal(17, remaining.Count);
            var ids = new HashSet<int>();
            foreach (var message in remaining)
                Assert.True(ids.Add(UserMessage.ReCreateMessage(message).MessageId));
            Assert.Contains(120, ids);
            Assert.Empty(_database.QueryUserMessages(other.UserName));
            foreach (var id in ids) Assert.True(await _database.RemoveUserMessage(user.UserName, id));
            Assert.Empty(_database.QueryUserMessages(user.UserName));
            var next = Message("after-empty");
            Assert.True(await _database.SaveUserMessage(user.PlayerName, next));
            Assert.True(next.MessageId > 136);
            foreach (var id in ids) Assert.True(await _database.RemoveUserMessage(user.UserName, id));
            Assert.Single(_database.QueryUserMessages(user.UserName));
            Assert.True(await _database.RemoveUserMessage(user.UserName, next.MessageId));
        }

        [SeasonMongoFact]
        public async Task AcknowledgingLastLegacyMessageSeedsCounterBeforeFirstUpgradedSend()
        {
            var user = NewUser("login", "display");
            user.UserMessages = new List<string> { Code(1), Code(250) };
            await _users.InsertOneAsync(user);
            Assert.True(await _database.RemoveUserMessage(user.UserName, 1));
            Assert.True(await _database.RemoveUserMessage(user.UserName, 250));
            Assert.Empty(_database.QueryUserMessages(user.UserName));
            var next = Message("first-upgraded-season");
            Assert.True(await _database.SaveUserMessage(user.PlayerName, next));
            Assert.True(next.MessageId > 250);
            Assert.True(await _database.RemoveUserMessage(user.UserName, 1));
            Assert.True(await _database.RemoveUserMessage(user.UserName, 250));
            Assert.Single(_database.QueryUserMessages(user.UserName));
        }

        [SeasonMongoFact]
        public async Task NullAndMissingLegacyQueuesSupportSaveAndIdempotentAck()
        {
            var user = NewUser("null-login", "null-display");
            user.UserMessages = null;
            var missing = NewUser("missing-login", "missing-display");
            await _users.InsertManyAsync(new[] { user, missing });
            await _users.UpdateOneAsync(x => x.Id == missing.Id, Builders<UserInfo>.Update.Unset(x => x.UserMessages));
            foreach (var account in new[] { user, missing })
            {
                Assert.Empty(_database.QueryUserMessages(account.UserName));
                Assert.True(await _database.RemoveUserMessage(account.UserName, 1));
                Assert.True(await _database.SaveUserMessage(account.PlayerName, Message("legacy")));
                Assert.Single(_database.QueryUserMessages(account.UserName));
            }
            Assert.False(await _database.RemoveUserMessage(_prefix + "absent", 1));
        }

        [SeasonMongoFact]
        public async Task HubResolvesConnectionOwnerAndRejectsStaleAckOwner()
        {
            var user = NewUser("owner", "display");
            user.UserMessages = new List<string> { Code(4) };
            var other = NewUser("victim", "victim-display");
            other.UserMessages = new List<string> { Code(4, "other") };
            await _users.InsertManyAsync(new[] { user, other });
            var server = (GwentServerService)RuntimeHelpers.GetUninitializedObject(typeof(GwentServerService));
            server._databaseService = _database;
            typeof(GwentServerService).GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(server,
                new ConcurrentDictionary<string, User>(new[] {
                    new KeyValuePair<string, User>("owner-connection", new User(user.UserName, "owner-connection")) }));
            var hub = new GwentHub(server) { Context = new MessageHubContext("owner-connection") };
            Assert.Equal(user.UserMessages, hub.GetUserMessages(other.PlayerName));
            Assert.False(await hub.RemoveUserMessage(other.UserName, 4));
            Assert.Single(_database.QueryUserMessages(user.UserName));
            Assert.True(await hub.RemoveUserMessage(user.UserName, 4));
            Assert.Empty(_database.QueryUserMessages(user.UserName));
            Assert.Single(_database.QueryUserMessages(other.UserName));
            hub.Context = new MessageHubContext("anonymous-connection");
            Assert.Empty(hub.GetUserMessages(user.UserName));
            Assert.False(await hub.RemoveUserMessage(other.UserName, 4));
        }

        private UserInfo NewUser(string login, string display) => new UserInfo
        {
            Id = _prefix + "-" + login, UserName = _prefix + "-" + login,
            PlayerName = _prefix + "-" + display
        };
        private static string Code(int id, string season = "old-unranked-season") =>
            $"UserSeasonEndMessage|{id}||||0|0|{season}";
        private static UserSeasonEndMessage Message(string season) => new UserSeasonEndMessage(
            "DisplaySeasonEndMessage", new string[0], new string[0], new string[0], 0, 0, season);
        public void Dispose()
        {
            _users.DeleteMany(Builders<UserInfo>.Filter.Regex(x => x.Id, new BsonRegularExpression("^" + _prefix)));
            _mongo.GetCollection<BsonDocument>("usermessagecounters").DeleteMany(new BsonDocument("_id", new BsonDocument("$regex", "^" + _prefix)));
            _provider.Dispose();
        }
    }

    internal sealed class MessageHubContext : HubCallerContext
    {
        public MessageHubContext(string id) => ConnectionId = id;
        public override string ConnectionId { get; }
        public override string UserIdentifier => null;
        public override ClaimsPrincipal User => new ClaimsPrincipal();
        public override IDictionary<object, object> Items { get; } = new Dictionary<object, object>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;
        public override void Abort() { }
    }
}
