Shader "DynamicCards/Legacy/VFX_Special_Unicorn_detmold_bubbles" {
Properties {
_MainTex("_MainTex",2D)="bump" {}
_Lightdirection("_Lightdirection",Vector)=(0.0,1.0,0.0,0.0)
_Fluidcolor("_Fluidcolor",Color)=(0.47058820724487305,0.0,0.0,1.0)
_Speccolor("_Speccolor",Color)=(1.0,0.3517240881919861,0.0,1.0)
_Specularexponent("_Specularexponent",Float)=100.0
_Specularmultiplier("_Specularmultiplier",Float)=0.10000000149011612
_Fresnelexponent("_Fresnelexponent",Float)=2.0
_Fresnelmultiplier("_Fresnelmultiplier",Float)=6.0
_FresnelSolid("_FresnelSolid",Float)=1.0
_noise("_noise",2D)="white" {}
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
SamplerState sampler_noise;
Texture2D<float4> _MainTex;
Texture2D<float4> _noise;
float _FresnelSolid;
float _Fresnelexponent;
float _Fresnelmultiplier;
float _Specularexponent;
float _Specularmultiplier;
float4 _Fluidcolor;
float4 _Lightdirection;
float4 _MainTex_ST;
float4 _Speccolor;
float4 _TimeEditor;
float4 _noise_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TANGENT0,
  float2 v3 : TEXCOORD0,
  float4 v4 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float3 o5 : TEXCOORD4,
  out float4 o6 : COLOR0)
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



  float4 r0,r1,r2;
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
  o1.xy = v3.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o3.xyz = r0.xyz;
  r1.xyz = dc_v_cb0[1].xyz * v2.yyy;
  r1.xyz = dc_v_cb0[0].xyz * v2.xxx + r1.xyz;
  r1.xyz = dc_v_cb0[2].xyz * v2.zzz + r1.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  o4.xyz = r1.xyz;
  r2.xyz = r1.yzx * r0.zxy;
  r0.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r0.xyz = v2.www * r0.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o5.xyz = r0.xyz * r0.www;
  o6.xyzw = v4.xyzw;
  return;
}






















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float3 v5 : TEXCOORD4,
  float4 v6 : COLOR0,
  uint v7 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[3];
dc_f_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_f_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_f_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Fluidcolor.x,_Fluidcolor.y,_Fluidcolor.z,_Fluidcolor.w);
dc_f_cb0[5]=float4(_Speccolor.x,_Speccolor.y,_Speccolor.z,_Speccolor.w);
dc_f_cb0[6]=float4(_Specularexponent,_Specularmultiplier,_FresnelSolid,0);
dc_f_cb0[7]=float4(_Lightdirection.x,_Lightdirection.y,_Lightdirection.z,_Lightdirection.w);
dc_f_cb0[8]=float4(_Fresnelexponent,_Fresnelmultiplier,0,0);
dc_f_cb0[9]=float4(_noise_ST.x,_noise_ST.y,_noise_ST.z,_noise_ST.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.xy = r0.xx * float2(0.200000003,0.200000003) + v1.xy;
  r0.xy = dc_f_cb0[9].xy * r0.xy;
  r0.xy = r0.xy * float2(0.5,0.5) + dc_f_cb0[9].zw;
  r0.xyzw = _noise.Sample(sampler_noise, r0.xy).xyzw;
  r0.xy = r0.xy * float2(0.100000001,0.100000001) + v1.xy;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.x = 1 + -r0.w;
  r1.x = r1.x * r0.w;
  r1.x = log2(r1.x);
  r1.x = dc_f_cb0[8].x * r1.x;
  r1.x = exp2(r1.x);
  r1.y = dc_f_cb0[8].y * r1.x;
  r1.x = -r1.x * dc_f_cb0[8].y + 1;
  r1.x = dc_f_cb0[6].z * r1.x + r1.y;
  r0.xyz = r0.xyz * float3(1,-1,1) + float3(0,1,0);
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r1.yzw = v5.xyz * r0.yyy;
  r1.yzw = r0.xxx * v4.xyz + r1.yzw;
  r0.xyz = r0.zzz * v3.xyz + r1.yzw;
  r1.y = dot(dc_f_cb0[7].xyz, dc_f_cb0[7].xyz);
  r1.y = rsqrt(r1.y);
  r1.yzw = dc_f_cb0[7].xyz * r1.yyy;
  r2.xyz = dc_f_cb2[1].xyz * r1.zzz;
  r2.xyz = dc_f_cb2[0].xyz * r1.yyy + r2.xyz;
  r1.yzw = dc_f_cb2[2].xyz * r1.www + r2.xyz;
  r0.x = dot(r0.xyz, r1.yzw);
  r0.x = saturate(r0.x * 0.5 + 0.5);
  r0.y = log2(r0.x);
  r0.y = dc_f_cb0[6].x * r0.y;
  r0.y = exp2(r0.y);
  r0.z = r0.y * dc_f_cb0[6].y + r1.x;
  r0.y = dc_f_cb0[6].y * r0.y;
  r0.z = r0.z * r0.w;
  o0.w = v6.w * r0.z;
  r1.xyz = dc_f_cb0[4].xyz * v6.xyz;
  r0.xzw = r1.xyz * r0.xxx;
  r1.xyz = dc_f_cb0[5].xyz * v6.xyz;
  o0.xyz = r1.xyz * r0.yyy + r0.xzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
