// Torrential Rain row effect, rebuilt from the original Gwent board effect (RainTokenEffect).
// A wet blue-grey layer covers the row and spills softly over its edge. Raindrops land at random places:
// a small splash animates and several ripple rings of different sizes spread and fade, like the original particles.
Shader "Weather/Rain"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _EdgeSoft("EdgeSoft", 2D) = "black" {}
        _Wet("Wet", 2D) = "black" {}
        _Normal("Normal", 2D) = "black" {}
        _Drops("Drops", 2D) = "black" {}
        _Ring("Ring", 2D) = "black" {}
        _Glow("Glow", 2D) = "black" {}
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
            sampler2D _Wet;
            sampler2D _Normal;
            sampler2D _Drops;
            sampler2D _Ring;
            sampler2D _Glow;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.25);

                // DarkBottomLayer, larger than the row.
                float2 lDark = quadLocal(uv, float2(0.49, 0.48), float2(1.38, 1.48), 1);
                layerOver(acc, float3(0.735, 0.744, 1), 0.128 * tex2D(_EdgeSoft, saturate(lDark)).r * edgeFade(lDark, float2(0.03, 0.1)) * reveal);

                // -- in front of the cards --
                // WaterEdge: the wet layer is 1.25 x 1.29 times the row; its ragged border fades out beyond the row edge.
                float2 lWet = quadLocalVaried(uv, float2(0.47, 0.5), float2(1.25, 1.29), 1);
                float4 wet = tex2D(_Wet, saturate(lWet));
                layerOver(accF, lerp(float3(0.36, 0.43, 0.43), wet.rgb * 2, 0.2), wet.a * 0.62 * edgeFade(lWet, float2(0.03, 0.1)) * reveal);

                // WaterDistortion: a faint, slow shimmer on the water.
                float2 lDist = quadLocal(uv, float2(0.49, 0.52), float2(1.08, 1.09), 1);
                float2 n = tex2D(_Normal, lDist * float2(0.5 * ROW_ASPECT, 0.15 * 7) + float2(2.23 + t * 0.015, 0)).rg - 0.5;
                layerAdd(accF, float3(0.515, 0.88, 1) * saturate(n.x * 3) * 0.1 * edgeFade(lDist, float2(0.03, 0.1)) * reveal);

                // Raindrops, following the original particle systems: a drop lands about 8 times a second somewhere on the
                // row. Each landing makes
                //  - one impact sprite (0.3-0.5 s, 3-5 units, growing from 29%, spinning up to 300 deg/s, playing its 4 frames),
                //  - five splash droplets thrown out in a cone (0.28-0.42 s, 4.7-6.5 units, growing then shrinking to 47%),
                //  - eight ripple rings (0.3-0.5 s, 15-27 units, growing to full size by 91% of their life, slowly turning).
                float drops = saturate(_Progress * 1.5);
                for (int i = 0; i < 6; i++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(i, t, 0.5, 0.25, seed, seconds) * drops;
                    float2 centre = float2(0.02 + seed.x * 0.96, lerp(0.24, 0.74, seed.y));
                    float2 d = (uv - centre) * float2(ROW_ASPECT, 1);
                    if (alive > 0 && abs(d.x) < 0.7 && abs(d.y) < 0.7)
                    {
                        float impactLife = saturate(seconds / lerp(0.3, 0.5, rand(seed, 1)));
                        float impactSize = lerp(3, 5, rand(seed, 2)) * UNITS_Y * lerp(0.29, 1, impactLife);
                        float impactAngle = (rand(seed, 3) * 2 - 1) * 5.236 * seconds;
                        float2 impactPos = centre - float2(rand(seed, 4) * 4.5 * UNITS_X * seconds, 0);
                        float4 impact = particleFlipbookBlend(_Drops, uv, impactPos, impactSize, impactAngle, float2(1, 4), impactLife * 2.999);
                        float impactAlpha = curve4(impactLife, float4(0.01, 0.09, 0.69, 1), float4(0, 1, 0.85, 0)) * step(impactLife, 0.999);
                        layerAdd(accF, impact.a * impactAlpha * 0.82 * 1.2);

                        for (int k = 0; k < 5; k++)
                        {
                            float splashLife = saturate(seconds / lerp(0.28, 0.42, rand(seed, 10 + k)));
                            float dir = rand(seed, 20 + k) * 6.283;
                            float speed = lerp(1, 3, rand(seed, 30 + k));
                            float2 pos = centre + float2(cos(dir) * UNITS_X, sin(dir) * UNITS_Y * 0.5) * speed * seconds * 3;
                            float size = lerp(4.7, 6.5, rand(seed, 40 + k)) * UNITS_Y * curve4(splashLife, float4(0, 0.81, 1, 1), float4(0, 1, 0.47, 0.47));
                            float angle = (rand(seed, 50 + k) * 2 - 1) * 3.142 + (rand(seed, 60 + k) * 2 - 1) * 4.468 * seconds;
                            float4 drop = particleFlipbookBlend(_Drops, uv, pos, size, angle, float2(1, 4), splashLife * 2.999);
                            float alpha = lerp(0.67, 0.46, rand(seed, 70 + k)) * curve4(splashLife, float4(0, 0.12, 0.46, 0.88), float4(0, 1, 0.79, 0));
                            layerAdd(accF, drop.a * alpha * 0.9);
                        }

                        for (int r = 0; r < 8; r++)
                        {
                            float ringLife = saturate(seconds / lerp(0.3, 0.5, rand(seed, 80 + r)));
                            float size = lerp(15, 27, rand(seed, 90 + r)) * UNITS_Y * max(saturate(ringLife / 0.91), 0.02);
                            float angle = (rand(seed, 100 + r) * 2 - 1) * 0.349 + (rand(seed, 110 + r) * 2 - 1) * 2.443 * seconds;
                            float4 ring = particleFlipbookBlend(_Ring, uv, centre, size, angle, float2(1, 2), ringLife * 0.999);
                            float alpha = lerp(0.42, 0.27, rand(seed, 120 + r)) * curve4(ringLife, float4(0, 0.1, 0.54, 1), float4(0, 1, 0.9, 0)) * step(ringLife, 0.999);
                            layerAdd(accF, ring.a * alpha * 1.6);
                        }
                    }
                }
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
