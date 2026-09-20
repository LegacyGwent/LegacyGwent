// UGUI adaptation of the original deckbuilder PremiumShine_Glitter*Slot and
// PremiumShine_*Slot materials. Uses their original noise, shine, timing and tint.
Shader "LegacyGwent/UI/PremiumBorder"
{
    Properties
    {
        [PerRendererData] _MainTex ("Border sprite", 2D) = "white" {}
        _NoiseTex ("Original Noise27White", 2D) = "white" {}
        _SecondTex ("Original PortalNoise", 2D) = "white" {}
        _ShineTex ("Original CardShineTex", 2D) = "black" {}
        _GlitterTint ("Glitter tint", Color) = (1,1,1,1)
        _ShineTint ("Shine tint", Color) = (1,1,1,.0980392)
        _SpriteUV ("Sprite UV bounds", Vector) = (0,0,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; float4 local : TEXCOORD1; };
            sampler2D _MainTex, _NoiseTex, _SecondTex, _ShineTex;
            float4 _GlitterTint, _ShineTint, _SpriteUV, _ClipRect, _TextureSampleAdd;
            v2f vert(appdata v)
            {
                v2f o; o.local = v.vertex; o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; o.color = v.color; return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                float4 border = tex2D(_MainTex, i.uv) + _TextureSampleAdd;
                float2 uv = (i.uv - _SpriteUV.xy) / max(_SpriteUV.zw - _SpriteUV.xy, .00001);
                // Original UI/PremiumGlitter: scrolling noise scales (4,1)/(3,2),
                // gains 10/1, alpha gain 3, alpha power 1.3.
                float4 noise = tex2D(_NoiseTex, (uv + _Time.y * float2(-.05,.025)) * float2(4,1));
                float4 second = tex2D(_SecondTex, (uv + _Time.y * float2(.03,-.015)) * float2(3,2));
                float modulation = second.r * second.a;
                float alpha = saturate(3 * pow(max(0, noise.a * modulation), 1.3));
                float3 glitter = noise.rgb * 10 * modulation * _GlitterTint.rgb;
                // Original MaskShader sweep: speed -.8, loop time 1.5,
                // shine scale (.1,1), offset (.5,.5), intensity tint.a * 4.
                float phase = fmod(_Time.y * -.8, 1.5);
                float2 shineUV = float2(uv.y - phase, uv.x + phase) * float2(.1,1) + .5;
                float3 shine = tex2D(_ShineTex, shineUV).rgb * _ShineTint.rgb * _ShineTint.a * 4;
                // Use this client's border silhouette instead of stretching the old horizontal slot mask.
                float4 color = float4(lerp(border.rgb, glitter, alpha) + shine, border.a) * i.color;
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.local.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - .001);
                #endif
                return color;
            }
            ENDCG
        }
    }
}
