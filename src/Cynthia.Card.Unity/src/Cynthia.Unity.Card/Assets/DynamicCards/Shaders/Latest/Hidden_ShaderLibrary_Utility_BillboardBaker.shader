Shader "DynamicCards/Latest/Hidden_ShaderLibrary_Utility_BillboardBaker" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Normal("_Normal",2D)="bump" {}
_Mask("_Mask",2D)="white" {}
_FlagValues("_FlagValues",Vector)=(0.0,0.0,0.0,0.0)
_RemapValues("_RemapValues",Vector)=(0.10000000149011612,0.699999988079071,0.8500000238418579,0.949999988079071)
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull Off
ZWrite On
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_Normal;
Texture2D<float4> _MainTex;
Texture2D<float4> _Normal;
float _Cutoff;
float4 _FlagValues;
float4 _MainTex_ST;
float4 _RemapValues;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  float3 v3 : NORMAL0,
  float4 v4 : TANGENT0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float4 o5 : TEXCOORD4,
  out float4 o6 : TEXCOORD5,
  out float3 o7 : TEXCOORD6)
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
dc_v_cb2[9]=float4(0,0,0,0);
dc_v_cb2[10]=float4(0,0,0,0);
dc_v_cb2[11]=float4(0,0,0,0);
dc_v_cb2[12]=float4(0,0,0,0);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[10];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
dc_v_cb1[7]=float4(unity_WorldToObject[0][3],unity_WorldToObject[1][3],unity_WorldToObject[2][3],unity_WorldToObject[3][3]);
dc_v_cb1[8]=float4(0,0,0,0);
dc_v_cb1[9]=float4(unity_WorldTransformParams.x,unity_WorldTransformParams.y,unity_WorldTransformParams.z,unity_WorldTransformParams.w);
float4 dc_v_cb0[3];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  o2.xyzw = v2.xyzw;
  o3.xyz = v3.xyz;
  o4.xyz = v0.xyz;
  r0.x = dot(v3.xyz, dc_v_cb1[4].xyz);
  r0.y = dot(v3.xyz, dc_v_cb1[5].xyz);
  r0.z = dot(v3.xyz, dc_v_cb1[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o5.xyz = r0.xyz;
  r1.xyz = dc_v_cb1[1].xyz * v4.yyy;
  r1.xyz = dc_v_cb1[0].xyz * v4.xxx + r1.xyz;
  r1.xyz = dc_v_cb1[2].xyz * v4.zzz + r1.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  o6.xyz = r1.xyz;
  r2.xyz = r1.yzx * r0.zxy;
  r0.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r0.w = dc_v_cb1[9].w * v4.w;
  o7.xyz = r0.xyz * r0.www;
  return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float4 v5 : TEXCOORD4,
  float4 v6 : TEXCOORD5,
  float3 v7 : TEXCOORD6,
  uint v8 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0,
  out float4 o1 : SV_Target1)
{
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_FlagValues.x,_FlagValues.y,_FlagValues.z,_FlagValues.w);
dc_f_cb0[6]=float4(_RemapValues.x,_RemapValues.y,_RemapValues.z,_RemapValues.w);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[8]=float4(_Cutoff,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r0.w = r0.w * dc_f_cb0[7].w + -dc_f_cb0[8].x;
  r0.xyz = dc_f_cb0[7].xyz * r0.xyz;
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r0.w = v3.y * r0.w;
  r0.w = v8.x ? r0.w : -r0.w;
  r0.w = -dc_f_cb0[6].z + r0.w;
  r1.xy = dc_f_cb0[6].yw + -dc_f_cb0[6].xz;
  r1.xy = float2(1,1) / r1.xy;
  r0.w = saturate(r1.y * r0.w);
  r1.y = r0.w * -2 + 3;
  r0.w = r0.w * r0.w;
  r0.w = r1.y * r0.w;
  r1.y = saturate(v4.y / dc_f_cb0[5].y);
  r1.y = -dc_f_cb0[6].x + r1.y;
  r1.x = saturate(r1.y * r1.x);
  r1.y = r1.x * -2 + 3;
  r1.x = r1.x * r1.x;
  r1.x = r1.y * r1.x;
  r0.w = r1.x * r0.w;
  r1.xyz = -r0.xyz * v2.www + float3(1,1,1);
  r0.xyz = v2.www * r0.xyz;
  r1.xyz = r0.www * r1.xyz + r0.xyz;
  r2.xy = cmp(dc_f_cb0[5].xy != float2(1,0));
  o0.xyz = r2.yyy ? r1.xyz : r0.xyz;
  o0.w = 1;
  r0.xyzw = _Normal.Sample(sampler_Normal, v1.xy).xyzw;
  r0.x = r0.x * r0.w;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r1.xyz = v7.xyz * r0.yyy;
  r1.xyz = r0.xxx * v6.xyz + r1.xyz;
  r0.x = dot(r0.xy, r0.xy);
  r0.x = min(1, r0.x);
  r0.x = 1 + -r0.x;
  r0.x = sqrt(r0.x);
  r0.xyz = r0.xxx * v5.xyz + r1.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  r0.w = dot(v5.xyz, v5.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = v5.xyz * r0.www;
  r0.xyz = r2.xxx ? r1.xyz : r0.xyz;
  r1.xyz = float3(-1,-1,1) * r0.xyz;
  r0.w = -r0.z;
  r0.z = cmp((int)v8.x == 0);
  r1.w = cmp(dc_f_cb0[5].z == 0.000000);
  r0.z = r0.z ? r1.w : 0;
  r0.xyz = r0.zzz ? r1.xyz : r0.xyw;
  o1.xyz = r0.xyz * float3(0.5,0.5,0.5) + float3(0.5,0.5,0.5);
  o1.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
