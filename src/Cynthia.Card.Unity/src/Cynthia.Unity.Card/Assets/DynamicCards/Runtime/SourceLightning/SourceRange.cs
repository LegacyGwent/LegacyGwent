namespace Assets.Script.DynamicCards.SourceLightning
{
    internal static class SourceRange
    {
        internal static float Remap(this float value,float low,float high,float targetLow,float targetHigh)
        { return (value-low)/(high-low)*(targetHigh-targetLow)+targetLow; }
    }
}
