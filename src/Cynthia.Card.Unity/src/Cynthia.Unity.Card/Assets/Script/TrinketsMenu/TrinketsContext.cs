using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cynthia.Card.Client;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.SceneManagement;

public class TrinketsContext : MonoBehaviour // this script generates a prefab of the show/select trinket menu on the righthand panel
{
    private GwentClientService _clientService;
    private LocalizationService _translator;
    public Text AvatarName;
    public Text AvatarsContext;
    public Text AvatarsButtonText;
    public Text BordersButtonText;
    public Text TitlesButtonText;
    public Text OwnedText;
    public Text TitleText;
    public Image AvatarArt;
    public Material LightGray;
    public GameObject SetAvatarButton;
    public GameObject SetBorderButton;
    public GameObject SetTitleButton;
    public GameObject TitlesBackground;
    private string avatarID;
    private string borderID;
    private string titleID;
    
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        
    }

    public void SetTrinketArt(string trinket, string trinkettype)
    {
        OwnedText.text = _translator.GetText("TrinketsMenu_TrinketOwned");
        if (trinkettype == "OwnedAvatars")
        {
            AvatarsButtonText.text = _translator.GetText("TrinketsMenu_SetAvatarButton");
            SetAvatarButton.SetActive(true);
            SetBorderButton.SetActive(false);
            if (!_clientService.User.OwnedAvatars.Contains(trinket))
                {
                    AvatarArt.material = LightGray;
                    SetAvatarButton.SetActive(false);
                    OwnedText.text = _translator.GetText("TrinketsMenu_TrinketNotOwned");
                }
            avatarID = trinket;
        }
        if (trinkettype == "OwnedBorders")
        {
            BordersButtonText.text = _translator.GetText("TrinketsMenu_SetBorderButton");
            SetAvatarButton.SetActive(false);
            SetBorderButton.SetActive(true);
            if (!_clientService.User.OwnedBorders.Contains(trinket))
                {
                    AvatarArt.material = LightGray;
                    SetBorderButton.SetActive(false);
                    OwnedText.text = _translator.GetText("TrinketsMenu_TrinketNotOwned");
                }
            borderID = trinket;
        }
        var op = Addressables.LoadAssetAsync<Sprite>(trinket);
        Sprite go = op.WaitForCompletion();
        AvatarArt.sprite = go;        
    }
    public void SetTitleLook(string title, Color color)
    {
        Debug.Log(title);
        OwnedText.text = _translator.GetText("TrinketsMenu_TrinketOwned");
        SetTitleButton.SetActive(true);
        TitlesButtonText.text = _translator.GetText("TrinketsMenu_SetTitleButton");
        TitleText.text = _translator.GetText(title+"Name");
        TitleText.color= color;
        TitlesBackground.SetActive(true);
        AvatarArt.gameObject.SetActive(false);
        SetAvatarButton.SetActive(false);
        
        
        if (!_clientService.User.OwnedTitles.Contains(title))
        {
            TitlesBackground.GetComponent<Image>().material = LightGray;
            SetTitleButton.SetActive(false);
            OwnedText.text = _translator.GetText("TrinketsMenu_TrinketNotOwned");
        }
        titleID = title;
    }
    public void SetTrinketContext(string avatar)
    {
        AvatarName.text = _translator.GetText(avatar+"Name");
        AvatarsContext.text = _translator.GetText(avatar+"Description");
    }
    // When the SetAvatarButton is clicked, set the current avatar of the user
    public async void SetAvatar()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateAvatar(_clientService.User.UserName, avatarID);
        _clientService.User.CurrentAvatar = avatarID;
    }
    // When the SetBorderButton is clicked, set the current border of the user
    public async void SetBorder()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateBorder(_clientService.User.UserName, borderID);
        _clientService.User.CurrentBorder = borderID;
    }
    // When the SetTitleButton is clicked, set the current title of the user
    public async void SetTitle()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateTitle(_clientService.User.UserName, titleID);
        _clientService.User.CurrentTitle = titleID;
    }
}
