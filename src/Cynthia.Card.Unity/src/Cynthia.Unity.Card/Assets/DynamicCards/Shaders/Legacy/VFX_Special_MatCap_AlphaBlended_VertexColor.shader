Shader "DynamicCards/Legacy/VFX_Special_MatCap_AlphaBlended_VertexColor" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MatCap("_MatCap",2D)="white" {}
_SpecBrightness("_SpecBrightness",Float)=0.0
_SpecContrast("_SpecContrast",Float)=1.0
_Color("_Color",Color)=(1.0,1.0,1.0,1.0)
_LightRotation("_LightRotation",Float)=0.0
_LightRotationOffset("_LightRotationOffset",Float)=0.0
_NormalPower("_NormalPower",Float)=1.0
_Normal_Detail("_Normal_Detail",Float)=0.0
_LightMask("_LightMask",2D)="white" {}
_CutOut("_CutOut",Float)=1.0
_ColorMult("_ColorMult",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="TransparentCutout"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend One OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_LightMask;
SamplerState sampler_MainTex;
SamplerState sampler_MatCap;
Texture2D<float4> _LightMask;
Texture2D<float4> _MainTex;
Texture2D<float4> _MatCap;
float _ColorMult;
float _CutOut;
float _LightRotation;
float _LightRotationOffset;
float _NormalPower;
float _Normal_Detail;
float _SpecBrightness;
float _SpecContrast;
float4 _Color;
float4 _LightMask_ST;
float4 _MainTex_ST;
float4 _MatCap_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  float4 v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2,
  out float4 o4 : COLOR0)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb0[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb0[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb0[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  o4.xyzw = v3.xyzw;
  return;
}


























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  float4 v4 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[12];
dc_f_cb2[0]=float4(0,0,0,0);
dc_f_cb2[1]=float4(0,0,0,0);
dc_f_cb2[2]=float4(0,0,0,0);
dc_f_cb2[3]=float4(0,0,0,0);
dc_f_cb2[4]=float4(0,0,0,0);
dc_f_cb2[5]=float4(0,0,0,0);
dc_f_cb2[6]=float4(0,0,0,0);
dc_f_cb2[7]=float4(0,0,0,0);
dc_f_cb2[8]=float4(0,0,0,0);
dc_f_cb2[9]=float4(unity_MatrixV[0][0],unity_MatrixV[1][0],unity_MatrixV[2][0],unity_MatrixV[3][0]);
dc_f_cb2[10]=float4(unity_MatrixV[0][1],unity_MatrixV[1][1],unity_MatrixV[2][1],unity_MatrixV[3][1]);
dc_f_cb2[11]=float4(unity_MatrixV[0][2],unity_MatrixV[1][2],unity_MatrixV[2][2],unity_MatrixV[3][2]);
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_MatCap_ST.x,_MatCap_ST.y,_MatCap_ST.z,_MatCap_ST.w);
dc_f_cb0[4]=float4(_NormalPower,_LightRotation,_SpecBrightness,_SpecContrast);
dc_f_cb0[5]=float4(_Color.x,_Color.y,_Color.z,_Color.w);
dc_f_cb0[6]=float4(_LightMask_ST.x,_LightMask_ST.y,_LightMask_ST.z,_LightMask_ST.w);
dc_f_cb0[7]=float4(_CutOut,_Normal_Detail,_ColorMult,_LightRotationOffset);



  float4 r0,r1,r2,r3,r4;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dot(v3.xyz, v3.xyz);
  r0.x = rsqrt(r0.x);
  r0.xyz = v3.xyz * r0.xxx;
  r0.xyz = dc_f_cb0[4].xxx * r0.xyz;
  r1.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  r2.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
  r0.w = r2.x + r2.y;
  r0.w = r0.w + r2.z;
  r0.w = 0.333333343 * r0.w;
  r3.xyz = r1.xyz * r0.www;
  r1.xyz = r3.xyz * float3(-2,-2,-2) + r1.xyz;
  r0.xyz = r1.xyz * dc_f_cb0[7].yyy + r0.xyz;
  r0.yw = dc_f_cb2[10].xy * r0.yy;
  r0.xy = dc_f_cb2[9].xy * r0.xx + r0.yw;
  r0.xy = dc_f_cb2[11].xy * r0.zz + r0.xy;
  r0.xy = float2(0.5,0.5) * r0.xy;
  r0.z = dc_f_cb0[7].w + dc_f_cb0[4].y;
  sincos(r0.z, r1.x, r3.x);
  r4.z = r1.x;
  r4.y = r3.x;
  r4.x = -r1.x;
  r1.y = dot(r0.xy, r4.xy);
  r1.x = dot(r0.xy, r4.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MatCap.Sample(sampler_MatCap, r0.xy).xyzw;
  r0.x = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r0.x = saturate(r0.x * dc_f_cb0[4].w + dc_f_cb0[4].z);
  r0.yzw = dc_f_cb0[5].xyz * r0.xxx;
  r1.xy = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _LightMask.Sample(sampler_LightMask, r1.xy).xyzw;
  r3.xyz = -r0.yzw * r1.xyz + float3(1,1,1);
  r1.xyz = r1.xyz * r0.xxx;
  r0.x = dot(r1.xyz, float3(0.300000012,0.589999974,0.109999999));
  r1.xyz = r2.xyz / r3.xyz;
  r1.w = dc_f_cb0[7].x * r2.w;
  r1.w = r1.w * v4.w + -r0.x;
  r0.x = dc_f_cb0[5].w * r1.w + r0.x;
  r1.xyz = r1.xyz * v4.xyz + -r0.yzw;
  r0.yzw = saturate(dc_f_cb0[5].www * r1.xyz + r0.yzw);
  r0.yzw = dc_f_cb0[7].zzz * r0.yzw;
  o0.xyz = r0.yzw * r0.xxx;
  o0.w = r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
