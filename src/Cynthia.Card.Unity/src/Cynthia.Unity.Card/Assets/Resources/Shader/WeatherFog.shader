// Impenetrable Fog row effect, rebuilt from the original Gwent board effect (FogTokenEffect).
// Pale mist covers the row, thickest around its edges and spilling softly over them, slowly churning.
// A glowing wave of mist rolls it in.
Shader "Weather/Fog"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _EdgeSoft("EdgeSoft", 2D) = "black" {}
        _FogEdge("FogEdge", 2D) = "black" {}
        _Cloud("Cloud", 2D) = "black" {}
        _Normal("Normal", 2D) = "black" {}
        _Wave("Wave", 2D) = "black" {}
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
            sampler2D _FogEdge;
            sampler2D _Cloud;
            sampler2D _Normal;
            sampler2D _Wave;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = weatherTime();
                float4 acc = 0;   // layers behind the cards
                float4 accF = 0;  // layers in front of the cards

                float reveal = sweepReveal(uv, 0.3);

                // DarkBackground: faint cold veil, larger than the row.
                float2 lBack = quadLocal(uv, float2(0.5, 0.45), float2(1.08, 1.31), 1);
                layerOver(acc, float3(0.735, 0.744, 1), 0.128 * tex2D(_EdgeSoft, saturate(lBack)).r * edgeFade(lBack, float2(0.03, 0.1)) * reveal);

                // -- in front of the cards --
                // EffectEdge (MistEdgeShader): quad 1.2 x 1.28 times the row with the original mask tiling (1.28, 1.3) and
                // offset (-0.05, -0.1). The mask texture repeats, which puts its bright rim just inside the top and bottom
                // of the row and brings it back at the left end; its darker middle leaves a thin mist over the whole row.
                float2 l = quadLocal(uv, float2(0.47, 0.5), float2(1.2, 1.28), 1);
                float2 distort = (tex2D(_Normal, l * float2(0.2, 0.1) * float2(ROW_ASPECT, 1) + float2(0.08 + t * 0.01, 0)).rg - 0.5) * 0.13;
                float mask = tex2D(_FogEdge, l * float2(1.28, 1.3) + float2(-0.05, -0.1) + distort * 0.2).r * edgeFade(l, float2(0.04, 0.18));
                float m1 = tex2D(_Cloud, l * float2(2, 1) + float2(t * 0.012, 0) + distort).r;
                float m2 = tex2D(_Cloud, l * float2(2, -1) + float2(-t * 0.009, 0.3) + distort).r;
                float mist = saturate((mask * 1.3 + 0.1 * edgeFade(l, float2(0.08, 0.15))) * pow(saturate(m1 * m2 * 2.4), 1.6));
                layerAdd(accF, float3(0.533, 0.853, 0.787) * mist * 0.45 * reveal);
                layerOver(accF, float3(0.82, 0.88, 0.82), mist * 0.22 * reveal);

                // InitialWave (original: a 2x15 flipbook spread over 1.4 x 1.5 times the row, 1.15 s after a 0.05 s delay,
                // colour going from teal to white): the rolling mist wave plays across the whole row.
                float waveLife = saturate((_Progress * 1.2 - 0.05) / 1.15);
                float2 lWave = quadLocal(uv, float2(0.5, 0.5), float2(1.4, 1.5), 1);
                float waveFrame = curve4(waveLife, float4(0, 0.52, 1, 1), float4(0, 0.64, 1, 1)) * 29;
                float4 wave = flipbook(_Wave, saturate(lWave), float2(2, 15), waveFrame) * edgeFade(lWave, float2(0.05, 0.1));
                float waveAlpha = curve4(waveLife, float4(0, 0.44, 0.94, 1), float4(0, 1, 1, 0)) * step(waveLife, 0.999);
                float3 waveColor = lerp(float3(0.04, 0.82, 0.82), float3(0.91, 0.96, 0.96), saturate(waveLife / 0.81));
                layerAdd(accF, waveColor * wave.a * waveAlpha * 3);
                float front = sweepFront(0.3);
                float trail = m1 * exp(-abs(uv.x - front) * 8) * rowFade(uv);
                layerAdd(accF, float3(0.82, 1, 0.92) * trail * 0.35 * sweepActive());
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
