Shader "DynamicCards/Latest/VFX_UI_HighlightGradientShader" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Progress("_Progress",Float)=0.0
_GlowValue("_GlowValue",Float)=0.0
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_Squash("_Squash",Float)=0.5
}
SubShader {
Tags {"RenderType"="Transparent"}
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
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float _GlowValue;
float _Progress;
float _Squash;
float3 _TintColor;
float4 _MainTex_ST;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float2 o0 : TEXCOORD0,
  out float4 o1 : SV_POSITION0)
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
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[3];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v1.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o1.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  return;
}














void dc_f(
  float2 v0 : TEXCOORD0,
  float4 v1 : SV_POSITION0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[5];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_GlowValue);
dc_f_cb0[4]=float4(_Progress,_Squash,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = -1 + dc_f_cb0[4].y;
  r0.x = r0.x * -30 + 10;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v0.xy).xyzw;
  r0.y = -1 + r1.z;
  r0.y = dc_f_cb0[4].x * 1.04999995 + r0.y;
  r0.x = saturate(r0.x * r0.y);
  r0.yzw = dc_f_cb0[3].xyz * r1.yyy;
  r0.yzw = r0.yzw * r0.xxx;
  r1.z = r1.x * r0.x;
  r1.x = saturate(r1.x + r1.y);
  o0.w = r1.x * r0.x;
  r0.x = dc_f_cb0[3].w * r1.z;
  o0.xyz = r0.yzw * float3(0.600000024,0.600000024,0.600000024) + r0.xxx;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
