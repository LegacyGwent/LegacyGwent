using Cynthia.Card.Client;
using UnityEngine;
using Autofac;
using UnityEngine.UI;

public class MainCode : MonoBehaviour
{
    private GlobalUIService _globalUIService;
    private GwentClientService _client;
    public GameObject Context;
    public GameObject MatchUI;
    public EditorInfo EditorMenu;
    public Button MatchMenuButton;
    public Button DoMatchButton;

    //async Task AutoTest()
    //{
    //    var hub = DependencyResolver.Container.Resolve<HubConnection>();
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
        if (_client.IsAutoPlay || GlobalState.IsToMatch)
        {
            MatchMenuButton.onClick.Invoke();
            //DoMatchButton.onClick.Invoke();
        }
    }
    public async void ExitGameClick()
    {
        if (await _globalUIService.YNMessageBox("退出游戏?", "是否退出游戏"))
        {
            //进行一些处理
            Application.Quit();
            return;
        }
    }
}
