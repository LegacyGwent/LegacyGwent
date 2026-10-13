// Korathi Heatwave row effect, rebuilt from the original Gwent board effect (DroughtTokenEffect (Drought)).
// The row dries out into cracked earth; heat haze shimmers low over it and fine sand drifts across.
Shader "Weather/Drought"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Cracks("Cracks", 2D) = "black" {}
        _EdgeMask("EdgeMask", 2D) = "black" {}
        _Haze("Haze", 2D) = "black" {}
        _HazeMask("HazeMask", 2D) = "black" {}
        _Sand("Sand", 2D) = "black" {}
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

            sampler2D _Cracks;
            sampler2D _EdgeMask;
            sampler2D _Haze;
            sampler2D _HazeMask;
            sampler2D _Sand;
            sampler2D _Glow;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.3);

                // Drought_Cracks: dry earth with cracks, on a quad slightly larger than the row.
                float2 l = quadLocal(uv, float2(0.49, 0.49), float2(1.14, 1.16), 1);
                float quadEdge = edgeFade(l, float2(0.03, 0.1));
                float2 lV = quadLocalVaried(uv, float2(0.49, 0.49), float2(1.14, 1.16), 1);
                float edge = tex2D(_EdgeMask, saturate(lV)).r;
                float4 earth = tex2D(_Cracks, saturate(lV));
                layerOver(acc, float3(0.838, 0.62, 0.4), (0.12 + edge * 0.2) * quadEdge * reveal);
                layerOver(acc, earth.rgb * float3(0.978, 0.86, 0.6), earth.a * (0.5 + edge * 0.3) * quadEdge * reveal);
                layerDarken(acc, earth.a * 0.12 * quadEdge * reveal);

                // BottomGlow: hot orange light from below.
                layerAdd(acc, float3(0.684, 0.359, 0.196) * smoothstep(0.75, 0, uv.y) * 0.45 * rowFade(uv) * reveal);

                // -- in front of the cards --
                // Haze (pPlane1 covers the lower 70% of the row): slowly wobbling heat lines.
                float hazeBand = smoothstep(0.75, 0.4, uv.y) * rowFade(uv);
                float hazeMask = tex2D(_HazeMask, saturate(float2(uv.x, uv.y / 0.75))).r;
                float haze = tex2D(_Haze, l * float2(2, 0.4) * 2 + float2(t * 0.015, -t * 0.03)).a;
                layerAdd(accF, float3(1, 0.965, 0.684) * haze * hazeMask * hazeBand * 0.22 * reveal);

                // SandDust (original: every 2 s a huge sheet of blowing sand, 2-2.5 s, sweeping in from the left at 40-50
                // units/s and growing): streaky sand clouds crossing the row.
                for (int s = 0; s < 2; s++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = 2.25;
                    float alive = particleSeconds(s + 30, t, lifetime, 1.6, seed, seconds);
                    float life = seconds / lifetime;
                    float width = lerp(0.35, 1, life);
                    float centreX = -0.25 + seconds * lerp(40, 50, seed.x) * UNITS_X;
                    float sheet = smoothstep(width * 0.5, 0, abs(uv.x - centreX)) * rowFade(uv);
                    float streaks = tex2D(_Haze, float2(uv.x * 1.5 - t * 0.2, uv.y * 0.3 + seed.y)).a;
                    float alpha = curve4(life, float4(0, 0.14, 0.88, 1), float4(0, 1, 1, 0));
                    layerOver(accF, float3(1, 0.83, 0.56), streaks * sheet * alpha * 0.53 * 0.6 * alive * reveal);
                }

                // BottomGlow_FX (original: 0.7/s, 15 s, 25-30 units, drifting right at 11 units/s): slow warm glows along the bottom.
                for (int g = 0; g < 10; g++)
                {
                    float2 seed;
                    float seconds;
                    float alive = particleSeconds(g + 50, t, 15, 0.1, seed, seconds);
                    float life = seconds / 15;
                    float2 pos = float2(-0.15 + seconds * 11 * UNITS_X, 0.08 + seed.y * 0.06);
                    float4 glow = particleSprite(_Glow, uv, pos, lerp(25, 30, seed.x) * UNITS_Y, 0);
                    float alpha = curve4(life, float4(0, 0.15, 0.85, 1), float4(0, 1, 1, 0));
                    // The original glow texture is missing from the beta; a soft radial glow at low strength stands in for it.
                    layerAdd(accF, float3(0.27, 0.23, 0.18) * 0.35 * glow.a * glow.a * alpha * alive * reveal);
                }

                // SandGrainsLoop (original: 50/s in gusts, 3-4 s, 10-25 units/s, turbulent, tiny stretched grains): grains are
                // blown to the right and drift up and away, shrinking a little as if carried off backwards.
                for (int i = 0; i < 24; i++)
                {
                    float2 seed;
                    float seconds;
                    float lifetime = lerp(3, 4, hash(float2(i, 2.2)));
                    float alive = particleSeconds(i, t, lifetime, 0.4, seed, seconds);
                    float life = seconds / lifetime;
                    float speed = lerp(10, 25, seed.x) * UNITS_X;
                    float rise = lerp(1.5, 4, rand(seed, 2)) * UNITS_Y;
                    float2 pos = float2(0.04 + rand(seed, 1) * 0.5 + speed * seconds,
                                        0.06 + seed.y * 0.3 + rise * seconds + sin(seconds * 3 + seed.x * 30) * 0.025);
                    float2 vel = float2(speed, rise);
                    float grain = streak(uv, pos, vel, 0.12, 0.012 * lerp(1, 0.5, life));
                    float alpha = curve4(life, float4(0, 0.09, 0.7, 1), float4(0, 1, 0.85, 0));
                    layerAdd(accF, float3(1, 0.87, 0.6) * grain * alpha * 0.35 * alive * reveal);
                }

                // DustTrail (original Front_FX / TrailTop / TrailBottom, as seen in the video): a billowing wall of sand clouds
                // rolls across with the front, well over the top and bottom of the row. Clouds are born at the front and stay
                // behind it, swelling, slowly turning and drifting up and away, as if blown backwards, while they fade.
                float introSeconds = sinceApplied();
                for (int p = 0; p < 28; p++)
                {
                    float h1 = hash(float2(p + _Seed * 57, 1.3));
                    float born = (p + h1) / 28 * 1.9;
                    float age = introSeconds - born;
                    float lifetime = lerp(0.6, 1.0, hash(float2(p + _Seed * 57, 2.4)));
                    float life = age / lifetime;
                    if (life > 0 && life < 1)
                    {
                        float bornFront = saturate(born / 2.0) * 1.6 - 0.3;
                        float2 pos = float2(bornFront + 0.04 + (hash(float2(p + _Seed * 57, 3.5)) - 0.5) * 0.08 - age * 0.03,
                                            lerp(-0.15, 1.05, hash(float2(p + _Seed * 57, 4.6))) + age * 0.3);
                        float size = lerp(1.0, 1.5, hash(float2(p + _Seed * 57, 5.7))) * lerp(0.7, 1.7, life);
                        float angle = hash(float2(p + _Seed * 57, 6.8)) * 6.283 + (hash(float2(p + _Seed * 57, 7.9)) * 2 - 1) * 0.6 * age;
                        float4 puff = particleFlipbook(_Sand, uv, pos, size, angle, float2(2, 1), floor(hash(float2(p + _Seed * 57, 8.1)) * 2));
                        float alpha = curve4(life, float4(0, 0.08, 0.35, 1), float4(0, 1, 0.7, 0));
                        layerOver(accF, puff.rgb * float3(0.98, 0.93, 0.85) * 1.5, puff.a * alpha * 0.8);
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
