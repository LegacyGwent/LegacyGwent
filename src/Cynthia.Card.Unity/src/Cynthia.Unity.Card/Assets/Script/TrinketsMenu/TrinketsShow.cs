using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cynthia.Card.Client;
using Assets.Script.Localization;
using Autofac;

public class TrinketsShow : MonoBehaviour // this script controls the avatar trinket prefab in the trinket select
{
    private GwentClientService _clientService;
    private LocalizationService _translator;
    public Image AvatarArt;
    public Image BorderArt;
    public Image TitlesBackground;
    public Text TitleText;
    public Color titleColor;
    private string trinketID; // stores the trinket ID to be used for the TrinketsContext script
    private GameObject TrinketContext;
    public GameObject TrinketContextPrefab;
    public Material LightGray;
    private static Dictionary<string, Color> mycolormap { get => ColorMap.colormap; }
    
    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();

    }

    public void AvatarClicked()
    {
        // remove previous AvatarContextPrefabs then instantiates the new prefab, then changes image and context
        foreach (Transform child in transform.root)
        {
            if (child.name.Contains("Prefab"))
            {
                Destroy(child.gameObject);
            }
        }
        TrinketContext = Instantiate(TrinketContextPrefab, Vector3.zero, Quaternion.identity, transform.root);
        TrinketContext.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedAvatars"); // sets the art in the preview
        TrinketContext.GetComponent<TrinketsContext>().SetAvatarContext(trinketID);
    }
    public void BorderClicked()
    {
        // remove previous BorderContextPrefabs then instantiates the new prefab, then changes image and context
        foreach (Transform child in transform.root)
        {
            if (child.name.Contains("Prefab"))
            {
                Destroy(child.gameObject);
            }
        }
        TrinketContext = Instantiate(TrinketContextPrefab, Vector3.zero, Quaternion.identity, transform.root);
        TrinketContext.GetComponent<TrinketsContext>().SetTrinketArt(trinketID, "OwnedBorders"); // sets the art in the preview
        TrinketContext.GetComponent<TrinketsContext>().SetBorderContext(trinketID);
    }

    public void TitleClicked()
    {
        // remove previous Prefabs then instantiates the new prefab, then changes image and context
        foreach (Transform child in transform.root)
        {
            if (child.name.Contains("Prefab"))
            {
                Destroy(child.gameObject);
            }
        }
        TrinketContext = Instantiate(TrinketContextPrefab, Vector3.zero, Quaternion.identity, transform.root);
        TrinketContext.GetComponent<TrinketsContext>().SetTitleLook(trinketID, titleColor); // sets the look in the preview
        TrinketContext.GetComponent<TrinketsContext>().SetTitleContext(trinketID);
    }

    public void SetAvatarArt(string avatar) // set the art of the avatar in the avatar list
    {
        if (!_clientService.User.OwnedAvatars.Contains(avatar))
        {
            AvatarArt.material = LightGray;
        }
        trinketID = avatar;
        var op = Addressables.LoadAssetAsync<Sprite>(avatar);
        Sprite go = op.WaitForCompletion();
        AvatarArt.sprite = go;
    }
    public void SetBorderArt(string border) // set the art of the border in the border list
    {
        if (!_clientService.User.OwnedBorders.Contains(border))
        {
            BorderArt.material = LightGray;
        }
        trinketID = border;
        if (trinketID != "NoBorder")
        {
            var op = Addressables.LoadAssetAsync<Sprite>(border);
            Sprite go = op.WaitForCompletion();
            BorderArt.sprite = go;
        }
    }
    public void SetTitleList (string title, string color) // set the look of the title in the title list
    {
        if (!_clientService.User.OwnedTitles.Contains(title))
        {
            TitlesBackground.material = LightGray;
        }
        TitleText.text = _translator.GetText(title+"Name");
        TitleText.color = mycolormap[color];
        trinketID = title;
        titleColor = mycolormap[color];
    }
}
