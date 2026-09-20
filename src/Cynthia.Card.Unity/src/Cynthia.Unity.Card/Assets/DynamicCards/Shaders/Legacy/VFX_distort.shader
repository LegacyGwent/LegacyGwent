Shader "DynamicCards/Legacy/VFX_distort" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_DistorValue("_DistorValue",Float)=0.0
_Map("_Map",2D)="white" {}
_timespeed("_timespeed",Float)=0.0
_timespeed2("_timespeed2",Float)=0.0
_UV_swich("_UV_swich",Float)=0.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,0.0)
_ColorMulti("_ColorMulti",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass { "Refraction" }
Pass {
Cull Back
ZWrite Off
ZTest Always
Blend One OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState samplerRefraction;
SamplerState sampler_MainTex;
SamplerState sampler_Map;
Texture2D<float4> Refraction;
Texture2D<float4> _MainTex;
Texture2D<float4> _Map;
float _ColorMulti;
float _DistorValue;
float _UV_swich;
float _timespeed2;
float _timespeed;
float4 _MainTex_ST;
float4 _Map_ST;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0,
  out float4 o3 : TEXCOORD1)
{
float4 dc_v_cb2[21];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
dc_v_cb2[4]=float4(0,0,0,0);
dc_v_cb2[5]=float4(0,0,0,0);
dc_v_cb2[6]=float4(0,0,0,0);
dc_v_cb2[7]=float4(0,0,0,0);
dc_v_cb2[8]=float4(0,0,0,0);
dc_v_cb2[9]=float4(unity_MatrixV[0][0],unity_MatrixV[1][0],unity_MatrixV[2][0],unity_MatrixV[3][0]);
dc_v_cb2[10]=float4(unity_MatrixV[0][1],unity_MatrixV[1][1],unity_MatrixV[2][1],unity_MatrixV[3][1]);
dc_v_cb2[11]=float4(unity_MatrixV[0][2],unity_MatrixV[1][2],unity_MatrixV[2][2],unity_MatrixV[3][2]);
dc_v_cb2[12]=float4(unity_MatrixV[0][3],unity_MatrixV[1][3],unity_MatrixV[2][3],unity_MatrixV[3][3]);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r1.xyzw;
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  r0.y = dc_v_cb2[10].z * r0.y;
  r0.x = dc_v_cb2[9].z * r0.x + r0.y;
  r0.x = dc_v_cb2[11].z * r0.z + r0.x;
  r0.x = dc_v_cb2[12].z * r0.w + r0.x;
  o3.z = -r0.x;
  r0.x = dc_v_cb0[5].x * r1.y;
  r0.w = 0.5 * r0.x;
  r0.xz = float2(0.5,0.5) * r1.xw;
  o3.w = r1.w;
  o3.xy = r0.xw + r0.zz;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  float4 v3 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_DistorValue,0,0,0);
dc_f_cb0[4]=float4(_Map_ST.x,_Map_ST.y,_Map_ST.z,_Map_ST.w);
dc_f_cb0[5]=float4(_timespeed,_timespeed2,_UV_swich,0);
dc_f_cb0[6]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[7]=float4(_ColorMulti,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy + -v1.yx;
  r0.xy = dc_f_cb0[5].zz * r0.xy + v1.yx;
  r0.xy = dc_f_cb1[0].xx * dc_f_cb0[5].xy + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xw = dc_f_cb0[3].xx * r0.yz;
  r0.xy = r0.xw * r0.yz;
  r0.zw = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Map.Sample(sampler_Map, r0.zw).xyzw;
  r0.xy = r1.xy * r0.xy;
  r0.z = v2.w * r0.x;
  r0.z = r0.z / dc_f_cb0[3].x;
  r0.z = dc_f_cb0[6].w * r0.z;
  r0.z = v2.w * r0.z;
  r1.xy = v3.xy / v3.ww;
  r0.xy = r0.xy * v2.ww + r1.xy;
  r1.xyzw = Refraction.Sample(samplerRefraction, r0.xy).xyzw;
  r0.xyw = dc_f_cb0[6].xyz * dc_f_cb0[7].xxx + -r1.xyz;
  o0.xyz = r0.zzz * r0.xyw + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
