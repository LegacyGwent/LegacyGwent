Shader "DynamicCards/Latest/VFX_PremiumCards_MaskShader" {
Properties {
_LoopTime("_LoopTime",Float)=1.0
_MainTex("_MainTex",2D)="black" {}
_Shine("_Shine",2D)="black" {}
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_Speed("_Speed",Float)=1.0
_ColorMask("_ColorMask",Color)=(1.0,1.0,1.0,1.0)
_Stencil("_Stencil",Float)=0.0
_StencilReadMask("_StencilReadMask",Float)=255.0
_StencilWriteMask("_StencilWriteMask",Float)=255.0
_StencilComp("_StencilComp",Float)=8.0
_StencilOp("_StencilOp",Float)=0.0
_StencilOpFail("_StencilOpFail",Float)=0.0
_StencilOpZFail("_StencilOpZFail",Float)=0.0
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
SamplerState sampler_MainTex;
SamplerState sampler_Shine;
Texture2D<float4> _MainTex;
Texture2D<float4> _Shine;
float _LoopTime;
float _Speed;
float4 _ColorMask;
float4 _MainTex_ST;
float4 _Shine_ST;
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
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LoopTime,0,0,0);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Shine_ST.x,_Shine_ST.y,_Shine_ST.z,_Shine_ST.w);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_Speed,0,0,0);
dc_f_cb0[7]=float4(_ColorMask.x,_ColorMask.y,_ColorMask.z,_ColorMask.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y * dc_f_cb0[6].x;
  r0.x = r0.x / dc_f_cb0[2].x;
  r0.y = cmp(r0.x >= -r0.x);
  r0.x = frac(abs(r0.x));
  r0.x = r0.y ? r0.x : -r0.x;
  r1.x = -r0.x * dc_f_cb0[2].x + v1.y;
  r1.y = r0.x * dc_f_cb0[2].x + v1.x;
  r0.xy = r1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r0.xyzw = _Shine.Sample(sampler_Shine, r0.xy).xyzw;
  r1.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[5].xyz * r0.xyz;
  r0.xyz = dc_f_cb0[5].www * r0.xyz;
  r0.xyz = float3(4,4,4) * r0.xyz;
  r0.xyz = dc_f_cb0[7].xyz * r0.xyz;
  o0.xyz = dc_f_cb0[7].www * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
