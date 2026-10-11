using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

//Builds a UI hierarchy from a layout exported from the 0.9.24 beta arena prefabs (Resources/Arena/Layout/*.json).
//Every node keeps its beta name, rect, image (sprite from Resources/Arena/Sprites), text and layout group.
public class ArenaLayout
{
    public RectTransform Root { get; private set; }
    private readonly Dictionary<string, RectTransform> _byPath = new Dictionary<string, RectTransform>();
    private readonly Dictionary<string, RectTransform> _byName = new Dictionary<string, RectTransform>();

    public static Font RegularFont => Resources.Load<Font>("FountInfo/PFDinTextCondPro-Regular");
    public static Font BoldFont => Resources.Load<Font>("FountInfo/PFDinTextCondPro-Bold");
    public static Font MediumFont => Resources.Load<Font>("FountInfo/PFDinTextCondPro-Medium");

    public static ArenaLayout Build(string layoutName, Transform parent)
    {
        var asset = Resources.Load<TextAsset>("Arena/Layout/" + layoutName);
        var layout = new ArenaLayout();
        layout.Root = layout.BuildNode(JObject.Parse(asset.text), parent, null);
        return layout;
    }

    //"Content/LeftContainer/LeftContent" (path below the root) or just a unique name
    public RectTransform this[string pathOrName]
    {
        get
        {
            RectTransform rect;
            if (_byPath.TryGetValue(pathOrName, out rect) || _byName.TryGetValue(pathOrName, out rect))
                return rect;
            Debug.LogError($"ArenaLayout: no node '{pathOrName}'");
            return null;
        }
    }

    public Text Text(string pathOrName) => this[pathOrName].GetComponent<Text>();
    public Image Image(string pathOrName) => this[pathOrName].GetComponent<Image>();

    public static Sprite LoadSprite(string name) => Resources.Load<Sprite>("Arena/Sprites/" + name);

    private RectTransform BuildNode(JObject node, Transform parent, string parentPath)
    {
        var name = (string)node["n"];
        var go = new GameObject(name, typeof(RectTransform));
        var rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        //the root itself is not part of the paths
        var path = parentPath == null ? null : (parentPath == "" ? name : parentPath + "/" + name);
        if (path != null)
        {
            if (!_byPath.ContainsKey(path)) _byPath[path] = rect;
            if (!_byName.ContainsKey(name)) _byName[name] = rect;
        }
        ApplyRect(rect, (JObject)node["r"]);
        if (node["c"] != null)
            foreach (JObject comp in node["c"])
                AddComponent(go, comp);
        if (node["k"] != null)
            foreach (JObject child in node["k"])
                BuildNode(child, rect, path ?? "");
        if (node["off"] != null)
            go.SetActive(false);
        return rect;
    }

    private static Vector2 V2(JToken t) => new Vector2((float)t[0], (float)t[1]);

    private static void ApplyRect(RectTransform rect, JObject r)
    {
        rect.anchorMin = V2(r["amin"]);
        rect.anchorMax = V2(r["amax"]);
        rect.pivot = V2(r["pivot"]);
        rect.sizeDelta = V2(r["size"]);
        rect.anchoredPosition = V2(r["pos"]);
        var s = r["scale"];
        //some beta nodes start at scale 0 and are grown by an animation we do not have
        var scale = new Vector3((float)s[0], (float)s[1], (float)s[2]);
        rect.localScale = scale.x == 0 || scale.y == 0 ? Vector3.one : scale;
        var q = r["rot"];
        rect.localRotation = new Quaternion((float)q[0], (float)q[1], (float)q[2], (float)q[3]);
    }

    private static Color ColorOf(JToken t) => t == null || t.Type == JTokenType.Null ? Color.white : new Color((float)t[0], (float)t[1], (float)t[2], (float)t[3]);

    private static void AddComponent(GameObject go, JObject c)
    {
        switch ((string)c["t"])
        {
            case "Image":
            case "RawImage":
                {
                    var image = go.AddComponent<Image>();
                    var spriteName = (string)c["sprite"];
                    if (!string.IsNullOrEmpty(spriteName))
                        image.sprite = LoadSprite(spriteName);
                    image.color = ColorOf(c["color"]);
                    //no sprite: the beta rendered something else here (render textures, masks); keep it invisible
                    if (image.sprite == null)
                        image.color = new Color(1, 1, 1, 0);
                    var type = c["type"] == null || c["type"].Type == JTokenType.Null ? 0 : (int)c["type"];
                    image.type = (Image.Type)type;
                    image.preserveAspect = c["preserve"] != null && c["preserve"].Type != JTokenType.Null && (int)c["preserve"] == 1;
                    image.raycastTarget = c["ray"] != null && c["ray"].Type != JTokenType.Null && (int)c["ray"] == 1;
                    image.fillCenter = c["fill"] == null || c["fill"].Type == JTokenType.Null || (int)c["fill"] == 1;
                    break;
                }
            case "TextMeshProUGUI":
            case "Text":
                {
                    var text = go.AddComponent<Text>();
                    text.font = RegularFont;
                    var size = c["size"] == null || c["size"].Type == JTokenType.Null ? 24 : Mathf.RoundToInt((float)c["size"]);
                    text.fontSize = size;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMaxSize = size;
                    text.resizeTextMinSize = Mathf.Max(8, size / 2);
                    text.alignment = Alignment(c["align"]);
                    text.color = c["color"] == null || c["color"].Type == JTokenType.Null ? new Color(0.24f, 0.17f, 0.10f) : ColorOf(c["color"]);
                    text.horizontalOverflow = HorizontalWrapMode.Wrap;
                    text.verticalOverflow = VerticalWrapMode.Truncate;
                    text.raycastTarget = false;
                    text.text = "";
                    break;
                }
            case "HorizontalLayoutGroup":
            case "VerticalLayoutGroup":
                {
                    HorizontalOrVerticalLayoutGroup group = (string)c["t"] == "HorizontalLayoutGroup"
                        ? (HorizontalOrVerticalLayoutGroup)go.AddComponent<HorizontalLayoutGroup>()
                        : go.AddComponent<VerticalLayoutGroup>();
                    group.spacing = c["spacing"] == null || c["spacing"].Type == JTokenType.Null ? 0 : (float)c["spacing"];
                    group.childAlignment = c["align"] == null || c["align"].Type == JTokenType.Null ? TextAnchor.UpperLeft : (TextAnchor)(int)c["align"];
                    group.childControlWidth = Flag(c["ctrl"], 0);
                    group.childControlHeight = Flag(c["ctrl"], 1);
                    group.childForceExpandWidth = Flag(c["expand"], 0);
                    group.childForceExpandHeight = Flag(c["expand"], 1);
                    group.padding = Padding(c["pad"]);
                    break;
                }
            case "GridLayoutGroup":
                {
                    var grid = go.AddComponent<GridLayoutGroup>();
                    if (c["cell"] != null && c["cell"].Type == JTokenType.Object)
                        grid.cellSize = new Vector2((float)c["cell"]["x"], (float)c["cell"]["y"]);
                    if (c["spacing"] != null && c["spacing"].Type == JTokenType.Object)
                        grid.spacing = new Vector2((float)c["spacing"]["x"], (float)c["spacing"]["y"]);
                    grid.childAlignment = c["align"] == null || c["align"].Type == JTokenType.Null ? TextAnchor.UpperLeft : (TextAnchor)(int)c["align"];
                    if (c["constraint"] != null && c["constraint"].Type != JTokenType.Null && (int)c["constraint"] > 0)
                    {
                        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                        grid.constraintCount = (int)c["constraint"];
                    }
                    grid.padding = Padding(c["pad"]);
                    break;
                }
            case "RectMask2D":
                go.AddComponent<RectMask2D>();
                break;
            case "LayoutElement":
                {
                    var element = go.AddComponent<LayoutElement>();
                    var pref = c["pref"];
                    if (pref != null && pref[0].Type != JTokenType.Null) element.preferredWidth = (float)pref[0];
                    if (pref != null && pref[1].Type != JTokenType.Null) element.preferredHeight = (float)pref[1];
                    break;
                }
            case "CanvasGroup":
                {
                    var group = go.AddComponent<CanvasGroup>();
                    if (c["alpha"] != null && c["alpha"].Type != JTokenType.Null) group.alpha = (float)c["alpha"];
                    break;
                }
        }
    }

    private static bool Flag(JToken pair, int index) =>
        pair != null && pair.Type == JTokenType.Array && pair[index].Type != JTokenType.Null && (int)pair[index] == 1;

    private static RectOffset Padding(JToken pad) =>
        pad == null || pad.Type != JTokenType.Object ? new RectOffset()
            : new RectOffset((int)pad["m_Left"], (int)pad["m_Right"], (int)pad["m_Top"], (int)pad["m_Bottom"]);

    //TextMesh Pro alignment flags -> UGUI TextAnchor
    private static TextAnchor Alignment(JToken token)
    {
        var value = token == null || token.Type == JTokenType.Null ? 514 : (int)token;
        var horizontal = value & 0xFF;
        var vertical = value & 0xFF00;
        var column = (horizontal & 4) != 0 ? 2 : ((horizontal & (2 | 32)) != 0 ? 1 : 0);
        var row = vertical == 256 ? 0 : (vertical == 1024 ? 2 : 1);
        return (TextAnchor)(row * 3 + column);
    }
}
