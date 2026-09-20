Shader "DynamicCards/Legacy/Custom_Cards_CardCore_ImageLayerShader_Zbuffer_Fresnel_DetailMap" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_DetailTex("_DetailTex",2D)="white" {}
_DetailColorMultiplier("_DetailColorMultiplier",Color)=(0.5,0.5,0.5,1.0)
_AlphaExponent("_AlphaExponent",Float)=1.0
_AlphaMultiplier("_AlphaMultiplier",Float)=1.0
_DetailMoveU("_DetailMoveU",Float)=0.0
_DetailMoveV("_DetailMoveV",Float)=0.0
_FresnelPower("_FresnelPower",Float)=1.0
_FresnelMult("_FresnelMult",Float)=1.0
_FresnelColor("_FresnelColor",Color)=(0.6000000238418579,0.8999999761581421,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="TransparentCutout"}
Pass {
Cull Back
ZWrite On
ZTest LEqual
Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_DetailTex;
SamplerState sampler_MainTex;
Texture2D<float4> _DetailTex;
Texture2D<float4> _MainTex;
float _AlphaExponent;
float _AlphaMultiplier;
float _DetailMoveU;
float _DetailMoveV;
float _FresnelMult;
float _FresnelPower;
float4 _DetailColorMultiplier;
float4 _DetailTex_ST;
float4 _FresnelColor;
float4 _MainTex_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb0[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb0[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb0[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_DetailTex_ST.x,_DetailTex_ST.y,_DetailTex_ST.z,_DetailTex_ST.w);
dc_f_cb0[4]=float4(_DetailColorMultiplier.x,_DetailColorMultiplier.y,_DetailColorMultiplier.z,_DetailColorMultiplier.w);
dc_f_cb0[5]=float4(_AlphaExponent,_AlphaMultiplier,_DetailMoveU,_DetailMoveV);
dc_f_cb0[6]=float4(_FresnelPower,_FresnelMult,0,0);
dc_f_cb0[7]=float4(_FresnelColor.x,_FresnelColor.y,_FresnelColor.z,_FresnelColor.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = log2(r0.w);
  r0.w = dc_f_cb0[5].x * r0.w;
  r0.w = exp2(r0.w);
  r0.w = saturate(dc_f_cb0[5].y * r0.w);
  r1.x = -0.5 + r0.w;
  o0.w = r0.w;
  r0.w = cmp(r1.x < 0);
  if (r0.w != 0) discard;
  r1.xy = dc_f_cb1[0].yy * dc_f_cb0[5].zw + v1.xy;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _DetailTex.Sample(sampler_DetailTex, r1.xy).xyzw;
  r1.xyz = dc_f_cb0[4].xyz * r1.xyz;
  r0.xyz = r1.xyz * r1.www + r0.xyz;
  r1.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  r0.w = dot(v3.xyz, r1.xyz);
  r0.w = max(0, r0.w);
  r0.w = 1 + -r0.w;
  r0.w = log2(r0.w);
  r0.w = dc_f_cb0[6].x * r0.w;
  r0.w = exp2(r0.w);
  r0.w = dc_f_cb0[6].y * r0.w;
  o0.xyz = r0.www * dc_f_cb0[7].xyz + r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
