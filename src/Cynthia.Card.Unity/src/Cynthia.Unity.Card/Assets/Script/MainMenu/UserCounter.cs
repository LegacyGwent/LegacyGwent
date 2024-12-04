using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using System.Linq;
using System;
using Assets.Script.Localization;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;



public class UserCounter : MonoBehaviour
{
    
    [SerializeField] counterHUD counterHUD;
    [SerializeField] displayflag displayflag;
    //-----------------------------------
    private LocalizationService _translator;
    private GwentClientService server;


    public float timer=-0;
    public float interval=-5;
    void Update()
    {
        if (timer<interval)
        {
            timer=timer+Time.deltaTime;
        }
        else
        {
            CountUsers();
            GetUsersInMatchCount();
            GetUsersvsAICount();
            GetUsersInRankedCount();
            GetUsersInCasualCount();
            GetIsCasualQueue();
            GetMatchmakingIsCasualQueue();
            GetMatchmakingUsersInCasualCount();
            GetMatchmakingUsersInRankedCount();
            timer=0;
        }
    }
    
    private async void Start ()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        
    }

    private async void CountUsers()
    {
            int usercount =  await server.GetUserCount();
            // await Task.Delay(5);
            counterHUD.Users = usercount;
            await Task.CompletedTask;
            return;
    }
    
    private async void GetUsersInMatchCount()
    {
            int usercount =  await server.GetUsersInMatchCount();
            // await Task.Delay(5);
            counterHUD.UsersInMatch = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetUsersvsAICount()
    {
            int usercount =  await server.GetUsersvsAICount();
            // await Task.Delay(5);
            counterHUD.UsersvsAI = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetUsersInRankedCount()
    {
            int usercount =  await server.GetUsersInRankedCount();
            // await Task.Delay(5);
            counterHUD.UsersInRanked = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetUsersInCasualCount()
    {
            int usercount =  await server.GetUsersInCasualCount();
            // await Task.Delay(5);
            counterHUD.UsersInCasual = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetIsCasualQueue()
    {
            int usercount =  await server.GetIsCasualQueue();
                Debug.Log(usercount);
            bool iscasualqueue = false;
            Debug.Log(usercount);
            if (usercount == 1) {iscasualqueue = true;}
            else {iscasualqueue = false;}
            // await Task.Delay(5);
            displayflag.IsCasualFlag = iscasualqueue;
            await Task.CompletedTask;
            return;
    }
    private async void GetMatchmakingUsersInRankedCount()
    {
            int usercount =  await server.GetUsersInRankedCount();
            // await Task.Delay(5);
            counterHUD.UsersInRanked = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetMatchmakingUsersInCasualCount()
    {
            int usercount =  await server.GetUsersInCasualCount();
            // await Task.Delay(5);
            counterHUD.UsersInCasual = usercount;
            await Task.CompletedTask;
            return;
    }
    private async void GetMatchmakingIsCasualQueue()
    {
            int usercount =  await server.GetIsCasualQueue();
                Debug.Log(usercount);
            bool iscasualqueue = false;
            Debug.Log(usercount);
            if (usercount == 1) {iscasualqueue = true;}
            else {iscasualqueue = false;}
            // await Task.Delay(5);
            displayflag.IsCasualFlag = iscasualqueue;
            await Task.CompletedTask;
            return;
    }
}