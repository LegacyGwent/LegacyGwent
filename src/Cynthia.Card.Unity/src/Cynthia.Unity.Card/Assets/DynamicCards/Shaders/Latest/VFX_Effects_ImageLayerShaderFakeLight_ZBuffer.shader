Shader "DynamicCards/Latest/VFX_Effects_ImageLayerShaderFakeLight_ZBuffer" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_TintMainTex("_TintMainTex",Color)=(0.5,0.5,0.5,1.0)
_Light("_Light",2D)="white" {}
_LightMask("_LightMask",2D)="white" {}
_Turbulence("_Turbulence",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_LightIntensity("_LightIntensity",Float)=1.0
_Speed("_Speed",Float)=1.0
_TimeOffset("_TimeOffset",Float)=0.5
_Cutoff("_Cutoff",Float)=0.5
_Distortion("_Distortion",Float)=0.0
_DistortionSize("_DistortionSize",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite On
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_Light;
SamplerState sampler_LightMask;
SamplerState sampler_MainTex;
SamplerState sampler_Turbulence;
Texture2D<float4> _Light;
Texture2D<float4> _LightMask;
Texture2D<float4> _MainTex;
Texture2D<float4> _Turbulence;
float _Cutoff;
float _Distortion;
float _DistortionSize;
float _LightIntensity;
float _Speed;
float _TimeOffset;
float4 _MainTex_ST;
float4 _TintColor;
float4 _TintMainTex;
float4 _Turbulence_ST;












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
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_Turbulence_ST.x,_Turbulence_ST.y,_Turbulence_ST.z,_Turbulence_ST.w);
dc_f_cb0[6]=float4(_LightIntensity,_Speed,_TimeOffset,_Cutoff);
dc_f_cb0[7]=float4(_Distortion,_DistortionSize,0,0);
dc_f_cb0[8]=float4(_TintMainTex.x,_TintMainTex.y,_TintMainTex.z,_TintMainTex.w);
dc_f_cb0[9]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 0.5 + dc_f_cb0[6].y;
  r0.y = dc_f_cb1[0].x + dc_f_cb0[6].z;
  r0.x = r0.x * r0.y;
  r0.yz = v1.xy * dc_f_cb0[7].yy + r0.xx;
  r0.xw = r0.xx * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _Turbulence.Sample(sampler_Turbulence, r0.xw).xyzw;
  r0.xyzw = _Turbulence.Sample(sampler_Turbulence, r0.yz).xyzw;
  r0.zw = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xy = r0.xy * dc_f_cb0[7].xx + r0.zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.z = -dc_f_cb0[6].w + r2.w;
  r2.xyzw = dc_f_cb0[8].xyzw * r2.xyzw;
  r2.xyzw = r2.xyzw + r2.xyzw;
  r0.z = cmp(r0.z < 0);
  if (r0.z != 0) discard;
  r3.xyzw = _Light.Sample(sampler_Light, r0.xy).xyzw;
  r0.xyzw = _LightMask.Sample(sampler_LightMask, r0.xy).xyzw;
  r0.xyz = r3.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[9].xyz * r0.xyz;
  r0.xyz = dc_f_cb0[6].xxx * r0.xyz;
  r0.xyz = r0.xyz * r1.xxx;
  o0.xyz = r0.xyz * float3(2,2,2) + r2.xyz;
  o0.w = r2.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
