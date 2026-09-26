Shader "DynamicCards/SourceLightning/Thronebreaker/Shader_Forge_LightningShader" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Turbulence("_Turbulence",2D)="white" {}
_Strength("_Strength",Float)=0.0
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_Width("_Width",Float)=1.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
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
SamplerState sampler_MainTex;
SamplerState sampler_Turbulence;
Texture2D<float4> _MainTex;
Texture2D<float4> _Turbulence;
float _Strength;
float _Width;
float4 _MainTex_ST;
float4 _TintColor;
float4 _Turbulence_ST;


















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TEXCOORD0,
  float2 v3 : TEXCOORD1,
  float4 v4 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1,
  out float4 o2 : TEXCOORD2,
  out float3 o3 : TEXCOORD3,
  out float4 o4 : COLOR0)
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
float4 dc_v_cb1[7];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(_Turbulence_ST.x,_Turbulence_ST.y,_Turbulence_ST.z,_Turbulence_ST.w);
dc_v_cb0[4]=float4(_Strength,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(_Width,0,0,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v3.xy * dc_v_cb0[3].xy + dc_v_cb0[3].zw;
  r0.xyzw = _Turbulence.SampleLevel(sampler_Turbulence, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.xyz = dc_v_cb0[4].xxx * r0.xyz;
  r1.xyz = dc_v_cb0[6].xxx * v1.xyz;
  r1.xyz = v4.zzz * r1.xyz;
  r0.xyz = v4.yyy * r0.xyz + r1.xyz;
  r0.xyz = v0.xyz + r0.xyz;
  r1.xyzw = dc_v_cb1[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb2[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb2[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r1.wwww + r0.xyzw;
  dc_pack_OSGN_1.xy = v2.xy;
  dc_pack_OSGN_1.zw = v3.xy;
  r0.x = dot(v1.xyz, dc_v_cb1[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb1[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb1[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  o4.xyzw = v4.xyzw;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  float4 v2 : TEXCOORD2,
  float3 v3 : TEXCOORD3,
  float4 v4 : COLOR0,
  uint v5 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyz = v4.xxx * r0.xyz;
  r0.xyz = dc_f_cb0[5].xyz * r0.xyz;
  o0.xyz = r0.xyz + r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
