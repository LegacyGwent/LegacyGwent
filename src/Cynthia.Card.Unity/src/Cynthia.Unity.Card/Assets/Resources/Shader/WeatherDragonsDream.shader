// Dragon's Dream row effect, rebuilt from the original Gwent board effect (RowEffect_DragonsDreamSmoke).
// Green toxic smoke fills the row, glowing wisps curl slowly through it and green flames flicker below.
// When applied, a smoke dragon head flies across the row.
Shader "Weather/DragonsDream"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Mask("Mask", 2D) = "black" {}
        _Wispy("Wispy", 2D) = "black" {}
        _NoiseAlpha("NoiseAlpha", 2D) = "black" {}
        _Smoke("Smoke", 2D) = "black" {}
        _Flame("Flame", 2D) = "black" {}
        _Head("Head", 2D) = "black" {}
        _HeadFlow("HeadFlow", 2D) = "black" {}
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

            sampler2D _Mask;
            sampler2D _Wispy;
            sampler2D _NoiseAlpha;
            sampler2D _Smoke;
            sampler2D _Flame;
            sampler2D _Head;
            sampler2D _HeadFlow;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.3);

                // Background layers are mirrored in both directions in the original.
                float2 l = quadLocal(uv, float2(0.5, 0.48), float2(1.07, 1.02), 1);
                l.y = 1 - l.y;
                float quadEdge = edgeFade(l, float2(0.03, 0.12));
                float mask = tex2D(_Mask, saturate(l)).r * quadEdge;

                // BackgroundDark: dark green smoke (original speeds).
                float dark = tex2D(_Wispy, l * float2(2, 0.5) + float2(0, 1) + t * float2(0.05, 0.07) * 0.5).r;
                float darkNoise = tex2D(_NoiseAlpha, l * float2(1, 0.25) + t * float2(0.1, 0.05) * 0.5).r;
                layerOver(acc, float3(0.01, 0.09, 0.05), saturate(0.2 + dark * darkNoise * 1.4) * mask * 0.75 * reveal);

                // BackgroundBright: glowing green wisps.
                float bright = tex2D(_Wispy, l * float2(1.6, 0.5) + t * float2(0.08, 0.03) * 0.5).r;
                layerAdd(acc, float3(0.109, 0.493, 0.4) * bright * mask * 1.6 * reveal);

                // -- in front of the cards --
                // Part01: long smoke trails drifting along the row (0.9 x row height).
                float2 l1 = quadLocal(uv, float2(0.5, 0.49), float2(1.07, 0.9), 1);
                float smoke = tex2D(_Smoke, l1 * float2(0.5 * 2, 1) + float2(t * 0.33 * 0.15, 0)).a * edgeFade(l1, float2(0.03, 0.15));
                layerAdd(accF, float3(0.247, 1, 0.782) * smoke * 0.25 * reveal);

                // Part03: green flames, in the lower part of the row.
                float2 l3 = quadLocal(uv, float2(0.5, 0.45), float2(1.07, 1.2), 1);
                float4 flame = tex2D(_Flame, l3 * float2(3, 3) * float2(1, 0.4) + float2(t * 0.2 * 0.3, -t * 0.07 * 0.5));
                layerAdd(accF, float3(0.334, 1, 0.421) * pow(flame.a, 1.5) * smoothstep(0.5, 0, uv.y) * edgeFade(l3, float2(0.03, 0.1)) * 1.1 * reveal);

                // Glow: green light from below.
                layerAdd(accF, float3(0.303, 1, 0.715) * smoothstep(0.75, 0, uv.y) * rowFade(uv) * 0.15 * reveal);

                // SparksLoop (original: 40/s in gusts, 1-2 s, drifting sideways at 10-25 units/s while swirling up and down,
                // stretched, pulsing in size, mint green to pale gold).
                for (int i = 0; i < 16; i++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(1, 2, hash(float2(i, 8.8)));
                    float alive = particleSeconds(i, t, lifetime, 0.3, seed, seconds);
                    float life = seconds / lifetime;
                    float speed = lerp(10, 25, seed.x) * UNITS_X;
                    float rise = 20 * UNITS_Y * curve4(life, float4(0, 0.2, 0.33, 0.47), float4(0, 0.44, 1, -0.39));
                    float2 pos = float2(0.11 + rand(seed, 1) * 0.78 + speed * seconds, 0.06 + 0.35 * sin(life * 2.4) + 0.1 * life);
                    float2 vel = float2(speed, rise);
                    float size = lerp(0.2, 1, rand(seed, 2)) * UNITS_Y * curve4(life, float4(0, 0.29, 0.64, 1), float4(1, 0.59, 1, 0.19));
                    float s = streak(uv, pos, vel, 0.05, max(size, 0.012));
                    float alpha = curve4(life, float4(0, 0.09, 0.93, 1), float4(0, 1, 1, 0));
                    float3 col = lerp(float3(0.61, 0.98, 0.89), float3(1, 0.87, 0.6), rand(seed, 3));
                    layerAdd(accF, col * s * alpha * 1.4 * alive * reveal);
                }

                // IntroGroup: the smoke dragon's head leads the sweep, its smoke flowing via the flow map.
                float front = sweepFront(0.3);
                float2 local = (uv - float2(front - 0.04, 0.55)) / float2(0.3, 1.5) + 0.5;
                local.x = 1 - local.x;
                float inside = edgeFade(local, float2(0.05, 0.05));
                float2 flow = tex2D(_HeadFlow, saturate(local)).rg * 2 - 1;
                float phase = frac(t * 1.5);
                float4 headA = tex2D(_Head, saturate(local + flow * phase * 0.05));
                float4 headB = tex2D(_Head, saturate(local + flow * frac(phase + 0.5) * 0.05));
                float4 head = lerp(headA, headB, abs(phase * 2 - 1));
                layerOver(accF, head.rgb * 2, head.a * 1.2 * inside * sweepActive());
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
