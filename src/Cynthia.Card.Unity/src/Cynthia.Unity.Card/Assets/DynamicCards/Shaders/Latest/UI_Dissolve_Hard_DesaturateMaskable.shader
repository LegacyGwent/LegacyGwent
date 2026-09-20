Shader "DynamicCards/Latest/UI_Dissolve_Hard_DesaturateMaskable" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_DesaturatedColor("_DesaturatedColor",Color)=(0.6039215922355652,0.4745098054409027,0.30980390310287476,0.5)
_DesaturationAmount("_DesaturationAmount",Float)=0.0
_Mask("_Mask",2D)="white" {}
_Dissolve("_Dissolve",Float)=1.0
_DissolveProgress("_DissolveProgress",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"CanUseSpriteAtlas"="true" "IGNOREPROJECTOR"="true" "PreviewType"="Plane" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_Mask;
Texture2D<float4> _MainTex;
Texture2D<float4> _Mask;
float _DesaturationAmount;
float _Dissolve;
float _DissolveProgress;
float4 _DesaturatedColor;
float4 _MainTex_ST;
float4 _Mask_ST;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
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
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_DesaturatedColor.x,_DesaturatedColor.y,_DesaturatedColor.z,_DesaturatedColor.w);
dc_f_cb0[4]=float4(_Dissolve,0,0,0);
dc_f_cb0[5]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[6]=float4(_DesaturationAmount,_DissolveProgress,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _Mask.Sample(sampler_Mask, r0.xy).xyzw;
  r0.x = dot(r0.xxx, float3(0.300000012,0.589999974,0.109999999));
  r0.y = dc_f_cb0[4].x + r0.x;
  r0.x = dc_f_cb0[6].y + r0.x;
  r0.xy = floor(r0.xy);
  r0.x = r0.y + -r0.x;
  r0.x = saturate(1 + r0.x);
  r0.yz = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.yz).xyzw;
  r0.yzw = v2.xyz * r1.xyz;
  r2.x = dot(r0.yzw, float3(0.300000012,0.589999974,0.109999999));
  r2.xyz = -r1.xyz * v2.xyz + r2.xxx;
  r2.xyz = dc_f_cb0[6].xxx * r2.xyz + r0.yzw;
  r2.xyz = saturate(dc_f_cb0[3].xyz + r2.xyz);
  r1.xyz = -r1.xyz * v2.xyz + r2.xyz;
  r1.xyz = dc_f_cb0[6].xxx * r1.xyz;
  o0.xyz = r0.xxx * r1.xyz + r0.yzw;
  r0.y = dc_f_cb0[3].w + -r1.w;
  r0.y = dc_f_cb0[6].x * r0.y;
  r0.x = r0.x * r0.y + r1.w;
  r0.x = v2.w * r0.x;
  o0.w = r1.w * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
