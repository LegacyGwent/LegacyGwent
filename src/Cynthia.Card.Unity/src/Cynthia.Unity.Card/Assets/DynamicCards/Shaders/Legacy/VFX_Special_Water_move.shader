Shader "DynamicCards/Legacy/VFX_Special_Water_move" {
Properties {
_TimeSpeed("_TimeSpeed",Float)=1.0
_MainText("_MainText",2D)="white" {}
_Noise_contrast("_Noise_contrast",Float)=1.0
_Noise_Brightnes("_Noise_Brightnes",Float)=0.0
_TintColor_01("_TintColor_01",Color)=(1.0,1.0,1.0,1.0)
_GradientBlur("_GradientBlur",Float)=1.0
_GradientSize("_GradientSize",Float)=0.0
_RadialLinear("_RadialLinear",Float)=0.0
_OpacityNoise("_OpacityNoise",2D)="white" {}
_OpacityBlur("_OpacityBlur",Float)=1.0
_OpacityPower("_OpacityPower",Float)=0.0
_FloatMask("_FloatMask",2D)="white" {}
_MaskContrast("_MaskContrast",Float)=1.0
_MaskBrightness("_MaskBrightness",Float)=0.0
_Brightnes("_Brightnes",Float)=0.0
_Contrast("_Contrast",Float)=1.0
_SpecularPlacement("_SpecularPlacement",Float)=0.0
_SpecularSize("_SpecularSize",Float)=0.0
_SpecularColour("_SpecularColour",Color)=(1.0,1.0,1.0,1.0)
_camrot("_camrot",Float)=1.0
_camrot_offset("_camrot_offset",Float)=1.0
_camrot2("_camrot2",Float)=1.0
_SpecularBrightessosso("_SpecularBrightessosso",Float)=1.0
_reflectionsmgf("_reflectionsmgf",Float)=1.0
_Tint2("_Tint2",Color)=(0.0,0.0,0.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass { "Refraction" }
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
SamplerState samplerRefraction;
SamplerState sampler_FloatMask;
SamplerState sampler_MainText;
SamplerState sampler_OpacityNoise;
Texture2D<float4> Refraction;
Texture2D<float4> _FloatMask;
Texture2D<float4> _MainText;
Texture2D<float4> _OpacityNoise;
float _Brightnes;
float _Contrast;
float _GradientBlur;
float _GradientSize;
float _MaskBrightness;
float _MaskContrast;
float _Noise_Brightnes;
float _Noise_contrast;
float _OpacityBlur;
float _OpacityPower;
float _RadialLinear;
float _SpecularBrightessosso;
float _SpecularPlacement;
float _SpecularSize;
float _TimeSpeed;
float _camrot2;
float _camrot;
float _camrot_offset;
float _reflectionsmgf;
float4 _FloatMask_ST;
float4 _MainText_ST;
float4 _OpacityNoise_ST;
float4 _SpecularColour;
float4 _TimeEditor;
float4 _Tint2;
float4 _TintColor_01;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1)
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
  r0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r0.xyzw;
  o2.xyzw = r0.xyzw;
  o1.xy = v1.xy;
  return;
}




























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[15];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_OpacityNoise_ST.x,_OpacityNoise_ST.y,_OpacityNoise_ST.z,_OpacityNoise_ST.w);
dc_f_cb0[4]=float4(_TimeSpeed,_Noise_contrast,_Noise_Brightnes,0);
dc_f_cb0[5]=float4(_MainText_ST.x,_MainText_ST.y,_MainText_ST.z,_MainText_ST.w);
dc_f_cb0[6]=float4(_TintColor_01.x,_TintColor_01.y,_TintColor_01.z,_TintColor_01.w);
dc_f_cb0[7]=float4(_GradientBlur,_GradientSize,_OpacityBlur,_OpacityPower);
dc_f_cb0[8]=float4(_FloatMask_ST.x,_FloatMask_ST.y,_FloatMask_ST.z,_FloatMask_ST.w);
dc_f_cb0[9]=float4(_MaskBrightness,_MaskContrast,_Brightnes,_Contrast);
dc_f_cb0[10]=float4(_SpecularPlacement,_SpecularSize,0,0);
dc_f_cb0[11]=float4(_SpecularColour.x,_SpecularColour.y,_SpecularColour.z,_SpecularColour.w);
dc_f_cb0[12]=float4(_camrot,_camrot_offset,_camrot2,_SpecularBrightessosso);
dc_f_cb0[13]=float4(_reflectionsmgf,_RadialLinear,0,0);
dc_f_cb0[14]=float4(_Tint2.x,_Tint2.y,_Tint2.z,_Tint2.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.z = v1.y;
  r0.w = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r0.xy = -r0.ww * dc_f_cb0[4].xx + v1.xy;
  r1.xy = r0.ww * dc_f_cb0[4].xx + v1.xy;
  r0.xz = r0.xz * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r2.w = 0.236540005 + r0.y;
  r0.xyzw = _MainText.Sample(sampler_MainText, r0.xz).xyzw;
  r2.x = 0.236540005 + r1.x;
  r2.yz = v1.yx;
  r0.yz = r2.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xw = r2.zw * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r2.xyzw = _MainText.Sample(sampler_MainText, r1.xw).xyzw;
  r3.xyzw = _MainText.Sample(sampler_MainText, r0.yz).xyzw;
  r0.x = r3.x * r0.x;
  r1.z = v1.x;
  r0.yz = r1.zy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _MainText.Sample(sampler_MainText, r0.yz).xyzw;
  r0.x = r1.x * r0.x;
  r0.x = r0.x * r2.x;
  r0.y = 0.100000001 * dc_f_cb0[4].z;
  r0.x = saturate(r0.x * dc_f_cb0[4].y + r0.y);
  r0.y = r0.x * 2 + -1;
  r0.y = dc_f_cb0[13].x * r0.y;
  r1.xy = v2.xy / v2.ww;
  r1.z = dc_f_cb1[5].x * r1.y;
  r0.z = dot(r1.xz, r1.xz);
  r0.z = sqrt(r0.z);
  r0.z = 1 + -r0.z;
  r0.zw = dc_f_cb0[12].zx * r0.zz;
  r0.zw = r0.zw * float2(-0.00999999978,0.00999999978) + r1.xz;
  r0.zw = r0.zw * float2(0.5,0.5) + float2(0.5,0.5);
  r1.xy = r0.zw * float2(0.600000024,0.600000024) + float2(0.200000003,0.200000003);
  r1.xy = r1.xy + -r0.zw;
  r0.yz = r0.yy * r1.xy + r0.zw;
  r1.xyzw = Refraction.Sample(samplerRefraction, r0.yz).xyzw;
  r0.xyz = r1.xyz * r0.xxx;
  r0.xyz = r0.xyz * dc_f_cb0[9].www + dc_f_cb0[9].zzz;
  r0.xyz = saturate(dc_f_cb0[6].xyz * r0.xyz);
  r1.xyz = float3(1,1,1) + -r0.xyz;
  r0.w = dot(r1.xyz, float3(0.300000012,0.589999974,0.109999999));
  r1.xy = v1.xy * float2(2,2) + float2(-1,-1);
  r1.x = dot(r1.xy, r1.xy);
  r1.x = sqrt(r1.x);
  r1.x = 1 + -r1.x;
  r1.x = -v1.y + r1.x;
  r1.x = dc_f_cb0[13].y * r1.x + v1.y;
  r1.x = r1.x * dc_f_cb0[7].x + dc_f_cb0[7].y;
  r1.y = dc_f_cb0[12].x * dc_f_cb0[12].y + dc_f_cb0[10].x;
  r1.y = r1.x + -r1.y;
  r1.zw = v1.xy * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r2.xyzw = _FloatMask.Sample(sampler_FloatMask, r1.zw).xyzw;
  r1.z = dot(r2.xyz, float3(0.300000012,0.589999974,0.109999999));
  r1.z = r1.z * dc_f_cb0[9].y + dc_f_cb0[9].x;
  r1.w = r1.z + r1.y;
  r1.y = -dc_f_cb0[10].y + r1.y;
  r1.xy = r1.zz + r1.xy;
  r2.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _OpacityNoise.Sample(sampler_OpacityNoise, r2.xy).xyzw;
  r1.z = r2.x * dc_f_cb0[7].z + dc_f_cb0[7].w;
  r1.xyw = saturate(r1.xyw + r1.zzz);
  r1.y = 1 + -r1.y;
  r1.z = r1.x + -r1.w;
  o0.w = r1.x;
  r1.x = r1.z + -r1.y;
  r1.x = max(0, r1.x);
  r1.x = dc_f_cb0[12].w * r1.x;
  r1.xyz = dc_f_cb0[11].xyz * r1.xxx;
  r1.xyz = dc_f_cb0[14].xyz * r0.www + r1.xyz;
  o0.xyz = r1.xyz + r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
