using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using Cynthia.Card.Common.Models;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using Autofac;
using Cynthia.Card.Client;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine.Events;
using static UnityEngine.UI.Scrollbar;
using Cynthia.Card.Common.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class RewardTableElement : MonoBehaviour
{
    public Text ConditionText;
    public Image AvatarImage;
    public Image BorderImage;
    public Image TitleImage;
    public Text TitleText;
    public GameObject RewardsTable;
    public GameObject TitleRewardPrefab;
    private IList<TrinketAvatar> _avatars { get => TrinketMap.GetAvatars().ToList(); }
    private IList<Border> _borders { get => TrinketMap.GetBorders().ToList(); }
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); }
    private LocalizationService _translator;
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; }
    private IList<string> Avatar_rewards;
    private IList<string> Border_rewards;
    private IList<string> Titles_rewards;

    void Awake()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    

    public async Task<bool> GenerateReward(SeasonReward reward)
    {
        TitleImage.enabled = false;
        AvatarImage.enabled = false;
        BorderImage.enabled = false;
        TitleText.enabled = false;
        
        if (reward.minimalPosition == null)
            return false;

        ConditionText.text = reward.minimalPosition.ToString();
        if (reward.avatar != null)
            GenerateAvatar(reward.avatar);
        if (reward.border != null)
            GenerateBorder(reward.border);
        if (reward.title != null)
            GenerateTitle(reward.title);
        return true;
    }

    private void GenerateAvatar(string avatar_id)
    {
        AvatarImage.enabled = true;
        var op = Addressables.LoadAssetAsync<Sprite>(avatar_id);
        Sprite avatar_img = op.WaitForCompletion();
        AvatarImage.sprite = avatar_img;
    }

    private void GenerateBorder(string border_id)
    {
        BorderImage.enabled = true;
        var op = Addressables.LoadAssetAsync<Sprite>(border_id);
        Sprite border_img = op.WaitForCompletion();
        BorderImage.sprite = border_img;
    }

    private void GenerateTitle(string title_id)
    {
        TitleImage.enabled = true;
        TitleText.enabled = true;
        TitleText.text = _translator.GetText(title_id + "Name");
        TitleText.color = mycolormap[_titles.FirstOrDefault(x => x.ID == title_id)?.TitleColor];
        
    }
    
}
