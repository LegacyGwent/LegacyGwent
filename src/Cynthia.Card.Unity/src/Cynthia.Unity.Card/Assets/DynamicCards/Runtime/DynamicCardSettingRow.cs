using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    public sealed class DynamicCardSettingRow : MonoBehaviour
    {
        private ChoseValue choice;
        private Text label;
        private bool refreshing;
        public static void Install(GameObject template)
        {
            if (template == null) return;
            // QualityPanel points to the value selector, while its parent owns the label and divider.
            var templateRow = template.transform.parent;
            if (templateRow.parent.Find("DynamicCardsOption") != null) return;
            var row = Instantiate(templateRow.gameObject, templateRow.parent);
            row.name = "DynamicCardsOption";
            var choice = row.GetComponentInChildren<ChoseValue>(true);
            // Replace the event object: RemoveAllListeners alone leaves serialized quality callbacks.
            choice.onValueChanged = new ChoseValue.ChoseValueEvent();
            var controller = row.AddComponent<DynamicCardSettingRow>();
            controller.choice = choice;
            foreach (var text in row.GetComponentsInChildren<Text>(true))
                if (!text.transform.IsChildOf(choice.transform) && text.text.Length > 1)
                { controller.label = text; break; }
            choice.onValueChanged.AddListener(index =>
            {
                if (!controller.refreshing) DynamicCardSettings.Quality = (DynamicCardQuality)index;
            });
            var rect = (RectTransform)row.transform;
            if (row.transform.parent.GetComponent<LayoutGroup>() == null)
            {
                float bottom = 0;
                foreach (RectTransform sibling in row.transform.parent)
                    if (sibling != rect) bottom = Mathf.Min(bottom, sibling.anchoredPosition.y - sibling.rect.height * (1 - sibling.pivot.y));
                rect.anchoredPosition = new Vector2(((RectTransform)templateRow).anchoredPosition.x, bottom - rect.rect.height * rect.pivot.y - 8);
                var parent = row.transform.parent as RectTransform;
                if (parent != null) parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(parent.rect.height, -bottom + rect.rect.height + 24));
            }
            controller.RefreshLabels();
        }
        private void OnEnable()
        {
            TextLocalization.LanguageChanged += RefreshLabels;
            DynamicCardSettings.Changed += RefreshLabels;
            if (choice != null) RefreshLabels();
        }
        private void OnDisable()
        {
            TextLocalization.LanguageChanged -= RefreshLabels;
            DynamicCardSettings.Changed -= RefreshLabels;
        }
        private void RefreshLabels()
        {
            if (choice == null) return;
            refreshing = true;
            try
            {
                choice.ChoseList = new List<string> { "Settings_Off", "Settings_AnimatedCardsLow", "Settings_AnimatedCardsMedium", "Settings_AnimatedCardsHigh" };
                if (label != null) label.text = LocalizedLabel.Get("Settings_AnimatedCardQuality");
                choice.Index = (int)DynamicCardSettings.Quality;
            }
            finally { refreshing = false; }
        }
    }
}
