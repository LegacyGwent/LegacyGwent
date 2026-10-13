// Ragh Nar Roog row effect, rebuilt from the original Gwent board effect (RaghNarRoogTokenEffect).
// The row turns into scorched stones with lava glowing through the cracks, flames licking along the bottom
// and embers drifting up. A wall of fire sweeps it in.
Shader "Weather/RaghNarRoog"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Stones("Stones", 2D) = "black" {}
        _Emission("Emission", 2D) = "black" {}
        _Cloud("Cloud", 2D) = "black" {}
        _Fire("Fire", 2D) = "black" {}
        _GroundFire("GroundFire", 2D) = "black" {}
        _Wave("Wave", 2D) = "black" {}
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

            sampler2D _Stones;
            sampler2D _Emission;
            sampler2D _Cloud;
            sampler2D _Fire;
            sampler2D _GroundFire;
            sampler2D _Wave;
            sampler2D _Glow;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.25);

                // burned_wood (RagNarRoog_Edge): stones on a quad a bit larger than the row; lava noise shines through the emission mask.
                float2 lStones = quadLocal(uv, float2(0.5, 0.45), float2(1.12, 1.19), 1);
                float2 lStonesV = quadLocalVaried(uv, float2(0.5, 0.45), float2(1.12, 1.19), 1);
                float4 stones = tex2D(_Stones, saturate(lStonesV));
                float emission = tex2D(_Emission, saturate(lStonesV)).r;
                float lavaNoise = tex2D(_Cloud, lStones * float2(6, 1) + float2(t * 0.01, -t * 0.02)).r;
                float lava = pow(lavaNoise, 4) * 5;
                float3 stoneCol = stones.rgb + emission * (float3(0.176, 0.047, 0) + float3(0.868, 0.197, 0) * lava);
                // More transparent than the video measurement (about 0.8) on request, so the wood shows through.
                layerOver(acc, stoneCol, stones.a * 0.65 * edgeFade(lStones, float2(0.03, 0.1)) * reveal);

                // BottomGlow: warm light from below, spilling a little under the row.
                layerAdd(acc, float3(0.368, 0.103, 0.005) * 1.83 * smoothstep(0.6, 0, uv.y) * rowFade(uv) * reveal);

                // -- in front of the cards --
                // fire (RagNarRoog_fire_row): only the bottom 38% of the row; tiling fire rising slowly.
                float2 lFire = quadLocal(uv, float2(0.51, 0.15), float2(1.08, 0.38), 1);
                float fireNoise = tex2D(_Cloud, lFire * float2(2, 0.2) + float2(t * 0.04, -t * 0.05)).r;
                float3 fire = tex2D(_Fire, lFire * float2(1.3, 0.1) + float2(fireNoise * 0.05, -t * 0.04)).rgb;
                float fireShape = smoothstep(1, 0.2, lFire.y) * edgeFade(lFire, float2(0.03, 0.15));
                layerAdd(accF, float3(0.926, 0.268, 0) * pow(fire.r, 2) * 1.5 * fireShape * (0.3 + fireNoise) * reveal);

                // fire_small_ground (original: 2/s, 1-1.5 s, 12 units, growing from 44%, brightening over its life,
                // drifting sideways 4 units/s along the bottom edge).
                for (int i = 0; i < 3; i++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(1, 1.5, hash(float2(i, 3.1)));
                    float alive = particleSeconds(i, t, lifetime, 0.3, seed, seconds);
                    float life = seconds / lifetime;
                    float size = 12 * UNITS_Y * lerp(0.44, 1, life);
                    float2 pos = float2(0.08 + seed.x * 0.84 + seconds * 4 * UNITS_X, 0.02 + size * 0.45);
                    float4 flame = particleFlipbook(_GroundFire, uv, pos, size, 0, float2(7, 5), life * 34);
                    float alpha = curve4(life, float4(0, 0.13, 0.51, 1), float4(0, 0.95, 1, 0));
                    layerAdd(accF, float3(1, 0.42, 0.08) * flame.a * alpha * lerp(0.3, 1.2, life) * alive * reveal);
                }

                // sparks (original: 16/s over the whole row, 0.25-1 s, flying in random directions at up to 12 x 22 units/s
                // with some turbulence, stretched along their motion, pulsing in size, orange to red).
                for (int j = 0; j < 12; j++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(0.25, 1, hash(float2(j, 5.3)));
                    float alive = particleSeconds(j + 10, t, lifetime, 0.35, seed, seconds);
                    float life = seconds / lifetime;
                    float2 vel = float2((rand(seed, 1) * 2 - 1) * 12 * UNITS_X, (rand(seed, 2) * 2 - 1) * 22 * UNITS_Y);
                    float2 wobble = float2(sin(seconds * 7 + seed.x * 20), cos(seconds * 6 + seed.y * 20)) * float2(0.004, 0.05) * seconds;
                    float2 pos = float2(0.08 + seed.x * 0.84, seed.y) + vel * seconds + wobble;
                    float size = lerp(1, 2, rand(seed, 3)) * UNITS_Y * curve4(life, float4(0, 0.29, 0.54, 1), float4(0, 0.81, 1, 0.19));
                    float s = streak(uv, pos, vel, 0.06, size * 0.4);
                    float alpha = curve4(life, float4(0, 0.09, 0.93, 1), float4(0, 1, 1, 0));
                    float3 col = lerp(float3(0.98, 0.37, 0), float3(0.87, 0.09, 0), rand(seed, 4)) * float3(1, 0.785, 0.706) * 2;
                    layerAdd(accF, col * s * alpha * alive * reveal);
                }

                // InitialWave (original: 2x15 flipbook over 1.4 x 1.5 times the row, 1.25 s, orange to pale yellow).
                float waveLife = saturate(_Progress * 1.03 / 1.25);
                float2 lWave = quadLocal(uv, float2(0.5, 0.5), float2(1.4, 1.5), 1);
                float waveFrame = curve4(waveLife, float4(0, 0.52, 1, 1), float4(0, 0.64, 1, 1)) * 29;
                float4 wave = flipbook(_Wave, saturate(lWave), float2(2, 15), waveFrame) * edgeFade(lWave, float2(0.05, 0.1));
                float waveAlpha = curve4(waveLife, float4(0, 0.44, 0.94, 1), float4(0, 1, 1, 0)) * step(_Progress, 0.999);
                float3 waveColor = lerp(float3(1, 0.35, 0), float3(1, 0.83, 0.58), saturate(waveLife / 0.81)) * float3(0.94, 0.64, 0.51);
                layerAdd(accF, waveColor * wave.a * waveAlpha * 4);

                // Meteors (original RagNarRoog_Metheor): three burning stones come down from the upper left and hit the row,
                // each impact flashing up (44 units, 0.3 s) and throwing out sparks.
                float since = sinceApplied();
                for (int m = 0; m < 3; m++)
                {
                    float impactTime = 0.3 + m * 0.28 + hash(float2(m, _Seed * 17)) * 0.1;
                    float2 hit = float2(0.2 + hash(float2(m, _Seed * 31)) * 0.6, lerp(0.3, 0.7, hash(float2(_Seed * 31, m))));
                    float before = impactTime - since;
                    if (before > 0 && before < 0.25)
                    {
                        float2 vel = float2(0.9, -2.2) * float2(1.0 / ROW_ASPECT, 1);
                        float2 pos = hit - vel * before;
                        float stone = streak(uv, pos, vel, 0.12, 0.07);
                        layerAdd(accF, float3(1, 0.45, 0.1) * stone * 2.5);
                    }
                    float after = since - impactTime;
                    if (after > 0 && after < 1.2)
                    {
                        float4 flash = particleSprite(_Glow, uv, hit, 44 * UNITS_Y, 0);
                        float flashAlpha = curve4(after / 0.3, float4(0, 0.03, 1, 1), float4(0, 0.65, 0, 0));
                        layerAdd(accF, float3(0.91, 0.46, 0.16) * flash.a * flashAlpha * 2);
                        for (int k = 0; k < 10; k++)
                        {
                            float lifetime = lerp(0.25, 1, hash(float2(m + _Seed * 7, k)));
                            float life = after / lifetime;
                            float2 vel = float2(lerp(-11, 77, hash(float2(k, m + 9 + _Seed * 7))) * UNITS_X, lerp(-22, 22, hash(float2(k + 5, m + _Seed * 7))) * UNITS_Y);
                            float2 pos = hit + vel * after;
                            float s = streak(uv, pos, vel, 0.06, 0.03);
                            layerAdd(accF, float3(0.98, 0.3, 0) * s * curve4(life, float4(0, 0.09, 0.93, 1), float4(0, 1, 1, 0)) * step(life, 1) * 1.6);
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
