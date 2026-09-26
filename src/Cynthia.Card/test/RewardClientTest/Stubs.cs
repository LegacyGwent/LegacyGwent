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
    public class TextAsset { public string text; }
    public static class Resources { public static TextAsset Content; public static T Load<T>(string path) where T:class => Content as T; }
    public static class JsonUtility { public static T FromJson<T>(string value) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value); }
    public static class Application { public static bool isMobilePlatform; }
    public static class SystemInfo { public static int graphicsShaderLevel = 35; }
    public static class PlayerPrefs
    {
        public static readonly System.Collections.Generic.Dictionary<string,int> Values = new System.Collections.Generic.Dictionary<string,int>();
        public static int GetInt(string key,int fallback) => Values.TryGetValue(key,out var value)?value:fallback;
        public static void SetInt(string key,int value) => Values[key]=value;
        public static bool HasKey(string key) => Values.ContainsKey(key);
        public static void Save() { }
    }
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
