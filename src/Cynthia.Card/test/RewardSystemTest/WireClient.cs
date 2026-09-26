using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Actual SignalR JSON/WebSocket transport, independent of the server object under test.
sealed class WireClient : IDisposable
{
    readonly ClientWebSocket socket=new ClientWebSocket();
    readonly CancellationTokenSource stop=new CancellationTokenSource();
    readonly SemaphoreSlim sendLock=new SemaphoreSlim(1);
    readonly ConcurrentDictionary<string,TaskCompletionSource<JToken>> calls=new ConcurrentDictionary<string,TaskCompletionSource<JToken>>();
    int next;
    public readonly ConcurrentQueue<JObject> Events=new ConcurrentQueue<JObject>();
    public static async Task<WireClient> Connect(string address)
    {
        var c=new WireClient();await c.socket.ConnectAsync(new Uri(address),c.stop.Token);
        await c.Send(new{protocol="json",version=1});_ = c.Read();return c;
    }
    public async Task<T> Invoke<T>(string method,params object[] args)
    {
        string id=Interlocked.Increment(ref next).ToString();var completion=new TaskCompletionSource<JToken>(TaskCreationOptions.RunContinuationsAsynchronously);
        calls[id]=completion;
        await Send(new{type=1,invocationId=id,target=method,arguments=args});
        if(await Task.WhenAny(completion.Task,Task.Delay(15000))!=completion.Task)throw new TimeoutException(method);
        var value=await completion.Task;return value==null || value.Type==JTokenType.Null?default(T):value.ToObject<T>();
    }
    async Task Send(object value)
    {
        byte[] data=Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)+(char)30);
        await sendLock.WaitAsync();try{await socket.SendAsync(new ArraySegment<byte>(data),WebSocketMessageType.Text,true,stop.Token);}finally{sendLock.Release();}
    }
    async Task Read()
    {
        var buffer=new byte[131072];var pending=new StringBuilder();
        try
        {
            while(!stop.IsCancellationRequested)
            {
                var result=await socket.ReceiveAsync(new ArraySegment<byte>(buffer),stop.Token);
                if(result.MessageType==WebSocketMessageType.Close)throw new Exception("Server closed WebSocket");
                pending.Append(Encoding.UTF8.GetString(buffer,0,result.Count));
                string text=pending.ToString();int end;
                while((end=text.IndexOf((char)30))>=0)
                {
                    var part=text.Substring(0,end);text=text.Substring(end+1);if(part.Length==0)continue;
                    var message=JObject.Parse(part);string id=(string)message["invocationId"];
                    if((int?)message["type"]==3 && id!=null && calls.TryRemove(id,out var call))
                    {if(message["error"]!=null)call.TrySetException(new InvalidOperationException((string)message["error"]));else call.TrySetResult(message["result"]);}
                    else if(message["target"]!=null)Events.Enqueue(message);
                }
                pending.Clear();pending.Append(text);
            }
        }
        catch(Exception e){foreach(var call in calls.Values)call.TrySetException(e);}
    }
    public void Dispose(){stop.Cancel();socket.Abort();socket.Dispose();}
}
