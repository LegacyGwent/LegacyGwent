Shader "DynamicCards/Native/VFX_Special_glassFrost" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MixNoise("_MixNoise",2D)="white" {}
_Noisespeed("_Noisespeed",Float)=0.0
_newUV("_newUV",2D)="white" {}
_Emissionmultiplier("_Emissionmultiplier",Float)=1.0
_Emissionexponent("_Emissionexponent",Float)=1.0
_Opacityexponent("_Opacityexponent",Float)=0.0
_Opacitymultplier("_Opacitymultplier",Float)=0.0
_Opacityadd("_Opacityadd",Float)=0.0
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
SamplerState sampler_MixNoise;
SamplerState sampler_newUV;
Texture2D<float4> _MainTex;
Texture2D<float4> _MixNoise;
Texture2D<float4> _newUV;
float _Emissionexponent;
float _Emissionmultiplier;
float _Noisespeed;
float _Opacityadd;
float _Opacityexponent;
float _Opacitymultplier;
float4 _MainTex_ST;
float4 _MixNoise_ST;
float4 _TimeEditor;
float4 _newUV_ST;












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
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_MixNoise_ST.x,_MixNoise_ST.y,_MixNoise_ST.z,_MixNoise_ST.w);
dc_f_cb0[5]=float4(_Noisespeed,0,0,0);
dc_f_cb0[6]=float4(_newUV_ST.x,_newUV_ST.y,_newUV_ST.z,_newUV_ST.w);
dc_f_cb0[7]=float4(_Emissionmultiplier,_Emissionexponent,_Opacityexponent,_Opacitymultplier);
dc_f_cb0[8]=float4(_Opacityadd,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r0.xyzw = _newUV.Sample(sampler_newUV, r0.xy).xyzw;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.x = dc_f_cb0[5].x * r0.z;
  r1.y = 0;
  r0.xy = r1.xy + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r0.xyzw = _MixNoise.Sample(sampler_MixNoise, r0.xy).xyzw;
  r0.x = saturate(3.5 * r0.x);
  r0.x = saturate(-dc_f_cb0[8].x + r0.x);
  r0.x = 1 + -r0.x;
  r0.yz = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.yz).xyzw;
  r0.yzw = r1.xyz * r0.xxx;
  r2.xyz = log2(r0.yzw);
  r0.y = dot(r0.yzw, float3(0.300000012,0.589999974,0.109999999));
  r0.y = log2(r0.y);
  r0.y = dc_f_cb0[7].z * r0.y;
  r0.y = exp2(r0.y);
  r0.y = saturate(dc_f_cb0[7].w * r0.y);
  r0.y = dc_f_cb0[8].x + r0.y;
  o0.w = r1.w * r0.y;
  r0.yzw = dc_f_cb0[7].yyy * r2.xyz;
  r0.yzw = exp2(r0.yzw);
  r0.yzw = saturate(dc_f_cb0[7].xxx * r0.yzw);
  o0.xyz = saturate(r1.xyz * r0.xxx + r0.yzw);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
