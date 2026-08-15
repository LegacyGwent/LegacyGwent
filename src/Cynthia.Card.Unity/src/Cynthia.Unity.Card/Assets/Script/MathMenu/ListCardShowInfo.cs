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

public static class RuleCardListVisual
{
    private static readonly Color32 RuleInk = new Color32(239, 230, 201, 255);
    private static readonly Color32 RuleTeal = new Color32(21, 67, 66, 245);
    private static readonly Color32 RuleGold = new Color32(220, 174, 78, 255);

    public static void Apply(GameObject row, Font font, bool english)
    {
        if (row == null || row.transform.Find("RuleCardIdentity") != null) return;
        var info = row.GetComponent<ListCardShowInfo>();
        if (info != null)
        {
            if (info.Border != null) info.Border.color = new Color32(128, 213, 190, 255);
            if (info.Name != null)
            {
                info.Name.color = RuleInk;
                if (info.Name.GetComponent<Outline>() == null)
                {
                    var outline = info.Name.gameObject.AddComponent<Outline>();
                    outline.effectColor = new Color32(0, 14, 16, 220);
                    outline.effectDistance = new Vector2(1, -1);
                }
            }
            if (info.Count != null) info.Count.SetActive(false);
        }

        var identity = new GameObject("RuleCardIdentity", typeof(RectTransform));
        identity.transform.SetParent(row.transform, false);
        var identityRect = identity.GetComponent<RectTransform>();
        identityRect.anchorMin = Vector2.zero;
        identityRect.anchorMax = Vector2.one;
        identityRect.offsetMin = Vector2.zero;
        identityRect.offsetMax = Vector2.zero;
        identityRect.SetAsLastSibling();

        AddLine(identity.transform, "TopRuleLine", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -1), new Vector2(0, 2));
        AddLine(identity.transform, "BottomRuleLine", Vector2.zero, new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 2));

        var plaque = new GameObject("RulePlaque", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
        plaque.transform.SetParent(identity.transform, false);
        var plaqueRect = plaque.GetComponent<RectTransform>();
        plaqueRect.anchorMin = new Vector2(1, .5f);
        plaqueRect.anchorMax = new Vector2(1, .5f);
        plaqueRect.pivot = new Vector2(1, .5f);
        plaqueRect.anchoredPosition = new Vector2(-5, 0);
        plaqueRect.sizeDelta = new Vector2(54, 29);
        plaque.GetComponent<Image>().color = RuleTeal;
        plaque.GetComponent<Image>().raycastTarget = false;
        var plaqueOutline = plaque.GetComponent<Outline>();
        plaqueOutline.effectColor = RuleGold;
        plaqueOutline.effectDistance = new Vector2(1, -1);

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
        labelObject.transform.SetParent(plaque.transform, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(3, 1);
        labelRect.offsetMax = new Vector2(-3, -1);
        var label = labelObject.GetComponent<Text>();
        label.font = font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = english ? 11 : 14;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = RuleInk;
        label.text = english ? "RULE" : "规则";
        label.raycastTarget = false;
        var labelOutline = labelObject.GetComponent<Outline>();
        labelOutline.effectColor = new Color32(0, 12, 13, 230);
        labelOutline.effectDistance = new Vector2(1, -1);
    }

    private static void AddLine(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        var line = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        line.transform.SetParent(parent, false);
        var rect = line.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var image = line.GetComponent<Image>();
        image.color = RuleGold;
        image.raycastTarget = false;
    }
}
