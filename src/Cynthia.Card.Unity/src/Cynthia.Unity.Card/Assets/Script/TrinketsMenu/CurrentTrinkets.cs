using System.Collections;
using System.Collections.Generic;
using System;
using Cynthia.Card;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Autofac;
using Cynthia.Card.Client;
using Assets.Script.Localization;
using UnityEngine.Events;
using Cynthia.Card.Common.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class CurrentTrinkets : MonoBehaviour // this scripts updates the avatar/border/id in the profile whenever they are changed
{
    private GwentClientService _clientService;
    private LocalizationService _translator;
    public Image AvatarArt;
    public Image BorderArt;
    public Image RankIcon;
    public Text TitleText;
    public Text PlayerName;
    public Text MMR;
    private int mmr;
    private string OldAvatar;
    private string OldBorder;
    private string OldTitle;
    private int OldMMR;
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); } // lists all title cosmetics
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; } // stores the color of the title cosmetics
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    private void Start()
    {
        PlayerName.text = _clientService.User.PlayerName;
        mmr = _clientService.User.MMR;
        OldMMR = mmr;
        if (SceneManager.GetSceneByName("GamePlay").isLoaded == true || RankIcon == null)
        {
            return;
        }

        if (MMR != null)
        {
            MMR.text = mmr.ToString();
        }
        SwitchRankIcon(mmr);

    }

    private void SwitchRankIcon(int mymmr)
    {
        string rank = GetRankIcon(mymmr);
        var op = Addressables.LoadAssetAsync<Sprite>(rank);
        Sprite go = op.WaitForCompletion();
        RankIcon.sprite = go;
    }

    public void Update()
    {
        if (SceneManager.GetSceneByName("GamePlay").isLoaded == true)
        {
            return;
        }
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        var currentavatar = _clientService.User.CurrentAvatar;
        if (currentavatar != OldAvatar)
        {
            var op = Addressables.LoadAssetAsync<Sprite>(currentavatar);
            Sprite go = op.WaitForCompletion();
            AvatarArt.sprite = go;
            OldAvatar = currentavatar;
        }
        var currentborder = _clientService.User.CurrentBorder;
        if (currentborder != OldBorder)
        {
            var op = Addressables.LoadAssetAsync<Sprite>(currentborder);
            Sprite go = op.WaitForCompletion();
            BorderArt.sprite = go;
            OldBorder = currentborder;
        }
        var user = _clientService.User;
        var currenttitle = user.CurrentTitle;
        string color = _titles.Where(x => x.ID == currenttitle).Single().TitleColor;
        if (currentborder != OldTitle)
        {
            TitleText.text = _translator.GetText(currenttitle + "Name");
            TitleText.color = mycolormap[color];
            OldTitle = currenttitle;
        }
        if (MMR != null)
        {
            MMR.text = user.MMR.ToString();
        }
        if (mmr != OldMMR)
        {
            OldMMR = user.MMR;
            SwitchRankIcon(mmr);
        }
    }
    
    static public string GetRankIcon(int mmr)
    {
        string rank;
        switch (mmr)
        {
            case int i when i < 3450:
                rank = "rank_1";
                break;
            case int i when i >= 3450 && i < 3500:
                rank = "rank_2";
                break;
            case int i when i >= 3500 && i < 3550:
                rank = "rank_3";
                break;
            case int i when i >= 3550 && i < 3600:
                rank = "rank_4";
                break;
            case int i when i >= 3600 && i < 3650:
                rank = "rank_5";
                break;
            case int i when i >= 3650 && i < 3700:
                rank = "rank_6";
                break;
            case int i when i >= 3700 && i < 3750:
                rank = "rank_7";
                break;
            case int i when i >= 3750 && i < 3800:
                rank = "rank_8";
                break;
            case int i when i >= 3800 && i < 3850:
                rank = "rank_9";
                break;
            case int i when i >= 3850 && i < 3900:
                rank = "rank_10";
                break;
            case int i when i >= 3900 && i < 3950:
                rank = "rank_11";
                break;
            case int i when i >= 3950 && i < 4000:
                rank = "rank_12";
                break;
            case int i when i >= 4000 && i < 4050:
                rank = "rank_13";
                break;
            case int i when i >= 4050 && i < 4100:
                rank = "rank_14";
                break;
            case int i when i >= 4100 && i < 4150:
                rank = "rank_15";
                break;
            case int i when i >= 4150 && i < 4200:
                rank = "rank_16";
                break;
            case int i when i >= 4200 && i < 4250:
                rank = "rank_17";
                break;
            case int i when i >= 4250 && i < 4300:
                rank = "rank_18";
                break;
            case int i when i >= 4300 && i < 4350:
                rank = "rank_19";
                break;
            case int i when i >= 4350 && i < 4400:
                rank = "rank_20";
                break;
            default:
                rank = "rank_21";
                break;
        }
        return rank;
    }
}
