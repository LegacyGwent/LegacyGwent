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
    
    static public string GetRankIcon(float mmr)
    {
        string rank;
        if (mmr <= 3400)
        {
            return "rank_1";
        }
        int ranknumber = (int)Math.Truncate(mmr / 50 - 67);
        rank = "rank_" + ranknumber.ToString();
        return rank;
    }
}
