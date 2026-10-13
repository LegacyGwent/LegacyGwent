// Golden Froth row effect, rebuilt from the original Gwent board effect (GoldenFrothTokenEffect).
// The row fills with golden beer: an amber tint, foam around the edge and lots of tiny bubbles fizzing and popping.
Shader "Weather/GoldenFroth"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Fill("Fill", 2D) = "black" {}
        _Foam("Foam", 2D) = "black" {}
        _Bubble("Bubble", 2D) = "black" {}
        _Burst("Burst", 2D) = "black" {}
        _Normal("Normal", 2D) = "black" {}
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

            sampler2D _Fill;
            sampler2D _Foam;
            sampler2D _Bubble;
            sampler2D _Burst;
            sampler2D _Normal;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.3);

                // After the intro the beer settles down to a calmer level.
                float settle = lerp(1, 0.6, smoothstep(0.85, 1, _Progress));
                reveal *= settle;

                // BeerDistortion: amber fill on a quad larger than the row, with a gentle sheen.
                float2 l = quadLocal(uv, float2(0.5, 0.5), float2(1.1, 1.24), 0);
                float fill = tex2D(_Fill, saturate(l)).r * edgeFade(l, float2(0.03, 0.1));
                float2 n = tex2D(_Normal, l * float2(0.5 * ROW_ASPECT, 0.15 * 7) + float2(2.23 + t * 0.02, 0)).rg - 0.5;
                layerOver(acc, float3(0.867, 0.58, 0.071), fill * 0.28 * reveal);
                layerAdd(acc, float3(1, 0.786, 0.515) * saturate(n.x * 3) * fill * 0.12 * reveal);

                // BubblesLoop: the foam around the edge (one large static particle in the original).
                float2 lFoam = quadLocalVaried(uv, float2(0.5, 0.5), float2(0.97, 0.94), 0);
                float4 foam = tex2D(_Foam, saturate(lFoam + n * 0.004));
                layerOver(acc, foam.rgb * 1.15, foam.a * edgeFade(lFoam, float2(0.02, 0.06)) * reveal);

                // -- in front of the cards --
                // BurstingBubbles_static (original: 200/s, 1-3 s, 0.35-2 units): a dense fizz of tiny bubbles over the beer.
                float2 cellUv = uv * float2(ROW_ASPECT * 10, 10);
                float2 cell = floor(cellUv) + floor(_Seed * 997);
                float h = hash(cell);
                float period = lerp(1, 3, h) * 1.3;
                float cycle = t / period + hash(cell + 3.3);
                float bubbleLife = frac(cycle) * 1.3;
                float2 jitter = hash2(cell + floor(cycle));
                float bubbleSize = lerp(0.35, 2, jitter.y) * UNITS_Y * 0.7 * 10;
                float2 bubbleUv = (frac(cellUv) - 0.15 - jitter * 0.7) / max(bubbleSize, 0.05) + 0.5;
                float bubble = tex2D(_Bubble, saturate(bubbleUv)).a * insideQuad(bubbleUv);
                layerAdd(accF, float3(1, 0.904, 0.699) * 1.7 * bubble * curve4(saturate(bubbleLife), float4(0, 0.08, 0.95, 1), float4(0, 1, 1, 0)) * step(bubbleLife, 1) * fill * 0.4 * reveal);

                // BurstingBubbles (original: 240/s, 0.25-0.5 s, 0.35-2 units growing from 4%, rising 1-4.6 units/s).
                float2 riseUv = uv * float2(ROW_ASPECT * 6, 6);
                float2 rcell = floor(riseUv) + floor(_Seed * 997);
                float rh = hash(rcell + 7.7);
                float rperiod = lerp(0.25, 0.5, rh) * 2;
                float rcycle = t / rperiod + hash(rcell + 1.9);
                float rlife = frac(rcycle) * 2;
                float2 rjit = hash2(rcell + floor(rcycle) + 0.5);
                float rsize = lerp(0.35, 2, rjit.x) * UNITS_Y * 6 * lerp(0.04, 1, saturate(rlife));
                float2 rpos = float2(0.2 + rjit.x * 0.6, 0.15 + rjit.y * 0.3 + rlife * 0.5 * lerp(1, 4.6, rh) * UNITS_Y * 6 * 0.5);
                float2 rUv = (frac(riseUv) - rpos) / max(rsize, 0.02) + 0.5;
                float rbubble = tex2D(_Bubble, saturate(rUv)).a * insideQuad(rUv) * step(rlife, 1);
                layerAdd(accF, float3(1, 0.904, 0.699) * 1.7 * rbubble * curve4(saturate(rlife), float4(0, 0.08, 0.95, 1), float4(0, 1, 1, 0)) * fill * 0.7 * reveal);

                // BurstingBubbles_explo (original: 80/s, 0.25-0.5 s, 0.5-5 units growing from 4%, rising; 2x2 burst sheet).
                for (int j = 0; j < 8; j++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(0.25, 0.5, hash(float2(j, 3.7)));
                    float alive = particleSeconds(j + 20, t, lifetime, 0.15, seed, seconds);
                    float life = seconds / lifetime;
                    float size = lerp(0.5, 5, rand(seed, 1)) * UNITS_Y * lerp(0.04, 1, life) * 1.5;
                    float2 pos = float2(0.04 + seed.x * 0.92, 0.15 + seed.y * 0.7 + seconds * lerp(1, 4.6, rand(seed, 2)) * UNITS_Y);
                    float4 pop = particleFlipbook(_Burst, uv, pos, size, rand(seed, 3) * 6.283, float2(2, 2), life * 3.99);
                    layerAdd(accF, pop.rgb * pop.a * curve4(life, float4(0, 0.08, 0.95, 1), float4(0, 1, 1, 0)) * 1.2 * alive * reveal);
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
