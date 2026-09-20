Shader "DynamicCards/Latest/Hidden_FullscreenTriangleCopy" {
Properties {

}
SubShader {
Tags {}
Pass {
Cull Off
ZWrite Off
ZTest Always
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;








void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
{




  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v0.xy;
  o0.zw = float2(0,1);
  r0.xy = float2(1,1) + v0.xy;
  o1.xy = r0.xy * float2(0.5,-0.5) + float2(0,1);
  return;
}












void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{




  o0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
