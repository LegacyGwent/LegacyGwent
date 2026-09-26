Shader "DynamicCards/Legacy/VFX_Effects_MagicTrail" {
Properties {
_MoveSpeed("_MoveSpeed",Float)=0.0
_noiseSTR("_noiseSTR",Float)=0.0
_NoiseSpeed("_NoiseSpeed",Float)=0.0
_noise1("_noise1",2D)="white" {}
_MainTex("_MainTex",2D)="white" {}
_noise2("_noise2",2D)="white" {}
_NoiseSpeed_copy("_NoiseSpeed_copy",Float)=0.0
_strength("_strength",Float)=1.0
_Hue("_Hue",Float)=0.0
_range("_range",Float)=1.0
_min("_min",Float)=0.0
_max("_max",Float)=1.0
_mask("_mask",Float)=0.0
_Noise3Highligts("_Noise3Highligts",2D)="white" {}
_Noise3Speed("_Noise3Speed",Float)=0.0
_Noise3Contrast("_Noise3Contrast",Float)=0.0
_Noise3Brightness("_Noise3Brightness",Float)=1.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
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
SamplerState sampler_Noise3Highligts;
SamplerState sampler_noise1;
SamplerState sampler_noise2;
Texture2D<float4> _MainTex;
Texture2D<float4> _Noise3Highligts;
Texture2D<float4> _noise1;
Texture2D<float4> _noise2;
float _Hue;
float _MoveSpeed;
float _Noise3Brightness;
float _Noise3Contrast;
float _Noise3Speed;
float _NoiseSpeed;
float _NoiseSpeed_copy;
float _mask;
float _max;
float _min;
float _noiseSTR;
float _range;
float _strength;
float4 _MainTex_ST;
float4 _Noise3Highligts_ST;
float4 _TimeEditor;
float4 _noise1_ST;
float4 _noise2_ST;












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
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[12];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_noiseSTR,_NoiseSpeed,0,0);
dc_f_cb0[4]=float4(_noise1_ST.x,_noise1_ST.y,_noise1_ST.z,_noise1_ST.w);
dc_f_cb0[5]=float4(_NoiseSpeed_copy,0,0,0);
dc_f_cb0[6]=float4(_noise2_ST.x,_noise2_ST.y,_noise2_ST.z,_noise2_ST.w);
dc_f_cb0[7]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[8]=float4(_strength,_Hue,_range,_min);
dc_f_cb0[9]=float4(_max,_MoveSpeed,_mask,0);
dc_f_cb0[10]=float4(_Noise3Highligts_ST.x,_Noise3Highligts_ST.y,_Noise3Highligts_ST.z,_Noise3Highligts_ST.w);
dc_f_cb0[11]=float4(_Noise3Speed,_Noise3Contrast,_Noise3Brightness,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r1.y = dc_f_cb0[3].y * r0.x + v1.y;
  r1.x = r0.x * dc_f_cb0[9].y + v1.x;
  r0.yz = r1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r2.xyzw = _noise1.Sample(sampler_noise1, r0.yz).xyzw;
  r1.z = v1.y;
  r0.yz = r1.xz * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _noise1.Sample(sampler_noise1, r0.yz).xyzw;
  r0.y = dot(r2.xx, r1.xx);
  r0.y = -1 + r0.y;
  r0.yz = dc_f_cb0[3].xx * r0.yy + v1.xy;
  r0.yz = r0.yz * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.yz).xyzw;
  r0.y = saturate(r1.x);
  r2.z = dc_f_cb0[5].x * r0.x + v1.x;
  r2.y = dc_f_cb0[11].x * r0.x + v1.x;
  r2.xw = v1.yy;
  r0.xz = r2.zw * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.yz = r2.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r2.xyzw = _Noise3Highligts.Sample(sampler_Noise3Highligts, r1.yz).xyzw;
  r1.yzw = log2(r2.xyz);
  r1.yzw = dc_f_cb0[11].yyy * r1.yzw;
  r1.yzw = exp2(r1.yzw);
  r1.yzw = dc_f_cb0[11].zzz * r1.yzw;
  r2.xyzw = _noise2.Sample(sampler_noise2, r0.xz).xyzw;
  r0.x = r2.x * 2 + dc_f_cb0[8].x;
  r0.x = saturate(-1 + r0.x);
  r0.z = dc_f_cb0[8].w * r0.x;
  r0.w = dc_f_cb0[9].x * r0.x + -r0.z;
  r0.x = r1.x * r0.x + dc_f_cb0[8].y;
  r2.xyz = r0.xxx * dc_f_cb0[8].zzz + float3(0,-0.333333343,0.333333343);
  r2.xyz = frac(r2.xyz);
  r2.xyz = -r2.xyz * float3(2,2,2) + float3(1,1,1);
  r2.xyz = saturate(abs(r2.xyz) * float3(3,3,3) + float3(-1,-1,-1));
  r0.x = saturate(r0.y * r0.w + r0.z);
  r0.y = dc_f_cb0[9].z + v1.x;
  r0.z = 1 + -r0.y;
  r0.yz = r1.xx * r0.yz;
  r0.x = r0.z * r0.x;
  r0.x = r0.y * r0.x;
  r0.x = 5 * r0.x;
  r0.xyz = r0.xxx * r2.xyz;
  o0.xyz = r1.yzw * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
