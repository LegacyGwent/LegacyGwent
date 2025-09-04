using Assets.Script.Localization;
using Cynthia.Card.Client;
using UnityEngine;
using Autofac;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using System.Linq;

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
    void Start()
    {
        //_ = AutoTest();
        _globalUIService = DependencyResolver.Container.Resolve<GlobalUIService>();
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
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
        UpdateUserInfo();
    }
    private async void UpdateUserInfo()
    {
        _client.User = await _client.QueryUserInfo(_client.User.UserName, _client.User.PassWord);
        if (_client.User.NewlyUnlockedTrinkets.HasNewTrinkets)
        {
            // Display notifications for new trinkets
            if (_client.User.NewlyUnlockedTrinkets.NewAvatars.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewAvatars)
                {
                    TrinketUnlock = Instantiate(TrinketUnlockPrefab, Vector3.zero, Quaternion.identity, Canevas.transform);
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedAvatars"); // sets the art in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetAvatarContext(trinketID);
                }

            }
            if (_client.User.NewlyUnlockedTrinkets.NewBorders.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewBorders)
                {
                    TrinketUnlock = Instantiate(TrinketUnlockPrefab, Vector3.zero, Quaternion.identity, Canevas.transform);
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedBorders"); // sets the art in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetBorderContext(trinketID);
                }

            }
            if (_client.User.NewlyUnlockedTrinkets.NewTitles.Count > 0)
            {
                foreach (var trinketID in _client.User.NewlyUnlockedTrinkets.NewTitles)
                {
                    TrinketUnlock = Instantiate(TrinketUnlockPrefab, Vector3.zero, Quaternion.identity, Canevas.transform);
                    string color = _titles.Where(x => x.ID == trinketID).Single().TitleColor;
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTitleLook(trinketID, mycolormap[color]); // sets the look in the preview
                    TrinketUnlock.GetComponent<TrinketsContext>().SetTitleContext(trinketID);
                }

            }
            // // Clear the notifications after displaying them
            // await _client.ClearNewlyUnlockedTrinkets(_client.User.UserName);
            // // await hubConnection.InvokeAsync("ClearNewlyUnlockedTrinkets");
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
