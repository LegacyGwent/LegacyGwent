Shader "DynamicCards/Latest/ShaderLibrary_Environment_GwentWaterMKII" {
Properties {
_MainTex("_MainTex",2D)="grey" {}
_Normal("_Normal",2D)="grey" {}
_ColorDeep("_ColorDeep",Color)=(0.0,0.0,1.0,1.0)
_ColorShallow("_ColorShallow",Color)=(0.0,1.0,1.0,0.0)
_SpecularPower("_SpecularPower",Float)=10.0
_FlowAnimSpeed("_FlowAnimSpeed",Float)=1.0
_FlowAnimDist("_FlowAnimDist",Float)=1.0
_TerrainSize("_TerrainSize",Vector)=(100.0,30.0,50.0,0.0)
_SpecNormalModifier("_SpecNormalModifier",Vector)=(1.0,1.0,1.0,1.0)
_ShoreFalloff("_ShoreFalloff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_Normal;
SamplerState samplerunity_SpecCube0;
Texture2D<float4> _MainTex;
Texture2D<float4> _Normal;
TextureCube<float4> unity_SpecCube0;
float _FlowAnimDist;
float _FlowAnimSpeed;
float _ShoreFalloff;
float _SpecularPower;
float3 _SpecNormalModifier;
float4 _ColorDeep;
float4 _ColorShallow;
float4 _Normal_ST;
float4 _TerrainSize;


















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : NORMAL0,
  float4 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float3 o5 : TEXCOORD4)
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
float4 dc_v_cb3[7];
dc_v_cb3[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb3[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb3[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb3[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb3[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb3[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb3[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb2[46];
dc_v_cb2[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
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
dc_v_cb2[42]=float4(unity_SHBr.x,unity_SHBr.y,unity_SHBr.z,unity_SHBr.w);
dc_v_cb2[43]=float4(unity_SHBg.x,unity_SHBg.y,unity_SHBg.z,unity_SHBg.w);
dc_v_cb2[44]=float4(unity_SHBb.x,unity_SHBb.y,unity_SHBb.z,unity_SHBb.w);
dc_v_cb2[45]=float4(unity_SHC.x,unity_SHC.y,unity_SHC.z,unity_SHC.w);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[9];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,_FlowAnimSpeed,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb3[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb3[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb3[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb3[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb4[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb4[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb4[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb4[20].xyzw * r0.wwww + r1.xyzw;
  o1.xyzw = float4(1,-1,1,1) * v2.xyzw;
  r0.xyz = dc_v_cb3[1].xyz * v0.yyy;
  r0.xyz = dc_v_cb3[0].xyz * v0.xxx + r0.xyz;
  r0.xyz = dc_v_cb3[2].xyz * v0.zzz + r0.xyz;
  r0.xyz = dc_v_cb3[3].xyz * v0.www + r0.xyz;
  o2.xyz = r0.xyz;
  r0.xyz = -r0.xyz * dc_v_cb2[0].www + dc_v_cb2[0].xyz;
  r1.x = dot(v1.xyz, dc_v_cb3[4].xyz);
  r1.y = dot(v1.xyz, dc_v_cb3[5].xyz);
  r1.z = dot(v1.xyz, dc_v_cb3[6].xyz);
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  r0.w = r1.y * r1.y;
  r0.w = r1.x * r1.x + -r0.w;
  r2.xyzw = r1.xyzz * r1.yzzx;
  r3.x = dot(dc_v_cb2[42].xyzw, r2.xyzw);
  r3.y = dot(dc_v_cb2[43].xyzw, r2.xyzw);
  r3.z = dot(dc_v_cb2[44].xyzw, r2.xyzw);
  r2.xyz = dc_v_cb2[45].xyz * r0.www + r3.xyz;
  r1.w = 1;
  r3.x = dot(dc_v_cb2[39].xyzw, r1.xyzw);
  r3.y = dot(dc_v_cb2[40].xyzw, r1.xyzw);
  r3.z = dot(dc_v_cb2[41].xyzw, r1.xyzw);
  r0.w = saturate(dot(r1.xyz, r0.xyz));
  o5.xyz = r0.xyz;
  r0.xyz = r3.xyz + r2.xyz;
  r0.xyz = max(float3(0,0,0), r0.xyz);
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(0.416666657,0.416666657,0.416666657) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = r0.xyz * float3(1.05499995,1.05499995,1.05499995) + float3(-0.0549999997,-0.0549999997,-0.0549999997);
  r0.xyz = max(float3(0,0,0), r0.xyz);
  o3.xyz = r0.www * dc_v_cb0[2].xyz + r0.xyz;
  r0.x = dc_v_cb1[0].x * dc_v_cb0[8].y + 0.5;
  o4.y = frac(r0.x);
  r0.x = dc_v_cb1[0].x * dc_v_cb0[8].y;
  r0.x = frac(r0.x);
  r0.y = 0.5 + -r0.x;
  o4.x = r0.x;
  r0.x = r0.y + r0.y;
  o4.z = abs(r0.x);
  return;
}


























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float3 v5 : TEXCOORD4,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[4];
dc_f_cb2[0]=float4(0,0,0,0);
dc_f_cb2[1]=float4(0,0,0,0);
dc_f_cb2[2]=float4(0,0,0,0);
dc_f_cb2[3]=float4(unity_SpecCube0_HDR.x,unity_SpecCube0_HDR.y,unity_SpecCube0_HDR.z,unity_SpecCube0_HDR.w);
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_Normal_ST.x,_Normal_ST.y,_Normal_ST.z,_Normal_ST.w);
dc_f_cb0[5]=float4(_TerrainSize.x,_TerrainSize.y,_TerrainSize.z,_TerrainSize.w);
dc_f_cb0[6]=float4(_ColorDeep.x,_ColorDeep.y,_ColorDeep.z,_ColorDeep.w);
dc_f_cb0[7]=float4(_ColorShallow.x,_ColorShallow.y,_ColorShallow.z,_ColorShallow.w);
dc_f_cb0[8]=float4(_SpecularPower,0,_FlowAnimDist,_ShoreFalloff);
dc_f_cb0[9]=float4(_SpecNormalModifier.x,_SpecNormalModifier.y,_SpecNormalModifier.z,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_f_cb0[4].xyxy * v2.xzxz;
  r1.xyzw = dc_f_cb0[8].zzzz * v1.xyxy;
  r0.xyzw = r1.xyzw * v4.xxyy + r0.xyzw;
  r1.xyzw = _Normal.Sample(sampler_Normal, r0.zw).xyzw;
  r0.xyzw = _Normal.Sample(sampler_Normal, r0.xy).xyzw;
  r1.xyz = r1.xyz + -r0.xyz;
  r0.xyz = v4.zzz * r1.xyz + r0.xyz;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r1.xy = -dc_f_cb0[5].xz * float2(0.5,0.5) + v2.xz;
  r1.xy = r0.xy * float2(0.100000001,0.100000001) + r1.xy;
  r1.xy = r1.xy / dc_f_cb0[5].xz;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r2.xyzw = -dc_f_cb0[7].xyzw + dc_f_cb0[6].xyzw;
  r2.xyzw = v1.zzzz * r2.xyzw + dc_f_cb0[7].xyzw;
  r1.xyz = r2.xyz * r1.xyz;
  r1.xyz = r0.zzz * v1.www + r1.xyz;
  r1.xyz = v3.xyz * r1.xyz;
  r0.xz = r2.ww * r0.xy;
  r0.y = 1;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  r2.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r2.xyz, r2.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = r2.xyz * r0.www;
  r0.w = dot(-r2.xyz, r0.xyz);
  r0.w = r0.w + r0.w;
  r3.xyz = r0.xyz * -r0.www + -r2.xyz;
  r3.xyzw = unity_SpecCube0.Sample(samplerunity_SpecCube0, r3.xyz).xyzw;
  r0.w = -1 + r3.w;
  r0.w = dc_f_cb2[3].w * r0.w + 1;
  r0.w = dc_f_cb2[3].x * r0.w;
  r3.xyz = r0.www * r3.xyz + -r1.xyz;
  r0.w = saturate(dot(r0.xyz, r2.xyz));
  r2.xyz = v5.xyz * dc_f_cb0[9].xyz + r2.xyz;
  r0.w = 1 + -r0.w;
  r1.xyz = r0.www * r3.xyz + r1.xyz;
  r0.w = dot(r2.xyz, r2.xyz);
  r0.w = max(0.00100000005, r0.w);
  r0.w = rsqrt(r0.w);
  r2.xyz = r2.xyz * r0.www;
  r0.x = saturate(dot(r0.xyz, r2.xyz));
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[8].x * r0.x;
  r0.x = exp2(r0.x);
  r0.x = min(1, r0.x);
  o0.xyz = r0.xxx * dc_f_cb0[2].xyz + r1.xyz;
  r0.x = 1 / dc_f_cb0[8].w;
  r0.x = saturate(v1.z * r0.x);
  o0.w = r2.w * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
