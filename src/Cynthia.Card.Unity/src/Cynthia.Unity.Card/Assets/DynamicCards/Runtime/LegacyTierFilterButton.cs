using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // UGUI adapter for the original client's TierTemplate views and sprite states.
    public sealed class LegacyTierFilterButton : Button
    {
        public Image InnerBackground, Icon;
        public GameObject Hovered, Down, Selected;
        public Sprite[] TierIcons;
        public Color[] TierColors;

        public void SetTier(int index)
        {
            Icon.sprite = TierIcons[index];
            Icon.rectTransform.localScale = Vector3.one * .8f;
            InnerBackground.color = TierColors[index];
        }

        public void SetSelected(bool selected) { Selected.SetActive(selected); }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (Hovered != null) Hovered.SetActive(state == SelectionState.Highlighted);
            if (Down != null) Down.SetActive(state == SelectionState.Pressed);
        }
    }
}
