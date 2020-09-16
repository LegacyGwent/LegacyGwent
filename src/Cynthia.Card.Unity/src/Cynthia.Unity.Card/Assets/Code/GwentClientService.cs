using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using Assets.Script.Localization;
using Assets.Script.Localization.Serializables;
using Assets.Script.ResourceManagement;
using Microsoft.AspNetCore.Internal;
using UnityEditor;

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
            var clientVersion = new Version(PlayerPrefs.GetString("CardMapVersion", GwentMap.CardMapVersion.ToString()));
            var serverVersion = new Version(await GetCardMapVersion());

            infoText.text = _translator.GetText("LoginMenu_CardDataCheck");

            if (clientVersion != serverVersion)
            {
                infoText.text = _translator.GetText("LoginMenu_CardDataUpdating");
                try
                {
                    var loadedCardMap = JsonConvert.DeserializeObject<Dictionary<string, GwentCard>>(await GetCardMap());
                    GwentMap.CardMap = loadedCardMap;
                }
                catch (Exception e)
                {
                    Debug.Log($"Error loading card abilities: {e.Message}");
                }
            }

            var fileHandler = new TextLocalizationFileHandler("Locales");
            if (clientVersion != serverVersion && !fileHandler.AreFilesDownloaded()) 
            {
                var loadedGameLocales = JsonConvert.DeserializeObject<IList<GameLocale>>(await GetGameLocales());
                fileHandler.SaveGameLocales(loadedGameLocales);
            }

            if (fileHandler.AreFilesDownloaded())
            {
                _translator.TextLocalization.ResourceHandler = fileHandler;
            }
            infoText.text = _translator.GetText("LoginMenu_CardDataUpdated");
        }

        /*public async Task AutoUpdateCardMapVersion(Text text)
        {
            var localVersion = new Version(PlayerPrefs.GetString("CardMapVersion", GwentMap.CardMapVersion.ToString()));
            text.text = _translator.GetText("LoginMenu_CardDataCheck");
            var serverVersion = new Version(await GetCardMapVersion());
            Debug.Log($"正在对比版本号...{localVersion},{serverVersion}");

            var loadedCards = default(IDictionary<string, GwentCard>);

            if (localVersion != serverVersion)
            {
                //Debug.Log($"发现不一致,更新");
                text.text = _translator.GetText("LoginMenu_CardDataUpdating");
                try
                {
                    //Load card abilities from the server
                    loadedCards = JsonConvert.DeserializeObject<IList<GwentCard>>(await GetCardMap());
                    GwentMap.CardMap.LoadCards(loadedCards);
                }
                catch (Exception e)
                {
                    Debug.Log("判断是否存在时出现问题:" + e.Message);
                }
                //Debug.Log($"成功获取到数据,进行缓存");

                //WriteCardMapData(JsonConvert.SerializeObject(newData));
                // WE DONT WANT PLAYERS TO EDIT CARD DATA

                //Debug.Log($"更新本地版本号为:{serverVersion}");

                PlayerPrefs.SetString("CardMapVersion", serverVersion.ToString());

                //var message = PlayerPrefs.GetString("CardMapVersion", "存储失败..?");
                //Debug.Log(message);
            }
            else
            {
                //Debug.Log($"版本一致,不做变化");

                //loadedCards = ReadCardMapData();
                //GwentMap.CardMap = loadedCards;
                //WE DONT WANT PLAYERS TO HAVE CUSTOM CARDS THAT THEY CAN EDIT

            }






            text.text = _translator.GetText("LoginMenu_CardDataUpdated");
        }*/

        public IDictionary<string, GwentCard> ReadCardMapData()
        {
            var path = Application.persistentDataPath;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            path = Application.dataPath + "/StreamingFile" + "/CardData.json";
            if (!Directory.Exists(Application.dataPath + "/StreamingFile"))
            {
                Directory.CreateDirectory(Application.dataPath + "/StreamingFile");
            }
            if (!File.Exists(path))
            {
                Debug.Log("路径不存在,返回默认");
                return GwentMap.CardMap;
            }
#elif UNITY_ANDROID
            path = Application.persistentDataPath + "/CardData.json";
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            if (!File.Exists(path))
            {
                Debug.Log("路径不存在,返回默认");
                return GwentMap.CardMap;
            }
#endif
            StreamReader sr = new StreamReader(path);
            var data = sr.ReadToEnd();
            sr.Close();
            Debug.Log("读取完成,大小为:" + data.Length);
            // Debug.Log(data);
            return JsonConvert.DeserializeObject<IDictionary<string, GwentCard>>(data);
        }

        public void WriteCardMapData(string cardMapJson)
        {
            var path = Application.persistentDataPath;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            Debug.Log("进入Win/Editor保存数据分支");
            path = Application.dataPath + "/StreamingFile" + "/CardData.json";
            if (!Directory.Exists(Application.dataPath + "/StreamingFile"))
            {
                Directory.CreateDirectory(Application.dataPath + "/StreamingFile");
            }
            if (!File.Exists(path))
            {
                Debug.Log("路径不存在,创造路径");
                File.Create(path).Dispose();
            }
#elif UNITY_ANDROID
            Debug.Log("进入Android保存数据分支");
            path = Application.persistentDataPath + "/CardData.json";
            if(!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            if(!File.Exists(path))
            {
                Debug.Log("路径不存在,创造路径");
                File.Create(path).Dispose();
            }
#endif
            Debug.Log("进入公共数据分支");
            StreamWriter sw = new StreamWriter(path);
            sw.Write(cardMapJson);
            sw.Close();
            Debug.Log("写入完成");
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
        public Task JoinEditor() => HubConnection.InvokeAsync<bool>("JoinEditor");
        public Task LeaveEditor() => HubConnection.InvokeAsync<bool>("LeaveEditor");
        public Task<bool> AddDeck(DeckModel deck) => HubConnection.InvokeAsync<bool>("AddDeck", deck);
        public Task<bool> RemoveDeck(string deckId) => HubConnection.InvokeAsync<bool>("RemoveDeck", deckId);
        public Task<bool> ModifyDeck(string deckId, DeckModel deck) => HubConnection.InvokeAsync<bool>("ModifyDeck", deckId, deck);
        public Task SendOperation(Task<Operation<UserOperationType>> operation) => HubConnection.SendAsync("GameOperation", operation);

        //开启连接,断开连接
        public Task StartAsync() => HubConnection.StartAsync();
        public Task StopAsync() => HubConnection.StopAsync();
    }
}
