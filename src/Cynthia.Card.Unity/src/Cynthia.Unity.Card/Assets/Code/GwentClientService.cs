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
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card.Common.Models.Localization;
using Cynthia.Card.Common.Models;
using Cynthia.Card;
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
        public UserInfo Avatars { get; set; }
        public UserInfo Borders { get; set; }
        public UserInfo Titles { get; set; }
        public bool IsAutoPlay { get; set; } = false;
        private GlobalUIService _globalUIService;
        private ITubeInlet sender;/*待修改*/
        private ITubeOutlet receiver;/*待修改*/
        private ITubeInlet Sender;//sender for taunts
        private ITubeOutlet Receiver;// receiver for taunts
        private LocalizationService _translator;
        private ClientMessagesReaderService _messagesReaderService;
        private readonly SemaphoreSlim _connectionStartGate = new SemaphoreSlim(1, 1);

        private readonly object _lock = new object();
        private ClientState _clientState = ClientState.Standby;
        public ClientState ClientState
        {
             get { return _clientState; }
            set
            {
                _clientState = value;
                OnClientStateChanged();
            }
        }

        public event Action ClientStateChanged;

        private void OnClientStateChanged()
        {
            ClientStateChanged?.Invoke();
        }


        /*待修改*/
        public Task<IList<string>> CheckUserMessages(string playername)
        {
            return HubConnection.InvokeAsync<IList<string>>("GetUserMessages", playername);
        }

        public Task<bool> RemoveUserMessage(int messageId)
        {
            return HubConnection.InvokeAsync<bool>("RemoveUserMessage", User.UserName, messageId);
        }



        public Task<bool> MatchResult()
        {
            return receiver.ReceiveAsync<bool>();
        }
        public Task<string> PlayTaunt()
        {
            try
            {
                return Receiver.ReceiveAsync<string>();
            }
            catch
            {
                return Task.Delay(1).ContinueWith(t => ""); // if no taunt was received yet, return "" to avoid errors
            }
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
            hubConnection.On<string>("PlayTaunt", async x =>
            {
                (Sender, Receiver) = Tube.CreateSimplex();
                await Sender.SendAsync<string>(x);
            });
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
        }

        public async Task EnsureConnectedAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _connectionStartGate.WaitAsync(cancellationToken);
            try
            {
                if (HubConnection.State == HubConnectionState.Connected)
                {
                    return;
                }

                if (HubConnection.State != HubConnectionState.Disconnected)
                {
                    throw new InvalidOperationException($"Cannot start the game connection while it is {HubConnection.State}.");
                }

                await HubConnection.StartAsync(cancellationToken);
                if (HubConnection.State != HubConnectionState.Connected)
                {
                    throw new InvalidOperationException($"Game connection startup ended in state {HubConnection.State}.");
                }
            }
            finally
            {
                _connectionStartGate.Release();
            }
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

        public async Task<string> GetCardMapVersion(CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.Log("尝试获取");
            var version = await HubConnection.InvokeAsync<string>("GetCardMapVersion", cancellationToken);
            Debug.Log($"成功获取CardMap版本号:{version}");
            return version;
        }
        public async Task<string> GetTrinketMapVersion(CancellationToken cancellationToken = default(CancellationToken)) // get the version of the Trinket Map to decide if it needs an update
        {
            Debug.Log("getting trinket map version");
            var version = await HubConnection.InvokeAsync<string>("GetTrinketMapVersion", cancellationToken);
            Debug.Log($"the version is:{version}");
            return version;
        }

        public async Task AutoUpdateGame(Text infoText, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var clientVersion = new Version(GwentMap.CardMapVersion.ToString());
            var clientTrinketMapVersion = new Version(TrinketMap.TrinketMapVersion.ToString());
            Debug.Log($"the client trinket map version is {TrinketMap.TrinketMapVersion.ToString()}");
            Debug.Log($"the client version is {GwentMap.CardMapVersion.ToString()}");
            var localesWereLastUpdatedTo = new Version(PlayerPrefs.GetString("LocalizationVersion", "0.0.0.0"));
            var serverVersion = new Version(await GetCardMapVersion(cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
            Debug.Log($"the server version is {serverVersion}");
            // If the client is outdated, load cosmetics information from the server
            var severTrinketMapVersion = new Version(await GetTrinketMapVersion(cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (clientTrinketMapVersion != severTrinketMapVersion)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    infoText.text = "loading trinkets information";
                    var loadedAvatarMap = JsonConvert.DeserializeObject<Dictionary<string, TrinketAvatar>>(await GetAvatarMap(cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                    TrinketMap.AvatarMap = loadedAvatarMap;
                    var loadedBorderMap = JsonConvert.DeserializeObject<Dictionary<string, Border>>(await GetBorderMap(cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                    TrinketMap.BorderMap = loadedBorderMap;
                    var loadedTitleMap = JsonConvert.DeserializeObject<Dictionary<string, Title>>(await GetTitleMap(cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                    TrinketMap.TitleMap = loadedTitleMap;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading cosmetics: {e.Message}");
                cancellationToken.ThrowIfCancellationRequested();
                infoText.text = string.Format(_translator.GetText("LoginMenu_UpdateError"), e.Message);
            }
            cancellationToken.ThrowIfCancellationRequested();
            infoText.text = _translator.GetText("LoginMenu_CardDataCheck");
            var fileHandler = new TextLocalizationFileHandler("Locales");
            try
            {
                // If the client is outdated, load card abilities from the server
                // (Don't save it, we don't want players to mess with card abilities)
                if (clientVersion != serverVersion)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    infoText.text = _translator.GetText("LoginMenu_CardDataUpdating");
                    Debug.Log($"start loading new card map");
                    var loadedCardMap = JsonConvert.DeserializeObject<Dictionary<string, GwentCard>>(await GetCardMap(cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                    Debug.Log($"end loading new card map");
                    GwentMap.CardMap = loadedCardMap;
                    GwentMap.InitializeCardMap();
                }
                // Download locales from the server if:
                // 1. Locales have not been downloaded on this installation
                // 2. There came out a new version of locales since the last time we downloaded them

                if (LocalizationUpdatePolicy.ShouldDownloadLocales(
                    fileHandler.AreFilesDownloaded(),
                    localesWereLastUpdatedTo,
                    serverVersion))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    infoText.text = _translator.GetText("LoginMenu_LanguagesUpdating");
                    var loadedGameLocales = JsonConvert.DeserializeObject<IList<GameLocale>>(await GetGameLocales(cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                    fileHandler.SaveGameLocales(loadedGameLocales);
                    PlayerPrefs.SetString("LocalizationVersion", serverVersion.ToString());
                }
                cancellationToken.ThrowIfCancellationRequested();
                infoText.text = _translator.GetText("LoginMenu_GameUpdated");
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading card abilities: {e.Message}");
                cancellationToken.ThrowIfCancellationRequested();
                infoText.text = string.Format(_translator.GetText("LoginMenu_UpdateError"), e.Message);
            }

            cancellationToken.ThrowIfCancellationRequested();
            // If there are locale files downloaded, use them instead of default game resources
            if (fileHandler.AreFilesDownloaded())
            {
                _translator.TextLocalization.ResourceHandler = fileHandler;
            }

            // After maps/locales are up to date, release trinkets for the active season
            try
            {
                var activeSeason = await GetSeasonData(true, 0, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                TrinketMap.ReleaseSeasonRewards(activeSeason?.seasonalRewards);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.Log($"Error releasing seasonal trinkets: {e.Message}");
            }
        }

        public Task<string> GetCardMap(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetCardMap", cancellationToken);
        }
        public Task<string> GetAvatarMap(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetAvatarMap", cancellationToken);
        }
        public Task<string> GetBorderMap(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetBorderMap", cancellationToken);
        }
        public Task<string> GetTitleMap(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetTitleMap", cancellationToken);
        }
        // Player Count 
        public async Task<int> GetUserCount()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetUserCount");
            return playercount;
        }
        public async Task<int> GetUsersInMatchCount()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetUsersInMatchCount");
            return playercount;
        }
        public async Task<int> GetUsersInRankedCount()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetUsersInRankedCount");
            return playercount;
        }
        public async Task<int> GetUsersInCasualCount()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetUsersInCasualCount");
            return playercount;
        }
        public async Task<int> GetUsersvsAICount()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetUsersvsAICount");
            return playercount;
        }
        public async Task<int> GetIsRankQueue()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetIsRankQueue");
            return playercount;
        }
        public async Task<int> GetIsCasualQueue()
        {
            int playercount = await HubConnection.InvokeAsync<int>("GetIsCasualQueue");
            return playercount;
        }              

        public Task<string> GetGameLocales(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetGameLocales", cancellationToken);
        }

        public Task<string> GetLatestClientVersion(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetLatestClientVersion", cancellationToken);
        }
        public Task<string> GetNotes(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetNotes", cancellationToken);
        }
        public Task<string> GetNotesEN(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetNotesEN", cancellationToken);
        }
        public Task<string> GetDownloadLink(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HubConnection.InvokeAsync<string>("GetDownloadLink", cancellationToken);
        }

        public Task<int> GetPalyernameMMR(string playername)
        {
            return HubConnection.InvokeAsync<int>("GetPalyernameMMR", playername);
        }

        public Task<Tuple<int, int>> GetPalyernameMMRandPeak(string playername)
        {
            return HubConnection.InvokeAsync<Tuple<int, int>>("GetPalyernameMMRandPeak", playername);
        }


        public Task<int[]> GetPlayernameStreak(string playername)
        {
            return HubConnection.InvokeAsync<int[]>("GetPlayernameStreak", playername);
        }
        
        public Task<List<SeasonReward>> GetSeasonRewards(int seasonID, string type) 
        {
            return HubConnection.InvokeAsync<List<SeasonReward>>("GetSeasonRewards", seasonID, type);
        }

        public async Task<SeasonInfo> GetSeasonData(bool active = true, int id = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await HubConnection.InvokeAsync<SeasonInfo>("GetSeasonData", active, id, cancellationToken);
        }
        public Task<IList<SeasonInfo>>GetSeasons()
        {
            return HubConnection.InvokeAsync<IList<SeasonInfo>>("GetSeasons");
        }

        public Task<IList<Tuple<string, string, string, string, int, int, IList<int[]>>>> GetAllMMR(int offset, int limit)
        {
            return HubConnection.InvokeAsync<IList<Tuple<string, string, string, string, int, int, IList<int[]>>>>("GetAllMMRExtended", offset, limit);
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
        // get the version of the Trinket Map to decide if it needs an update
        public async Task<UserInfo> QueryUserInfo(string username, string password)
        {
            
            User = await HubConnection.InvokeAsync<UserInfo>("QueryUserInfo", username, password);
            return User;
        }
        //开始匹配与停止匹配
        public Task<bool> NewMatchOfPassword(string deckId, string password, int usingBlacklist)
        {
            Player.Deck = User.Decks.Single(x => x.Id == deckId);
            return HubConnection.InvokeAsync<bool>("NewMatchOfPassword", deckId, password, usingBlacklist);
        }
        public Task<bool> SendGG(string myname, string enemyname)
        {
            return HubConnection.InvokeAsync<bool>("SendGG", myname, enemyname);
        }
        public Task<bool> SendTaunt(string enemyName, string tauntid)
        {
            return HubConnection.InvokeAsync<bool>("SendTaunt", enemyName, tauntid);
        }
        // Set the current Avatar of the User
        public Task<bool> UpdateAvatar(string playername, string AvatarID) => HubConnection.InvokeAsync<bool>("UpdateAvatar", playername, AvatarID);        
        // Set the current Border of the User
        public Task<bool> UpdateBorder(string playername, string BorderID) => HubConnection.InvokeAsync<bool>("UpdateBorder", playername, BorderID);
        // Set the current Title of the User
        public Task<bool> UpdateTitle(string playername, string TitleID) => HubConnection.InvokeAsync<bool>("UpdateTitle", playername, TitleID);
        //
        public Task<bool> ClearNewlyUnlockedTrinkets(string username)
        {
            return HubConnection.InvokeAsync<bool>("ClearNewlyUnlockedTrinkets", username);
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
        public Task<bool> SwapDecks(string firstDeckId, string secondDeckId) => HubConnection.InvokeAsync<bool>("SwapDecks", firstDeckId, secondDeckId);
        public Task<bool> ModifyDeck(string deckId, DeckModel deck) => HubConnection.InvokeAsync<bool>("ModifyDeck", deckId, deck);
        public Task<bool> ModifyBlacklist(BlacklistModel blacklist) => HubConnection.InvokeAsync<bool>("ModifyBlacklist", blacklist);

        public Task SendOperation(Task<Operation<int>> operation) => HubConnection.SendAsync("GameOperation", operation);

        //开启连接,断开连接
        public Task StartAsync() => EnsureConnectedAsync();
        public async Task StopAsync()
        {
            await _connectionStartGate.WaitAsync();
            try
            {
                if (HubConnection.State != HubConnectionState.Disconnected)
                {
                    await HubConnection.StopAsync();
                }
            }
            finally
            {
                _connectionStartGate.Release();
            }
        }
    }
}
