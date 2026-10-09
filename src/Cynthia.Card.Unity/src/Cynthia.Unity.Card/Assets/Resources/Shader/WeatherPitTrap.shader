// Pit Trap row effect, rebuilt from the original Gwent board effect (PitfallTrapTokenEffect).
// Cracks run through the boards, then the row caves in to a pit with spikes, framed by grass.
Shader "Weather/PitTrap"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Hole("Hole", 2D) = "black" {}
        _Spikes("Spikes", 2D) = "black" {}
        _Grass("Grass", 2D) = "black" {}
        _Cracks("Cracks", 2D) = "black" {}
        _SmallCracks("SmallCracks", 2D) = "black" {}
        _Blades("Blades", 2D) = "black" {}
        _Wood("Wood", 2D) = "black" {}
        _Puff("Puff", 2D) = "black" {}
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

            sampler2D _Hole;
            sampler2D _Spikes;
            sampler2D _Grass;
            sampler2D _Cracks;
            sampler2D _SmallCracks;
            sampler2D _Blades;
            sampler2D _Wood;
            sampler2D _Puff;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                // The original slides jagged masks across the layers in stages; here each stage is a ragged sweep.
                float pCracks = saturate(_Progress / 0.45);
                float pHole = saturate((_Progress - 0.25) / 0.5);
                float pSpikes = saturate((_Progress - 0.5) / 0.5);
                float pGrass = saturate((_Progress - 0.4) / 0.6);

                // CracksSmall: fine dark cracks over the row.
                float2 lSmall = quadLocalVaried(uv, float2(0.5, 0.52), float2(1.06, 1.21), 1);
                float small = tex2D(_SmallCracks, lSmall * float2(1.38 * 4, 0.16 * 4) + float2(0, 0.12)).a;
                layerDarken(acc, small * 0.3 * edgeFade(lSmall, float2(0.03, 0.1)) * sweepRevealAt(uv, pCracks, 0.3));

                // Cracks: the main crack splitting the boards (sits a little low in the row).
                float2 lCrack = quadLocalVaried(uv, float2(0.5, 0.37), float2(1.06, 1.05), 0);
                float4 crack = tex2D(_Cracks, saturate(lCrack));
                layerOver(acc, crack.rgb, crack.a * edgeFade(lCrack, float2(0.03, 0.1)) * sweepRevealAt(uv, pCracks, 0.2));

                // Hole: the row caves in to a pit.
                float2 lHole = quadLocalVaried(uv, float2(0.5, 0.49), float2(1.07, 1.02), 0);
                float4 hole = tex2D(_Hole, saturate(lHole));
                layerOver(acc, hole.rgb, hole.a * edgeFade(lHole, float2(0.02, 0.08)) * sweepRevealAt(uv, pHole, 0.25));

                // Spikes at the bottom of the pit.
                float2 lSpikes = quadLocalVaried(uv, float2(0.5, 0.48), float2(1.07, 1.07), 0);
                float4 spikes = tex2D(_Spikes, saturate(lSpikes));
                layerOver(acc, spikes.rgb, spikes.a * edgeFade(lSpikes, float2(0.03, 0.1)) * sweepRevealAt(uv, pSpikes, 0.2));

                // Grass around the edge of the pit, swaying gently.
                float2 lGrass = quadLocal(uv, float2(0.5, 0.5), float2(1.07, 1.09), 0);
                float sway = sin(t * 1.2 + uv.x * 20) * 0.003 * (1 - abs(lGrass.y * 2 - 1));
                float4 grass = tex2D(_Grass, saturate(lGrass + float2(sway, 0)));
                layerOver(acc, grass.rgb, grass.a * edgeFade(lGrass, float2(0.02, 0.05)) * sweepRevealAt(uv, pGrass, 0.25));

                // -- in front of the cards --
                // Left/Right/Up_Grass (original: bursts of grass blades a few times every 5.3 s from the left and right ends and
                // along the top edge, 2-3 s, 6-11 units, spinning, pushed outward and shrinking away).
                for (int i = 0; i < 12; i++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(i, t, 2.6, 1.8, seed, seconds);
                    float life = seconds / 2.6;
                    float side = floor(rand(seed, 1) * 3);
                    float2 start = side < 1 ? float2(0.03, lerp(0.4, 0.6, seed.y)) : side < 2 ? float2(0.99, lerp(0.4, 0.62, seed.y)) : float2(lerp(0.13, 0.87, seed.x), 1.0);
                    float2 push = side < 1 ? float2(-7.4 * UNITS_X, 0) : side < 2 ? float2(13.2 * UNITS_X, 0) : float2(0, 6 * UNITS_Y);
                    float2 pos = start + push * seconds * (1 - life * 0.5);
                    float size = lerp(6.3, 11.4, rand(seed, 2)) * UNITS_Y * (1 - life);
                    float spin = lerp(1.86, 3.49, rand(seed, 3)) * (side < 1 ? 1 : -1);
                    float4 blade = particleFlipbook(_Blades, uv, pos, size, rand(seed, 4) * 6.283 + spin * seconds, float2(2, 2), floor(rand(seed, 5) * 2));
                    float alpha = curve4(life, float4(0, 0.28, 0.91, 1), float4(0, 1, 1, 0));
                    layerOver(accF, blade.rgb * 1.6, blade.a * alpha * alive * pGrass);
                }

                // Intro (original: wood splinters thrown from the left end at high speed, spinning, and a few dust puffs).
                float since = sinceApplied();
                for (int w = 0; w < 10; w++)
                {
                    float lifetime = lerp(0.8, 1.27, hash(float2(w + _Seed * 57, 1.1)));
                    float delay = hash(float2(w + _Seed * 57, 2.2)) * 0.3;
                    float age = since - delay;
                    float life = age / lifetime;
                    if (life > 0 && life < 1)
                    {
                        float2 vel = float2(lerp(10, 148, hash(float2(w + _Seed * 57, 3.3))) * UNITS_X, lerp(-18.5, 18.5, hash(float2(w + _Seed * 57, 4.4))) * UNITS_Y);
                        float2 pos = float2(0.03, 0.5) + vel * age;
                        float size = lerp(4.69, 6.08, hash(float2(w + _Seed * 57, 5.5))) * UNITS_Y * curve4(life, float4(0, 0.16, 0.59, 1), float4(0.04, 0.63, 1, 0));
                        float4 wood = particleFlipbook(_Wood, uv, pos, size, lerp(3.49, 5.81, hash(float2(w + _Seed * 57, 6.6))) * age, float2(4, 4), floor(hash(float2(w + _Seed * 57, 7.7)) * 8));
                        float3 col = lerp(float3(0.11, 0.04, 0.01), float3(0.67, 0.51, 0.43), hash(float2(w + _Seed * 57, 8.8)));
                        layerOver(accF, wood.rgb * col * 2, wood.a * smoothstep(1, 0.91, life));
                    }
                }
                for (int d = 0; d < 3; d++)
                {
                    float lifetime = lerp(0.82, 1.25, hash(float2(d + _Seed * 57, 9.9)));
                    float life = since / lifetime;
                    if (life > 0 && life < 1)
                    {
                        float size = lerp(15.5, 19.1, hash(float2(d + _Seed * 57, 1.2))) * UNITS_Y * curve4(life, float4(0, 0.52, 1, 1), float4(0.29, 0.82, 0, 0));
                        float4 puff = particleSprite(_Puff, uv, float2(0.03 + d * 0.05, lerp(0.3, 0.7, hash(float2(d + _Seed * 57, 2.3)))), size * 1.6, 0);
                        float3 col = lerp(float3(0.41, 0.33, 0.31), float3(0.48, 0.44, 0.41), hash(float2(d + _Seed * 57, 3.4)));
                        layerOver(accF, col, puff.a * 0.8 * curve4(life, float4(0, 0.12, 0.54, 1), float4(0, 1, 0, 0)));
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
