Shader "DynamicCards/Latest/Cards_BaseDissolve" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_OverrideST("_OverrideST",Vector)=(1.0,1.0,0.0,0.0)
_Dissolve("_Dissolve",2D)="white" {}
_DissolveProgress("_DissolveProgress",Float)=0.0
_DissolveRamp("_DissolveRamp",2D)="black" {}
_OutlineColor("_OutlineColor",Color)=(0.5,0.5,0.5,1.0)
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
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
SamplerState sampler_Dissolve;
SamplerState sampler_DissolveRamp;
SamplerState sampler_MainTex;
Texture2D<float4> _Dissolve;
Texture2D<float4> _DissolveRamp;
Texture2D<float4> _MainTex;
float _DissolveProgress;
float4 _DissolveRamp_ST;
float4 _Dissolve_ST;
float4 _OutlineColor;
float4 _OverrideST;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float2 o0 : TEXCOORD0,
  out float4 o1 : SV_POSITION0)
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

  o0.xy = v1.xy;
  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb1[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb1[19].xyzw * r0.zzzz + r1.xyzw;
  o1.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  return;
}






















void dc_f(
  float2 v0 : TEXCOORD0,
  float4 v1 : SV_POSITION0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_OverrideST.x,_OverrideST.y,_OverrideST.z,_OverrideST.w);
dc_f_cb0[3]=float4(_Dissolve_ST.x,_Dissolve_ST.y,_Dissolve_ST.z,_Dissolve_ST.w);
dc_f_cb0[4]=float4(_DissolveProgress,0,0,0);
dc_f_cb0[5]=float4(_DissolveRamp_ST.x,_DissolveRamp_ST.y,_DissolveRamp_ST.z,_DissolveRamp_ST.w);
dc_f_cb0[6]=float4(_OutlineColor.x,_OutlineColor.y,_OutlineColor.z,_OutlineColor.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _Dissolve.Sample(sampler_Dissolve, r0.xy).xyzw;
  r0.y = 1 + -dc_f_cb0[4].x;
  r0.x = r0.x + -r0.y;
  r0.y = -0.00100000005 + -r0.x;
  r0.x = saturate(0.100000001 + r0.x);
  r0.x = dc_f_cb0[5].x * r0.x;
  r1.x = 7 * r0.x;
  r0.x = cmp(r0.y < 0);
  if (r0.x != 0) discard;
  r1.y = 0;
  r0.xy = dc_f_cb0[5].zw + r1.xy;
  r0.xyzw = _DissolveRamp.Sample(sampler_DissolveRamp, r0.xy).xyzw;
  r1.xy = v0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  o0.xyzw = r0.xyzw * dc_f_cb0[6].xyzw + r1.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
