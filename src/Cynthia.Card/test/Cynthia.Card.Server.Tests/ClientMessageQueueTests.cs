using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Xunit;

// Minimal UI/transport fakes: the production Unity message reader is compiled above.
namespace Microsoft.AspNetCore.SignalR.Client
{
    public class HubConnection
    {
        public Func<IList<string>, IList<string>, IList<string>, int, int, string, Task> SeasonHandler { get; private set; }
        public IDisposable On<T1, T2, T3, T4, T5, T6>(string method, Func<T1, T2, T3, T4, T5, T6, Task> handler)
        {
            SeasonHandler = (Func<IList<string>, IList<string>, IList<string>, int, int, string, Task>)(object)handler;
            return new Subscription();
        }
        private class Subscription : IDisposable { public void Dispose() { } }
    }
}
namespace Assets.Script.Localization
{
    public class LocalizationService
    {
        public string GetText(string key) => key == "Season_EndMessageRewards" ? "{0}|{1}|{2}" : key;
    }
}
namespace UnityEngine
{
    public static class Debug
    {
        public static void LogWarning(object text) { }
        public static void LogException(Exception exception) => throw exception;
    }
}
namespace Cynthia.Card.Client
{
    public enum ClientState { Match, Play, Standby }
    public static class DependencyResolver { public static IContainer Container { get; set; } }
    public class GwentClientService
    {
        public UserInfo User { get; set; } = new UserInfo { UserName = "test-login" };
        public ClientState ClientState { get; set; } = ClientState.Standby;
        public event Action ClientStateChanged;
        public Func<Task<IList<string>>> Read { get; set; }
        public Func<int, string, Task<bool>> Ack { get; set; }
        public int StateListeners => ClientStateChanged?.GetInvocationList().Length ?? 0;
        public Task<IList<string>> CheckUserMessages(string name) => Read();
        public Task<bool> RemoveUserMessage(int id, string owner) => Ack(id, owner);
        public void ChangeState(ClientState state) { ClientState = state; ClientStateChanged?.Invoke(); }
    }
    public class GlobalUIService
    {
        public Func<string, Task> Show { get; set; }
        public Task YNMessageBoxEnhanced(string title, string message, string yes, string no, bool isOnlyYes,
            string message2, string message3, IList<string> avatars, IList<string> borders, IList<string> titles) => Show(message);
    }
}
namespace Cynthia.Card.Server.Tests
{
    public class ClientMessageQueueTests
    {
        [Fact]
        public async Task BacklogAwaitsAckBeforeReadingAndAcceptsLegacyFalseResponse()
        {
            var queue = new List<string> { Code(1), Code(2), Code(3) };
            var ackStarted = new TaskCompletionSource<bool>();
            var releaseAck = new TaskCompletionSource<bool>();
            var finished = new TaskCompletionSource<bool>();
            var reads = 0;
            var shown = new List<string>();
            var client = new GwentClientService
            {
                Read = () => { reads++; if (queue.Count == 0) finished.TrySetResult(true); return Task.FromResult<IList<string>>(new List<string>(queue)); },
                Ack = async (id, owner) =>
                {
                    if (id == 1) { ackStarted.SetResult(true); await releaseAck.Task; }
                    queue.Remove(Code(id));
                    return false; // Legacy server returns false even after successful removal.
                }
            };
            using var container = Container(client);
            var reader = new ClientMessagesReaderService(container, new GlobalUIService
            {
                Show = text => { shown.Add(text); return Task.CompletedTask; }
            });
            await ackStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(1, reads);
            Assert.Single(shown);
            await reader.CheckMessages(); // A second entry while the ack is pending must not spawn another modal.
            Assert.Single(shown);
            releaseAck.SetResult(true);
            await finished.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(3, shown.Count);
            Assert.Empty(queue);
            Assert.Equal(4, reads);
        }

        [Fact]
        public async Task FailedAckStopsWithoutRedisplayingAndLaterRetryCanDrain()
        {
            var queue = new List<string> { Code(1) };
            var shown = 0;
            var client = new GwentClientService
            {
                Read = () => Task.FromResult<IList<string>>(new List<string>(queue)),
                Ack = (id, owner) => Task.FromResult(false)
            };
            using var container = Container(client);
            var reader = new ClientMessagesReaderService(container, new GlobalUIService
            {
                Show = text => { shown++; return Task.CompletedTask; }
            });
            Assert.Equal(1, shown);
            Assert.Single(queue);
            client.Ack = (id, owner) => { queue.Clear(); return Task.FromResult(true); };
            await reader.CheckMessages();
            Assert.Equal(2, shown);
            Assert.Empty(queue);
        }

        [Fact]
        public async Task StandbySubscriptionDetachesBeforeShowingAndRepeatedStateChangesDoNotDuplicate()
        {
            var queue = new List<string> { Code(1) };
            var popup = new TaskCompletionSource<bool>();
            var finished = new TaskCompletionSource<bool>();
            var shown = 0;
            var client = new GwentClientService
            {
                ClientState = ClientState.Play,
                Read = () => { if (queue.Count == 0) finished.TrySetResult(true); return Task.FromResult<IList<string>>(new List<string>(queue)); },
                Ack = (id, owner) => { queue.Clear(); return Task.FromResult(true); }
            };
            using var container = Container(client);
            var reader = new ClientMessagesReaderService(container, new GlobalUIService
            {
                Show = text => { shown++; Assert.Equal(0, client.StateListeners); return popup.Task; }
            });
            Assert.Equal(1, client.StateListeners);
            Assert.Equal(0, shown);
            client.ChangeState(ClientState.Standby);
            client.ChangeState(ClientState.Standby);
            await reader.CheckMessages();
            Assert.Equal(1, shown);
            Assert.Equal(0, client.StateListeners);
            popup.SetResult(true);
            await finished.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Empty(queue);
        }

        [Fact]
        public async Task LiveSignalRHandlerCompletesBeforeStandbyOrUserInteraction()
        {
            var popup = new TaskCompletionSource<bool>();
            var shown = 0;
            var client = new GwentClientService
            {
                ClientState = ClientState.Play,
                Read = () => Task.FromResult<IList<string>>(new string[0]),
                Ack = (id, owner) => throw new InvalidOperationException("Live notification has no persisted acknowledgement.")
            };
            using var container = Container(client);
            var reader = new ClientMessagesReaderService(container, new GlobalUIService
            {
                Show = text => { shown++; return popup.Task; }
            });
            var hub = container.ResolveNamed<Microsoft.AspNetCore.SignalR.Client.HubConnection>("game");
            var dispatched = hub.SeasonHandler(new string[0], new string[0], new string[0], 0, 0, "live");
            Assert.True(dispatched.IsCompletedSuccessfully);
            Assert.Equal(0, shown);
            client.ChangeState(ClientState.Standby);
            Assert.Equal(1, shown);
            Assert.True(dispatched.IsCompletedSuccessfully);
            popup.SetResult(true);
        }

        [Fact]
        public async Task ChangedAccountCannotAcknowledgeOldPopupAndItsQueuedCheckRunsAfterwards()
        {
            var oldPopup = new TaskCompletionSource<bool>();
            var done = new TaskCompletionSource<bool>();
            var shown = 0;
            var ackedNames = new List<string>();
            var queues = new Dictionary<string, IList<string>>
            {
                ["old-login"] = new List<string> { Code(1) },
                ["new-login"] = new List<string> { Code(1) }
            };
            var client = new GwentClientService { User = new UserInfo { UserName = "old-login" } };
            client.Read = () =>
            {
                var queue = queues[client.User.UserName];
                if (client.User.UserName == "new-login" && queue.Count == 0) done.TrySetResult(true);
                return Task.FromResult<IList<string>>(new List<string>(queue));
            };
            client.Ack = (id, owner) =>
            {
                ackedNames.Add(owner);
                queues[owner].Clear();
                return Task.FromResult(true);
            };
            using var container = Container(client);
            var reader = new ClientMessagesReaderService(container, new GlobalUIService
            {
                Show = text => { shown++; return shown == 1 ? oldPopup.Task : Task.CompletedTask; }
            });
            Assert.Equal(1, shown);
            client.User = new UserInfo { UserName = "new-login" };
            await reader.CheckMessages();
            oldPopup.SetResult(true);
            await done.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(2, shown);
            Assert.Equal(new[] { "new-login" }, ackedNames);
            Assert.Single(queues["old-login"]);
            Assert.Empty(queues["new-login"]);
        }

        private static IContainer Container(GwentClientService client)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(client);
            builder.RegisterInstance(new Assets.Script.Localization.LocalizationService());
            builder.RegisterInstance(new Microsoft.AspNetCore.SignalR.Client.HubConnection()).Named<Microsoft.AspNetCore.SignalR.Client.HubConnection>("game");
            var container = builder.Build();
            DependencyResolver.Container = container;
            return container;
        }
        private static string Code(int id) => $"UserSeasonEndMessage|{id}||||0|0|unranked-{id}";
    }
}
