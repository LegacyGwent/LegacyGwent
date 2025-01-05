using Assets.Script.Localization;
using Cynthia.Card.Client;
using UnityEngine;
using Autofac;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
