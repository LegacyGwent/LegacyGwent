using Alsein.Extensions.IO;
using Alsein.Extensions.LifetimeAnnotations;
using Assets.Script.Localization;
using Assets.Script.ResourceManagement;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.Common.Models.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cynthia.Card.Client
{
    public enum ClientState
    {
        Match,
        Play,
        Standby
    }
    [Singleton]
    public class GwentClientService
    {
        public HubConnection HubConnection { get; set; }
        public LocalPlayer Player { get; set; }
        public UserInfo User { get; set; }
        public bool IsAutoPlay { get; set; } = false;
        private GlobalUIService _globalUIService;
        private ITubeInlet sender;/*待修改*/
        private ITubeOutlet receiver;/*待修改*/

        private LocalizationService _translator;

        public ClientState ClientState { get; set; } = ClientState.Standby;

        /*待修改*/
        public Task<bool> MatchResult()
        {
            return receiver.ReceiveAsync<bool>();
        }

        public GwentClientService(IContainer container, GlobalUIService globalUIService)
        {
            _translator = container.Resolve<LocalizationService>();
            _globalUIService = globalUIService;
            /*待修改*/
            (sender, receiver) = Tube.CreateSimplex();
            /*待修改*/

            var hubConnection = container.ResolveNamed<HubConnection>("game");
            Debug.Log(hubConnection);
            hubConnection.On<bool>("MatchResult", async x =>
            {
                await sender.SendAsync<bool>(x);
            });
            hubConnection.On("RepeatLogin", async () =>
            {
                SceneManager.LoadScene("LoginScene");
                ClientState = ClientState.Standby;
                await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox(
                    _translator.GetText("PopupWindow_LoggedOutTitle"),
                    _translator.GetText("PopupWindow_LoggedOutDesc"));
            });
            hubConnection.Closed += (async x =>
            {
                (sender, receiver) = Tube.CreateSimplex();
                SceneManager.LoadScene("LoginScene");
                ClientState = ClientState.Standby;
                Player.ResetTube();

                // LayoutRebuilder.ForceRebuildLayoutImmediate(Context);
                await _globalUIService.YNMessageBox(
                    _translator.GetText("PopupWindow_DisconnectedTitle"),
                    _translator.GetText("PopupWindow_DisconnectedDesc"),
                    "PopupWindow_OkButton", isOnlyYes: true);
                Application.Quit();
                // var messageBox = GameObject.Find("GlobalUI").transform.Find("MessageBoxBg").gameObject.GetComponent<MessageBox>();//.Show("断开连接", "请尝试重新登陆\n注意! 在目前版本中,如果处于对局或匹配时断线,需要重新启动客户端,否则下次游戏开始时会异常卡死。\nNote!\nIn the current version, if you are disconnected when matching or Playing, you need to restart the client, otherwise the next game will start with an abnormal.".Replace("\\n", "\n"), isOnlyYes: true);
                // messageBox.Buttons.SetActive(true);
                // messageBox.YesButton.SetActive(true);
                // messageBox.NoButton.SetActive(false);
                // messageBox.TitleText.text = "断开连接";
                // messageBox.MessageText.text = "请尝试重新登陆\n注意! 在目前版本中,如果处于对局或匹配时断线,需要重新启动客户端,否则下次游戏开始时会异常卡死。\nNote!\nIn the current version, if you are disconnected when matching or Playing, you need to restart the client, otherwise the next game will start with an abnormal.".Replace("\\n", "\n");
                // messageBox.YesText.text = "确定";
                // messageBox.gameObject.SetActive(true);
                // await messageBox.receiver.ReceiveAsync<bool>();
                // LayoutRebuilder.ForceRebuildLayoutImmediate(messageBox.Context);
            });
            hubConnection.On("ExitGame", () =>
            {
                Application.Quit();
            });
            hubConnection.On<string, string, string, string, bool>("ShowMessageBox", (string title, string message, string yes, string no, bool isyes) =>
            {
                _globalUIService.YNMessageBox(title, message, yes, no, isyes);
            });
            hubConnection.On<string, string>("Wait", (string title, string message) =>
            {
                _globalUIService.Wait(title, message);
            });
            hubConnection.On("Close", () =>
            {
                _globalUIService.Close();
            });
            hubConnection.On<string>("Test", message => Debug.Log($"收到了服务端来自Debug的信息:{message}"));
            Player = new LocalPlayer(hubConnection);
            HubConnection = hubConnection;
            hubConnection.StartAsync();
        }
        public async void ExitGameClick()
        {
            if (await _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_QuitTitle"), _translator.GetText("PopupWindow_QuitDesc")))
            // if (await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("断开连接", "请尝试重新登陆\n注意! 在目前版本中,如果处于对局或匹配时断线,需要重新启动客户端,否则下次游戏开始时会异常卡死。\nNote!\nIn the current version, if you are disconnected when matching or Playing, you need to restart the client, otherwise the next game will start with an abnormal."))
            {
                Application.Quit();
                return;
            }
        }

        public Task<string> GetLatestVersion()
        {
            return HubConnection.InvokeAsync<string>("GetLatestVersion");
        }

        public async Task<string> GetCardMapVersion()
        {
            Debug.Log("尝试获取");
            var version = await HubConnection.InvokeAsync<string>("GetCardMapVersion");
            Debug.Log($"成功获取CardMap版本号:{version}");
            return version;
        }

        public async Task AutoUpdateGame(Text infoText)
        {
            var clientVersion = new Version(GwentMap.CardMapVersion.ToString());
            Debug.Log($"the client version is {GwentMap.CardMapVersion.ToString()}");
            var localesWereLastUpdatedTo = new Version(PlayerPrefs.GetString("LocalizationVersion", GwentMap.CardMapVersion.ToString()));
            var serverVersion = new Version(await GetCardMapVersion());
            Debug.Log($"the server version is {serverVersion}");

            infoText.text = _translator.GetText("LoginMenu_CardDataCheck");
            var fileHandler = new TextLocalizationFileHandler("Locales");
            try
            {
                // If the client is outdated, load card abilities from the server
                // (Don't save it, we don't want players to mess with card abilities)
                if (clientVersion != serverVersion)
                {
                    infoText.text = _translator.GetText("LoginMenu_CardDataUpdating");
                    Debug.Log($"start loading new card map");
                    var loadedCardMap = JsonConvert.DeserializeObject<Dictionary<string, GwentCard>>(await GetCardMap());
                    Debug.Log($"end loading new card map");
                    GwentMap.CardMap = loadedCardMap;
                    GwentMap.InitializeCardMap();
                }
                // Download locales from the server if:
                // 1. Locales are not downloaded and the client is outdated
                // 2. There came out a new version of locales since the last time we downloaded them

                if (!fileHandler.AreFilesDownloaded() && clientVersion != serverVersion ||
                    localesWereLastUpdatedTo != serverVersion)
                {
                    infoText.text = _translator.GetText("LoginMenu_LanguagesUpdating");
                    var loadedGameLocales = JsonConvert.DeserializeObject<IList<GameLocale>>(await GetGameLocales());
                    fileHandler.SaveGameLocales(loadedGameLocales);
                    PlayerPrefs.SetString("LocalizationVersion", serverVersion.ToString());
                }
                infoText.text = _translator.GetText("LoginMenu_GameUpdated");
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading card abilities: {e.Message}");
                infoText.text = string.Format(_translator.GetText("LoginMenu_UpdateError"), e.Message);
            }

            // If there are locale files downloaded, use them instead of default game resources
            if (fileHandler.AreFilesDownloaded())
            {
                _translator.TextLocalization.ResourceHandler = fileHandler;
            }
        }

        public Task<string> GetCardMap()
        {
            return HubConnection.InvokeAsync<string>("GetCardMap");
        }

        public Task<string> GetGameLocales()
        {
            return HubConnection.InvokeAsync<string>("GetGameLocales");
        }

        public Task<string> GetNotes()
        {
            return HubConnection.InvokeAsync<string>("GetNotes");
        }

        public Task<int> GetPalyernameMMR(string playername)
        {
            return HubConnection.InvokeAsync<int>("GetPalyernameMMR", playername);
        }

        public Task<IList<Tuple<string, int>>> GetAllMMR(int offset, int limit)
        {
            return HubConnection.InvokeAsync<IList<Tuple<string, int>>>("GetAllMMR", offset, limit);
        }

        public Task<Tuple<IList<Tuple<string, int>>, IList<Tuple<string, string, string>>, IList<Tuple<string, string, string>>>> GetUsers()
        {
            return HubConnection.InvokeAsync<Tuple<IList<Tuple<string, int>>, IList<Tuple<string, string, string>>, IList<Tuple<string, string, string>>>>("GetUsers");
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
        public Task<bool> NewMatchOfPassword(string deckId, string password, int usingBlacklist)
        {
            Player.Deck = User.Decks.Single(x => x.Id == deckId);
            return HubConnection.InvokeAsync<bool>("NewMatchOfPassword", deckId, password, usingBlacklist);
        }
        public Task<bool> StopMatch()
        {
            return HubConnection.InvokeAsync<bool>("StopMatch");
        }
        public Task<bool> Surrender()
        {
            return HubConnection.InvokeAsync<bool>("Surrender");
        }
        //新建卡组,删除卡组,修改卡组
        public Task JoinEditor() => HubConnection.InvokeAsync<bool>("JoinEditor");
        public Task LeaveEditor() => HubConnection.InvokeAsync<bool>("LeaveEditor");
        public Task<bool> AddDeck(DeckModel deck) => HubConnection.InvokeAsync<bool>("AddDeck", deck);
        public Task<bool> RemoveDeck(string deckId) => HubConnection.InvokeAsync<bool>("RemoveDeck", deckId);
        public Task<bool> ModifyDeck(string deckId, DeckModel deck) => HubConnection.InvokeAsync<bool>("ModifyDeck", deckId, deck);
        public Task<bool> ModifyBlacklist(BlacklistModel blacklist) => HubConnection.InvokeAsync<bool>("ModifyBlacklist", blacklist);

        public Task SendOperation(Task<Operation<int>> operation) => HubConnection.SendAsync("GameOperation", operation);

        //开启连接,断开连接
        public Task StartAsync() => HubConnection.StartAsync();
        public Task StopAsync() => HubConnection.StopAsync();
    }
}
