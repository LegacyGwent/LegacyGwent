Shader "DynamicCards/Native/VFX_Special_Fog_MistEdgeShader" {
Properties {
_Mask1("_Mask1",2D)="white" {}
_Mask2("_Mask2",2D)="white" {}
_MainMask("_MainMask",2D)="black" {}
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_AlphaGradient("_AlphaGradient",2D)="white" {}
_NormalMap("_NormalMap",2D)="bump" {}
_DistortionVal("_DistortionVal",Float)=0.0
_EndMask("_EndMask",2D)="white" {}
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
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
SamplerState sampler_AlphaGradient;
SamplerState sampler_EndMask;
SamplerState sampler_MainMask;
SamplerState sampler_Mask1;
SamplerState sampler_Mask2;
SamplerState sampler_NormalMap;
Texture2D<float4> _AlphaGradient;
Texture2D<float4> _EndMask;
Texture2D<float4> _MainMask;
Texture2D<float4> _Mask1;
Texture2D<float4> _Mask2;
Texture2D<float4> _NormalMap;
float _DistortionVal;
float4 _AlphaGradient_ST;
float4 _EndMask_ST;
float4 _MainMask_ST;
float4 _Mask1_ST;
float4 _Mask2_ST;
float4 _NormalMap_ST;
float4 _TintColor;












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
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_Mask1_ST.x,_Mask1_ST.y,_Mask1_ST.z,_Mask1_ST.w);
dc_f_cb0[3]=float4(_Mask2_ST.x,_Mask2_ST.y,_Mask2_ST.z,_Mask2_ST.w);
dc_f_cb0[4]=float4(_MainMask_ST.x,_MainMask_ST.y,_MainMask_ST.z,_MainMask_ST.w);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_AlphaGradient_ST.x,_AlphaGradient_ST.y,_AlphaGradient_ST.z,_AlphaGradient_ST.w);
dc_f_cb0[7]=float4(_NormalMap_ST.x,_NormalMap_ST.y,_NormalMap_ST.z,_NormalMap_ST.w);
dc_f_cb0[8]=float4(_DistortionVal,0,0,0);
dc_f_cb0[9]=float4(_EndMask_ST.x,_EndMask_ST.y,_EndMask_ST.z,_EndMask_ST.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r0.xyzw = _NormalMap.Sample(sampler_NormalMap, r0.xy).xyzw;
  r0.x = r0.x * r0.w;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = -v1.xy + r0.xy;
  r0.xy = dc_f_cb0[8].xx * r0.xy + v1.xy;
  r0.zw = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _Mask2.Sample(sampler_Mask2, r0.zw).xyzw;
  r0.zw = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xy = r0.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r2.xyzw = _MainMask.Sample(sampler_MainMask, r0.xy).xyzw;
  r0.x = 2.5 * r2.x;
  r2.xyzw = _Mask1.Sample(sampler_Mask1, r0.zw).xyzw;
  r0.y = -0.5 + r2.x;
  r0.y = r0.y + r0.y;
  r0.y = max(r1.x, r0.y);
  r0.z = r2.x + r2.x;
  r0.w = cmp(0.5 < r2.x);
  r0.z = min(r1.x, r0.z);
  r0.y = saturate(r0.w ? r0.y : r0.z);
  r0.x = r0.x * r0.y;
  r0.x = dc_f_cb0[5].w * r0.x;
  r0.yz = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _AlphaGradient.Sample(sampler_AlphaGradient, r0.yz).xyzw;
  r0.x = r1.w * r0.x;
  r0.x = 0.666666687 * r0.x;
  r0.xyz = dc_f_cb0[5].xyz * r0.xxx;
  r1.xy = v1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r1.xyzw = _EndMask.Sample(sampler_EndMask, r1.xy).xyzw;
  o0.xyz = r1.xyz * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
