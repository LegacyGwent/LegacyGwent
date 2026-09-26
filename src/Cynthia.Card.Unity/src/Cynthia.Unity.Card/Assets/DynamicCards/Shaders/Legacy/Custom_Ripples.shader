Shader "DynamicCards/Legacy/Custom_Ripples" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Paralaxmask("_Paralaxmask",2D)="white" {}
_TileablePatern("_TileablePatern",2D)="white" {}
_DisplamentRadius("_DisplamentRadius",Float)=0.0
_WavesNormal("_WavesNormal",2D)="white" {}
_MaskOffsetX("_MaskOffsetX",Float)=0.0
_MaskOffsetY("_MaskOffsetY",Float)=0.0
_TimeOffset("_TimeOffset",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_Paralaxmask;
SamplerState sampler_TileablePatern;
SamplerState sampler_WavesNormal;
Texture2D<float4> _MainTex;
Texture2D<float4> _Paralaxmask;
Texture2D<float4> _TileablePatern;
Texture2D<float4> _WavesNormal;
float _DisplamentRadius;
float _MaskOffsetX;
float _MaskOffsetY;
float _TimeOffset;
float4 _MainTex_ST;
float4 _Paralaxmask_ST;
float4 _TileablePatern_ST;
float4 _TimeEditor;
float4 _WavesNormal_ST;












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
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Paralaxmask_ST.x,_Paralaxmask_ST.y,_Paralaxmask_ST.z,_Paralaxmask_ST.w);
dc_f_cb0[5]=float4(_TileablePatern_ST.x,_TileablePatern_ST.y,_TileablePatern_ST.z,_TileablePatern_ST.w);
dc_f_cb0[6]=float4(_DisplamentRadius,0,0,0);
dc_f_cb0[7]=float4(_WavesNormal_ST.x,_WavesNormal_ST.y,_WavesNormal_ST.z,_WavesNormal_ST.w);
dc_f_cb0[8]=float4(_MaskOffsetX,_MaskOffsetY,_TimeOffset,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = float2(-0.5,-0.5) + v1.xy;
  r1.x = dot(r0.xy, float2(0.996801734,-0.0799146891));
  r1.y = dot(r0.xy, float2(0.0799146891,0.996801734));
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * float2(0.800000012,4.44000006) + dc_f_cb0[8].xy;
  r0.xy = r0.xy * float2(-2,-2) + float2(1,1);
  r0.x = dot(r0.xy, r0.xy);
  r0.x = sqrt(r0.x);
  r0.x = 1 + -r0.x;
  r0.x = saturate(r0.x * r0.x + dc_f_cb0[6].x);
  r0.yz = dc_f_cb1[0].xy + dc_f_cb0[2].xy;
  r0.z = r0.z * 0.140000001 + dc_f_cb0[8].z;
  r0.yw = r0.yy * float2(-0.0299999993,-0.0299999993) + v1.xy;
  r0.yw = r0.yw * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r1.xyzw = _WavesNormal.Sample(sampler_WavesNormal, r0.yw).xyzw;
  r0.yw = r1.wy * float2(2,2) + float2(-1,-1);
  r1.y = r0.x * r0.x + r0.z;
  r0.x = 5 * r0.x;
  r0.x = min(1, r0.x);
  r1.x = v1.x;
  r1.xy = r1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _TileablePatern.Sample(sampler_TileablePatern, r1.xy).xyzw;
  r1.x = saturate(r1.x);
  r0.z = 1 + -r1.x;
  r1.xy = -r0.yw * float2(0.819999993,0.819999993) + r0.zz;
  r0.yz = float2(0.819999993,0.819999993) * r0.yw;
  r0.xy = r0.xx * r1.xy + r0.yz;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = float2(0.00999999978,0.00999999978) * r0.xy;
  r0.zw = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Paralaxmask.Sample(sampler_Paralaxmask, r0.zw).xyzw;
  r0.xy = r1.xx * r0.xy;
  r0.z = r1.y * r1.x;
  r0.z = -0.00999999978 * r0.z;
  r1.xz = float2(1,1) + -v1.xy;
  r0.zw = r0.zz * r1.xz + v1.xy;
  r0.xy = r0.xy * r1.yy + r0.zw;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  o0.xyz = r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
