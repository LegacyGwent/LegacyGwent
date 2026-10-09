// Blood Moon / Full Moon row effect, rebuilt from the original Gwent board effect (BloodMoon, Moonlight).
// A moon rises over the top edge of the row in a coloured night sky with slowly drifting clouds.
// Blood Moon and Full Moon share this shader; WeatherMaterials sets their colours and the moon position.
Shader "Weather/Moon"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _EdgeSoft("EdgeSoft", 2D) = "black" {}
        _SoftGlow("SoftGlow", 2D) = "black" {}
        _Blur("Blur", 2D) = "black" {}
        _Clouds("Clouds", 2D) = "black" {}
        _Moon("Moon", 2D) = "black" {}
        _Gradient("Gradient", 2D) = "black" {}
        _FogMask("FogMask", 2D) = "black" {}
        _Star("Star", 2D) = "black" {}
        _MoonX("MoonX", Float) = 0.5
        _GlowOffset("GlowOffset", Float) = 0
        _GlowMaskOffset("GlowMaskOffset", Float) = 0
        _TintColor("TintColor", Color) = (1, 1, 1, 1)
        _GlowColor("GlowColor", Color) = (1, 1, 1, 1)
        _CloudDark("CloudDark", Color) = (1, 1, 1, 1)
        _CloudLit("CloudLit", Color) = (1, 1, 1, 1)
        _MoonColor("MoonColor", Color) = (1, 1, 1, 1)
        _TopColor("TopColor", Color) = (1, 1, 1, 1)
        _StarColor("StarColor", Color) = (1, 1, 1, 1)
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
            sampler2D _SoftGlow;
            sampler2D _Blur;
            sampler2D _Clouds;
            sampler2D _Moon;
            sampler2D _Gradient;
            sampler2D _FogMask;
            sampler2D _Star;
            float _MoonX;
            float _GlowOffset;
            float _GlowMaskOffset;
            float4 _TintColor;
            float4 _GlowColor;
            float4 _CloudDark;
            float4 _CloudLit;
            float4 _MoonColor;
            float4 _TopColor;
            float4 _StarColor;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.35);
                float fade = rowFade(uv);

                // BackTint: the moon's colour cast over the row.
                float2 lTint = quadLocal(uv, float2(0.5, 0.49), float2(1.07, 1.06), 0);
                layerOver(acc, _TintColor.rgb, 0.18 * edgeFade(lTint, float2(0.03, 0.1)) * reveal);

                // BackBigGlow / FrontSmallGlow: big soft glows around the moon (glow and mask offsets follow the moon).
                float2 lGlow = quadLocal(uv, float2(0.5, 0.47), float2(1.03, 1.47), 1);
                float glow = tex2D(_SoftGlow, saturate(lGlow * float2(1.6, 1) + float2(_GlowOffset, -0.28))).a;
                float glowMask = tex2D(_Blur, saturate(lGlow * float2(1.15, 1) + float2(_GlowMaskOffset, 0.03))).r;
                layerAdd(acc, _GlowColor.rgb * glow * glowMask * edgeFade(lGlow, float2(0.03, 0.1)) * 1.4 * reveal);

                // CloudsFakePerspective: two cloud layers drifting slowly through the upper part of the row.
                float cloudsA = tex2D(_Clouds, float2(uv.x * 1.2 + t * 0.008, uv.y * 0.7 + 0.25)).a;
                float cloudsB = tex2D(_Clouds, float2(uv.x * 1.6 + t * 0.012, uv.y * 0.7 + 0.6)).a;
                float sky = smoothstep(0.2, 1, uv.y) * fade;
                layerOver(acc, _CloudDark.rgb, cloudsA * sky * 0.6 * reveal);
                layerAdd(acc, _CloudLit.rgb * cloudsB * sky * 0.45 * reveal);

                // Moon: the original quad and tiling, so the moon peeks over the top edge; it rises during the intro.
                float rise = lerp(0.35, 0, smoothstep(0, 1, _Progress));
                float2 lMoon = quadLocal(uv + float2(0, rise), float2(_MoonX, 0.55), float2(0.31, 1.22), 1);
                float2 moonUv = lMoon * float2(3, 1.5) + float2(-0.5, -0.86);
                float4 moon = tex2D(_Moon, saturate(moonUv)) * insideQuad(moonUv);
                float moonFade = tex2D(_EdgeSoft, saturate(lMoon + float2(0, 0.14))).r;
                layerOver(acc, moon.rgb * _MoonColor.rgb * 1.6, moon.a * moonFade * _MoonColor.a);

                // -- in front of the cards --
                // CardTopGlow: light from the top edge (upper 76% of the row).
                float2 lTop = quadLocal(uv, float2(0.5, 0.77), float2(1.02, 0.76), 1);
                float topGlow = tex2D(_Gradient, saturate(float2(0.5, 1 - lTop.y))).r * tex2D(_EdgeSoft, saturate(lTop)).r * edgeFade(lTop, float2(0.03, 0.1));
                layerAdd(accF, _TopColor.rgb * topGlow * 1.4 * reveal);

                // CardBotDarkGlow: the bottom of the row sinks into darkness.
                float2 lBot = quadLocal(uv, float2(0.5, 0.45), float2(1.42, 1.04), 1);
                float bottomDark = tex2D(_FogMask, saturate(lBot)).a * saturate(1 - lBot.y * 1.6) * edgeFade(lBot, float2(0.03, 0.1));
                layerDarken(accF, bottomDark * 0.45 * reveal);

                // -- behind the cards --
                // Stars (Full Moon only; original: big 2/s and small 8/s, 3 s each, growing in and twinkling).
                for (int i = 0; i < 6; i++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(i, t, 3, 0.1, seed, seconds);
                    float life = seconds / 3;
                    float size = 1 * UNITS_Y * curve4(life, float4(0, 0.3, 0.81, 1), float4(0, 1, 1, 0)) * 2.5;
                    float4 star = particleSprite(_Star, uv, float2(0.02 + seed.x * 0.67, lerp(0.5, 0.95, seed.y)), size, rand(seed, 1) * 6.283);
                    float alpha = curve4(life, float4(0, 0.32, 0.6, 1), float4(0, 1, 0, 0)) + curve4(life, float4(0.6, 0.83, 1, 1), float4(0, 0.49, 0, 0));
                    layerAdd(acc, _StarColor.rgb * 2 * star.a * alpha * 2 * alive * reveal);
                }
                for (int j = 0; j < 20; j++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(j + 10, t, 3, 0.1, seed, seconds);
                    float life = seconds / 3;
                    float4 star = particleSprite(_Star, uv, float2(0.02 + seed.x * 0.67, lerp(0.22, 0.67, seed.y)), lerp(0.6, 0.9, rand(seed, 1)) * UNITS_Y * 2.5, rand(seed, 2) * 6.283);
                    float alpha = curve4(life, float4(0, 0.61, 1, 1), float4(0, 1, 0, 0));
                    layerAdd(acc, _StarColor.rgb * 0.45 * 2 * star.a * alpha * 2 * alive * reveal);
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
