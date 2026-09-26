using Cynthia.Card;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    public static class CardCopyBadge
    {
        // Only the deck editor's premium copper cards display their existing counter.
        public static bool ShowsCount(CardStatus card) => ClientContent.HasPremiumContent && card != null &&
            card.Group == Group.Copper && card.IsPremium == true;

        public static void Apply(CardShowInfo view, int count)
        {
            var core = view.GetComponent<EditorUICoreCard>();
            if (core != null) return; // EditorUICoreCard.Count owns its existing counter.
            var counter = view.transform.Find("CountIcon");
            if (counter != null) counter.gameObject.SetActive(false);
        }

        private static readonly Dictionary<Sprite, Dictionary<Group, Material>> BorderMaterials =
            new Dictionary<Sprite, Dictionary<Group, Material>>();

        public static void StyleBorder(Image border, bool premium, Group group = Group.Gold)
        {
            premium = premium && ClientContent.HasPremiumContent;
            if (border == null) return;
            var child = border.transform.Find("PremiumEdge");
            if (child == null && premium)
            {
                var rect = PremiumCollectionPanel.Rect("PremiumEdge", border.transform, new Vector2(.5f, .5f), Vector2.zero, Vector2.zero);
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
                var edge = rect.gameObject.AddComponent<Image>();
                edge.raycastTarget = false;
                child = rect;
            }
            if (child != null)
            {
                child.gameObject.SetActive(premium);
                var edge = child.GetComponent<Image>(); edge.sprite = border.sprite; edge.type = border.type;
                edge.preserveAspect = border.preserveAspect;
                if (premium && border.sprite != null) edge.material = BorderMaterial(border.sprite, group);
            }
        }

        private static Material BorderMaterial(Sprite sprite, Group group)
        {
            if (!BorderMaterials.TryGetValue(sprite, out var variants))
                BorderMaterials[sprite] = variants = new Dictionary<Group, Material>();
            if (variants.TryGetValue(group, out var material) && material != null) return material;
            material = new Material(Resources.Load<Shader>("PremiumCrafting/LegacyPremiumBorder"))
                { name = "Original premium border " + group, hideFlags = HideFlags.HideAndDontSave };
            material.SetTexture("_NoiseTex", Resources.Load<Texture2D>("PremiumCrafting/Noise27White"));
            material.SetTexture("_SecondTex", Resources.Load<Texture2D>("PremiumCrafting/PortalNoise"));
            material.SetTexture("_ShineTex", Resources.Load<Texture2D>("PremiumCrafting/CardShineTex"));
            material.SetVector("_SpriteUV", UnityEngine.Sprites.DataUtility.GetOuterUV(sprite));
            material.SetColor("_GlitterTint", group == Group.Copper ? new Color(.5882353f,.3289607f,.2422145f) :
                group == Group.Silver ? new Color(.5375882f,.6710634f,.7254902f) : new Color(.7254902f,.4941177f,.1960784f));
            material.SetColor("_ShineTint", group == Group.Copper ? new Color(1,.6473866f,.5294118f,.09803922f) :
                group == Group.Silver ? new Color(.709f,.9518344f,1,.09803922f) : new Color(1,.8344828f,.2941176f,.09803922f));
            variants[group] = material;
            return material;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetMaterials()
        {
            foreach (var variants in BorderMaterials.Values)
                foreach (var material in variants.Values) if (material != null) Object.Destroy(material);
            BorderMaterials.Clear();
        }
    }
}
