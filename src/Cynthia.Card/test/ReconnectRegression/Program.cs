using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cynthia.Card;
using Cynthia.Card.Server;
using Microsoft.AspNetCore.SignalR;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static FieldInfo Field(object target, string name) => target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
static Task Run(GwentServerGame game, Func<Task> action) => (Task)typeof(GwentServerGame).GetMethod("RunGameAction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(game, new object[] { action });
static Task Input(GwentServerGame game, ClientPlayer player, User user) => (Task)typeof(GwentServerGame).GetMethod("AcceptInput", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(game, new object[] { player, user, Operation.Create(UserOperationType.SelectRowInfo, RowPosition.MyRow1) });

var wire = new Wire();
var clients = DispatchProxy.Create<IHubClients, ClientsProxy>();
((ClientsProxy)(object)clients).Wire = wire;
var hub = new Hub(clients);
var leader = GwentMap.CardMap.First(x => x.Value.Group == Group.Leader).Key;
var old = new User("account", "old", UserState.Play) { PlayerName = "old" };
var player = new ClientPlayer(old, () => hub) { Deck = new DeckModel { Leader = leader } };
var enemyUser = new User("enemy", "enemy", UserState.Play) { PlayerName = "enemy" };
var enemy = new ClientPlayer(enemyUser, () => hub) { Deck = new DeckModel { Leader = leader } };
old.CurrentPlayer = player;
enemyUser.CurrentPlayer = enemy;
var game = new GwentServerGame(player, enemy);
var room = new GwentRoom(player, "") { Player2 = enemy, CurrentGame = game };
var matchs = new GwentMatchs(() => hub, null, null);
matchs.GwentRooms.Add(room);
var service = (GwentServerService)RuntimeHelpers.GetUninitializedObject(typeof(GwentServerService));
var users = new ConcurrentDictionary<string, User>();
var tickets = new Dictionary<string, (User User, CancellationTokenSource Cancel)>();
Field(service, "_users").SetValue(service, users);
Field(service, "_waitReconnectList").SetValue(service, tickets);
Field(service, "_gwentMatchs").SetValue(service, matchs);

// Force the contender to wait outside the session gate, then disconnect the
// Standby connection. The original check-before-lock implementation consumes
// the old ticket and transfers ownership to this removed connection.
var fresh = new User("account", "fresh");
users[fresh.ConnectionId] = fresh;
var ticket = new CancellationTokenSource();
tickets[old.UserName] = (old, ticket);
Task<bool> claim;
lock (tickets)
{
    claim = Task.Run(() => service.Reconnect(fresh.ConnectionId));
    Thread.Sleep(80);
    service.Disconnect(fresh.ConnectionId).GetAwaiter().GetResult();
}
Check(!await claim.WaitAsync(TimeSpan.FromSeconds(3)), "Removed connection reclaimed a ticket");
Check(tickets.ContainsKey(old.UserName) && !ticket.IsCancellationRequested && player.CurrentUser == old, "Failed claimant destroyed old ownership/deadline");
Console.WriteLine("PASS: disconnect while reconnect claim waits preserves ticket and owner");

// Wire payloads must detach at creation, including deferred list enumerations.
var status = new CardStatus(leader);
var payload = Operation.Create(ServerOperationType.SetMyDeck, new[] { status }.Select(x => x));
var frozen = payload.Arguments.Single();
status.Strength += 7;
Check(payload.Arguments.Single() == frozen, "Operation payload changed after creation");
Console.WriteLine("PASS: operation freezes mutable/deferred payload");

var pending = (IList<Operation<ServerOperationType>>[])Field(game, "_pendingRequests").GetValue(game);
pending[0].Add(Operation.Create(ServerOperationType.SelectRow, RowPosition.MyRow1));
var captured = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
wire.Block = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
wire.Captured = captured;
var receiving = Run(game, async () => { await game.ReceiveAsync(0); });
var restore = game.ResendGameState(0);
await captured.Task.WaitAsync(TimeSpan.FromSeconds(3));
var answer = Input(game, player, old);
await Task.WhenAll(answer, receiving).WaitAsync(TimeSpan.FromSeconds(3));
Check(pending[0].Count == 0, "Accepted answer still pending");
var batch = wire.Batches.First();
Check(batch.OfType<Operation<ServerOperationType>>().Any(x => x.OperationType == ServerOperationType.SelectRow), "Snapshot lost request present at capture");
wire.Block.TrySetResult(true);
Check(await restore.WaitAsync(TimeSpan.FromSeconds(3)), "Restore failed");
await game.ResendGameState(0).WaitAsync(TimeSpan.FromSeconds(3));
Check(!wire.Batches.Last().OfType<Operation<ServerOperationType>>().Any(x => x.OperationType == ServerOperationType.SelectRow), "Answered request replayed");
Console.WriteLine("PASS: answer during snapshot transport consumes pending request at common scheduler boundary");

pending[0].Add(Operation.Create(ServerOperationType.SelectRow, RowPosition.MyRow1));
var staleUser = new User("account", "stale");
await Input(game, player, staleUser);
Check(pending[0].Count == 1, "Stale owner consumed request");
Field(game, "_finished").SetValue(game, true);
Check(!matchs.IsInGame(player) && !await game.ResendGameState(0), "Terminal game allowed ownership/replay");
try { await game.ReceiveAsync(0); throw new Exception("Ended receive accepted"); }
catch (OperationCanceledException) { }
Console.WriteLine("PASS: stale owner and terminal game cannot accept/resume input");

// An expired wait retained from an earlier disconnect cannot settle the newer ticket.
var replacement = new CancellationTokenSource();
tickets[old.UserName] = (old, replacement);
typeof(GwentServerService).GetMethod("ExpireReconnect", BindingFlags.NonPublic | BindingFlags.Instance)
    .Invoke(service, new object[] { old, ticket });
Check(tickets[old.UserName].Cancel == replacement && !replacement.IsCancellationRequested, "Old timeout consumed replacement ticket");
Console.WriteLine("PASS: late old timeout cannot consume a replacement disconnect ticket");

Field(game, "_finished").SetValue(game, false);
var results = 0;
game.GameResultEvent = _ => results++;
var waitingInput = Run(game, async () => { await game.ReceiveAsync(0); });
await Run(game, () => Task.CompletedTask);
await game.GameEnd(1, null);
try { await waitingInput.WaitAsync(TimeSpan.FromSeconds(3)); throw new Exception("Terminal input remained pending"); }
catch (OperationCanceledException) { }
await game.GameOverExecute();
await game.GameEnd(0, null);
Check(results == 1, "Natural/forced settlement published duplicate results");
Console.WriteLine("PASS: terminal settlement cancels pending input and natural/forced paths publish once");

sealed class Wire : IClientProxy
{
    public readonly List<IList<object>> Batches = new();
    public TaskCompletionSource<bool> Captured;
    public TaskCompletionSource<bool> Block;
    public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
    {
        if (method == "GameOperation")
        {
            Batches.Add(((IEnumerable<object>)args[0]).ToList());
            Captured?.TrySetResult(true);
        }
        return Block?.Task ?? Task.CompletedTask;
    }
}
public class ClientsProxy : DispatchProxy
{
    internal IClientProxy Wire;
    protected override object Invoke(MethodInfo method, object[] args) => Wire;
}
sealed class Hub : IHubContext<GwentHub>
{
    public Hub(IHubClients clients) { Clients = clients; }
    public IHubClients Clients { get; }
    public IGroupManager Groups => null;
}
