namespace Assets.Script.DynamicCards
{
    // Persisted values: do not reorder. Independent of Unity's global QualitySettings.
    public enum DynamicCardQuality { Off = 0, Low = 1, Medium = 2, High = 3 }

    public sealed class DynamicCardQualityProfile
    {
        public readonly int CardResolution, PreviewResolution;
        public readonly int CardFramesPerSecond, PreviewFramesPerSecond;
        public readonly int PostProcessDownsample;

        private DynamicCardQualityProfile(int cardResolution, int previewResolution,
            int cardFramesPerSecond, int previewFramesPerSecond, int postProcessDownsample)
        {
            CardResolution = cardResolution; PreviewResolution = previewResolution;
            CardFramesPerSecond = cardFramesPerSecond; PreviewFramesPerSecond = previewFramesPerSecond;
            PostProcessDownsample = postProcessDownsample;
        }

        private static readonly DynamicCardQualityProfile Off = new DynamicCardQualityProfile(0, 0, 0, 0, 1);
        private static readonly DynamicCardQualityProfile Low = new DynamicCardQualityProfile(192, 512, 10, 30, 2);
        private static readonly DynamicCardQualityProfile Medium = new DynamicCardQualityProfile(256, 768, 15, 30, 2);
        // Zero preview FPS means every frame, exactly as before quality presets existed.
        private static readonly DynamicCardQualityProfile High = new DynamicCardQualityProfile(384, 1024, 24, 0, 1);

        public static DynamicCardQualityProfile Get(DynamicCardQuality quality)
        {
            switch (quality)
            {
                case DynamicCardQuality.Off: return Off;
                case DynamicCardQuality.Low: return Low;
                case DynamicCardQuality.Medium: return Medium;
                default: return High;
            }
        }

        public int Resolution(bool preview) { return preview ? PreviewResolution : CardResolution; }
        public int FramesPerSecond(bool preview) { return preview ? PreviewFramesPerSecond : CardFramesPerSecond; }
    }
}
