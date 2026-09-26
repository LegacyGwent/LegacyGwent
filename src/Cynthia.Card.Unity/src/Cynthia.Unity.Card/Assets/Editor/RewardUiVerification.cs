using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.Script.DynamicCards;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class RewardUiVerification
{
    const string Root="C:/UnityProjects/LegacyGwent/work/RewardSystem/";
    static bool running;
    static RewardUiVerification()
    {
        EditorApplication.update+=Poll;
        if(Directory.Exists(Root))File.WriteAllText(Root+"ui-editor-ready.txt",DateTime.UtcNow.ToString("O"));
    }
    static async void Poll()
    {
        if(running || EditorApplication.isCompiling || !EditorApplication.isPlaying || !File.Exists(Root+"ui.request"))return;
        string request;try{request=File.ReadAllText(Root+"ui.request");File.Delete(Root+"ui.request");}catch(IOException){return;}
        running=true;GwentClientService client=null;HubConnection oldHub=null,testHub=null;UserInfo oldUser=null;string work=null;
        bool previousRunInBackground=Application.runInBackground;
        try
        {
            var input=JObject.Parse(request);work=Path.GetFullPath((string)input["work"]);
            if(!work.StartsWith(Path.GetFullPath(Root),StringComparison.OrdinalIgnoreCase))throw new Exception("Invalid test evidence directory");
            var endpoint=new Uri((string)input["endpoint"]);
            if(endpoint.Port!=5016 || !endpoint.Host.StartsWith("192.168.") || !((string)input["username"]).StartsWith("reward-ui-"))throw new Exception("UI verification is restricted to the isolated LAN reward fixture");
            Application.runInBackground=true;
            await Task.Delay(12000);
            client=DependencyResolver.Container.Resolve<GwentClientService>();oldHub=client.HubConnection;oldUser=client.User;
            testHub=new HubConnectionBuilder().WithUrl(endpoint,HttpTransportType.WebSockets,o=>o.SkipNegotiation=true)
                .AddJsonProtocol(o=>o.PayloadSerializerOptions.Converters.Add(new BoolConverter()))
                .AddJsonProtocol(o=>o.PayloadSerializerOptions.Converters.Add(new ListOperationConverter())).Build();
            await testHub.StartAsync();client.HubConnection=testHub;client.User=null;
            await client.Login((string)input["username"],"RewardTest2026");
            SceneManager.LoadScene("Game");
            EditorInfo editor=null;
            await Until(()=>{
                editor=Resources.FindObjectsOfTypeAll<EditorInfo>().FirstOrDefault(x=>
                    x.gameObject.scene==SceneManager.GetActiveScene() && x.gameObject.scene.isLoaded &&
                    x.MainUI!=null && x.EditorUI!=null);
                return editor!=null;
            },20);
            bool initializedOnLogin=editor.MainUI.GetComponent<DailyQuestPanel>()!=null;
            bool editorActive=editor.gameObject.activeInHierarchy;
            if(!initializedOnLogin)
            {
                // Exercise the existing deck-builder activation path so the remaining UI
                // checks can run; the missing login HUD is reported as a separate failure.
                editor.EditorUI.SetActive(true);
                await Until(()=>editor.MainUI.GetComponent<DailyQuestPanel>()!=null,5);
            }
            // Opening the deck builder later must not add a second task component or HUD.
            editor.EditorUI.SetActive(true);await Task.Delay(300);
            bool singlePanel=editor.MainUI.GetComponents<DailyQuestPanel>().Length==1 &&
                editor.MainUI.GetComponentsInChildren<Transform>(true).Count(x=>x.name=="DailyTasksButton")==1;
            File.WriteAllText(Path.Combine(work,"ui-login-initialization.json"),
                Newtonsoft.Json.JsonConvert.SerializeObject(new{initializedOnLogin,editorActive,singlePanel}));
            editor.MainUI.SetActive(true);editor.EditorUI.SetActive(false);
            var panel=editor.MainUI.GetComponent<DailyQuestPanel>();panel.Open();await Task.Delay(500);
            await Snapshot(work,"initial",panel);
            for(int i=0;i<4;i++)
            {
                var path=Path.Combine(work,"ui-phase-"+i+".json");await Until(()=>File.Exists(path),90);
                var phase=JObject.Parse(File.ReadAllText(path));string label=(string)phase["label"];
                long balance=(long)phase["balance"];
                if(label=="automatic-midnight")
                    await Until(()=>PremiumCollectionClient.Account?.DailyQuests?.Day==(string)phase["day"],80);
                else if(label=="switch")
                    await client.Login((string)phase["username"],"RewardTest2026");
                else {panel.Close();panel.Open();await DailyQuestClient.Refresh(true);}
                await Until(()=>PremiumCollectionClient.Account?.MeteoritePowder==balance,12);
                await Task.Delay(1200);await Snapshot(work,label,panel);
            }
            File.WriteAllText(Path.Combine(work,"ui-complete.json"),"{\"passed\":true}");
        }
        catch(Exception e){if(work!=null)File.WriteAllText(Path.Combine(work,"ui-error.txt"),e.ToString());Debug.LogException(e);}
        finally
        {
            Application.runInBackground=previousRunInBackground;
            if(client!=null && oldHub!=null){client.HubConnection=oldHub;client.User=oldUser;PremiumCollectionClient.Reset();}
            if(testHub!=null)await testHub.DisposeAsync();
            running=false;
        }
    }
    static async Task Until(Func<bool> predicate,int seconds)
    {
        var deadline=DateTime.UtcNow.AddSeconds(seconds);
        while(!predicate()){if(DateTime.UtcNow>deadline)throw new TimeoutException("Reward UI condition timeout");await Task.Delay(200);}
    }
    static async Task Snapshot(string work,string label,DailyQuestPanel panel)
    {
        if(!panel.isActiveAndEnabled || !panel.IsOpen || DailyQuestClient.State?.Success!=true)throw new Exception("Daily task UI inactive or unsynchronized");
        double before=DailyQuestClient.RemainingSeconds;await Task.Delay(1100);
        bool ticking=before<=0 || DailyQuestClient.RemainingSeconds<before;
        var text=Resources.FindObjectsOfTypeAll<Text>().Where(x=>x.gameObject.activeInHierarchy && x.transform.root.name=="DailyQuestPage").Select(x=>x.text).ToArray();
        ScreenCapture.CaptureScreenshot(Path.Combine(work,"ui-"+label+".png"));
        File.WriteAllText(Path.Combine(work,"ui-"+label+".json"),Newtonsoft.Json.JsonConvert.SerializeObject(new{passed=ticking,account=PremiumCollectionClient.Account,state=DailyQuestClient.State,remaining=DailyQuestClient.RemainingSeconds,text},Newtonsoft.Json.Formatting.Indented));
    }
}
