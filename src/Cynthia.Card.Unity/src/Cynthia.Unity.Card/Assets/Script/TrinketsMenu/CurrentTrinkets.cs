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
        var user = _clientService?.User;
        if (user == null) return;
        if (PlayerName != null) PlayerName.text = user.PlayerName;
        mmr = user.MMR;
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
        if (RankIcon == null) return;
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
        var user = _clientService?.User;
        if (user == null) return;
        var currentavatar = user.CurrentAvatar;
        if (!string.IsNullOrEmpty(currentavatar) && currentavatar != OldAvatar)
        {
            var op = Addressables.LoadAssetAsync<Sprite>(currentavatar);
            Sprite go = op.WaitForCompletion();
            if (AvatarArt != null) AvatarArt.sprite = go;
            OldAvatar = currentavatar;
        }
        var currentborder = user.CurrentBorder;
        if (!string.IsNullOrEmpty(currentborder) && currentborder != OldBorder)
        {
            var op = Addressables.LoadAssetAsync<Sprite>(currentborder);
            Sprite go = op.WaitForCompletion();
            if (BorderArt != null) BorderArt.sprite = go;
            OldBorder = currentborder;
        }
        var currenttitle = user.CurrentTitle;
        var title = _titles.FirstOrDefault(x => x.ID == currenttitle);
        if (currenttitle != OldTitle)
        {
            if (title == null || string.IsNullOrEmpty(currenttitle))
            {
                if (TitleText != null) TitleText.text = string.Empty;
            }
            else
            {
                if (TitleText != null) TitleText.text = _translator.GetText(currenttitle + "Name");
                Color titleColor;
                if (mycolormap.TryGetValue(title.TitleColor, out titleColor))
                {
                    if (TitleText != null) TitleText.color = titleColor;
                }
            }
            OldTitle = currenttitle;
        }
        if (MMR != null)
        {
            MMR.text = user.MMR.ToString();
        }
        mmr = user.MMR;
        if (mmr != OldMMR)
        {
            OldMMR = mmr;
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
