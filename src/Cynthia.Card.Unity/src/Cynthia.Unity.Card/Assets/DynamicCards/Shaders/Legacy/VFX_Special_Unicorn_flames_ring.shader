Shader "DynamicCards/Legacy/VFX_Special_Unicorn_flames_ring" {
Properties {
_textures("_textures",2D)="white" {}
_noise("_noise",2D)="white" {}
_brightness("_brightness",Float)=11.0
_power("_power",Float)=2.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend One One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_noise;
SamplerState sampler_textures;
Texture2D<float4> _noise;
Texture2D<float4> _textures;
float _brightness;
float _power;
float4 _noise_ST;
float4 _textures_ST;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
{
float4 dc_v_cb1[21];
dc_v_cb1[0]=float4(0,0,0,0);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(0,0,0,0);
dc_v_cb1[6]=float4(0,0,0,0);
dc_v_cb1[7]=float4(0,0,0,0);
dc_v_cb1[8]=float4(0,0,0,0);
dc_v_cb1[9]=float4(0,0,0,0);
dc_v_cb1[10]=float4(0,0,0,0);
dc_v_cb1[11]=float4(0,0,0,0);
dc_v_cb1[12]=float4(0,0,0,0);
dc_v_cb1[13]=float4(0,0,0,0);
dc_v_cb1[14]=float4(0,0,0,0);
dc_v_cb1[15]=float4(0,0,0,0);
dc_v_cb1[16]=float4(0,0,0,0);
dc_v_cb1[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb1[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb1[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb1[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb1[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb1[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[5];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_textures_ST.x,_textures_ST.y,_textures_ST.z,_textures_ST.w);
dc_f_cb0[3]=float4(_noise_ST.x,_noise_ST.y,_noise_ST.z,_noise_ST.w);
dc_f_cb0[4]=float4(_brightness,_power,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xz = float2(3,3) * v1.xy;
  r0.y = dc_f_cb1[0].y * -0.300000012 + r0.x;
  r0.xy = r0.yz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _noise.Sample(sampler_noise, r0.xy).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[4].yyy * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = min(float3(1,1,1), r0.xyz);
  r1.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _textures.Sample(sampler_textures, r1.xy).xyzw;
  r1.xyz = v2.xyz * r1.xyz;
  o0.w = v2.w * r1.w;
  r1.xyz = v2.www * r1.xyz;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[4].xxx * r0.xyz;
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(1.20000005,1.20000005,1.20000005) * r0.xyz;
  o0.xyz = exp2(r0.xyz);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
