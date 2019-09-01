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

        public GwentClientService(HubConnection hubConnection)
        {
            /*待修改*/
            (sender, receiver) = Tube.CreateSimplex();
            /*待修改*/
            hubConnection.On<bool>("MatchResult", async x =>
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
                SceneManager.LoadScene("LoginSecen");
                await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("断开连接", "请尝试重新登陆\n注意! 在目前版本中,如果处于对于或匹配时断线,需要重新启动客户端,否则下次游戏开始时会异常卡死。\nNote! In the current version, if you are disconnected when matching or Confrontation, you need to restart the client, otherwise the next game will start with an abnormal.");
            });
            hubConnection.On("ExitGame", () =>
            {

            });
            hubConnection.On("ShowMessageBox", () =>
            {

            });
            hubConnection.On("Close", () =>
            {

            });
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
        public Task<bool> MatchOfPassword(string deckId, string password)
        {
            Player.Deck = User.Decks.Single(x => x.Id == deckId);
            return HubConnection.InvokeAsync<bool>("MatchOfPassword", deckId, password);
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