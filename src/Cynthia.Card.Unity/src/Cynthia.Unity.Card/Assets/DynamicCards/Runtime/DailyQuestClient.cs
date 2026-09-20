using System;
using Assets.Script.Localization;
using System.Globalization;
using System.Threading.Tasks;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    public static class DailyQuestClient
    {
        public static event Action Changed;
        public static DailyQuestResult State { get; private set; }
        private static string errorKey;
        public static string Error => errorKey == null ? null : LocalizedLabel.Get(errorKey);
        private static int session;
        private static Task pending;
        private static bool refreshQueued;
        private static float sampledAt, lastAttempt = -100;
        private static double secondsToReset;
        public static double RemainingSeconds => Math.Max(0,secondsToReset-(Time.realtimeSinceStartup-sampledAt));
        public static void Reset() { session++; State=null; errorKey=null; pending=null; refreshQueued=false; lastAttempt=-100; Changed?.Invoke(); }
        public static Task Refresh(bool force=false)
        {
            if (pending != null)
            {
                // A reward may have committed after the in-flight snapshot was read.
                if (force) refreshQueued=true;
                return pending;
            }
            if (!force && Time.realtimeSinceStartup-lastAttempt<5) return Task.CompletedTask;
            int version=session;
            var completion=new TaskCompletionSource<bool>();
            // Publish the task before Fetch can complete synchronously or raise Changed.
            pending=completion.Task;
            _=FetchQueued(version,completion);
            return completion.Task;
        }
        private static async Task FetchQueued(int version,TaskCompletionSource<bool> completion)
        {
            try
            {
                do
                {
                    refreshQueued=false;
                    await Fetch(version);
                } while(version==session && refreshQueued);
            }
            finally
            {
                if(version==session)pending=null;
                completion.TrySetResult(true);
            }
        }
        private static async Task Fetch(int version)
        {
            try
            {
                var client=DependencyResolver.Container.Resolve<GwentClientService>();
                string user=client.User?.Id;
                if (user==null || client.HubConnection.State!=HubConnectionState.Connected) return;
                lastAttempt=Time.realtimeSinceStartup;
                var result=await client.HubConnection.InvokeAsync<DailyQuestResult>("GetDailyQuests");
                if (version!=session || client.User?.Id!=user) return;
                if (!result.Success) { errorKey="DailyQuest_SyncError"; Changed?.Invoke(); return; }
                var server=DateTimeOffset.Parse(result.ServerUtc,CultureInfo.InvariantCulture);
                var reset=DateTimeOffset.Parse(result.ResetUtc,CultureInfo.InvariantCulture);
                sampledAt=Time.realtimeSinceStartup; secondsToReset=(reset-server).TotalSeconds;
                State=result; errorKey=null;
                PremiumCollectionClient.Accept(result.Wallet,user);
                Changed?.Invoke();
            }
            catch (Exception e) { if(version==session) { errorKey="DailyQuest_SyncError"; Changed?.Invoke(); Debug.LogWarning("Daily quests: "+e.Message); } }
        }
    }
}
