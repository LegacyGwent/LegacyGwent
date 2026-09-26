Shader "DynamicCards/Latest/Custom_triss_butterfly" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Uscale("_Uscale",Float)=1.0
_OffsetAmount("_OffsetAmount",Float)=10.0
_UtimeMultiplier("_UtimeMultiplier",Float)=1.0
_Vscale("_Vscale",Float)=1.0
_Timeoffset("_Timeoffset",Float)=1.0
_MinLightning("_MinLightning",Float)=0.20000000298023224
_MaxLightning("_MaxLightning",Float)=0.800000011920929
_Lightcolor("_Lightcolor",Color)=(0.36862748861312866,0.6352940797805786,0.847058892250061,1.0)
_Lightexponent("_Lightexponent",Float)=10.0
_Lightmultiplier("_Lightmultiplier",Float)=1.0
_Utiles("_Utiles",Float)=2.0
_Vtiles("_Vtiles",Float)=2.0
_Minoffsetmultiplier("_Minoffsetmultiplier",Float)=0.20000000298023224
_Umidvalue("_Umidvalue",Float)=0.0
_Uwidth("_Uwidth",Float)=1.0
_ColorSwitch("_ColorSwitch",Float)=0.0
_TINT("_TINT",Color)=(0.5,0.5,0.5,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
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
float _ColorSwitch;
float _Lightexponent;
float _Lightmultiplier;
float _MaxLightning;
float _MinLightning;
float _Minoffsetmultiplier;
float _OffsetAmount;
float _Timeoffset;
float _Umidvalue;
float _Uscale;
float _Utiles;
float _UtimeMultiplier;
float _Uwidth;
float _Vscale;
float _Vtiles;
float4 _Lightcolor;
float4 _MainTex_ST;
float4 _TINT;
float4 _TimeEditor;
















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
float4 dc_v_cb0[9];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_Uscale,_Vscale,_OffsetAmount,_UtimeMultiplier);
dc_v_cb0[5]=float4(_Timeoffset,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_Minoffsetmultiplier,_Umidvalue,_Uwidth,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.y = dc_v_cb0[4].w * v3.y;
  r0.x = r0.x * r0.y;
  r0.x = v3.y * dc_v_cb0[5].x + r0.x;
  r0.x = v2.y * dc_v_cb0[4].y + r0.x;
  r0.y = -dc_v_cb0[8].y + v2.x;
  r0.y = r0.y / dc_v_cb0[8].z;
  r0.x = r0.x + abs(r0.y);
  r0.x = dc_v_cb0[4].x * r0.x;
  r0.x = sin(r0.x);
  r0.z = 1 + -v3.z;
  r0.w = r0.z * r0.z;
  r0.z = dc_v_cb0[4].z * r0.z;
  r0.x = r0.x * r0.w + 1;
  r0.w = 1 + -dc_v_cb0[8].x;
  r0.x = r0.x * r0.w;
  r0.x = r0.x * 0.5 + dc_v_cb0[8].x;
  r0.x = abs(r0.y) * r0.x;
  r0.xyw = v1.xyz * r0.xxx;
  r0.xyz = r0.xyw * r0.zzz + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb2[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb3[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb3[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb3[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb2[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb2[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb2[6].xyz);
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
  uint v5 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Uscale,_Vscale,0,_UtimeMultiplier);
dc_f_cb0[5]=float4(_Timeoffset,_MinLightning,_MaxLightning,0);
dc_f_cb0[6]=float4(_Lightcolor.x,_Lightcolor.y,_Lightcolor.z,_Lightcolor.w);
dc_f_cb0[7]=float4(_Lightexponent,_Lightmultiplier,_Utiles,_Vtiles);
dc_f_cb0[8]=float4(0,_Umidvalue,_Uwidth,_ColorSwitch);
dc_f_cb0[9]=float4(_TINT.x,_TINT.y,_TINT.z,_TINT.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.y = dc_f_cb0[4].w * v4.y;
  r0.x = r0.x * r0.y;
  r0.x = v4.y * dc_f_cb0[5].x + r0.x;
  r0.x = v1.y * dc_f_cb0[4].y + r0.x;
  r0.y = -dc_f_cb0[8].y + v1.x;
  r0.y = r0.y / dc_f_cb0[8].z;
  r0.x = r0.x + abs(r0.y);
  r0.x = dc_f_cb0[4].x * r0.x;
  r0.x = sin(r0.x);
  r0.z = 1 + -v4.z;
  r0.z = r0.z * r0.z;
  r0.x = r0.x * r0.z;
  r0.z = cmp(0 < r0.y);
  r0.y = cmp(r0.y < 0);
  r0.y = (int)-r0.z + (int)r0.y;
  r0.y = (int)r0.y;
  r0.x = r0.x * r0.y;
  r0.x = r0.x * 0.5 + 0.5;
  r0.y = dc_f_cb0[5].z + -dc_f_cb0[5].y;
  r0.x = r0.x * r0.y + dc_f_cb0[5].y;
  r0.y = log2(r0.x);
  r0.y = dc_f_cb0[7].x * r0.y;
  r0.y = exp2(r0.y);
  r0.yzw = dc_f_cb0[6].xyz * r0.yyy;
  r0.yzw = dc_f_cb0[7].yyy * r0.yzw;
  r1.x = dc_f_cb0[7].z * dc_f_cb0[7].w;
  r1.x = v4.y * r1.x + -1;
  r1.x = trunc(r1.x);
  r1.yz = float2(1,1) / dc_f_cb0[7].zw;
  r1.w = r1.x * r1.y;
  r2.y = floor(r1.w);
  r2.x = -dc_f_cb0[7].z * r2.y + r1.x;
  r1.xw = abs(v1.xy) + r2.xy;
  r1.xy = r1.xw * r1.yz;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r2.xyz = r1.xyz * dc_f_cb0[9].xyz + -r1.xyz;
  r1.xyz = dc_f_cb0[8].www * r2.xyz + r1.xyz;
  r1.w = v4.w * r1.w;
  o0.w = dc_f_cb0[9].w * r1.w;
  o0.xyz = r1.xyz * r0.xxx + r0.yzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
