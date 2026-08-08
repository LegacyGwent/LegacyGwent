using Assets.Script.Localization;
using Cynthia.Card.Client;
using UnityEngine;
using Autofac;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cynthia.Card.Common.Models;
using Cynthia.Card;
using System.Linq;
using System.Collections.Generic;

public class MainCode : MonoBehaviour
{
    public GameObject UserCount;
    private GlobalUIService _globalUIService;
    private GwentClientService _client;
    private LocalizationService _translator;
    public GameObject Context;
    public GameObject MatchUI;
    public EditorInfo EditorMenu;
    public Button MatchMenuButton;
    public Button RankMatchMenuButton;
    public Button DoMatchButton;
    private GameObject TrinketUnlock;
    public GameObject TrinketUnlockPrefab;
    public GameObject Canevas;
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; } // stores the color of the title cosmetics
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); } // lists all title cosmetics

    private ClientMessagesReaderService _messagesReaderService;


    //async Task AutoTest()
    //{
    //    var hub = DependencyResolver.Container.ResolveNamed<HubConnection>("game");
    //    while (true)
    //    {
    //        await Task.Delay(500);
    //        if (hub.State == HubConnectionState.Disconnected)
    //        {
    //            Debug.Log("MainCode检测到断线!");
    //            SceneManager.LoadScene("LoginSecen");
    //            _ = DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("断开连接", "请尝试重新登陆");
    //            return;
    //        }
    //    }
    //}
    async Task Start()
    {
        //_ = AutoTest();
        _globalUIService = DependencyResolver.Container.Resolve<GlobalUIService>();
        _messagesReaderService = DependencyResolver.Container.Resolve<ClientMessagesReaderService>();

        _client = DependencyResolver.Container.Resolve<GwentClientService>();

        // Refresh the authenticated account before consuming persistent
        // notices. Doing this in the opposite order can display and then try
        // to acknowledge a previously logged-in account's season message.
        await UpdateUserInfo();
        await _messagesReaderService.CheckMessages(_client.User?.UserName);

        if (_client.IsAutoPlay || ClientGlobalInfo.IsToMatch)
        {
            if (ClientGlobalInfo.IsPreviousRankMatch)
            {
                RankMatchMenuButton.onClick.Invoke();
            }
            else
            {
                MatchMenuButton.onClick.Invoke();
            }
            //DoMatchButton.onClick.Invoke();
        }
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    private async Task UpdateUserInfo()
    {
        _client.User = await _client.QueryUserInfo(_client.User.UserName, _client.User.PassWord);
        if (_client.User.NewlyUnlockedTrinkets.HasNewTrinkets)
        {
            var pendingNotifications = new List<GameObject>();

            if (Canevas == null)
            {
                var canvas = FindObjectOfType<Canvas>();
                Canevas = canvas == null ? null : canvas.gameObject;
            }

            if (TrinketUnlockPrefab == null)
                TrinketUnlockPrefab = Resources.Load<GameObject>("Prefab/Trinkets/TrinketUnlockPrefab");

            if (Canevas == null || TrinketUnlockPrefab == null)
            {
                Debug.LogWarning("Cannot display newly unlocked trinkets because the notification canvas or prefab is missing.");
                await _client.ClearNewlyUnlockedTrinkets(_client.User.UserName);
                return;
            }

            // Display notifications for new trinkets
            if (_client.User.NewlyUnlockedTrinkets.NewAvatars.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewAvatars)
                {
                    TrinketUnlock = CreateTrinketUnlock();
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedAvatars"); // sets the art in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetAvatarContext(trinketID);
                    pendingNotifications.Add(TrinketUnlock);
                }

            }
            if (_client.User.NewlyUnlockedTrinkets.NewBorders.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewBorders)
                {
                    TrinketUnlock = CreateTrinketUnlock();
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedBorders"); // sets the art in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetBorderContext(trinketID);
                    pendingNotifications.Add(TrinketUnlock);
                }

            }
            if (_client.User.NewlyUnlockedTrinkets.NewTitles.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewTitles)
                {
                    var title = _titles.FirstOrDefault(x => x.ID == trinketID);
                    Color titleColor;
                    if (title == null || !mycolormap.TryGetValue(title.TitleColor, out titleColor))
                    {
                        Debug.LogWarning("Skipping unknown unlocked title: " + trinketID);
                        continue;
                    }
                    TrinketUnlock = CreateTrinketUnlock();
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTitleLook(trinketID, titleColor); // sets the look in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTitleContext(trinketID);
                    pendingNotifications.Add(TrinketUnlock);
                }

            }
            ShowTrinketUnlockQueue(pendingNotifications);
            // Clear the notifications after displaying them
            await _client.ClearNewlyUnlockedTrinkets(_client.User.UserName);
        }
    }

    private GameObject CreateTrinketUnlock()
    {
        // This is a UI prefab. Instantiating it at world position zero makes
        // its canvas-local position resolution-dependent and can place the
        // reward panel off-screen while its backdrop still blocks all input.
        var notification = Instantiate(TrinketUnlockPrefab, Canevas.transform, false);
        var rectTransform = notification.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        // Several default cosmetics can unlock on the first login. Keep later
        // notifications hidden until the current one is acknowledged instead
        // of stacking multiple modal backdrops and panels on top of each other.
        notification.SetActive(false);
        notification.transform.SetAsLastSibling();
        return notification;
    }

    private static void ShowTrinketUnlockQueue(IList<GameObject> notifications)
    {
        var queuedNotifications = new List<KeyValuePair<GameObject, Button>>();
        foreach (var notification in notifications)
        {
            var okButton = notification
                .GetComponentsInChildren<Button>(true)
                .FirstOrDefault(button => button.gameObject.name == "OkButton");

            if (okButton == null)
            {
                Debug.LogWarning("Discarding a trinket notification without an OkButton so it cannot block the reward queue.");
                Object.Destroy(notification);
                continue;
            }

            queuedNotifications.Add(new KeyValuePair<GameObject, Button>(notification, okButton));
        }

        for (var index = 0; index < queuedNotifications.Count - 1; index++)
        {
            var nextNotification = queuedNotifications[index + 1].Key;
            queuedNotifications[index].Value.onClick.AddListener(() =>
            {
                nextNotification.SetActive(true);
                nextNotification.transform.SetAsLastSibling();
            });
        }

        if (queuedNotifications.Count > 0)
        {
            queuedNotifications[0].Key.SetActive(true);
            queuedNotifications[0].Key.transform.SetAsLastSibling();
        }
    }

    void Awake()
    {
        RectTransform rectTransform = UserCount.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public async void ExitGameClick()
    {
        // SceneManager.LoadScene("LoginSecen");
        if (await _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_QuitTitle"), _translator.GetText("PopupWindow_QuitDesc")))
        {
            //进行一些处理
            Application.Quit();
            return;
        }
    }
}
