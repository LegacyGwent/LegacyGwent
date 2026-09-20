Shader "DynamicCards/Native/VFX_Effects_MotionVectorsShader" {
Properties {
_Tint("_Tint",Color)=(1.0,1.0,1.0,1.0)
_VectorStrenght("_VectorStrenght",Float)=1.0
_XSize("_XSize",Float)=1.0
_YSize("_YSize",Float)=1.0
_MainTex("_MainTex",2D)="white" {}
_MotionVectors("_MotionVectors",2D)="white" {}
_AlphaPower("_AlphaPower",Float)=1.0
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
SamplerState sampler_MotionVectors;
Texture2D<float4> _MainTex;
Texture2D<float4> _MotionVectors;
float _AlphaPower;
float _VectorStrenght;
float _XSize;
float _YSize;
float4 _MainTex_ST;
float4 _MotionVectors_ST;
float4 _Tint;












void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
  o1.xyzw = v1.xyzw;
  o2.xyzw = v2.xyzw;
  return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_Tint.x,_Tint.y,_Tint.z,_Tint.w);
dc_f_cb0[3]=float4(_VectorStrenght,_XSize,_YSize,0);
dc_f_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[5]=float4(_MotionVectors_ST.x,_MotionVectors_ST.y,_MotionVectors_ST.z,_MotionVectors_ST.w);
dc_f_cb0[6]=float4(_AlphaPower,0,0,0);



  float4 r0,r1,r2,r3,r4;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 0.5 * v1.z;
  r0.y = dc_f_cb0[3].y * dc_f_cb0[3].z;
  r0.z = r0.x * r0.y;
  r0.xy = r0.xx * r0.yy + float2(-0.5,0.5);
  r0.w = ceil(r0.z);
  r1.xyz = frac(r0.zxy);
  r0.z = r0.w + r0.w;
  r2.x = 1 / dc_f_cb0[3].y;
  r0.w = r2.x * r0.z;
  r3.y = floor(r0.w);
  r3.x = -dc_f_cb0[3].y * r3.y + r0.z;
  r0.zw = v1.xy + r3.xy;
  r2.y = 1 / -dc_f_cb0[3].z;
  r0.zw = r2.xy * r0.zw;
  r2.zw = r0.zw * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r3.xyzw = _MotionVectors.Sample(sampler_MotionVectors, r2.zw).xyzw;
  r2.zw = r3.xy * float2(2,2) + float2(-1,-1);
  r0.x = ceil(r0.x);
  r0.x = r0.x * 2 + 1;
  r3.xy = float2(-0.00100000005,0.00200000009) * dc_f_cb0[3].xx;
  r1.xy = r1.xy * r3.yy + r3.xx;
  r0.y = -0.5 + r1.z;
  r0.y = abs(r0.y) + abs(r0.y);
  r0.zw = r2.zw * r1.xx + r0.zw;
  r0.zw = r0.zw * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r3.xyzw = _MainTex.Sample(sampler_MainTex, r0.zw).xyzw;
  r0.z = r0.x * r2.x;
  r4.y = floor(r0.z);
  r4.x = -dc_f_cb0[3].y * r4.y + r0.x;
  r0.xz = v1.xy + r4.xy;
  r0.xz = r0.xz * r2.xy;
  r1.xz = r0.xz * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r2.xyzw = _MotionVectors.Sample(sampler_MotionVectors, r1.xz).xyzw;
  r1.xz = r2.xy * float2(2,2) + float2(-1,-1);
  r0.xz = r1.xz * r1.yy + r0.xz;
  r0.xz = r0.xz * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.xz).xyzw;
  r2.xyzw = r3.xyzw + -r1.xyzw;
  r0.xyzw = r0.yyyy * r2.xyzw + r1.xyzw;
  r0.w = v2.w * r0.w;
  r0.xyz = dc_f_cb0[2].xyz * r0.xyz;
  o0.xyz = v2.xyz * r0.xyz;
  r0.x = log2(r0.w);
  r0.x = dc_f_cb0[6].x * r0.x;
  o0.w = exp2(r0.x);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
