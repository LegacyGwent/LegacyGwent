Shader "DynamicCards/Latest/VFX_Common_AlphaBlended_moveInDirection" {
Properties {
_Umultiplier("_Umultiplier",Float)=0.0
_Vmultiplier("_Vmultiplier",Float)=0.0
_Tint("_Tint",Color)=(0.5,0.5,0.5,1.0)
_Smoothopacityonedges("_Smoothopacityonedges",Float)=0.0
_Softhardalpha("_Softhardalpha",Float)=0.0
_Alphahardnesthreshold("_Alphahardnesthreshold",Float)=0.0
_Alphamultiplier("_Alphamultiplier",Float)=1.0
_AlphaPower("_AlphaPower",Float)=0.0
_MaskBorder("_MaskBorder",Float)=1.0
_MainTex("_MainTex",2D)="white" {}
_morphingShape("_morphingShape",Float)=0.0
_TrxtureColor("_TrxtureColor",Float)=0.0
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
float _AlphaPower;
float _Alphahardnesthreshold;
float _Alphamultiplier;
float _MaskBorder;
float _Smoothopacityonedges;
float _Softhardalpha;
float _TrxtureColor;
float _Umultiplier;
float _Vmultiplier;
float _morphingShape;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 _Tint;












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
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Umultiplier,_Vmultiplier,0,0);
dc_f_cb0[4]=float4(_Tint.x,_Tint.y,_Tint.z,_Tint.w);
dc_f_cb0[5]=float4(_Alphamultiplier,_Smoothopacityonedges,_Softhardalpha,_Alphahardnesthreshold);
dc_f_cb0[6]=float4(_AlphaPower,_MaskBorder,0,0);
dc_f_cb0[7]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[8]=float4(_morphingShape,_TrxtureColor,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = v3.xyz * r0.www;
  r0.x = dot(r1.xyz, r0.xyz);
  r0.x = max(0, r0.x);
  r0.x = 1 + -r0.x;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[6].y * r0.x;
  r0.x = exp2(r0.x);
  r0.x = 1 + -r0.x;
  r0.x = v4.w * r0.x;
  r0.x = dc_f_cb0[4].w * r0.x;
  r0.yz = float2(1,1) + -v1.xy;
  r0.yz = v1.xy * r0.yz;
  r0.y = 4 * r0.y;
  r0.y = r0.z * r0.y;
  r0.y = 4 * r0.y;
  r0.y = rsqrt(r0.y);
  r0.y = 1 / r0.y;
  r0.y = -1 + r0.y;
  r0.y = dc_f_cb0[5].y * r0.y + 1;
  r0.x = r0.x * r0.y;
  r0.y = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.zw = dc_f_cb0[3].xy * r0.yy;
  r1.xy = r0.yy * dc_f_cb0[3].xy + v1.xy;
  r1.xy = r1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r0.yz = r0.zw * float2(0.300000012,0.300000012) + v1.xy;
  r0.yz = r0.yz * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r0.yz).xyzw;
  r0.y = dot(r2.ww, r1.ww);
  r0.y = r0.y + -r1.w;
  r0.y = dc_f_cb0[8].x * r0.y + r1.w;
  r1.xyz = dc_f_cb0[4].xyz * r1.xyz;
  r0.y = saturate(r0.y * dc_f_cb0[5].x + dc_f_cb0[6].x);
  r0.x = saturate(r0.x * r0.y);
  r0.y = -dc_f_cb0[5].w + r0.x;
  r0.y = ceil(r0.y);
  r0.y = r0.y + -r0.x;
  o0.w = dc_f_cb0[5].z * r0.y + r0.x;
  r0.xyz = dc_f_cb0[4].xyz * v4.xyz;
  r1.xyz = r1.xyz * v4.xyz + -r0.xyz;
  o0.xyz = dc_f_cb0[8].yyy * r1.xyz + r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
