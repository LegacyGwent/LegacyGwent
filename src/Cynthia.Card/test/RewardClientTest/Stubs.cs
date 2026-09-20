// Only transport, dependency lookup, clock and rendering environment are substituted.
// DailyQuestClient and PremiumCollectionClient are compiled directly from the Unity sources.
using System;
using System.Threading.Tasks;
using Cynthia.Card;
namespace Assets.Script.Localization
{
    public static class LocalizedLabel
    {
        public static string Get(string key, params object[] values) => key;
    }
}
namespace UnityEngine
{
    public static class Time { public static float realtimeSinceStartup; }
    public static class Debug { public static void LogWarning(object text){} }
}
namespace UnityEngine.SceneManagement
{
    public struct Scene { public bool isLoaded; }
    public static class SceneManager { public static bool Playing; public static Scene GetSceneByName(string name)=>new Scene{isLoaded=Playing}; }
}
namespace Microsoft.AspNetCore.SignalR.Client
{
    public enum HubConnectionState {Disconnected,Connected}
    public sealed class HubConnection
    {
        public HubConnectionState State=HubConnectionState.Connected;
        public int Calls;
        public Func<string,object[],Task<object>> Respond;
        public Task<T> InvokeAsync<T>(string method,params object[] args)=>InvokeCoreAsync<T>(method,args);
        public async Task<T> InvokeCoreAsync<T>(string method,object[] args)
        {Calls++;return (T)await Respond(method,args);}
    }
}
namespace Cynthia.Card.Client
{
    public class GwentClientService
    {public UserInfo User;public Microsoft.AspNetCore.SignalR.Client.HubConnection HubConnection=new Microsoft.AspNetCore.SignalR.Client.HubConnection();}
}
public static class DependencyResolver
{
    public static TestContainer Container=new TestContainer();
    public sealed class TestContainer
    {public Cynthia.Card.Client.GwentClientService Client;public T Resolve<T>()=>(T)(object)Client;}
}
