using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // Per-view state: no changes to shared card textures or shared UI materials.
    public sealed class PremiumCardAppearance : MonoBehaviour
    {
        private readonly Dictionary<Graphic, Material> originals = new Dictionary<Graphic, Material>();
        private Material grayscale;
        private string cardId;
        private bool waitingForPremium;
        public bool IsLocked { get; private set; }

        public static void Apply(Image art, CardStatus card, params Graphic[] frame)
        {
            if (art == null) return;
            var appearance = art.GetComponent<PremiumCardAppearance>();
            bool locked = card != null && card.IsPremium == true && !card.IsCardBack && !card.Conceal &&
                !UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePlay").isLoaded &&
                !PremiumCollectionClient.Owns(card.CardId);
            if (appearance == null && !locked) return;
            if (appearance == null) appearance = art.gameObject.AddComponent<PremiumCardAppearance>();
            var dynamic = art.GetComponent<DynamicCardView>();
            bool keep = appearance.cardId == card?.CardId && appearance.IsLocked && !locked &&
                dynamic != null && dynamic.KeepStaticDuringUpgrade && !dynamic.IsPresentationReady;
            appearance.cardId = card?.CardId;
            appearance.waitingForPremium = keep;
            if (locked || keep)
            {
                appearance.SetGray(art);
                foreach (var graphic in frame) appearance.SetGray(graphic);
                appearance.IsLocked = true;
            }
            else appearance.Restore();
        }

        private void SetGray(Graphic graphic)
        {
            if (graphic == null || originals.ContainsKey(graphic)) return;
            if (grayscale == null) grayscale = new Material(Resources.Load<Shader>("PremiumCrafting/LockedPremium"));
            originals.Add(graphic, graphic.material);
            graphic.material = grayscale;
        }
        private void LateUpdate()
        {
            if (!waitingForPremium) return;
            var view = GetComponent<DynamicCardView>();
            if (view == null || view.IsPresentationReady) Restore();
        }
        private void Restore()
        {
            foreach (var pair in originals) if (pair.Key != null) pair.Key.material = pair.Value;
            originals.Clear(); IsLocked = false; waitingForPremium = false;
        }
        private void OnDestroy() { Restore(); if (grayscale != null) Destroy(grayscale); }
    }
}
