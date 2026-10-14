using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Cynthia.Card;
using Cynthia.Card.Client;
using Cynthia.Card.Server;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static FieldInfo Field(object target, string name) => target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
static Task Run(GwentServerGame game, Func<Task> action) => (Task)typeof(GwentServerGame)
    .GetMethod("RunGameAction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(game, new object[] { action });
static async Task Until(Func<bool> condition)
{
    var deadline = DateTime.UtcNow.AddSeconds(5);
    while (!condition())
    {
        if (DateTime.UtcNow >= deadline) throw new TimeoutException("Production disconnect did not create a reconnect ticket");
        await Task.Delay(10);
    }
}
static HubConnection ConnectTo(string url) => new HubConnectionBuilder()
    .WithUrl(url, options => options.Transports = HttpTransportType.WebSockets).Build();
static (Channel<IList<Operation<ServerOperationType>>> Batches, Task Pump) Consume(LocalPlayer player)
{
    var batches = Channel.CreateUnbounded<IList<Operation<ServerOperationType>>>();
    return (batches, Pump());
    async Task Pump()
    {
        try
        {
            while (!player.IsRetired)
            {
                var batch = await player.ReceiveAsync();
                if (batch.Count > 0) await batches.Writer.WriteAsync(batch);
            }
        }
        finally { batches.Writer.TryComplete(); }
    }
}
static Task<IList<Operation<ServerOperationType>>> Read(Channel<IList<Operation<ServerOperationType>>> batches)
    => batches.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));

// Only test assembly seeds authenticated sessions. No Login, MongoDB, hosted reward
// workers or production servers are started; the exercised Hub and lifecycle are real.
var service = (GwentServerService)RuntimeHelpers.GetUninitializedObject(typeof(GwentServerService));
var users = new ConcurrentDictionary<string, User>();
var tickets = new Dictionary<string, (User User, CancellationTokenSource Cancel)>();
Field(service, "_users").SetValue(service, users);
Field(service, "_waitReconnectList").SetValue(service, tickets);
var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddSignalR().AddJsonProtocol(options =>
    options.PayloadSerializerOptions.Converters.Add(new DailyQuestProgressJsonConverter()));
var frameworkProtocol = builder.Services.Single(x => x.ServiceType == typeof(IHubProtocol)
    && x.ImplementationType == typeof(JsonHubProtocol));
builder.Services.Remove(frameworkProtocol);
// Register the actual internal production protocol by type, without copying its implementation.
builder.Services.AddSingleton(typeof(IHubProtocol), typeof(GwentHub).Assembly.GetType("Cynthia.Card.Server.AsciiSafeJsonHubProtocol", true));
builder.Services.AddSingleton(service);
await using var app = builder.Build();
app.MapHub<GwentHub>("/hub/gwent");
var hub = app.Services.GetRequiredService<IHubContext<GwentHub>>();
var matches = new GwentMatchs(() => hub, new GwentCardDataService(), service);
Field(service, "_hub").SetValue(service, hub);
Field(service, "_gwentMatchs").SetValue(service, matches);
await app.StartAsync();
var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.Single();
var url = address + "/hub/gwent";
await using var oldConnection = ConnectTo(url);
await using var enemyConnection = ConnectTo(url);
await using var freshConnection = ConnectTo(url);
var oldClient = new LocalPlayer(oldConnection);
var enemyClient = new LocalPlayer(enemyConnection);
var freshClient = new LocalPlayer(freshConnection);
var oldConsume = Consume(oldClient);
var enemyConsume = Consume(enemyClient);
var freshConsume = Consume(freshClient);
try
{
    await oldConnection.StartAsync();
    await enemyConnection.StartAsync();
    Check(oldConnection.ConnectionId != null && enemyConnection.ConnectionId != null, "Real SignalR connections were not established");
    var oldUser = new User("loopback-account", oldConnection.ConnectionId, UserState.Play) { PlayerName = "回环玩家" };
    var enemyUser = new User("loopback-enemy", enemyConnection.ConnectionId, UserState.Play) { PlayerName = "对手" };
    var player = new ClientPlayer(oldUser, () => hub) { Deck = GwentDeck.CreateBasicDeck(0) };
    var enemy = new ClientPlayer(enemyUser, () => hub) { Deck = GwentDeck.CreateBasicDeck(0) };
    oldUser.CurrentPlayer = player;
    enemyUser.CurrentPlayer = enemy;
    users[oldUser.ConnectionId] = oldUser;
    users[enemyUser.ConnectionId] = enemyUser;
    var game = new GwentServerGame(player, enemy);
    var room = new GwentRoom(player, "") { Player2 = enemy, CurrentGame = game };
    matches.GwentRooms.Add(room);
    var card = game.PlayersDeck[0][0];
    var allowed = new List<RowPosition> { RowPosition.MyRow1, RowPosition.MyRow2 };
    RowPosition answered = RowPosition.Banish;
    var selection = Run(game, async () => { answered = await game.GetSelectRow(0, card, allowed); });
    var initial = await Read(oldConsume.Batches);
    Check(initial.Any(x => x.OperationType == ServerOperationType.SelectRow), "Production request did not cross real wire to old LocalPlayer");
    Console.WriteLine("PASS: real WebSocket + production protocol carries production selection request into LocalPlayer");

    // Graceful transport closure is a real socket/Hub disconnect, not OS packet loss.
    oldClient.Retire();
    await oldConnection.StopAsync();
    await Until(() => { lock (tickets) return tickets.ContainsKey(oldUser.UserName); });
    Check(!users.ContainsKey(oldUser.ConnectionId) && !selection.IsCompleted, "Disconnect lost the pending game instead of holding it");
    await oldConsume.Pump.WaitAsync(TimeSpan.FromSeconds(5));
    await freshConnection.StartAsync();
    var freshUser = new User(oldUser.UserName, freshConnection.ConnectionId) { PlayerName = oldUser.PlayerName };
    users[freshUser.ConnectionId] = freshUser;
    Check(await freshConnection.InvokeAsync<bool>("Reconnect").WaitAsync(TimeSpan.FromSeconds(5)), "Production Reconnect hub refused the new session");
    var snapshot = await Read(freshConsume.Batches);
    foreach (var required in new[] { ServerOperationType.SetAllInfo, ServerOperationType.SetMyDeck,
        ServerOperationType.SetMyCemetery, ServerOperationType.SetEnemyCemetery, ServerOperationType.SetCoinInfo, ServerOperationType.SelectRow })
        Check(snapshot.Any(x => x.OperationType == required), "Restored wire snapshot missing " + required);
    Check(snapshot.Single(x => x.OperationType == ServerOperationType.SetMyDeck).Arguments.Single()
        .ToType<IList<CardStatus>>().Count == player.Deck.Deck.Count, "Wire snapshot deck did not deserialize as production card statuses");
    Check(snapshot.Single(x => x.OperationType == ServerOperationType.SetAllInfo).Arguments.Single().Contains("回环玩家"),
        "Production Unicode protocol lost player identity");
    var restoredRequest = snapshot.Single(x => x.OperationType == ServerOperationType.SelectRow);
    Check(restoredRequest.Arguments.ElementAt(1).ToType<IList<RowPosition>>().SequenceEqual(allowed), "Restored request arguments failed typed deserialization");
    lock (tickets) Check(!tickets.ContainsKey(oldUser.UserName), "Ownership claim left reconnect ticket active");
    Check(player.CurrentUser == freshUser && freshUser.CurrentPlayer == player && freshUser.UserState == UserState.Play,
        "Production reconnect did not transfer session ownership");
    Console.WriteLine("PASS: actual Hub disconnect/new connection/Reconnect delivers typed snapshot and pending request to fresh LocalPlayer");

    await oldClient.SendAsync(UserOperationType.SelectRowInfo, RowPosition.MyRow2);
    await Task.Delay(50);
    Check(!selection.IsCompleted, "Retired client answered through the new session");
    await freshClient.SendAsync(UserOperationType.SelectRowInfo, RowPosition.MyRow1);
    await selection.WaitAsync(TimeSpan.FromSeconds(5));
    Check(answered == RowPosition.MyRow1, "Production LocalPlayer answer did not finish the pending server selection");
    Check(!((IList<Operation<ServerOperationType>>[])Field(game, "_pendingRequests").GetValue(game))[0].Any(),
        "Answered request remained pending after actual client response");
    Console.WriteLine("PASS: fresh LocalPlayer sends actual Hub GameOperation completing original production selection; retired instance isolated");

    // This branch has no spectator port. On the integrated candidate the same test
    // also verifies appended operation 44 over the actual spectator wire consumer.
    if (Enum.TryParse<ServerOperationType>("SetEnemyDeck", out var enemyDeck))
    {
        Check((int)enemyDeck == 44, "SetEnemyDeck protocol value shifted");
        var deckPayload = game.PlayersDeck[1].Select(x => x.Status).ToList();
        await hub.Clients.Client(freshUser.ConnectionId).SendAsync("ViewerGameOperation",
            new[] { Operation.Create(enemyDeck, deckPayload) });
        var spectatorBatch = await Read(freshConsume.Batches);
        var operation = spectatorBatch.Single();
        Check(operation.OperationType == enemyDeck && operation.Arguments.Single().ToType<IList<CardStatus>>().Count == deckPayload.Count,
            "SetEnemyDeck 44 failed production LocalPlayer typed wire consumption");
        Console.WriteLine("PASS: SetEnemyDeck 44 crosses production spectator wire consumer with typed card payload");
    }
    else Console.WriteLine("SKIP: SetEnemyDeck is absent on this reconnect-only base; rerun after spectator integration");
    matches.GwentRooms.Clear();
    users.Clear();
    Console.WriteLine("BOUNDARY: no Login/Mongo/reward persistence/Unity scene; StopAsync is graceful transport close, not OS network outage or deployed acceptance.");
}
finally
{
    oldClient.Retire(); enemyClient.Retire(); freshClient.Retire();
    users.Clear(); matches.GwentRooms.Clear();
    await Task.WhenAll(oldConsume.Pump, enemyConsume.Pump, freshConsume.Pump).WaitAsync(TimeSpan.FromSeconds(5));
    await freshConnection.StopAsync(); await enemyConnection.StopAsync();
    await app.StopAsync();
}
namespace UnityEngine { }
