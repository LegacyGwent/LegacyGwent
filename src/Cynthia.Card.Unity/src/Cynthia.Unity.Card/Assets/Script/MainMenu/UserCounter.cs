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
    //-----------------------------------   
//     public Text MainMenu_PlayersOnlineText;
//     public Text MainMenu_PlayingPvpText;
//     public Text MainMenu_PlayingvsAIText;
//     public Text MainMenu_PlayingCasualText;
//     public Text MainMenu_PlayingRankText;
//     public Text MainMenu_CasualFlagText;
//     public Text MainMenu_RankFlagText;
//     public Text Matchmaking_Menu_CasualText;
//     public Text Matchmaking_Menu_RankText;
//     public Text Matchmaking_Menu_CasualFlagText;
//     public Text Matchmaking_Menu_RankFlagText;
    private float timer=0;
    private float interval=1;
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
            GetIsRankQueue();
            GetMatchmakingIsCasualQueue();
            GetMatchmakingUsersInCasualCount();
            GetMatchmakingUsersInRankedCount();
            timer=0;
        }
    }
    
    private async void Start ()
    {
        // _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        // MainMenu_PlayersOnlineText.text = _translator.GetText("MainMenu_PlayersOnlineText");
        // MainMenu_PlayingPvpText.text = _translator.GetText("MainMenu_PlayingPvpText");
        // MainMenu_PlayingvsAIText.text = _translator.GetText("MainMenu_PlayingvsAIText");
        // MainMenu_PlayingCasualText.text = _translator.GetText("MainMenu_PlayingCasualText");
        // MainMenu_PlayingRankText.text = _translator.GetText("MainMenu_PlayingRankText");
        // MainMenu_CasualFlagText.text = _translator.GetText("MainMenu_CasualFlagText");
        // MainMenu_RankFlagText.text = _translator.GetText("MainMenu_RankFlagText");
        // Matchmaking_Menu_CasualFlagText.text = _translator.GetText("Matchmaking_Menu_CasualFlagText");
        // Matchmaking_Menu_RankFlagText.text = _translator.GetText("Matchmaking_Menu_RankFlagText");
        // Matchmaking_Menu_CasualText.text = _translator.GetText("Matchmaking_Menu_CasualText");
        // Matchmaking_Menu_RankText.text = _translator.GetText("Matchmaking_Menu_RankText");   
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
            bool iscasualqueue = false;
            if (usercount == 1) {iscasualqueue = true;}
            else {iscasualqueue = false;}
            // await Task.Delay(5);
            displayflag.IsCasualFlag = iscasualqueue;
            await Task.CompletedTask;
            return;
    }
        private async void GetIsRankQueue()
    {
            int usercount =  await server.GetIsRankQueue();
            bool isrankqueue = false;
            if (usercount == 1) {isrankqueue = true;}
            else {isrankqueue = false;}
            // await Task.Delay(5);
            displayflag.IsRankFlag = isrankqueue;
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
            bool iscasualqueue = false;
            if (usercount != 1) {iscasualqueue =false;}
            else {iscasualqueue = true;}
            // await Task.Delay(5);
            if (iscasualqueue)
           { displayflag.IsCasualFlag = iscasualqueue;
            await Task.CompletedTask;}
            return;
    }
    private async void GetMatchmakingIsRankQueue()
    {
            int usercount =  await server.GetIsRankQueue();
            bool isrankqueue = false;
            if (usercount != 1) {isrankqueue =false;}
            else {isrankqueue = true;}
            if (isrankqueue)
            // await Task.Delay(5);
            {displayflag.IsRankFlag = isrankqueue;
            await Task.CompletedTask;}
            return;
    }
}
