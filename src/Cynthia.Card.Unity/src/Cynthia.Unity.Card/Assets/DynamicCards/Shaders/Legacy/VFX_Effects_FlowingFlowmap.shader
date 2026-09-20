Shader "DynamicCards/Legacy/VFX_Effects_FlowingFlowmap" {
Properties {
_MainTex("_MainTex",2D)="bump" {}
_FlowMap("_FlowMap",2D)="white" {}
_Offsetamount("_Offsetamount",Float)=-0.6499999761581421
_Timemultiplier("_Timemultiplier",Float)=0.25
_Secondaryflowmap("_Secondaryflowmap",2D)="white" {}
_Secondaryamount("_Secondaryamount",Float)=0.10000000149011612
_Secondaryspeed("_Secondaryspeed",Float)=0.10000000149011612
_Rotatespeed("_Rotatespeed",Float)=0.20000000298023224
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_FlowMap;
SamplerState sampler_MainTex;
SamplerState sampler_Secondaryflowmap;
Texture2D<float4> _FlowMap;
Texture2D<float4> _MainTex;
Texture2D<float4> _Secondaryflowmap;
float _Offsetamount;
float _Rotatespeed;
float _Secondaryamount;
float _Secondaryspeed;
float _Timemultiplier;
float4 _FlowMap_ST;
float4 _MainTex_ST;
float4 _Secondaryflowmap_ST;
float4 _TimeEditor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Offsetamount,_Timemultiplier,0,0);
dc_f_cb0[5]=float4(_Secondaryflowmap_ST.x,_Secondaryflowmap_ST.y,_Secondaryflowmap_ST.z,_Secondaryflowmap_ST.w);
dc_f_cb0[6]=float4(_FlowMap_ST.x,_FlowMap_ST.y,_FlowMap_ST.z,_FlowMap_ST.w);
dc_f_cb0[7]=float4(_Secondaryamount,_Secondaryspeed,_Rotatespeed,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = float2(-0.5,-0.5) + v1.xy;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.xy = dc_f_cb0[7].zy * r0.zz;
  sincos(r1.x, r1.x, r2.x);
  r0.w = frac(r1.y);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.xy, r3.xy);
  r1.x = dot(r0.xy, r3.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _Secondaryflowmap.Sample(sampler_Secondaryflowmap, r0.xy).xyzw;
  r0.xy = r1.xy * float2(2,2) + float2(-1,-1);
  r0.xy = dc_f_cb0[7].xx * r0.xy;
  r1.x = dc_f_cb0[7].y * r0.z + 0.5;
  r1.x = frac(r1.x);
  r1.xy = r0.xy * r1.xx + v1.xy;
  r0.xy = r0.xy * r0.ww + v1.xy;
  r0.w = -0.5 + r0.w;
  r0.w = r0.w + r0.w;
  r0.xy = r0.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r2.xyzw = _FlowMap.Sample(sampler_FlowMap, r0.xy).xyzw;
  r0.xy = r1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _FlowMap.Sample(sampler_FlowMap, r0.xy).xyzw;
  r0.xy = r1.xy + -r2.xy;
  r0.xy = abs(r0.ww) * r0.xy + r2.xy;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.w = r0.z * dc_f_cb0[4].y + 0.5;
  r0.xyz = dc_f_cb0[4].xxy * r0.xyz;
  r0.zw = frac(r0.zw);
  r1.xy = r0.xy * r0.ww + v1.xy;
  r0.xy = r0.xy * r0.zz + v1.xy;
  r0.z = -0.5 + r0.z;
  r0.z = r0.z + r0.z;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.xyzw = r1.xyzw + -r2.xyzw;
  o0.xyzw = abs(r0.zzzz) * r1.xyzw + r2.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
