Shader "DynamicCards/Legacy/VFX_Special_Unicorn_iorveth_leafs" {
Properties {
_node_6318("_node_6318",2D)="white" {}
_desaturate("_desaturate",Float)=0.0
_colour("_colour",Color)=(0.5,0.5,0.5,1.0)
_emission("_emission",Float)=0.4000000059604645
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
Pass {
Cull Off
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
SamplerState sampler_node_6318;
Texture2D<float4> _node_6318;
float _desaturate;
float _emission;
float4 _colour;
float4 _node_6318_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  float4 v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2,
  out float4 o4 : COLOR0)
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
  o4.xyzw = v3.xyzw;
  return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  float4 v4 : COLOR0,
  uint v5 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[1];
dc_f_cb2[0]=float4(glstate_lightmodel_ambient.x,glstate_lightmodel_ambient.y,glstate_lightmodel_ambient.z,glstate_lightmodel_ambient.w);
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(_node_6318_ST.x,_node_6318_ST.y,_node_6318_ST.z,_node_6318_ST.w);
dc_f_cb0[4]=float4(_desaturate,0,0,0);
dc_f_cb0[5]=float4(_colour.x,_colour.y,_colour.z,_colour.w);
dc_f_cb0[6]=float4(_emission,0,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _node_6318.Sample(sampler_node_6318, r0.xy).xyzw;
  r0.w = -0.5 + r0.w;
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r0.w = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r1.xyz = r0.www + -r0.xyz;
  r0.xyz = dc_f_cb0[4].xxx * r1.xyz + r0.xyz;
  r0.xyz = dc_f_cb0[5].xyz * r0.xyz;
  r1.xyz = dc_f_cb0[6].xxx * r0.xyz;
  r2.xyz = r0.xyz * float3(3,3,3) + -r1.xyz;
  r1.xyz = v4.www * r2.xyz + r1.xyz;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = v3.xyz * r0.www;
  r0.w = v5.x ? 1 : -1;
  r2.xyz = r2.xyz * r0.www;
  r0.w = dot(dc_f_cb1[0].xyz, dc_f_cb1[0].xyz);
  r0.w = rsqrt(r0.w);
  r3.xyz = dc_f_cb1[0].xyz * r0.www;
  r0.w = dot(r2.xyz, r3.xyz);
  r0.w = max(0, r0.w);
  r2.xyz = dc_f_cb2[0].xyz + dc_f_cb2[0].xyz;
  r2.xyz = r0.www * dc_f_cb0[2].xyz + r2.xyz;
  o0.xyz = r2.xyz * r0.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
