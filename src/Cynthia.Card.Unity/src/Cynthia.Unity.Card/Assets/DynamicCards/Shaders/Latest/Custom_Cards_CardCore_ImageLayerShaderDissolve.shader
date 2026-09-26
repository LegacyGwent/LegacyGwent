Shader "DynamicCards/Latest/Custom_Cards_CardCore_ImageLayerShaderDissolve" {
Properties {
_DissolveMask("_DissolveMask",2D)="white" {}
_MainTex("_MainTex",2D)="white" {}
_DissolveColorRamp("_DissolveColorRamp",2D)="white" {}
_ColorGRadInfluence("_ColorGRadInfluence",Float)=1.0
_BlackBurnInfluence("_BlackBurnInfluence",Float)=0.0
_Progress("_Progress",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
Pass {
Cull Back
ZWrite On
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_DissolveColorRamp;
SamplerState sampler_DissolveMask;
SamplerState sampler_MainTex;
Texture2D<float4> _DissolveColorRamp;
Texture2D<float4> _DissolveMask;
Texture2D<float4> _MainTex;
float _BlackBurnInfluence;
float _ColorGRadInfluence;
float _Progress;
float4 _DissolveColorRamp_ST;
float4 _DissolveMask_ST;
float4 _MainTex_ST;












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
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_DissolveMask_ST.x,_DissolveMask_ST.y,_DissolveMask_ST.z,_DissolveMask_ST.w);
dc_f_cb0[4]=float4(_DissolveColorRamp_ST.x,_DissolveColorRamp_ST.y,_DissolveColorRamp_ST.z,_DissolveColorRamp_ST.w);
dc_f_cb0[5]=float4(_ColorGRadInfluence,_BlackBurnInfluence,_Progress,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _DissolveMask.Sample(sampler_DissolveMask, r0.xy).xyzw;
  r0.y = 1 + -dc_f_cb0[5].z;
  r0.x = r0.y * 2 + r0.x;
  r0.x = -1 + r0.x;
  r0.yz = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.yz).xyzw;
  r0.y = r1.w * r0.x + -0.5;
  r0.x = r0.x * 14 + -7;
  r0.y = cmp(r0.y < 0);
  if (r0.y != 0) discard;
  r0.y = saturate(r0.x);
  r0.x = saturate(-dc_f_cb0[5].y + r0.x);
  r0.y = 1 + -r0.y;
  r2.x = dc_f_cb0[4].x * r0.y;
  r2.y = 0;
  r0.yz = dc_f_cb0[4].zw + r2.xy;
  r2.xyzw = _DissolveColorRamp.Sample(sampler_DissolveColorRamp, r0.yz).xyzw;
  r0.yzw = dc_f_cb0[5].xxx * r2.xyz;
  o0.xyz = r1.xyz * r0.xxx + r0.yzw;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
