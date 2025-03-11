Shader "Custom/GreyscaleShader" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

 
        Pass 
        {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
 
                #include "UnityCG.cginc"
 
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
 
                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };
 
                sampler2D _MainTex;
                float4 _MainTex_ST;
 
                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }
 
                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    return fixed4(grey, grey, grey, col.a);
                }
                ENDCG
        }
        
    }
    FallBack "Diffuse"
}
