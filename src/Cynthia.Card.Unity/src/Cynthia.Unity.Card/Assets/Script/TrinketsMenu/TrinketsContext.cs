using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cynthia.Card.Client;
using Cynthia.Card;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.SceneManagement;
using System.Linq;


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
    public Text Progress;
    public GameObject ProgresObject;
    public Image AvatarArt;
    public Material LightGray;
    public GameObject SetAvatarButton;
    public GameObject SetBorderButton;
    public GameObject SetTitleButton;
    public GameObject TitlesBackground;
    private string avatarID;
    private string borderID;
    private string titleID;
    private IList<TrinketAvatar> _avatars { get => TrinketMap.GetAvatars().ToList(); } // lists all avatar cosmetics
    private IList<Border> _borders { get => TrinketMap.GetBorders().ToList(); } // lists all title cosmetics
    private IList<Title> _titles { get => TrinketMap.GetTitles().ToList(); } // lists all title cosmetics
    
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        
    }

    public void SetTrinketArt(string trinket, string trinkettype)
    {
        if (OwnedText != null) // avoid triggering the update when in prefab mode
        {
            LocalizedLabel.Set(OwnedText, "TrinketsMenu_TrinketOwned");
        }
        if (trinkettype == "OwnedAvatars")
        {

            if (SetAvatarButton != null) // do not trigger in prefab mode
            {
                LocalizedLabel.Set(AvatarsButtonText, "TrinketsMenu_SetAvatarButton");
                SetAvatarButton.SetActive(true);
                SetBorderButton.SetActive(false);
                if (!_clientService.User.OwnedAvatars.Contains(trinket))
                {
                    AvatarArt.material = LightGray;
                    SetAvatarButton.SetActive(false);
                    LocalizedLabel.Set(OwnedText, "TrinketsMenu_TrinketNotOwned");
                }
            }
            
            avatarID = trinket;
        }
        if (trinkettype == "OwnedBorders")
        {
            if (SetBorderButton != null) // do not trigger in prefab mode
                {
                    LocalizedLabel.Set(BordersButtonText, "TrinketsMenu_SetBorderButton");
                    SetAvatarButton.SetActive(false);
                    SetBorderButton.SetActive(true);
                    if (!_clientService.User.OwnedBorders.Contains(trinket))
                        {
                            AvatarArt.material = LightGray;
                            SetBorderButton.SetActive(false);
                            LocalizedLabel.Set(OwnedText, "TrinketsMenu_TrinketNotOwned");
                        }
                }
            borderID = trinket;
        }
        var displayId = trinkettype == "OwnedAvatars"
            ? TrinketMap.ResolveAvatar(trinket).ID : TrinketMap.ResolveBorder(trinket).ID;
        var op = Addressables.LoadAssetAsync<Sprite>(displayId);
        Sprite go = op.WaitForCompletion();
        AvatarArt.sprite = go;        
    }
    public void SetTitleLook(string title, Color color)
    {
        if (OwnedText != null) // avoid triggering the update when in prefab mode
        {
            LocalizedLabel.Set(OwnedText, "TrinketsMenu_TrinketOwned");
            SetTitleButton.SetActive(true);
            LocalizedLabel.Set(TitlesButtonText, "TrinketsMenu_SetTitleButton");
            SetAvatarButton.SetActive(false);
            if (!_clientService.User.OwnedTitles.Contains(title))
                {
                    TitlesBackground.GetComponent<Image>().material = LightGray;
                    SetTitleButton.SetActive(false);
                    LocalizedLabel.Set(OwnedText, "TrinketsMenu_TrinketNotOwned");
                }
        }
        LocalizedLabel.Set(TitleText, title+"Name");
        TitleText.color= color;
        TitlesBackground.SetActive(true);
        AvatarArt.gameObject.SetActive(false);
        titleID = title;
    }
    public void SetAvatarContext(string avatar) // set name, decription and if necessary, the progress towards unlock
    {
        LocalizedLabel.Set(AvatarName, avatar+"Name");
        LocalizedLabel.Set(AvatarsContext, avatar+"Description");
        var definition = TrinketMap.ResolveAvatar(avatar);
        SetUnlockProgress(definition.UnlockStat, definition.UnlockCounter);
    }
    public void SetBorderContext(string border) // set name, decription and if necessary, the progress towards unlock
    {
        LocalizedLabel.Set(AvatarName, border+"Name");
        LocalizedLabel.Set(AvatarsContext, border+"Description");
        var definition = TrinketMap.ResolveBorder(border);
        SetUnlockProgress(definition.UnlockStat, definition.UnlockCounter);
    }
    public void SetTitleContext(string title) // set name, decription and if necessary, the progress towards unlock
    {
        LocalizedLabel.Set(AvatarName, title+"Name");
        LocalizedLabel.Set(AvatarsContext, title+"Description");
        var definition = TrinketMap.ResolveTitle(title);
        SetUnlockProgress(definition.UnlockStat, definition.UnlockCounter);
    }
    // Selection panels have progress widgets; unlock notifications intentionally omit them.
    private void SetUnlockProgress(string stat, int counter)
    {
        if (ProgresObject == null || Progress == null)
            return;
        ProgresObject.SetActive(stat != null);
        if (stat != null)
            Progress.text = _clientService.User[stat] + "/" + counter;
    }

    // When the SetAvatarButton is clicked, set the current avatar of the user
    public async void SetAvatar()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateAvatar(_clientService.User.PlayerName, avatarID);
        _clientService.User.CurrentAvatar = avatarID;
    }
    // When the SetBorderButton is clicked, set the current border of the user
    public async void SetBorder()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateBorder(_clientService.User.PlayerName, borderID);
        _clientService.User.CurrentBorder = borderID;
    }
    // When the SetTitleButton is clicked, set the current title of the user
    public async void SetTitle()
    {   
        if (SceneManager.GetSceneByName("Game").isLoaded == false)
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().UpdateTitle(_clientService.User.PlayerName, titleID);
        _clientService.User.CurrentTitle = titleID;
    }
}
