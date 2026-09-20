Shader "DynamicCards/Legacy/Custom_Cape" {
Properties {
_UVmultiply("_UVmultiply",Float)=10.0
_OffsetAmount("_OffsetAmount",Float)=0.0
_MainTex("_MainTex",2D)="white" {}
_Timemultiply("_Timemultiply",Float)=1.0
_Vmin("_Vmin",Float)=0.0
_Vto1correction("_Vto1correction",Float)=0.0
_Updownmovement("_Updownmovement",Float)=1.0
_VTimeMultiply("_VTimeMultiply",Float)=1.0
_Vmultiply("_Vmultiply",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float _OffsetAmount;
float _Timemultiply;
float _UVmultiply;
float _Updownmovement;
float _VTimeMultiply;
float _Vmin;
float _Vmultiply;
float _Vto1correction;
float4 _MainTex_ST;
float4 _TimeEditor;
















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float3 o2 : TEXCOORD1)
{
float4 dc_v_cb3[21];
dc_v_cb3[0]=float4(0,0,0,0);
dc_v_cb3[1]=float4(0,0,0,0);
dc_v_cb3[2]=float4(0,0,0,0);
dc_v_cb3[3]=float4(0,0,0,0);
dc_v_cb3[4]=float4(0,0,0,0);
dc_v_cb3[5]=float4(0,0,0,0);
dc_v_cb3[6]=float4(0,0,0,0);
dc_v_cb3[7]=float4(0,0,0,0);
dc_v_cb3[8]=float4(0,0,0,0);
dc_v_cb3[9]=float4(0,0,0,0);
dc_v_cb3[10]=float4(0,0,0,0);
dc_v_cb3[11]=float4(0,0,0,0);
dc_v_cb3[12]=float4(0,0,0,0);
dc_v_cb3[13]=float4(0,0,0,0);
dc_v_cb3[14]=float4(0,0,0,0);
dc_v_cb3[15]=float4(0,0,0,0);
dc_v_cb3[16]=float4(0,0,0,0);
dc_v_cb3[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb3[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb3[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb3[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb2[7];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb2[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb2[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb2[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(_UVmultiply,_OffsetAmount,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_Timemultiply,_Vmin,_Vto1correction,_Updownmovement);
dc_v_cb0[6]=float4(_VTimeMultiply,_Vmultiply,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb0[6].y * v2.y;
  r0.y = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.x = r0.y * dc_v_cb0[6].x + r0.x;
  r0.x = sin(r0.x);
  r1.xz = dc_v_cb0[5].ww;
  r1.y = 1;
  r0.xzw = r1.xyz * r0.xxx;
  r1.y = dc_v_cb0[5].w;
  r1.w = 1 + -v2.y;
  r1.w = -dc_v_cb0[5].y + r1.w;
  r1.x = dc_v_cb0[5].z * r1.w;
  r0.xzw = r1.xyx * r0.xzw;
  r1.z = 0;
  r0.xzw = r1.zxz * r0.xzw;
  r1.y = dc_v_cb0[3].x * v2.y;
  r0.y = r0.y * dc_v_cb0[5].x + r1.y;
  r0.y = sin(r0.y);
  r0.y = r0.y * 0.5 + 0.5;
  r1.yzw = v1.xyz * r0.yyy;
  r1.xyz = r1.yzw * r1.xxx;
  r0.xyz = r1.xyz * dc_v_cb0[3].yyy + r0.xzw;
  r0.xyz = v0.xyz + r0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb2[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb2[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb2[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o2.xyz = r0.xyz * r0.www;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float3 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[5];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  o0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
