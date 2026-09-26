using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // Original ForwardGlitchSettings field names are retained for imported animation curves.
    public sealed class DynamicCardGlitch : DynamicCardPostEffect
    {
        public float speed = 1, intensity = 1, staticIntensity = 1, Delay = .01f;
        public Texture2D StaticNoise;

        protected override void Configure(Material target)
        {
            target.SetTexture("_Glitch", StaticNoise);
            target.SetFloat("_Speed", speed);
            target.SetFloat("_Intensity", intensity);
            target.SetFloat("_GlitchVisibility", staticIntensity);
        }
    }
}
