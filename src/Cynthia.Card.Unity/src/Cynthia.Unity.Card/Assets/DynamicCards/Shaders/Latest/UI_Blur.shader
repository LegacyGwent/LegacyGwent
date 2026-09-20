Shader "DynamicCards/Latest/UI_Blur" {
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
SamplerState sampler_UIGrab;
Texture2D<float4> _MainTex;
Texture2D<float4> _UIGrab;








void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2)
{




  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v0.xy;
  o0.zw = float2(0,1);
  r0.xy = float2(1,1) + v0.xy;
  o1.xy = r0.xy * float2(0.5,-0.5) + float2(0,1);
  o2.xyzw = float4(0,0,0,0);
  o3.xyzw = float4(0,0,0,0);
  return;
}
















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{




  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _UIGrab.Sample(sampler_UIGrab, v1.xy).xyzw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r0.xyzw = -r1.xyzw + r0.xyzw;
  o0.xyzw = r0.xyzw * float4(0.0879999995,0.0879999995,0.0879999995,0.0879999995) + r1.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
