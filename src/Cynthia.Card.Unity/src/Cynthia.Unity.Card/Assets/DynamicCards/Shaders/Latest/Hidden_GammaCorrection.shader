Shader "DynamicCards/Latest/Hidden_GammaCorrection" {
Properties {
_InvertedGamma("_InvertedGamma",Float)=1.0
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
SamplerState sampler_LUT;
SamplerState sampler_MainTex;
Texture2D<float4> _LUT;
Texture2D<float4> _MainTex;
float4 _LUTStripParams;








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
float4 dc_f_cb0[3];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LUTStripParams.x,_LUTStripParams.y,_LUTStripParams.z,_LUTStripParams.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[2].y;
  r0.y = 0;
  r0.zw = float2(0.5,0.5) * dc_f_cb0[2].xy;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r2.xyz = dc_f_cb0[2].zzz * r1.zxy;
  r3.yz = r2.yz * dc_f_cb0[2].xy + r0.zw;
  r0.z = floor(r2.x);
  r3.x = r0.z * dc_f_cb0[2].y + r3.y;
  r0.z = r1.z * dc_f_cb0[2].z + -r0.z;
  o0.w = r1.w;
  r0.xy = r3.xz + r0.xy;
  r1.xyzw = _LUT.Sample(sampler_LUT, r3.xz).xyzw;
  r2.xyzw = _LUT.Sample(sampler_LUT, r0.xy).xyzw;
  r0.xyw = r2.xyz + -r1.xyz;
  o0.xyz = r0.zzz * r0.xyw + r1.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
