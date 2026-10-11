// UI version of the beta VFX shaders used by the arena (VFX/Common/*_moveInDirection_VertexColor_UVNoise, AdditiveAlpha,
// AlphaBlended_alphaPower): the main texture scrolls (_MainU/_MainV per second) and is distorted by a scrolling noise
// texture (_NoiseU/_NoiseV, strength _NoiseMult). Drawn on a RawImage (its texture is _MainTex, its colour the vertex colour).
// _Additive 1 = Blend One One, 0 = alpha blended. _Polar maps the quad to polar coordinates for the mirror vortex:
// u = angle + radius * twist, v = radius between inner (z) and outer (w) radius.
Shader "Arena/FX"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "gray" {}
        _TintColor ("Tint", Color) = (1,1,1,1)
        _MainScale ("Main tiling (xy) offset (zw)", Vector) = (1,1,0,0)
        _NoiseScale ("Noise tiling (xy) offset (zw)", Vector) = (1,1,0,0)
        _MainU ("Main scroll U", Float) = 0
        _MainV ("Main scroll V", Float) = 0
        _NoiseU ("Noise scroll U", Float) = 0
        _NoiseV ("Noise scroll V", Float) = 0
        _NoiseMult ("Noise strength", Float) = 0
        _ColorGain ("Colour gain", Float) = 1
        _AlphaPower ("Alpha power", Float) = 1
        _AlphaMult ("Alpha multiplier", Float) = 1
        _Intensity ("Intensity", Float) = 1
        _Edge ("Soft edge x, y (uv fraction)", Vector) = (0,0,0,0)
        _Polar ("Polar: on, twist, inner, outer", Vector) = (0,0,0,1)
        _TimeOffset ("Time offset", Float) = 0
        _Additive ("Additive", Float) = 1
        _SrcBlend ("Src blend", Float) = 1
        _DstBlend ("Dst blend", Float) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #pragma multi_compile __ UNITY_UI_CLIP_RECT

            struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; float4 world : TEXCOORD1; };

            sampler2D _MainTex, _NoiseTex;
            fixed4 _TintColor;
            float4 _MainScale, _NoiseScale, _Edge, _Polar, _ClipRect;
            float _MainU, _MainV, _NoiseU, _NoiseV, _NoiseMult, _ColorGain, _AlphaPower, _AlphaMult, _Intensity, _TimeOffset, _Additive;

            v2f vert(appdata v)
            {
                v2f o;
                o.world = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y + _TimeOffset;
                float2 uv = i.uv;
                float mask = 1;
                if (_Polar.x > 0.5)
                {
                    float2 d = uv * 2 - 1;
                    float r = length(d);
                    float a = atan2(d.y, d.x) / 6.2831853 + 0.5;
                    uv = float2(a + r * _Polar.y, saturate((r - _Polar.z) / max(0.001, _Polar.w - _Polar.z)));
                    mask = smoothstep(_Polar.w, _Polar.w * 0.8, r) * smoothstep(_Polar.z * 0.6, _Polar.z, r);
                }
                float2 noise = tex2D(_NoiseTex, uv * _NoiseScale.xy + _NoiseScale.zw + float2(_NoiseU, _NoiseV) * t).rg - 0.5;
                float2 muv = (uv + noise * _NoiseMult) * _MainScale.xy + _MainScale.zw + float2(_MainU, _MainV) * t;
                fixed4 c = tex2D(_MainTex, muv);
                c.rgb *= _TintColor.rgb * i.color.rgb * _ColorGain;
                float alpha = pow(saturate(c.a), _AlphaPower) * _TintColor.a * i.color.a * _AlphaMult * _Intensity * mask;
                if (_Edge.x > 0) alpha *= smoothstep(0, _Edge.x, i.uv.x) * smoothstep(0, _Edge.x, 1 - i.uv.x);
                if (_Edge.y > 0) alpha *= smoothstep(0, _Edge.y, i.uv.y) * smoothstep(0, _Edge.y, 1 - i.uv.y);
                #ifdef UNITY_UI_CLIP_RECT
                alpha *= UnityGet2DClipping(i.world.xy, _ClipRect);
                #endif
                return _Additive > 0.5 ? fixed4(c.rgb * alpha, 0) : fixed4(c.rgb, saturate(alpha));
            }
            ENDCG
        }
    }
}
