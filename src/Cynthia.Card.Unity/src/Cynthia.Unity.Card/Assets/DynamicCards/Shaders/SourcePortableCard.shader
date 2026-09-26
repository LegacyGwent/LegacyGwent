Shader "DynamicCards/Compatibility/PortableCard"
{
    Properties
    {
        _MainTex ("Image", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _NoiseTex ("UV noise", 2D) = "gray" {}
        _MatCap ("Reflection", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _TintColor ("Tint", Color) = (1,1,1,1)
        _UVSpeed ("UV speed", Vector) = (0,0,0,0)
        _NoiseAmount ("UV distortion", Float) = 0
        _Reflection ("Reflection amount", Float) = 0
        _Brightness ("Brightness", Float) = 1
        _Cutoff ("Alpha threshold", Range(0,1)) = 0.01
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source blend", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 10
        [Enum(Off,0,On,1)] _ZWrite ("Depth write", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            ZWrite [_ZWrite]
            Cull [_Cull]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex, _Mask, _NoiseTex, _MatCap;
            float4 _MainTex_ST, _Mask_ST, _NoiseTex_ST, _UVSpeed;
            fixed4 _Color, _TintColor;
            float _NoiseAmount, _Reflection, _Brightness, _Cutoff;
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float2 mask:TEXCOORD1; float2 matcap:TEXCOORD2; fixed4 color:COLOR; };
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + _Time.y * _UVSpeed.xy;
                o.mask = TRANSFORM_TEX(v.uv, _Mask);
                o.matcap = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal).xy * .5 + .5;
                o.color = v.color * _Color * _TintColor;
                return o;
            }
            fixed4 frag(v2f i):SV_Target
            {
                float2 noise = (tex2D(_NoiseTex, i.uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw).rg - .5) * _NoiseAmount;
                fixed4 color = tex2D(_MainTex, i.uv + noise) * i.color;
                color.a *= tex2D(_Mask, i.mask).a;
                clip(color.a - _Cutoff);
                color.rgb = color.rgb * _Brightness + tex2D(_MatCap, i.matcap).rgb * _Reflection * color.a;
                return color;
            }
            ENDCG
        }
    }
}
