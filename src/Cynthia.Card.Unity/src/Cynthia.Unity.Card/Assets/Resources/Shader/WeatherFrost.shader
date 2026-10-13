// Biting Frost row effect, rebuilt from the original Gwent board effect (FrostTokenEffect).
// Ice creeps over the row behind an icy wave front; sparkles twinkle along and over its edges and frosty mist drifts.
Shader "Weather/Frost"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _EdgeSoft("EdgeSoft", 2D) = "black" {}
        _Overlay("Overlay", 2D) = "black" {}
        _Grain("Grain", 2D) = "black" {}
        _FrostBase("FrostBase", 2D) = "black" {}
        _DetailMask("DetailMask", 2D) = "black" {}
        _EdgeMask("EdgeMask", 2D) = "black" {}
        _Rim("Rim", 2D) = "black" {}
        _Clusters("Clusters", 2D) = "black" {}
        _Sparkles("Sparkles", 2D) = "black" {}
        _Mist("Mist", 2D) = "black" {}
        _IceWave("IceWave", 2D) = "black" {}
        _WaveMask("WaveMask", 2D) = "black" {}
        _Progress("Progress", Range(0, 1)) = 1
        _TimeOffset("Time Offset", Float) = 0
        // Set from code; declared so they survive copying the material for the front layer.
        _Pad("Padding", Vector) = (0, 0, 0, 0)
        _Seed("Seed", Float) = 0
        _Flip("Flip", Float) = 0
        _AppliedAt("Applied At", Float) = -1000
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // The effect is drawn twice: behind the cards (default) and in front of them (WEATHER_FRONT),
            // so cards stand inside the effect instead of on top of it.
            #pragma multi_compile __ WEATHER_FRONT
            #include "WeatherCommon.cginc"

            sampler2D _EdgeSoft;
            sampler2D _Overlay;
            sampler2D _Grain;
            sampler2D _FrostBase;
            sampler2D _DetailMask;
            sampler2D _EdgeMask;
            sampler2D _Rim;
            sampler2D _Clusters;
            sampler2D _Sparkles;
            sampler2D _Mist;
            sampler2D _IceWave;
            sampler2D _WaveMask;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.25);

                // SemitransparentBackOverlay: faint cold veil.
                float2 lBack = quadLocal(uv, float2(0.5, 0.48), float2(1.0, 0.95), 1);
                layerOver(acc, float3(0.735, 0.744, 1), 0.128 * tex2D(_EdgeSoft, saturate(lBack)).r * edgeFade(lBack, float2(0.03, 0.1)) * reveal);

                // Overlay1: the icy blue edge texture (WeatherDisplayMesh, about the size of the row). The frost quads are centred
                // on our rows; the beta prefab was offset by ~1% for its own board, which shifted the frost to the left here.
                float2 lRow = quadLocalVaried(uv, float2(0.5, 0.5), float2(1.0, 0.98), 1);
                float rowEdge = edgeFade(lRow, float2(0.02, 0.06));
                float4 ice = tex2D(_Overlay, saturate(lRow * float2(1.05, 1.1) + float2(-0.025, -0.05)));
                layerOver(acc, ice.rgb, saturate(ice.a * 1.3) * rowEdge * reveal);

                // Overlay2: static glittering grain along the soft row edge (a bit larger than the row).
                float2 l2 = quadLocalVaried(uv, float2(0.5, 0.5), float2(1.08, 1.13), 1);
                float grain = tex2D(_Grain, l2 * float2(6, 1.25)).r;
                float edge = tex2D(_EdgeMask, saturate(l2)).a * edgeFade(l2, float2(0.03, 0.1));
                layerAdd(acc, float3(0.347, 0.836, 0.963) * grain * edge * 0.3 * reveal);

                // Overlay3 (SmallAccent): fine frost crystals in the lower part of the row only.
                float2 l3 = quadLocalVaried(uv, float2(0.5, 0.34), float2(1.08, 0.81), 1);
                float3 crystals = tex2D(_FrostBase, l3 * float2(7.97, 1)).rgb;
                float detail = tex2D(_DetailMask, saturate(l3)).r * edgeFade(l3, float2(0.03, 0.1));
                layerAdd(acc, float3(0.434, 0.719, 1) * crystals * detail * 1.4 * reveal);

                // Edge spikes and the crystal clusters in the corners.
                float4 rim = tex2D(_Rim, saturate(lRow));
                layerOver(acc, rim.rgb * 1.06, rim.a * rowEdge * reveal);
                float2 lCover = quadLocalVaried(uv, float2(0.5, 0.5), float2(1.03, 1.1), 1);
                float4 clusters = tex2D(_Clusters, saturate(lCover));
                layerOver(acc, clusters.rgb, clusters.a * 0.634 * edgeFade(lCover, float2(0.03, 0.1)) * reveal);

                // FrostAddSparkles (original: 45/s bottom, 24/s top, 0.3-0.6 s): sprites of the sparkle-dot texture,
                // 4.5-7 units, growing from 35%, each at a random angle and spinning up to 90 deg/s, tinted between icy
                // blue and white. Bottom ones blink fully on/off three times (alpha curve); top ones pulse in brightness.
                for (int i = 0; i < 32; i++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(i, t, 0.45, 0.12, seed, seconds);
                    float life = seconds / lerp(0.3, 0.6, rand(seed, 9));
                    alive *= step(life, 1);
                    bool bottom = i < 21;
                    float2 centre = float2(0.03 + seed.x * 0.94, bottom ? lerp(-0.05, 0.31, seed.y) : lerp(0.61, 1.0, seed.y));
                    float size = lerp(4.5, 7, rand(seed, 1)) * UNITS_Y * lerp(0.35, 1, life);
                    float angle = rand(seed, 2) * 6.283 + (rand(seed, 3) * 2 - 1) * 1.571 * seconds;
                    float3 tint = lerp(float3(0.75, 0.95, 1), float3(1, 1, 1), rand(seed, 4));
                    float twinkle = bottom
                        ? curve7(life, float4(0, 0.13, 0.29, 0.46), float4(0, 1, 0, 1), float3(0.65, 0.82, 1), float3(0, 1, 0))
                        : curve7(life, float4(0, 0.12, 0.3, 0.46), float4(0, 1, 0.35, 1), float3(0.65, 0.82, 1), float3(0.27, 1, 0));
                    float4 sp = particleSprite(_Sparkles, uv, centre, size, angle);
                    layerAdd(acc, tint * sp.r * 0.75 * twinkle * alive * reveal);
                }

                // Only the mist drifts over the cards, so their values stay readable.
                // -- in front of the cards --
                // FrostMist (original: 3/s, 4-6.5 s, 27-35 units): big, faint mist sheets drifting slowly sideways and
                // turning, tinted between blue and white; fading in over the first quarter and out over the last fifth.
                for (int j = 0; j < 14; j++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = 5.25;
                    float alive = particleSeconds(j + 40, t, lifetime, 0.3, seed, seconds);
                    float life = seconds / lifetime;
                    float2 centre = float2(-0.05 + seed.x * 1.1 + seconds * 2 * UNITS_X, lerp(0.3, 0.7, seed.y));
                    float size = lerp(27, 35, rand(seed, 1)) * UNITS_Y;
                    float angle = rand(seed, 2) * 6.283 + (rand(seed, 3) * 2 - 1) * 0.122 * seconds;
                    float k = rand(seed, 4);
                    float4 tintA = lerp(float4(0.29, 0.62, 1, 0.12), float4(0.84, 0.99, 1, 0.06), k);
                    float4 mist = particleFlipbookBlend(_Mist, uv, centre, size, angle, float2(1, 4), life * 3);
                    float fade = curve4(life, float4(0, 0.25, 0.8, 1), float4(0, 1, 1, 0));
                    layerAdd(accF, tintA.rgb * mist.a * tintA.a * 2.5 * fade * alive * reveal);
                }

                // WaveFront: icy band riding the sweep while the frost is applied.
                float front = sweepFront(0.25);
                float2 waveUv = float2((uv.x - front) / 0.25 + 0.5, uv.y);
                float waveMask = tex2D(_WaveMask, saturate(waveUv)).a * edgeFade(waveUv, float2(0.1, 0.1));
                float3 waveIce = tex2D(_IceWave, uv * float2(7.97, 1)).rgb;
                layerAdd(accF, float3(0.824, 0.912, 1) * waveIce * waveMask * sweepActive() * 1.5);
            #ifdef WEATHER_FRONT
                return accF * padFade(uv) * IN.color.a;
            #else
                return acc * padFade(uv) * IN.color.a;
            #endif
            }
            ENDCG
        }
    }
}
