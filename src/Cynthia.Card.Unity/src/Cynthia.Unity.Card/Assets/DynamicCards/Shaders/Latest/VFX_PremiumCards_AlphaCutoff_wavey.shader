Shader "DynamicCards/Latest/VFX_PremiumCards_AlphaCutoff_wavey" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Offsettexturebluechannel("_Offsettexturebluechannel",2D)="white" {}
_UseUorV("_UseUorV",Float)=0.0
_Timemultiplier("_Timemultiplier",Float)=1.0
_Timeoffset("_Timeoffset",Float)=0.0
_Offsetamount("_Offsetamount",Float)=0.10000000149011612
_Wavelength("_Wavelength",Float)=1.0
_VerticalHorizontalmovement("_VerticalHorizontalmovement",Float)=0.0
_AlphaCutoff("_AlphaCutoff",Float)=0.5
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
SamplerState sampler_MainTex;
SamplerState sampler_Offsettexturebluechannel;
Texture2D<float4> _MainTex;
Texture2D<float4> _Offsettexturebluechannel;
float _AlphaCutoff;
float _Offsetamount;
float _Timemultiplier;
float _Timeoffset;
float _UseUorV;
float _VerticalHorizontalmovement;
float _Wavelength;
float4 _MainTex_ST;
float4 _Offsettexturebluechannel_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(_UseUorV,_Timemultiplier,_Offsetamount,_Wavelength);
dc_v_cb0[4]=float4(_Timeoffset,0,0,0);
dc_v_cb0[5]=float4(_Offsettexturebluechannel_ST.x,_Offsettexturebluechannel_ST.y,_Offsettexturebluechannel_ST.z,_Offsettexturebluechannel_ST.w);
dc_v_cb0[6]=float4(_VerticalHorizontalmovement,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = v1.y + -v1.x;
  r0.x = dc_v_cb0[3].x * r0.x + v1.x;
  r0.y = dc_v_cb1[0].y * dc_v_cb0[3].y;
  r0.x = r0.x * dc_v_cb0[3].w + r0.y;
  r0.x = dc_v_cb0[4].x + r0.x;
  r0.x = sin(r0.x);
  r0.y = dc_v_cb0[3].z * r0.x;
  r0.xz = float2(0,0);
  r1.xyz = r0.yzz + -r0.zyz;
  r0.xyz = dc_v_cb0[6].xxx * r1.xyz + r0.xyz;
  r1.xy = v1.xy * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r1.xyzw = _Offsettexturebluechannel.SampleLevel(sampler_Offsettexturebluechannel, r1.xy, 0).xyzw;
  r0.xyz = r0.xyz * r1.zzz + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,_AlphaCutoff,0,0);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = r0.w * dc_f_cb0[6].y + -0.5;
  o0.xyz = r0.xyz;
  r0.x = cmp(r0.w < 0);
  if (r0.x != 0) discard;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
