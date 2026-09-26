using System;
using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // Build-owned capability, deliberately separate from the player's quality preference.
    public static class ClientContent
    {
        [Serializable] private class Manifest { public int schema; public string variant; }
        private static bool? premium;
        public static bool HasPremiumContent
        {
            get
            {
#if UNITY_EDITOR
                return true; // Source authoring supports previews independent of package selection.
#else
                if (!premium.HasValue)
                {
                    var asset = Resources.Load<TextAsset>("ClientContent");
                    var manifest = asset == null ? null : JsonUtility.FromJson<Manifest>(asset.text);
                    premium = manifest != null && manifest.schema == 1 && manifest.variant == "premium";
                }
                return premium.Value;
#endif
            }
        }

        public static bool CanAnimate => HasPremiumContent &&
            (!Application.isMobilePlatform || SystemInfo.graphicsShaderLevel >= 35);

        public static DeckModel ForServer(DeckModel deck)
        {
            if (deck == null || HasPremiumContent) return deck;
            // Absence preserves server-side premium choices. An empty dictionary would clear them.
            return new DeckModel { Id = deck.Id, Name = deck.Name, Leader = deck.Leader,
                Deck = new List<string>(deck.Deck) };
        }
    }
}
