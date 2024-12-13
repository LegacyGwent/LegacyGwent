using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;

public class counterHUD : MonoBehaviour
{
    [SerializeField] Text usersText;
    [SerializeField] Text usersinmatchText;
    [SerializeField] Text usersvsaiText;
    [SerializeField] Text usersinrankedText;
    [SerializeField] Text usersincasualText;
    [SerializeField] Text inmatchusersText;
    [SerializeField] Text inmatchusersinrankText;
    [SerializeField] Text inmatchusersincasualText;
    public Text MainMenu_PlayersOnlineText;
    public Text MainMenu_PlayingPvpText;
    public Text MainMenu_PlayingvsAIText;
    public Text MainMenu_PlayingCasualText;
    public Text MainMenu_PlayingRankText;
    public Text MainMenu_CasualFlagText;
    public Text MainMenu_RankFlagText;
    public Text Matchmaking_Menu_CasualText;
    public Text Matchmaking_Menu_RankText;
    public Text Matchmaking_Menu_CasualFlagText;
    public Text Matchmaking_Menu_RankFlagText;
    public Text Matchmaking_Menu_PlayersOnlineText;
    private LocalizationService _translator;
    int points = 0;
    int usersinmatch = 0;
    int usersvsai = 0;
    int usersinranked = 0;
    int usersincasual = 0;
    int inmatchusers = 0;
    int inmatchusersinrank = 0;    
    int inmatchusersincasual = 0;

    private async void Start ()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        MainMenu_PlayersOnlineText.text = _translator.GetText("MainMenu_PlayersOnlineText");
        MainMenu_PlayingPvpText.text = _translator.GetText("MainMenu_PlayingPvpText");
        MainMenu_PlayingvsAIText.text = _translator.GetText("MainMenu_PlayingvsAIText");
        MainMenu_PlayingCasualText.text = _translator.GetText("MainMenu_PlayingCasualText");
        MainMenu_PlayingRankText.text = _translator.GetText("MainMenu_PlayingRankText");
        MainMenu_CasualFlagText.text = _translator.GetText("MainMenu_CasualFlagText");
        MainMenu_RankFlagText.text = _translator.GetText("MainMenu_RankFlagText");
        Matchmaking_Menu_CasualFlagText.text = _translator.GetText("Matchmaking_Menu_CasualFlagText");
        Matchmaking_Menu_RankFlagText.text = _translator.GetText("Matchmaking_Menu_RankFlagText");
        Matchmaking_Menu_CasualText.text = _translator.GetText("Matchmaking_Menu_CasualText");
        Matchmaking_Menu_RankText.text = _translator.GetText("Matchmaking_Menu_RankText");
        Matchmaking_Menu_PlayersOnlineText.text = _translator.GetText("Matchmaking_Menu_PlayersOnlineText"); 
    }
    private void Awake ()
    {
        UpdateHUD();
    }
    public int Users
    {
        get {return points;}
        set {
            points = value;
            UpdateHUD();
            }
    }
    public int UsersInMatch
    {
        get {return usersinmatch;}
        set {
            usersinmatch = value;
            UpdateHUD();
            }
    }
    public int UsersvsAI
    {
        get {return usersvsai;}
        set {
            usersvsai = value;
            UpdateHUD();
            }
    }
    public int UsersInRanked
    {
        get {return usersinranked;}
        set {
            usersinranked = value;
            UpdateHUD();
            }
    }
    public int UsersInCasual
    {
        get {return usersincasual;}
        set {
            usersincasual = value;
            UpdateHUD();
            }
    }
    //     public int InMatchUsers
    // {
    //     get {return inmatchusers;}
    //     set {
    //         inmatchusers = value;
    //         UpdateHUD();
    //         }
    // }
    //     public int InMatchUsersInCasual
    // {
    //     get {return inmatchusersincasual;}
    //     set {
    //         inmatchusersincasual = value;
    //         UpdateHUD();
    //         }
    // }
    //     public int InMatchUsersInRank
    // {
    //     get {return inmatchusersinrank;}
    //     set {
    //         inmatchusersinrank = value;
    //         UpdateHUD();
    //         }
    // }
    private void UpdateHUD()
    {
        usersText.text = points.ToString ();
        usersinmatchText.text = usersinmatch.ToString ();
        usersvsaiText.text = usersvsai.ToString();
        usersinrankedText.text = usersinranked.ToString();
        usersincasualText.text = usersincasual.ToString();
        inmatchusersText.text = points.ToString ();
        inmatchusersinrankText.text = usersinranked.ToString();
        inmatchusersincasualText.text = usersincasual.ToString();
    }
    

}
