Shader "DynamicCards/Latest/ShaderLibrary_VFX_Generic_UVScrollTwoTextures_AlphaTestMask_Additive" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_SecondaryTex("_SecondaryTex",2D)="black" {}
_CutoutMask("_CutoutMask",2D)="white" {}
_AlphaVal("_AlphaVal",Float)=0.5
_UVScrollSpeed("_UVScrollSpeed",Vector)=(0.0,0.0,0.0,0.0)
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_TintColorSecondary("_TintColorSecondary",Color)=(1.0,1.0,1.0,1.0)
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend SrcAlpha One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_CutoutMask;
SamplerState sampler_MainTex;
SamplerState sampler_SecondaryTex;
Texture2D<float4> _CutoutMask;
Texture2D<float4> _MainTex;
Texture2D<float4> _SecondaryTex;
float _AlphaVal;
float4 _MainTex_ST;
float4 _SecondaryTex_ST;
float4 _TintColor;
float4 _TintColorSecondary;
float4 _UVScrollSpeed;
















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float2 v3 : TEXCOORD2,
  out float2 o0 : TEXCOORD0,
  out float2 p0 : TEXCOORD1,
  out float2 o1 : TEXCOORD2,
  out float4 o2 : SV_POSITION0)
{
float4 dc_v_cb3[21];
dc_v_cb3[0]=float4(0,0,0,0);
dc_v_cb3[1]=float4(0,0,0,0);
dc_v_cb3[2]=float4(0,0,0,0);
dc_v_cb3[3]=float4(0,0,0,0);
dc_v_cb3[4]=float4(0,0,0,0);
dc_v_cb3[5]=float4(0,0,0,0);
dc_v_cb3[6]=float4(0,0,0,0);
dc_v_cb3[7]=float4(0,0,0,0);
dc_v_cb3[8]=float4(0,0,0,0);
dc_v_cb3[9]=float4(0,0,0,0);
dc_v_cb3[10]=float4(0,0,0,0);
dc_v_cb3[11]=float4(0,0,0,0);
dc_v_cb3[12]=float4(0,0,0,0);
dc_v_cb3[13]=float4(0,0,0,0);
dc_v_cb3[14]=float4(0,0,0,0);
dc_v_cb3[15]=float4(0,0,0,0);
dc_v_cb3[16]=float4(0,0,0,0);
dc_v_cb3[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb3[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb3[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb3[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb2[4];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[8];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[3]=float4(_SecondaryTex_ST.x,_SecondaryTex_ST.y,_SecondaryTex_ST.z,_SecondaryTex_ST.w);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(_UVScrollSpeed.x,_UVScrollSpeed.y,_UVScrollSpeed.z,_UVScrollSpeed.w);

float4 dc_pack_OSGN_0=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_v_cb0[2].zw + v1.xy;
  r1.xyzw = dc_v_cb1[0].yyyy * dc_v_cb0[7].xyzw;
  r0.zw = r1.xy / dc_v_cb0[2].xy;
  r1.xy = r1.zw / dc_v_cb0[3].xy;
  r0.xy = r0.xy + r0.zw;
  dc_pack_OSGN_0.xy = dc_v_cb0[2].xy * r0.xy;
  dc_pack_OSGN_0.xy = v1.xy;
  r0.xy = dc_v_cb0[3].zw + v1.xy;
  r0.xy = r0.xy + r1.xy;
  o1.xy = dc_v_cb0[3].xy * r0.xy;
  r0.xyzw = dc_v_cb2[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o2.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o0=dc_pack_OSGN_0.xy;
p0=dc_pack_OSGN_0.zw;
return;
}






















void dc_f(
  float2 v0 : TEXCOORD0,
  float2 w0 : TEXCOORD1,
  float2 v1 : TEXCOORD2,
  float4 v2 : SV_POSITION0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_AlphaVal,0,0,0);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_TintColorSecondary.x,_TintColorSecondary.y,_TintColorSecondary.z,_TintColorSecondary.w);

float4 dc_pack_ISGN_0=float4(v0.x,v0.y,w0.x,w0.y);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _CutoutMask.Sample(sampler_CutoutMask, dc_pack_ISGN_0.xy).xyzw;
  r1.x = 1 + -dc_f_cb0[4].x;
  r0.xyzw = -r1.xxxx + r0.xyzw;
  r0.xyzw = cmp(r0.xyzw < float4(0,0,0,0));
  r0.xy = (int2)r0.zw | (int2)r0.xy;
  r0.x = (int)r0.y | (int)r0.x;
  if (r0.x != 0) discard;
  r0.xyzw = _SecondaryTex.Sample(sampler_SecondaryTex, v1.xy).xyzw;
  r0.xyzw = dc_f_cb0[6].xyzw * r0.xyzw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, dc_pack_ISGN_0.xy).xyzw;
  o0.xyzw = r1.xyzw * dc_f_cb0[5].xyzw + r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
