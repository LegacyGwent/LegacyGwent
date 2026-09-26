Shader "DynamicCards/Latest/VFX_Special_ParalaxReflectionOpacityClip" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Paralaxmask("_Paralaxmask",2D)="white" {}
_Displacedirection("_Displacedirection",Vector)=(1.0,1.0,0.0,0.0)
_DisplaceNormal("_DisplaceNormal",2D)="bump" {}
_Unormaltiling("_Unormaltiling",Float)=1.0
_Vnormaltiling("_Vnormaltiling",Float)=1.0
_Flowdirection("_Flowdirection",Vector)=(0.0,0.0,0.0,0.0)
_Flowspeedmult("_Flowspeedmult",Float)=1.0
_Flowdisplaceamount("_Flowdisplaceamount",Float)=0.0
_UVnormalrotation("_UVnormalrotation",Float)=0.0
_Displaceamount("_Displaceamount",Float)=0.0
_AlphaExponent("_AlphaExponent",Float)=1.0
_AlphaMultiplier("_AlphaMultiplier",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"RenderType"="Opaque"}
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
SamplerState sampler_DisplaceNormal;
SamplerState sampler_MainTex;
SamplerState sampler_Paralaxmask;
Texture2D<float4> _DisplaceNormal;
Texture2D<float4> _MainTex;
Texture2D<float4> _Paralaxmask;
float _AlphaExponent;
float _AlphaMultiplier;
float _Displaceamount;
float _Flowdisplaceamount;
float _Flowspeedmult;
float _UVnormalrotation;
float _Unormaltiling;
float _Vnormaltiling;
float4 _DisplaceNormal_ST;
float4 _Displacedirection;
float4 _Flowdirection;
float4 _MainTex_ST;
float4 _Paralaxmask_ST;












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
dc_f_cb0[3]=float4(_Paralaxmask_ST.x,_Paralaxmask_ST.y,_Paralaxmask_ST.z,_Paralaxmask_ST.w);
dc_f_cb0[4]=float4(_Displacedirection.x,_Displacedirection.y,_Displacedirection.z,_Displacedirection.w);
dc_f_cb0[5]=float4(_DisplaceNormal_ST.x,_DisplaceNormal_ST.y,_DisplaceNormal_ST.z,_DisplaceNormal_ST.w);
dc_f_cb0[6]=float4(_Unormaltiling,_Vnormaltiling,0,0);
dc_f_cb0[7]=float4(_Flowdirection.x,_Flowdirection.y,_Flowdirection.z,_Flowdirection.w);
dc_f_cb0[8]=float4(_Flowspeedmult,_Flowdisplaceamount,_UVnormalrotation,_Displaceamount);
dc_f_cb0[9]=float4(_AlphaExponent,_AlphaMultiplier,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[6].x * dc_f_cb0[6].y;
  r0.yz = -r0.xx * float2(0.5,0.5) + v1.xy;
  sincos(dc_f_cb0[8].z, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.yz, r3.xy);
  r1.x = dot(r0.yz, r3.yz);
  r0.xy = r0.xx * float2(0.5,0.5) + r1.xy;
  r0.xy = dc_f_cb0[6].xy * r0.xy;
  r0.z = dot(dc_f_cb0[7].xy, dc_f_cb0[7].xy);
  r0.z = rsqrt(r0.z);
  r0.zw = dc_f_cb0[7].xy * r0.zz;
  r1.x = dc_f_cb1[0].y * dc_f_cb0[8].x;
  r0.xy = r0.zw * r1.xx + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _DisplaceNormal.Sample(sampler_DisplaceNormal, r0.xy).xyzw;
  r0.x = r0.x * r0.w;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = dc_f_cb0[8].yy * r0.xy;
  r0.zw = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _Paralaxmask.Sample(sampler_Paralaxmask, r0.zw).xyzw;
  r0.xy = r1.xx * r0.xy;
  r0.zw = dc_f_cb0[4].xy * r1.xx;
  r1.x = dc_f_cb0[8].w * r1.y;
  r0.zw = r1.xx * r0.zw;
  r1.xz = float2(1,1) + -v1.xy;
  r0.zw = r0.zw * r1.xz + v1.xy;
  r0.xy = r0.xy * r1.yy + r0.zw;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = log2(r0.w);
  o0.xyz = r0.xyz;
  r0.x = dc_f_cb0[9].x * r0.w;
  r0.x = exp2(r0.x);
  r0.x = saturate(dc_f_cb0[9].y * r0.x);
  r0.x = -0.5 + r0.x;
  r0.x = cmp(r0.x < 0);
  if (r0.x != 0) discard;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
