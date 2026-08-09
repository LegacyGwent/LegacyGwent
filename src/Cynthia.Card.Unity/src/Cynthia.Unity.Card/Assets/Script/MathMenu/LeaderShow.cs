using Autofac;
using Cynthia.Card;
using System.Collections.Generic;
using Assets.Script.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class LeaderShow : MonoBehaviour
{
    public Text Streng;
    public Text Name;
    public Image CardShow;
    public Image Title;
    public Image Miniature;
    public Sprite NorthernreaIcon;
    public Sprite ScoiataelIcon;
    public Sprite MonsterIcon;
    public Sprite SkelligeIcon;
    public Sprite NilfgaardIcon;
    private IDictionary<Faction, Sprite> _groupIconMap;
    public string CurrentId { get; private set; } = null;
    public void Start()
    {
        _groupIconMap = new Dictionary<Faction, Sprite>
         {
             {Faction.NorthernRealms,NorthernreaIcon},
             {Faction.ScoiaTael,ScoiataelIcon},
             {Faction.Monsters,MonsterIcon},
             {Faction.Skellige,SkelligeIcon},
             {Faction.Nilfgaard,NilfgaardIcon},
         };
    }
    public void SetLeader(string id)
    {
        CurrentId = id;
        if (_groupIconMap == null) Start();

        var card = GwentMap.CardMap[id];
        var translator = DependencyResolver.Container.Resolve<LocalizationService>();
        card.Name = translator.GetCardName(id);
        card.Info = translator.GetCardInfo(id);

        Name.text = card.Name;
        Streng.text = card.Strength.ToString();
        Title.sprite = _groupIconMap[card.Faction];
        string miniatureid = card.CardArtsId + "_slot";
        var op = Addressables.LoadAssetAsync<Sprite>(miniatureid);
        Sprite go = op.WaitForCompletion();
        Miniature.sprite = go;
        // Leader slot containers are wider than the 4:1 source sprites.  Keeping
        // the source aspect ratio leaves visible side bars inside the frame.
        Miniature.preserveAspect = false;
        LeaderSlotClip.Apply(Miniature);
        // Addressables.LoadAssetAsync<Sprite>(miniatureid).Completed += (obj) =>
        // {
        //     Miniature.sprite = obj.Result;
        // };
    }
}

public static class LeaderSlotClip
{
    private const float BorderInset = 3f;

    public static void Apply(Image image)
    {
        if (image == null) return;

        var artRect = image.rectTransform;
        if (artRect.parent != null && artRect.parent.name == "LeaderSlotClip") return;

        var frameRect = artRect.parent as RectTransform;
        if (frameRect == null) return;

        var worldCorners = new Vector3[4];
        artRect.GetWorldCorners(worldCorners);
        var artMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        var artMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        foreach (var worldCorner in worldCorners)
        {
            var localCorner = frameRect.InverseTransformPoint(worldCorner);
            artMin = Vector2.Min(artMin, localCorner);
            artMax = Vector2.Max(artMax, localCorner);
        }

        var frameBounds = frameRect.rect;
        var clipMin = new Vector2(
            Mathf.Max(artMin.x, frameBounds.xMin + BorderInset),
            Mathf.Max(artMin.y, frameBounds.yMin + BorderInset));
        var clipMax = new Vector2(
            Mathf.Min(artMax.x, frameBounds.xMax - BorderInset),
            Mathf.Min(artMax.y, frameBounds.yMax - BorderInset));
        if (clipMax.x <= clipMin.x || clipMax.y <= clipMin.y) return;

        var originalSibling = artRect.GetSiblingIndex();
        var clipObject = new GameObject("LeaderSlotClip", typeof(RectTransform), typeof(RectMask2D));
        clipObject.layer = image.gameObject.layer;
        var clipRect = clipObject.GetComponent<RectTransform>();
        clipRect.SetParent(frameRect, false);
        clipRect.SetSiblingIndex(originalSibling);
        clipRect.anchorMin = new Vector2(.5f, .5f);
        clipRect.anchorMax = new Vector2(.5f, .5f);
        clipRect.pivot = new Vector2(.5f, .5f);
        clipRect.anchoredPosition = (clipMin + clipMax) * .5f;
        clipRect.sizeDelta = clipMax - clipMin;

        // Preserve the authored crop and let the clip window remove only pixels
        // that cross the frame's inner boundary.
        artRect.SetParent(clipRect, true);
    }
}
