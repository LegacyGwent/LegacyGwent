Shader "DynamicCards/Native/VFX_Common_AlphaBlended_moveInDirection_VertexColor_Wavey_2Sided" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_MainU("_MainU",Float)=0.0
_MainV("_MainV",Float)=0.0
_Turbulence("_Turbulence",2D)="white" {}
_TurbU("_TurbU",Float)=0.0
_TurbV("_TurbV",Float)=0.0
_WaveyMask("_WaveyMask",2D)="white" {}
_VertexOffset("_VertexOffset",Float)=1.0
_AlphaPower("_AlphaPower",Float)=1.0
_AlphaMult("_AlphaMult",Float)=1.0
_ColorGain("_ColorGain",Float)=1.0
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
SamplerState sampler_Turbulence;
SamplerState sampler_WaveyMask;
Texture2D<float4> _MainTex;
Texture2D<float4> _Turbulence;
Texture2D<float4> _WaveyMask;
float _AlphaMult;
float _AlphaPower;
float _ColorGain;
float _MainU;
float _MainV;
float _TurbU;
float _TurbV;
float _VertexOffset;
float4 _MainTex_ST;
float4 _TintColor;
float4 _Turbulence_ST;
float4 _WaveyMask_ST;
























void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
float4 dc_v_cb2[4];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[9];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_Turbulence_ST.x,_Turbulence_ST.y,_Turbulence_ST.z,_Turbulence_ST.w);
dc_v_cb0[6]=float4(_TurbU,_TurbV,0,0);
dc_v_cb0[7]=float4(_WaveyMask_ST.x,_WaveyMask_ST.y,_WaveyMask_ST.z,_WaveyMask_ST.w);
dc_v_cb0[8]=float4(_VertexOffset,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_v_cb1[0].yy * dc_v_cb0[6].xy + v1.xy;
  r0.xy = r0.xy * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r0.xyzw = _Turbulence.SampleLevel(sampler_Turbulence, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.xyz = dc_v_cb0[8].xxx * r0.xyz;
  r1.xy = v1.xy * dc_v_cb0[7].xy + dc_v_cb0[7].zw;
  r1.xyzw = _WaveyMask.SampleLevel(sampler_WaveyMask, r1.xy, 0).xyzw;
  r0.xyz = r0.xyz * r1.xxx + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  return;
}
















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  uint v3 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[4]=float4(_MainU,_MainV,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(0,_AlphaPower,_AlphaMult,_ColorGain);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb1[0].yy * dc_f_cb0[4].xy + v1.xy;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = dc_f_cb0[3].xyzw * r0.xyzw;
  r0.w = v2.w * r0.w;
  o0.xyz = dc_f_cb0[8].www * r0.xyz;
  r0.x = log2(r0.w);
  r0.x = dc_f_cb0[8].y * r0.x;
  r0.x = exp2(r0.x);
  o0.w = dc_f_cb0[8].z * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
