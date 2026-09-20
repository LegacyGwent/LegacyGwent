using System.Collections;
using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ListCardShowInfo : MonoBehaviour
{
    public Text Strength;
    public Text Name;
    public GameObject Count;
    public Image Star;
    public Text CountText;
    public Image Border;
    public Image Miniature;
    public Sprite Copper;
    public Sprite Silver;
    public Sprite Gold;
    public Sprite CopperStar;
    public Sprite SilverStar;
    public Sprite GoldStar;
    public CardStatus CardStatus;
    private Sprite _croppedMiniature;
    private AsyncOperationHandle<Sprite>? _fullArtHandle;
    private int miniatureRequest;

    private void SetCardInfo(int strength, string name, int count = 1, Group group = Group.Gold, string artid = "15230800")
    {
        var request = ++miniatureRequest;
        // Deck rows use the original static _slot artwork, even when full cards animate.
        var animatedView = Miniature.GetComponent<Assets.Script.DynamicCards.DynamicCardView>();
        if (animatedView != null)
        {
            animatedView.enabled = false;
            Destroy(animatedView);
        }
        Border.sprite = Resources.Load<Sprite>("PremiumCrafting/dp_slot_" + (group == Group.Gold ? "gold" : group == Group.Silver ? "silver" : "bronze"))
            ?? (group == Group.Gold ? Gold : (group == Group.Silver ? Silver : Copper));
        Assets.Script.DynamicCards.CardCopyBadge.StyleBorder(Border, CardStatus?.IsPremium == true, group);
        Strength.text = strength.ToString();
        Name.text = name;

        if (_croppedMiniature != null)
        {
            Destroy(_croppedMiniature);
            _croppedMiniature = null;
        }
        if (_fullArtHandle.HasValue)
        {
            Addressables.Release(_fullArtHandle.Value);
            _fullArtHandle = null;
        }

        if (AssetExists(artid + "_slot"))
        {
            var op = Addressables.LoadAssetAsync<Sprite>(artid + "_slot");
            Sprite go = op.WaitForCompletion();
            Miniature.sprite = go;
            // Addressables.LoadAssetAsync<Sprite>(artid + "_slot").Completed += (obj) =>
            // {
            //     Miniature.sprite = obj.Result;
            // };
        }
        else if ((artid == "c10001100" || artid == "d19330000" || artid == "d19860000") && AssetExists(artid))
        {
            // These original atlas textures have no separate miniature. Use a
            // sprite rectangle over the original pixels, with the usual 8:1 ratio.
            _fullArtHandle = Addressables.LoadAssetAsync<Sprite>(artid);
            var fullArt = _fullArtHandle.Value.WaitForCompletion();
            var top = artid == "c10001100" ? 110 : (artid == "d19860000" ? 220 : 86);
            _croppedMiniature = Sprite.Create(fullArt.texture,
                new Rect(0, fullArt.texture.height - top - 62, 496, 62),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
            Miniature.sprite = _croppedMiniature;
        }
        else
        {
            Addressables.LoadAssetAsync<Sprite>("15230800").Completed += (obj) =>
            {
                if (this == null || Miniature == null || request != miniatureRequest || obj.Result == null) return;
                Miniature.sprite = obj.Result;
            };
        }
        Star.gameObject.SetActive(strength <= 0);
        Strength.gameObject.SetActive(strength > 0);
        if (strength <= 0)
        {
            Star.gameObject.SetActive(true);
            Strength.gameObject.SetActive(false);
            Star.sprite = (group == Group.Gold ? GoldStar : (group == Group.Silver ? SilverStar : CopperStar));
        }
        Count.SetActive(Assets.Script.DynamicCards.CardCopyBadge.ShowsCount(CardStatus));
        CountText.text = $"x{count}";
    }
    private void OnDestroy()
    {
        if (_croppedMiniature != null) Destroy(_croppedMiniature);
        if (_fullArtHandle.HasValue) Addressables.Release(_fullArtHandle.Value);
    }
    public void SetCardInfo(CardStatus card, int count = 1)
    {
        CardStatus = card;
        SetCardInfo(CardStatus.Strength, CardStatus.Name, count, CardStatus.Group, CardStatus.CardArtsId);
    }
    public void SetCardInfo(string id, int count = 1, bool premium = false)
    {
        var translator = DependencyResolver.Container.Resolve<LocalizationService>();
        CardStatus = new CardStatus(id) { IsPremium = premium };
        CardStatus.Name = translator.GetCardName(id);
        CardStatus.Info = translator.GetCardInfo(id);
        SetCardInfo(CardStatus.Strength, CardStatus.Name, count, CardStatus.Group, CardStatus.CardArtsId);
    }
    private static bool AssetExists(object key) {
            if (Application.isPlaying) {
                foreach (var l in Addressables.ResourceLocators) {
                    IList<IResourceLocation> locs;
                    if (l.Locate(key, null, out locs))
                        return true;
                }
                return false;
            }
//             else if (Application.isEditor && !Application.isPlaying) {
// #if UNITY_EDITOR
//                 // note: my keys are always asset file paths
//                 return FileExists(System.IO.Path.Combine(Application.dataPath, (string)key));
// #endif
//             }
            return false;
        }
}
