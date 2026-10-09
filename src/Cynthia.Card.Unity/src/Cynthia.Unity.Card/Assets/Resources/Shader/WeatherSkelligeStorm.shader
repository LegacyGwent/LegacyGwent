// Skellige Storm row effect, rebuilt from the original Gwent board effect (SkelligeStormTokenEffect).
// A rough sea floods the row: churning teal water, caustic light in the middle, foam along and under the bottom,
// clouds in the upper part and the odd lightning flash. A wave of sea water rolls it in.
Shader "Weather/SkelligeStorm"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Sea("Sea", 2D) = "black" {}
        _WaveNormal("WaveNormal", 2D) = "black" {}
        _Wavey("Wavey", 2D) = "black" {}
        _BoardMask("BoardMask", 2D) = "black" {}
        _BoardMaskInv("BoardMaskInv", 2D) = "black" {}
        _Caustics("Caustics", 2D) = "black" {}
        _Foam("Foam", 2D) = "black" {}
        _Clouds("Clouds", 2D) = "black" {}
        _Lightning("Lightning", 2D) = "black" {}
        _Splash("Splash", 2D) = "black" {}
        _Spray("Spray", 2D) = "black" {}
        _Churn("Churn", 2D) = "black" {}
        _WaveTex("WaveTex", 2D) = "black" {}
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

            sampler2D _Sea;
            sampler2D _WaveNormal;
            sampler2D _Wavey;
            sampler2D _BoardMask;
            sampler2D _BoardMaskInv;
            sampler2D _Caustics;
            sampler2D _Foam;
            sampler2D _Clouds;
            sampler2D _Lightning;
            sampler2D _Splash;
            sampler2D _Spray;
            sampler2D _Churn;
            sampler2D _WaveTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.3);
                float2 l = quadLocal(uv, float2(0.5, 0.51), float2(1.05, 1.05), 1);
                float quadEdge = edgeFade(l, float2(0.03, 0.1));

                // Wavy distortion shared by the water layers (WaveNormal2 / WaveyMask2, original speeds).
                float2 wn = tex2D(_WaveNormal, l * float2(2, 0.75) + float2(t * 0.25 * 0.4, 0.1)).rg - 0.5;
                float wavey = tex2D(_Wavey, l * float2(2, 0.7) + float2(t * 0.15 * 0.4, 0.15) + wn * 0.03).r;

                // BackTint (original: a multiplying teal tint over the board).
                float2 lTint = quadLocal(uv, float2(0.49, 0.49), float2(1.06, 1.14), 1);
                layerOver(acc, float3(0.1, 0.24, 0.27), 0.38 * edgeFade(lTint, float2(0.03, 0.1)) * reveal);

                // The waves along the bottom roll over the cards; everything in the upper part stays behind them.
                // -- in front of the cards --
                // BackWater (original SkelligeStorm_BackWater): the wave mask (WaveyMask2) has a band of water along the top and
                // the bottom with wavy crest lines; it scrolls along the row and is bent by a wave normal map, so waves roll
                // along both edges. The bands are filled with the foamy sea texture (WaveBase), strongest towards the row edges
                // (BoardMask x10), tinted sea green.
                float2 waveyUv = l * float2(2, 0.7) + float2(t * 0.15, 0.15) + wn * 0.03;
                float waves = saturate(tex2D(_Wavey, waveyUv).r * 2);
                float edges = saturate(tex2D(_BoardMask, saturate(l)).r * 10);
                float4 water = tex2D(_WaveTex, l * float2(3, 1) + float2(t * 0.1, 0) + wn * 0.2);
                float3 waterCol = lerp(float3(0.08, 0.2, 0.22), water.rgb * float3(0.592, 0.963, 0.84) * 2, 0.75);
                // In the video the waves fill the lower half of the row; the upper band of the mask is left to the clouds.
                float lower = smoothstep(0.62, 0.38, uv.y);
                waves *= lower;
                layerOver(accF, waterCol, waves * edges * (0.45 + water.a * 0.55) * quadEdge * reveal);
                float crestLine = saturate(tex2D(_Wavey, waveyUv + float2(0, 0.012)).r * 2) - waves;
                layerAdd(accF, float3(0.6, 0.85, 0.8) * saturate(abs(crestLine) * 6) * lower * edges * 0.5 * quadEdge * reveal);

                // -- behind the cards --
                // BackWaterEdge: caustic light through the middle of the row (BoardMaskInv), strongest under the waves.
                float centre = tex2D(_BoardMaskInv, saturate(l)).r;
                float caustic = tex2D(_Caustics, l * float2(3, 1) + float2(t * 0.2 * 0.4, 0) + wn * 0.2).a;
                layerAdd(acc, float3(0.866, 1, 0.713) * caustic * centre * (0.25 + waves * 0.5) * 0.35 * quadEdge * reveal);

                // -- in front of the cards --
                // BottomSplashes (original SkelligeStorm_Foam1): splash texture tiled 4x along the bottom, sharpened (power 2),
                // distorted by a noise; here the noise rolls along the row with the waves (left to right) so the surf heaves with them. The original noise
                // texture (Aard_Sign) is not in the beta's bundle; a similar noise stands in. Two layers at different speeds.
                float2 lFoam = quadLocal(uv, float2(0.5, 0.18), float2(1.06, 0.63), 1);
                float churnA = tex2D(_Churn, lFoam * float2(3, 0.3) + float2(t * 0.25, 0)).a;
                float churnB = tex2D(_Churn, lFoam * float2(2.3, 0.25) + float2(t * 0.17, 0) + 0.37).a;
                float surge = sin(t * 1.3 - uv.x * 9) * 0.06 + sin(t * 2.1 - uv.x * 14) * 0.04;
                float foamFade = edgeFade(lFoam, float2(0.03, 0.08));
                float4 foamA = tex2D(_Foam, float2(lFoam.x * 4 + t * 0.15 * 0.4, saturate(lFoam.y * 0.62 + (churnA - 0.5) * 0.3 + surge)));
                float4 foamB = tex2D(_Foam, float2(lFoam.x * 3.1 - t * 0.2 * 0.4 + 0.5, saturate(lFoam.y * 0.75 + (churnB - 0.5) * 0.28 - surge)));
                float foamA2 = pow(foamA.a, 2) * 1.3;
                float foamB2 = pow(foamB.a, 2) * 1.1;
                layerOver(accF, float3(0.659, 0.89, 0.813) * foamA.rgb, saturate(foamA2) * foamFade * 0.85 * reveal);
                layerOver(accF, float3(0.6, 0.85, 0.8) * foamB.rgb * 0.95, saturate(foamB2) * foamFade * 0.7 * reveal);
                layerAdd(accF, float3(0.35, 0.55, 0.5) * saturate(foamA2 + foamB2) * smoothstep(0.35, 0, uv.y) * 0.25 * foamFade * reveal);

                // Surf spray (the original's FoamBase splashes): bursts of white water thrown up along the bottom and falling back.
                for (int f = 0; f < 8; f++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(0.6, 0.9, hash(float2(f, 9.1)));
                    float alive = particleSeconds(f + 60, t, lifetime, 0.25, seed, seconds);
                    float life = seconds / lifetime;
                    float up = lerp(0.6, 1.1, rand(seed, 1));
                    float2 pos = float2(0.02 + seed.x * 0.96 + (rand(seed, 2) - 0.5) * 0.02 * seconds, 0.04 + up * seconds - 1.6 * seconds * seconds);
                    float size = lerp(0.35, 0.7, rand(seed, 3)) * lerp(0.6, 1.2, life);
                    float4 spray = particleSprite(_Spray, uv, pos, size, rand(seed, 4) * 6.283);
                    float alpha = curve4(life, float4(0, 0.15, 0.6, 1), float4(0, 1, 0.8, 0));
                    layerAdd(accF, float3(0.45, 0.7, 0.65) * spray.a * alpha * 0.9 * alive * reveal);
                }

                // -- behind the cards --
                // CloudsFakePerspective: two cloud layers passing over the upper part of the row; the nearer one is bigger,
                // brighter and faster (the original uses a perspective mesh for the same depth effect).
                float2 cloudUvFar = float2(uv.x * 1.6 - t * 0.025, (uv.y - 0.45) * 0.9);
                float2 cloudUvNear = float2(uv.x * 0.9 - t * 0.05 + 0.3, (uv.y - 0.35) * 0.6);
                float cloudsFar = tex2D(_Clouds, cloudUvFar).a;
                float cloudsNear = tex2D(_Clouds, cloudUvNear).a;
                float sky = smoothstep(0.35, 0.85, uv.y) * rowFade(uv);
                layerOver(acc, float3(0.42, 0.55, 0.58), cloudsFar * sky * 0.5 * reveal);
                layerOver(acc, float3(0.649, 0.899, 0.949) * 0.85, cloudsNear * sky * 0.75 * reveal);

                // Lightning: about one strike a second. A jagged bolt forks down from above the row, flickers three times like the
                // original's alpha curve and lights up the water around it.
                for (int i = 0; i < 2; i++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(0.35, 0.6, hash(float2(i, 4.4)));
                    float alive = particleSeconds(i, t, lifetime, 1.1, seed, seconds);
                    float life = seconds / lifetime;
                    float flicker = curve7(life, float4(0, 0.08, 0.2, 0.3), float4(0, 1, 0.15, 1), float3(0.45, 0.6, 1), float3(0.1, 0.8, 0)) * alive;
                    float x0 = 0.08 + seed.x * 0.84;
                    float bottom = lerp(0.05, 0.4, rand(seed, 1));
                    float top = 1.06;
                    float span = saturate((top - uv.y) / (top - bottom));
                    // Main bolt: zig-zag made of two octaves of noise, wider at the top.
                    float zig = (noise(float2(uv.y * 9, seed.y * 50)) - 0.5) * 0.05 + (noise(float2(uv.y * 30, seed.x * 50)) - 0.5) * 0.015;
                    float d = abs(uv.x - x0 - zig * span) * ROW_ASPECT;
                    float inBolt = step(bottom, uv.y) * smoothstep(bottom, bottom + 0.08, uv.y) * smoothstep(top, top - 0.06, uv.y);
                    float core = smoothstep(0.02, 0, d) * inBolt;
                    float glow = exp(-d * 18) * inBolt;
                    // A branch splitting off halfway down.
                    float by = lerp(bottom, 1, 0.55);
                    float bx = x0 + (noise(float2(by * 9, seed.y * 50)) - 0.5) * 0.05;
                    float bdir = rand(seed, 2) < 0.5 ? -1 : 1;
                    float bspan = saturate((by - uv.y) / 0.3);
                    float bd = abs(uv.x - bx - bdir * bspan * 0.04 - (noise(float2(uv.y * 25, seed.x * 30)) - 0.5) * 0.012) * ROW_ASPECT;
                    float branch = smoothstep(0.015, 0, bd) * step(uv.y, by) * step(by - 0.3, uv.y) * (1 - bspan);
                    layerAdd(acc, (float3(0.85, 0.97, 1) * (core + branch * 0.7) * 2.5 + float3(0.3, 0.6, 0.8) * glow * 0.8) * flicker * reveal);
                    layerAdd(acc, float3(0.05, 0.09, 0.11) * flicker * rowFade(uv) * reveal);
                }

                // FrameCollide (original: 30/s, 0.75-1 s, 5-20 units, shrinking to a third): spray where the incoming wave slams
                // into the right edge of the row, thrown up and out and falling back under gravity. Only when the wave arrives.
                float sprayWindow = smoothstep(1.3, 1.8, sinceApplied()) * smoothstep(3.4, 2.4, sinceApplied());
                for (int j = 0; j < 10; j++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(0.75, 1, hash(float2(j, 6.6)));
                    float alive = particleSeconds(j + 20, t, lifetime, 0.05, seed, seconds);
                    float life = seconds / lifetime;
                    float2 vel = float2(lerp(5, 20, seed.x) * UNITS_X, lerp(5, 15, seed.y) * UNITS_Y);
                    float2 pos = float2(0.985, lerp(0.15, 0.85, rand(seed, 1))) + vel * seconds - float2(0, 0.5 * 4 * 9.81 * UNITS_Y * seconds * seconds * 0.3);
                    float size = lerp(5, 20, rand(seed, 2)) * UNITS_Y * curve4(life, float4(0, 0.17, 1, 1), float4(1, 1, 0.33, 0.33));
                    float4 spray = particleSprite(_Spray, uv, pos, size, rand(seed, 3) * 6.283);
                    float alpha = curve4(life, float4(0, 0.25, 0.8, 1), float4(0, 1, 1, 0));
                    layerAdd(acc, float3(0.18, 0.29, 0.25) * 2.5 * spray.a * alpha * alive * sprayWindow);
                }

                // Only the incoming wave of the intro passes over the cards, so their values stay readable.
                // -- in front of the cards --
                // The incoming wave: a wall of sea water (WaveWaterBase: sea texture) with a splash at its crest.
                float3 sea = tex2D(_Sea, l * float2(2.4, 0.8) + float2(t * 0.03, -0.2) + wn * 0.05).rgb;
                float front = sweepFront(0.3);
                float wall = smoothstep(0.18, 0, abs(uv.x - front + 0.06)) * rowFade(uv);
                layerOver(accF, sea * 1.5 + 0.1, wall * 0.85 * sweepActive());
                float4 crest = sprite(_Splash, uv, float2(front, 0.6), float2(0.28, 1.6));
                layerOver(accF, float3(0.85, 1, 0.95) * crest.rgb, crest.a * sweepActive());
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
