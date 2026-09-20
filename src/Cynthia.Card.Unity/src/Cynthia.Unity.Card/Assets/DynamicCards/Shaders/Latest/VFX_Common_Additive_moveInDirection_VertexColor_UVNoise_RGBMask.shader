Shader "DynamicCards/Latest/VFX_Common_Additive_moveInDirection_VertexColor_UVNoise_RGBMask" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_AlphaMult("_AlphaMult",Float)=1.0
_AlphaPower("_AlphaPower",Float)=1.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_MainU("_MainU",Float)=0.0
_MainV("_MainV",Float)=0.0
_NoiseU("_NoiseU",Float)=0.0
_NoiseV("_NoiseV",Float)=0.0
_NoiseMult("_NoiseMult",Float)=0.0
_ColorGain("_ColorGain",Float)=1.0
_NoiseTex("_NoiseTex",2D)="white" {}
_Turbulence("_Turbulence",2D)="white" {}
_TurbV("_TurbV",Float)=0.0
_TurbU("_TurbU",Float)=0.0
_VertexOffset("_VertexOffset",Float)=0.20000000298023224
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
SamplerState sampler_MainTex;
SamplerState sampler_NoiseTex;
SamplerState sampler_Turbulence;
Texture2D<float4> _MainTex;
Texture2D<float4> _NoiseTex;
Texture2D<float4> _Turbulence;
float _AlphaMult;
float _AlphaPower;
float _ColorGain;
float _MainU;
float _MainV;
float _NoiseMult;
float _NoiseU;
float _NoiseV;
float _TurbU;
float _TurbV;
float _VertexOffset;
float4 _MainTex_ST;
float4 _NoiseTex_ST;
float4 _TintColor;
float4 _Turbulence_ST;




















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
float4 dc_v_cb0[10];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(_TurbU,_TurbV,0,0);
dc_v_cb0[8]=float4(_Turbulence_ST.x,_Turbulence_ST.y,_Turbulence_ST.z,_Turbulence_ST.w);
dc_v_cb0[9]=float4(_VertexOffset,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_v_cb1[0].yy * dc_v_cb0[7].xy + v1.xy;
  r0.xy = r0.xy * dc_v_cb0[8].xy + dc_v_cb0[8].zw;
  r0.xyzw = _Turbulence.SampleLevel(sampler_Turbulence, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.xyz = r0.xyz * dc_v_cb0[9].xxx + v0.xyz;
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
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[4]=float4(_MainU,_MainV,_NoiseU,_NoiseV);
dc_f_cb0[5]=float4(_NoiseMult,_AlphaMult,_AlphaPower,_ColorGain);
dc_f_cb0[6]=float4(_NoiseTex_ST.x,_NoiseTex_ST.y,_NoiseTex_ST.z,_NoiseTex_ST.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_f_cb1[0].yyyy * dc_f_cb0[4].zwxy + v1.xyxy;
  r0.xy = r0.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _NoiseTex.Sample(sampler_NoiseTex, r0.xy).xyzw;
  r0.xy = r1.xy * dc_f_cb0[5].xx + r0.zw;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = dc_f_cb0[3].xyzw * r0.xyzw;
  r0.xyzw = v2.xyzw * r0.xyzw;
  r0.xyzw = dc_f_cb0[5].wwwy * r0.xyzw;
  r0.w = log2(r0.w);
  r0.w = dc_f_cb0[5].z * r0.w;
  r0.w = exp2(r0.w);
  o0.xyz = r0.xyz * r0.www;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
