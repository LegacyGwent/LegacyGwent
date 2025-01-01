using System.Collections;
using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

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

    private void SetCardInfo(int strength, string name, int count = 1, Group group = Group.Gold, string artid = "15230800")
    {
        Border.sprite = (group == Group.Gold ? Gold : (group == Group.Silver ? Silver : Copper));
        Strength.text = strength.ToString();
        Name.text = name;

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
        else
        {
            Addressables.LoadAssetAsync<Sprite>("15230800").Completed += (obj) =>
            {
                Miniature.sprite = obj.Result;
            };
        }
        if (strength <= 0)
        {
            Star.gameObject.SetActive(true);
            Strength.gameObject.SetActive(false);
            Star.sprite = (group == Group.Gold ? GoldStar : (group == Group.Silver ? SilverStar : CopperStar));
        }
        if (count > 1)
        {
            Count.SetActive(true);
            CountText.text = $"x{count.ToString()}";
        }
    }
    public void SetCardInfo(CardStatus card, int count = 1)
    {
        CardStatus = card;
        SetCardInfo(CardStatus.Strength, CardStatus.Name, count, CardStatus.Group, CardStatus.CardArtsId);
    }
    public void SetCardInfo(string id, int count = 1)
    {
        var translator = DependencyResolver.Container.Resolve<LocalizationService>();
        CardStatus = new CardStatus(id);
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
