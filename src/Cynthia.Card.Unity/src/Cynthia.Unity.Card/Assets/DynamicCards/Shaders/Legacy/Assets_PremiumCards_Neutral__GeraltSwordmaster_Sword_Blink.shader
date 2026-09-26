Shader "DynamicCards/Legacy/Assets_PremiumCards_Neutral__GeraltSwordmaster_Sword_Blink" {
Properties {
_TEX_MAINHI_RES("_TEX_MAINHI_RES",2D)="white" {}
_TEX_BLINKLO_RES("_TEX_BLINKLO_RES",2D)="white" {}
_TEX_ShineRay("_TEX_ShineRay",2D)="white" {}
_ShineRay_UV_ROT("_ShineRay_UV_ROT",Float)=0.0
_BlinkOpacity("_BlinkOpacity",Float)=1.0
_Blink_Color("_Blink_Color",Color)=(0.5,0.5,0.5,1.0)
_ShineRay_Power("_ShineRay_Power",Float)=0.0
_ShineRay_Preview("_ShineRay_Preview",Float)=0.5
_OpacityONOFF("_OpacityONOFF",Float)=-1.0
_NormalMap("_NormalMap",2D)="bump" {}
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
SamplerState sampler_TEX_BLINKLO_RES;
SamplerState sampler_TEX_MAINHI_RES;
SamplerState sampler_TEX_ShineRay;
Texture2D<float4> _TEX_BLINKLO_RES;
Texture2D<float4> _TEX_MAINHI_RES;
Texture2D<float4> _TEX_ShineRay;
float _BlinkOpacity;
float _OpacityONOFF;
float _ShineRay_Power;
float _ShineRay_Preview;
float _ShineRay_UV_ROT;
float4 _Blink_Color;
float4 _TEX_BLINKLO_RES_ST;
float4 _TEX_MAINHI_RES_ST;
float4 _TEX_ShineRay_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TANGENT0,
  float2 v3 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float3 o4 : TEXCOORD3)
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



  float4 r0,r1,r2;
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
  o1.xy = v3.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o2.xyz = r0.xyz;
  r1.xyz = dc_v_cb0[1].xyz * v2.yyy;
  r1.xyz = dc_v_cb0[0].xyz * v2.xxx + r1.xyz;
  r1.xyz = dc_v_cb0[2].xyz * v2.zzz + r1.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  o3.xyz = r1.xyz;
  r2.xyz = r1.yzx * r0.zxy;
  r0.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r0.xyz = v2.www * r0.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o4.xyz = r0.xyz * r0.www;
  return;
}






















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float3 v4 : TEXCOORD3,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TEX_MAINHI_RES_ST.x,_TEX_MAINHI_RES_ST.y,_TEX_MAINHI_RES_ST.z,_TEX_MAINHI_RES_ST.w);
dc_f_cb0[3]=float4(_TEX_ShineRay_ST.x,_TEX_ShineRay_ST.y,_TEX_ShineRay_ST.z,_TEX_ShineRay_ST.w);
dc_f_cb0[4]=float4(_BlinkOpacity,0,0,0);
dc_f_cb0[5]=float4(_TEX_BLINKLO_RES_ST.x,_TEX_BLINKLO_RES_ST.y,_TEX_BLINKLO_RES_ST.z,_TEX_BLINKLO_RES_ST.w);
dc_f_cb0[6]=float4(_Blink_Color.x,_Blink_Color.y,_Blink_Color.z,_Blink_Color.w);
dc_f_cb0[7]=float4(_ShineRay_Power,_ShineRay_Preview,_OpacityONOFF,_ShineRay_UV_ROT);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 0.0174532924 * dc_f_cb0[7].w;
  sincos(r0.x, r0.x, r1.x);
  r2.z = r0.x;
  r0.yz = float2(-0.5,-0.5) + v1.xy;
  r2.y = r1.x;
  r2.x = -r0.x;
  r1.y = dot(r0.yz, r2.xy);
  r1.x = dot(r0.yz, r2.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _TEX_ShineRay.Sample(sampler_TEX_ShineRay, r0.xy).xyzw;
  r0.x = r0.x * r0.w;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[7].x * r0.x;
  r0.x = exp2(r0.x);
  r0.yz = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _TEX_MAINHI_RES.Sample(sampler_TEX_MAINHI_RES, r0.yz).xyzw;
  r0.yzw = dc_f_cb0[6].xyz + r1.xyz;
  r1.xyz = r0.xxx + -r0.yzw;
  r0.x = dc_f_cb0[4].x * r0.x;
  r0.yzw = dc_f_cb0[7].yyy * r1.xyz + r0.yzw;
  r1.xy = v1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r2.xyzw = _TEX_BLINKLO_RES.Sample(sampler_TEX_BLINKLO_RES, r1.xy).xyzw;
  r1.x = 1 + -r2.w;
  r1.x = r1.w + -r1.x;
  r0.x = r0.x * r1.x + -1;
  r0.x = dc_f_cb0[7].z * r0.x + 1;
  o0.xyz = r0.yzw * r0.xxx;
  o0.w = r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
