using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class GwentClientService
    {
        public HubConnection HubConnection { get; set; }
        public LocalPlayer Player { get; set; }
        public UserInfo User { get; set; }
        public bool IsAutoPlay { get; set; } = false;
        private ITubeInlet sender;/*待修改*/
        private ITubeOutlet receiver;/*待修改*/
        /*待修改*/
        public Task<bool> MatchResult()
        {
            return receiver.ReceiveAsync<bool>();
        }
        //public bool ResetPlayer()
        //{
        //    if (HubConnection != null && User != null)
        //    {
        //        Player = new LocalPlayer(HubConnection);
        //        return true;
        //    }
        //    return false;
        //}
        public GwentClientService(HubConnection hubConnection)
        {
            /*待修改*/
            (sender, receiver) = Tube.CreateSimplex();
            /*待修改*/
            hubConnection.On<bool>("MatchResult",async x=> 
            {
                await sender.SendAsync<bool>(x);
            });
            hubConnection.On("RepeatLogin", async () =>
            {
                SceneManager.LoadScene("LoginSecen");
                await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("账号被其他人强制登陆", "账号被登陆,被挤下了线");
            });
            hubConnection.Closed += (async x =>
            {
                Debug.Log("断线!");
                await Task.CompletedTask;
                //var user = DependencyResolver.Container.Resolve<GwentClientService>().User;
                //await Task.Delay(100);
                //await DependencyResolver.Container.Resolve<HubConnection>().StartAsync();
                //var result = await DependencyResolver.Container.Resolve<HubConnection>().InvokeAsync<bool>("Reconnect", user.UserName, user.PassWord);
                //if (result) Debug.Log("尝试重连成功");
                //else
                //{
                //    SceneManager.LoadScene("LoginSecen");
                //    await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("断开连接", "请尝试重新登陆");
                //}
            });
            //////////////////////////////
            hubConnection.On<string>("Test", message => Debug.Log($"收到了服务端来自Debug的信息:{message}"));
            Player = new LocalPlayer(hubConnection);
            HubConnection = hubConnection;
            hubConnection.StartAsync();
        }
        public Task<bool> Register(string username, string password, string playername) => HubConnection.InvokeAsync<bool>("Register", username, password, playername);
        public async Task<UserInfo> Login(string username, string password)
        {
            //登录,如果成功保存登录信息
            User = await HubConnection.InvokeAsync<UserInfo>("Login", username, password);
            if (User != null)
                Player.PlayerName = User.PlayerName;
            return User;
        }
        //开始匹配与停止匹配
        public Task<bool> Match(string deckId)
        {
            Player.Deck = User.Decks.Single(x=>x.Id==deckId);
            return HubConnection.InvokeAsync<bool>("Match", deckId);
        }
        public Task<bool> StopMatch()
        {
            return HubConnection.InvokeAsync<bool>("StopMatch");
        }
        //新建卡组,删除卡组,修改卡组
        public Task<bool> AddDeck(DeckModel deck) => HubConnection.InvokeAsync<bool>("AddDeck", deck);
        public Task<bool> RemoveDeck(string deckId) => HubConnection.InvokeAsync<bool>("RemoveDeck", deckId);
        public Task<bool> ModifyDeck(string deckId, DeckModel deck) => HubConnection.InvokeAsync<bool>("ModifyDeck", deckId, deck);
        public Task SendOperation(Task<Operation<UserOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);

        //开启连接,断开连接
        public Task StartAsync() => HubConnection.StartAsync();
        public Task StopAsync() => HubConnection.StopAsync();
    }
}