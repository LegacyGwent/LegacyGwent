using UnityEngine;
using UnityEngine.SceneManagement;
using Autofac;
using Cynthia.Card.Client;
using System;
using Microsoft.AspNetCore.SignalR.Client;

public class GameCode : MonoBehaviour
{
    public GameUIControl GameUIControl;
    public GameResultControl GameResultControl;
    public GameCardsControl GameCardsControl;
    public GameCardShowControl GameCardShowControl;
    public BigRoundControl BigRoundControl;
    public GameEvent GameEvent;
    public Animator MyRoundShow;
    public Transform GameScale;

    private void Start()
    {
        GameStart();
    }
    private async void GameStart()
    {
        // await DependencyResolver.Container.Resolve<GwentClientGameService>().Play(DependencyResolver.Container.Resolve<GwentClientService>().Player);
        try
        {
            await new GwentClientGameService(DependencyResolver.Container.Resolve<GameCodeService>(), DependencyResolver.Container.Resolve<GlobalUIService>()).Play(DependencyResolver.Container.Resolve<GwentClientService>().Player);
        }
        catch (Exception e)
        {
            Debug.Log($"被我捉住了!{e.Message}");
            SceneManager.LoadScene("LoginScene");
            DependencyResolver.Container.Resolve<GwentClientService>().ClientState = ClientState.Standby;
            if (ClientGlobalInfo.ViewingRoomId != "")
            {
                await DependencyResolver.Container.ResolveNamed<HubConnection>("game").InvokeAsync<bool>("LeaveViewList", "");
                ClientGlobalInfo.ViewingRoomId = "";
            }
        }
    }
    public async void LeaveGame()
    {
        ClientGlobalInfo.IsToMatch = true;
        if (ClientGlobalInfo.ViewingRoomId != "")
        {
            ClientGlobalInfo.IsToMatch = false;
            await DependencyResolver.Container.ResolveNamed<HubConnection>("game").InvokeAsync<bool>("LeaveViewList", "");
            ClientGlobalInfo.ViewingRoomId = "";
        }
        SceneManager.LoadScene("Game");
        DependencyResolver.Container.Resolve<GwentClientService>().ClientState = ClientState.Standby;
    }
}
