Shader "DynamicCards/Latest/ShaderLibrary_Environment_GwentClouds" {
Properties {
_MainTex("_MainTex",2D)="grey" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_LightInfluence("_LightInfluence",Float)=1.0
_FallOffRemap("_FallOffRemap",Vector)=(-0.5,1.0,0.0,0.0)
_Scale("_Scale",Vector)=(4.0,4.0,4.0,0.0)
_AnimSpeed("_AnimSpeed",Vector)=(0.0,0.0,0.0,0.0)
_FlowMap("_FlowMap",2D)="black" {}
_FlowAnimSpeed("_FlowAnimSpeed",Float)=1.0
_FlowAnimDist("_FlowAnimDist",Float)=0.0
_MaskDistortion("_MaskDistortion",2D)="black" {}
_MaskAnimSpeed("_MaskAnimSpeed",Vector)=(0.0,0.0,0.0,0.0)
_VertexAlphaRemap("_VertexAlphaRemap",Vector)=(0.0,1.0,0.0,0.0)
_LightAlphaContribution("_LightAlphaContribution",Float)=0.5
_TransparencyAlphaContribution("_TransparencyAlphaContribution",Float)=0.5
}
SubShader {
Tags {}
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
SamplerState sampler_FlowMap;
SamplerState sampler_MainTex;
SamplerState sampler_MaskDistortion;
Texture2D<float4> _FlowMap;
Texture2D<float4> _MainTex;
Texture2D<float4> _MaskDistortion;
float _FlowAnimDist;
float _FlowAnimSpeed;
float _LightAlphaContribution;
float _LightInfluence;
float _TransparencyAlphaContribution;
float2 _AnimSpeed;
float4 _FallOffRemap;
float4 _MainTex_ST;
float4 _MaskAnimSpeed;
float4 _MaskDistortion_ST;
float4 _Scale;
float4 _TintColor;
float4 _VertexAlphaRemap;


















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD3,
  out float3 o4 : TEXCOORD4,
  out float4 o5 : COLOR0,
  out float2 o6 : TEXCOORD5)
{
float4 dc_v_cb4[21];
dc_v_cb4[0]=float4(0,0,0,0);
dc_v_cb4[1]=float4(0,0,0,0);
dc_v_cb4[2]=float4(0,0,0,0);
dc_v_cb4[3]=float4(0,0,0,0);
dc_v_cb4[4]=float4(0,0,0,0);
dc_v_cb4[5]=float4(0,0,0,0);
dc_v_cb4[6]=float4(0,0,0,0);
dc_v_cb4[7]=float4(0,0,0,0);
dc_v_cb4[8]=float4(0,0,0,0);
dc_v_cb4[9]=float4(0,0,0,0);
dc_v_cb4[10]=float4(0,0,0,0);
dc_v_cb4[11]=float4(0,0,0,0);
dc_v_cb4[12]=float4(0,0,0,0);
dc_v_cb4[13]=float4(0,0,0,0);
dc_v_cb4[14]=float4(0,0,0,0);
dc_v_cb4[15]=float4(0,0,0,0);
dc_v_cb4[16]=float4(0,0,0,0);
dc_v_cb4[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb4[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb4[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb4[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb3[4];
dc_v_cb3[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb3[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb3[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb3[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb2[46];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
dc_v_cb2[4]=float4(0,0,0,0);
dc_v_cb2[5]=float4(0,0,0,0);
dc_v_cb2[6]=float4(0,0,0,0);
dc_v_cb2[7]=float4(0,0,0,0);
dc_v_cb2[8]=float4(0,0,0,0);
dc_v_cb2[9]=float4(0,0,0,0);
dc_v_cb2[10]=float4(0,0,0,0);
dc_v_cb2[11]=float4(0,0,0,0);
dc_v_cb2[12]=float4(0,0,0,0);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(0,0,0,0);
dc_v_cb2[18]=float4(0,0,0,0);
dc_v_cb2[19]=float4(0,0,0,0);
dc_v_cb2[20]=float4(0,0,0,0);
dc_v_cb2[21]=float4(0,0,0,0);
dc_v_cb2[22]=float4(0,0,0,0);
dc_v_cb2[23]=float4(0,0,0,0);
dc_v_cb2[24]=float4(0,0,0,0);
dc_v_cb2[25]=float4(0,0,0,0);
dc_v_cb2[26]=float4(0,0,0,0);
dc_v_cb2[27]=float4(0,0,0,0);
dc_v_cb2[28]=float4(0,0,0,0);
dc_v_cb2[29]=float4(0,0,0,0);
dc_v_cb2[30]=float4(0,0,0,0);
dc_v_cb2[31]=float4(0,0,0,0);
dc_v_cb2[32]=float4(0,0,0,0);
dc_v_cb2[33]=float4(0,0,0,0);
dc_v_cb2[34]=float4(0,0,0,0);
dc_v_cb2[35]=float4(0,0,0,0);
dc_v_cb2[36]=float4(0,0,0,0);
dc_v_cb2[37]=float4(0,0,0,0);
dc_v_cb2[38]=float4(0,0,0,0);
dc_v_cb2[39]=float4(unity_SHAr.x,unity_SHAr.y,unity_SHAr.z,unity_SHAr.w);
dc_v_cb2[40]=float4(unity_SHAg.x,unity_SHAg.y,unity_SHAg.z,unity_SHAg.w);
dc_v_cb2[41]=float4(unity_SHAb.x,unity_SHAb.y,unity_SHAb.z,unity_SHAb.w);
dc_v_cb2[42]=float4(0,0,0,0);
dc_v_cb2[43]=float4(0,0,0,0);
dc_v_cb2[44]=float4(0,0,0,0);
dc_v_cb2[45]=float4(unity_SHC.x,unity_SHC.y,unity_SHC.z,unity_SHC.w);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[14];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(_MaskDistortion_ST.x,_MaskDistortion_ST.y,_MaskDistortion_ST.z,_MaskDistortion_ST.w);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_AnimSpeed.x,_AnimSpeed.y,0,0);
dc_v_cb0[9]=float4(_Scale.x,_Scale.y,_Scale.z,_Scale.w);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(_MaskAnimSpeed.x,_MaskAnimSpeed.y,_MaskAnimSpeed.z,_MaskAnimSpeed.w);
dc_v_cb0[12]=float4(0,0,0,0);
dc_v_cb0[13]=float4(0,0,_FlowAnimSpeed,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb3[0].w;
  r0.y = dc_v_cb3[1].w;
  r0.z = dc_v_cb3[2].w;
  r0.w = dc_v_cb3[3].w;
  r1.xyz = v0.xyz;
  r1.w = 1;
  r0.x = dot(r0.xyzw, r1.xyzw);
  r1.x = v0.z;
  r1.y = 1;
  r2.x = dc_v_cb0[9].z;
  r2.y = dc_v_cb3[3].z;
  r0.y = dot(r2.xy, r1.xy);
  r1.z = dot(r2.xy, v0.zw);
  r2.xz = v0.xy;
  r2.yw = float2(1,1);
  r3.xz = dc_v_cb0[9].xy;
  r3.yw = dc_v_cb3[3].xy;
  r0.z = dot(r3.zw, r2.zw);
  r0.w = dot(r3.xy, r2.xy);
  r2.xyzw = dc_v_cb4[18].xyzw * r0.zzzz;
  r2.xyzw = dc_v_cb4[17].xyzw * r0.wwww + r2.xyzw;
  r2.xyzw = dc_v_cb4[19].xyzw * r0.yyyy + r2.xyzw;
  o0.xyzw = dc_v_cb4[20].xyzw * r0.xxxx + r2.xyzw;
  r1.x = dot(r3.xy, v0.xw);
  o4.y = dot(r3.zw, v0.yw);
  r0.xy = r1.xz * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  o1.xy = dc_v_cb0[8].xy * dc_v_cb1[0].yy + r0.xy;
  r0.xy = r1.xz * dc_v_cb0[6].xy + dc_v_cb0[6].zw;
  o4.xz = r1.xz;
  o1.zw = dc_v_cb0[11].xy * dc_v_cb1[0].yy + r0.xy;
  r0.x = dc_v_cb1[0].x * dc_v_cb0[13].z + 0.5;
  o2.y = frac(r0.x);
  r0.x = dc_v_cb1[0].x * dc_v_cb0[13].z;
  r0.x = frac(r0.x);
  r0.y = 0.5 + -r0.x;
  o2.x = r0.x;
  r0.x = r0.y + r0.y;
  o2.z = abs(r0.x);
  r0.x = dot(dc_v_cb2[39].yw, float2(1,1));
  r0.y = dot(dc_v_cb2[40].yw, float2(1,1));
  r0.z = dot(dc_v_cb2[41].yw, float2(1,1));
  r0.xyz = -dc_v_cb2[45].xyz + r0.xyz;
  r0.xyz = max(float3(0,0,0), r0.xyz);
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(0.416666657,0.416666657,0.416666657) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = r0.xyz * float3(1.05499995,1.05499995,1.05499995) + float3(-0.0549999997,-0.0549999997,-0.0549999997);
  o3.xyz = max(float3(0,0,0), r0.xyz);
  o5.xyzw = v2.xyzw;
  o6.xy = float2(0,0);
  return;
}


























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD3,
  float3 v4 : TEXCOORD4,
  float4 v5 : COLOR0,
  float2 v6 : TEXCOORD5,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[7];
dc_f_cb2[0]=float4(0,0,0,0);
dc_f_cb2[1]=float4(0,0,0,0);
dc_f_cb2[2]=float4(0,0,0,0);
dc_f_cb2[3]=float4(0,0,0,0);
dc_f_cb2[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_f_cb2[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_f_cb2[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_f_cb1[46];
dc_f_cb1[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(0,0,0,0);
dc_f_cb1[6]=float4(0,0,0,0);
dc_f_cb1[7]=float4(0,0,0,0);
dc_f_cb1[8]=float4(0,0,0,0);
dc_f_cb1[9]=float4(0,0,0,0);
dc_f_cb1[10]=float4(0,0,0,0);
dc_f_cb1[11]=float4(0,0,0,0);
dc_f_cb1[12]=float4(0,0,0,0);
dc_f_cb1[13]=float4(0,0,0,0);
dc_f_cb1[14]=float4(0,0,0,0);
dc_f_cb1[15]=float4(0,0,0,0);
dc_f_cb1[16]=float4(0,0,0,0);
dc_f_cb1[17]=float4(0,0,0,0);
dc_f_cb1[18]=float4(0,0,0,0);
dc_f_cb1[19]=float4(0,0,0,0);
dc_f_cb1[20]=float4(0,0,0,0);
dc_f_cb1[21]=float4(0,0,0,0);
dc_f_cb1[22]=float4(0,0,0,0);
dc_f_cb1[23]=float4(0,0,0,0);
dc_f_cb1[24]=float4(0,0,0,0);
dc_f_cb1[25]=float4(0,0,0,0);
dc_f_cb1[26]=float4(0,0,0,0);
dc_f_cb1[27]=float4(0,0,0,0);
dc_f_cb1[28]=float4(0,0,0,0);
dc_f_cb1[29]=float4(0,0,0,0);
dc_f_cb1[30]=float4(0,0,0,0);
dc_f_cb1[31]=float4(0,0,0,0);
dc_f_cb1[32]=float4(0,0,0,0);
dc_f_cb1[33]=float4(0,0,0,0);
dc_f_cb1[34]=float4(0,0,0,0);
dc_f_cb1[35]=float4(0,0,0,0);
dc_f_cb1[36]=float4(0,0,0,0);
dc_f_cb1[37]=float4(0,0,0,0);
dc_f_cb1[38]=float4(0,0,0,0);
dc_f_cb1[39]=float4(unity_SHAr.x,unity_SHAr.y,unity_SHAr.z,unity_SHAr.w);
dc_f_cb1[40]=float4(unity_SHAg.x,unity_SHAg.y,unity_SHAg.z,unity_SHAg.w);
dc_f_cb1[41]=float4(unity_SHAb.x,unity_SHAb.y,unity_SHAb.z,unity_SHAb.w);
dc_f_cb1[42]=float4(unity_SHBr.x,unity_SHBr.y,unity_SHBr.z,unity_SHBr.w);
dc_f_cb1[43]=float4(unity_SHBg.x,unity_SHBg.y,unity_SHBg.z,unity_SHBg.w);
dc_f_cb1[44]=float4(unity_SHBb.x,unity_SHBb.y,unity_SHBb.z,unity_SHBb.w);
dc_f_cb1[45]=float4(unity_SHC.x,unity_SHC.y,unity_SHC.z,unity_SHC.w);
float4 dc_f_cb0[15];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(_VertexAlphaRemap.x,_VertexAlphaRemap.y,_VertexAlphaRemap.z,_VertexAlphaRemap.w);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(_FallOffRemap.x,_FallOffRemap.y,_FallOffRemap.z,_FallOffRemap.w);
dc_f_cb0[13]=float4(_FlowAnimDist,_LightInfluence,0,_LightAlphaContribution);
dc_f_cb0[14]=float4(_TransparencyAlphaContribution,0,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _FlowMap.Sample(sampler_FlowMap, v1.xy).xyzw;
  r0.xy = float2(-0.00380000006,-0.00380000006) + r0.xy;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.zw = dc_f_cb0[13].xx * r0.xy;
  r0.xy = r0.xy * dc_f_cb0[13].xx + v1.zw;
  r1.xyzw = _MaskDistortion.Sample(sampler_MaskDistortion, r0.xy).xyzw;
  r0.x = r1.x * 2 + -1;
  r1.xyzw = r0.zwzw * v2.xxyy + v1.xyxy;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r1.zw).xyzw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r2.xyzw = r2.xyzw + -r1.xyzw;
  r1.xyzw = v2.zzzz * r2.xyzw + r1.xyzw;
  r0.yzw = r1.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r1.x = dot(r0.yzw, dc_f_cb2[4].xyz);
  r1.y = dot(r0.yzw, dc_f_cb2[5].xyz);
  r1.z = dot(r0.yzw, dc_f_cb2[6].xyz);
  r0.y = dot(r1.xyz, r1.xyz);
  r0.y = rsqrt(r0.y);
  r2.xyz = r1.xyz * r0.yyy;
  r0.y = r2.y * r2.y;
  r0.y = r2.x * r2.x + -r0.y;
  r3.xyzw = r2.xyzz * r2.yzzx;
  r1.x = dot(dc_f_cb1[42].xyzw, r3.xyzw);
  r1.y = dot(dc_f_cb1[43].xyzw, r3.xyzw);
  r1.z = dot(dc_f_cb1[44].xyzw, r3.xyzw);
  r0.yzw = dc_f_cb1[45].xyz * r0.yyy + r1.xyz;
  r2.w = 1;
  r1.x = dot(dc_f_cb1[39].xyzw, r2.xyzw);
  r1.y = dot(dc_f_cb1[40].xyzw, r2.xyzw);
  r1.z = dot(dc_f_cb1[41].xyzw, r2.xyzw);
  r0.yzw = r1.xyz + r0.yzw;
  r0.yzw = max(float3(0,0,0), r0.yzw);
  r0.yzw = log2(r0.yzw);
  r0.yzw = float3(0.416666657,0.416666657,0.416666657) * r0.yzw;
  r0.yzw = exp2(r0.yzw);
  r0.yzw = r0.yzw * float3(1.05499995,1.05499995,1.05499995) + float3(-0.0549999997,-0.0549999997,-0.0549999997);
  r0.yzw = max(float3(0,0,0), r0.yzw);
  r1.xyz = -v4.xyz * dc_f_cb1[0].www + dc_f_cb1[0].xyz;
  r1.x = dot(r2.xyz, r1.xyz);
  r1.y = dc_f_cb0[12].y + -dc_f_cb0[12].x;
  r1.z = 1 / r1.y;
  r1.y = dc_f_cb0[12].x / r1.y;
  r1.x = saturate(r1.x * r1.z + -r1.y);
  r1.xyz = dc_f_cb0[2].xyz * r1.xxx;
  r1.xyz = dc_f_cb0[13].yyy * r1.xyz;
  r2.xyzw = dc_f_cb0[7].xyzw * v5.xyzw;
  o0.xyz = saturate(r1.xyz * r2.xyz + r0.yzw);
  r0.y = dc_f_cb0[13].w * r1.x;
  r0.y = r1.w * dc_f_cb0[14].x + r0.y;
  r0.x = saturate(r2.w * 2 + r0.x);
  r0.x = r0.y * r0.x;
  r0.x = r0.x * r2.w;
  r0.y = dc_f_cb0[10].y + -dc_f_cb0[10].x;
  r0.z = 1 / r0.y;
  r0.y = dc_f_cb0[10].x / r0.y;
  o0.w = saturate(r0.x * r0.z + -r0.y);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
