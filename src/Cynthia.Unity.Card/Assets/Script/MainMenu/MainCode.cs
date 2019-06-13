using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        _globalUIService = DependencyResolver.Container.Resolve<GlobalUIService>();
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
        if(_client.IsAutoPlay)
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
