using System;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    public static class DynamicCardSettings
    {
        internal const string LegacyPreference = "DynamicCards.Enabled";
        internal const string QualityPreference = "DynamicCards.Quality";
        public static event Action Changed;

        public static DynamicCardQuality Quality
        {
            get
            {
                int legacy = PlayerPrefs.GetInt(LegacyPreference, 0) == 1
                    ? (int)DynamicCardQuality.High : (int)DynamicCardQuality.Off;
                int stored = PlayerPrefs.GetInt(QualityPreference, legacy);
                return stored >= 0 && stored <= (int)DynamicCardQuality.High
                    ? (DynamicCardQuality)stored : (DynamicCardQuality)legacy;
            }
            set
            {
                if (value < DynamicCardQuality.Off || value > DynamicCardQuality.High)
                    throw new ArgumentOutOfRangeException("value");
                bool changed = Quality != value;
                if (!changed && PlayerPrefs.HasKey(QualityPreference)) return;
                PlayerPrefs.SetInt(QualityPreference, (int)value);
                PlayerPrefs.SetInt(LegacyPreference, value == DynamicCardQuality.Off ? 0 : 1);
                PlayerPrefs.Save();
                if (changed && Changed != null) Changed();
            }
        }

        // Compatibility for existing availability/ownership checks and old callers.
        // Enabling an already enabled tier must not silently promote Medium/Low to High.
        public static bool Enabled
        {
            get { return Quality != DynamicCardQuality.Off; }
            set
            {
                if (Enabled == value) return;
                Quality = value ? DynamicCardQuality.High : DynamicCardQuality.Off;
            }
        }
    }
}
