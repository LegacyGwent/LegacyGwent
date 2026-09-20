Shader "DynamicCards/Native/VFX_Effects_Glass" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_NoiseRefl("_NoiseRefl",2D)="white" {}
_ReflNoiseStrenght("_ReflNoiseStrenght",Float)=0.0
_ReflectionFlowSpeed("_ReflectionFlowSpeed",Float)=0.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_OpacityMask("_OpacityMask",Float)=0.0
_OpacityBlur("_OpacityBlur",Float)=1.0
_FresnelValue("_FresnelValue",Float)=2.0
_FresnelBorder("_FresnelBorder",Float)=0.0
_SpecularNoise("_SpecularNoise",2D)="white" {}
_Reflection("_Reflection",Float)=0.0
_SpecularBlur("_SpecularBlur",Float)=1.0
_SpecularValue("_SpecularValue",Float)=0.0
_SpeccularColor("_SpeccularColor",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass {  }
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
SamplerState sampler_GrabTexture;
SamplerState sampler_MainTex;
SamplerState sampler_NoiseRefl;
SamplerState sampler_SpecularNoise;
Texture2D<float4> _GrabTexture;
Texture2D<float4> _MainTex;
Texture2D<float4> _NoiseRefl;
Texture2D<float4> _SpecularNoise;
float _FresnelBorder;
float _FresnelValue;
float _OpacityBlur;
float _OpacityMask;
float _ReflNoiseStrenght;
float _Reflection;
float _ReflectionFlowSpeed;
float _SpecularBlur;
float _SpecularValue;
float4 _MainTex_ST;
float4 _NoiseRefl_ST;
float4 _SpeccularColor;
float4 _SpecularNoise_ST;
float4 _TimeEditor;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3)
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
  r0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o0.xyzw = r0.xyzw;
  o4.xyzw = r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  return;
}




























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[12];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_ReflNoiseStrenght,_FresnelValue,0,0);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_OpacityMask,_OpacityBlur,_FresnelBorder,_Reflection);
dc_f_cb0[7]=float4(_NoiseRefl_ST.x,_NoiseRefl_ST.y,_NoiseRefl_ST.z,_NoiseRefl_ST.w);
dc_f_cb0[8]=float4(_SpecularNoise_ST.x,_SpecularNoise_ST.y,_SpecularNoise_ST.z,_SpecularNoise_ST.w);
dc_f_cb0[9]=float4(_SpecularBlur,_SpecularValue,0,0);
dc_f_cb0[10]=float4(_SpeccularColor.x,_SpeccularColor.y,_SpeccularColor.z,_SpeccularColor.w);
dc_f_cb0[11]=float4(_ReflectionFlowSpeed,0,0,0);



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
  r0.w = dot(-r0.xyz, r1.xyz);
  r0.w = r0.w + r0.w;
  r2.xy = r1.xy * -r0.ww + -r0.xy;
  r0.x = dot(r1.xyz, r0.xyz);
  r0.x = max(0, r0.x);
  r0.x = 1 + -r0.x;
  r0.x = saturate(r0.x * dc_f_cb0[4].y + dc_f_cb0[6].z);
  r0.yz = r2.xy * dc_f_cb0[6].ww + v1.xy;
  r0.yz = r0.yz * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r1.xyzw = _SpecularNoise.Sample(sampler_SpecularNoise, r0.yz).xyzw;
  r0.yzw = dc_f_cb0[10].xyz * r1.xxx;
  r0.yzw = r0.yzw * dc_f_cb0[9].xxx + dc_f_cb0[9].yyy;
  r1.x = dc_f_cb0[6].x + v1.y;
  r1.x = saturate(dc_f_cb0[6].y * r1.x);
  r0.yzw = saturate(r1.xxx * r0.yzw);
  r1.x = 1 + -r1.x;
  r1.yz = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r1.yz).xyzw;
  r0.yzw = r2.xyz + r0.yzw;
  r1.y = saturate(r2.w + r0.x);
  r0.xyz = dc_f_cb0[5].xyz * r0.xxx + r0.yzw;
  r0.w = dc_f_cb0[5].w * r1.y;
  r1.y = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r2.y = r1.y * dc_f_cb0[11].x + v1.y;
  r2.x = v1.x;
  r1.yz = r2.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r2.xyzw = _NoiseRefl.Sample(sampler_NoiseRefl, r1.yz).xyzw;
  r1.xy = r2.xy * r1.xx;
  r1.z = -dc_f_cb1[5].x * dc_f_cb1[5].x;
  r2.xy = v4.xy / v4.ww;
  r2.z = r2.y * r1.z;
  r1.zw = r2.xz * float2(0.5,0.5) + float2(0.5,0.5);
  r1.xy = r1.xy * dc_f_cb0[4].xx + r1.zw;
  r1.xyzw = _GrabTexture.Sample(sampler_GrabTexture, r1.xy).xyzw;
  r0.xyz = -r1.xyz + r0.xyz;
  o0.xyz = r0.www * r0.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
