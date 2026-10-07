using Alsein.Extensions.IO;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
await using var hub = new HubConnectionBuilder().WithUrl("http://127.0.0.1:1/gwent").Build();
var old = new LocalPlayer(hub);
var oldReads = old.ReceiveAsync();
var oldSends = 0;
((Player)old).Receive += _ => { oldSends++; return Task.CompletedTask; };
old.Retire();
old.Retire();
Check((await oldReads.WaitAsync(TimeSpan.FromSeconds(2))).Count == 0, "retired receive must stop");
await old.SendAsync(UserOperationType.SelectRowInfo, RowPosition.MyRow1);
await old.SendAsync(Operation.Create((int)UserOperationType.SelectRowInfo, RowPosition.MyRow1));
Check(oldSends == 0, "old scene callbacks must not send into a new session");
Console.WriteLine("PASS: retirement is idempotent, releases receive wait and rejects both stale send paths");

var fresh = new LocalPlayer(hub);
var freshReads = fresh.ReceiveAsync();
var upstream = (ITubeEndPoint)typeof(Player).GetField("_upstream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(fresh);
await upstream.SendAsync(new List<Operation<int>> { Operation.Create((int)ServerOperationType.SetCoinInfo, true) });
var received = await freshReads.WaitAsync(TimeSpan.FromSeconds(2));
Check(!fresh.IsRetired && received.Count == 1 && received[0].OperationType == ServerOperationType.SetCoinInfo,
    "retiring an old instance must not consume or disable fresh session messages");
fresh.Retire();
Console.WriteLine("PASS: fresh player independently receives the restored session");

foreach (var resource in new[] { "production.GwentClientService.cs", "production.GwentClientGameService.cs" })
{
    using var source = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resource));
    var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source.ReadToEnd(),
        new Microsoft.CodeAnalysis.CSharp.CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_3));
    var errors = tree.GetDiagnostics().Where(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToArray();
    Check(errors.Length == 0, string.Join("\n", errors.Select(x => x.ToString())));
}
Console.WriteLine("PASS: client service and scene loop parse using C# 7.3 (syntax only; no Unity native acceptance)");

// LocalPlayer does not use Unity APIs; this keeps the production source linked.
namespace UnityEngine { }
