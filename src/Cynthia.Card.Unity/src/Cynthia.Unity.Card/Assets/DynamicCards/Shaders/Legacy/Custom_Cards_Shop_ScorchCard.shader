Shader "DynamicCards/Legacy/Custom_Cards_Shop_ScorchCard" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Noise("_Noise",2D)="white" {}
_Progress("_Progress",Float)=-1.0
_CutOff("_CutOff",Float)=-1.0
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_HeatColor("_HeatColor",Color)=(0.5,0.5,0.5,1.0)
_BurntTint("_BurntTint",Color)=(0.5,0.5,0.5,1.0)
}
SubShader {
Tags {"RenderType"="Transparent"}
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
SamplerState sampler_MainTex;
SamplerState sampler_Noise;
Texture2D<float4> _MainTex;
Texture2D<float4> _Noise;
float _CutOff;
float _Progress;
float4 _BurntTint;
float4 _HeatColor;
float4 _MainTex_ST;
float4 _Noise_ST;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1)
{
float4 dc_v_cb2[21];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
dc_v_cb2[4]=float4(0,0,0,0);
dc_v_cb2[5]=float4(0,0,0,0);
dc_v_cb2[6]=float4(0,0,0,0);
dc_v_cb2[7]=float4(0,0,0,0);
dc_v_cb2[8]=float4(0,0,0,0);
dc_v_cb2[9]=float4(0,0,0,0);
dc_v_cb2[10]=float4(0,0,0,0);
dc_v_cb2[11]=float4(0,0,0,0);
dc_v_cb2[12]=float4(0,0,0,0);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[3]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  dc_pack_OSGN_1.xy = v2.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  dc_pack_OSGN_1.zw = v2.xy * dc_v_cb0[3].xy + dc_v_cb0[3].zw;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_CutOff,_Progress,0,0);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_HeatColor.x,_HeatColor.y,_HeatColor.z,_HeatColor.w);
dc_f_cb0[7]=float4(_BurntTint.x,_BurntTint.y,_BurntTint.z,_BurntTint.w);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, dc_pack_ISGN_1.xy).xyzw;
  r1.xyzw = _Noise.Sample(sampler_Noise, dc_pack_ISGN_1.zw).xyzw;
  r1.x = dc_f_cb0[4].y + r1.x;
  r1.y = r1.x * r1.x;
  r1.z = r1.y * r1.y;
  r1.y = saturate(r1.y * r1.z + -3);
  r1.z = r1.y * r1.y;
  r1.z = r1.z * r1.z;
  r1.z = r1.y * r1.z;
  r0.w = r1.z * -r0.w + r0.w;
  r1.z = cmp(r0.w < dc_f_cb0[4].x);
  if (r1.z != 0) discard;
  r1.x = log2(r1.x);
  r1.xz = float2(12,30) * r1.xx;
  r1.xz = exp2(r1.xz);
  r1.x = min(1, r1.x);
  r1.z = saturate(-0.400000006 + r1.z);
  r1.y = 0.5 + r1.y;
  r2.xyz = r1.yyy * -dc_f_cb0[7].xyz + dc_f_cb0[7].xyz;
  r2.xyz = r2.xyz * r0.xyz;
  r1.y = r1.z * r1.z;
  r1.y = r1.y * r1.y;
  r1.y = r1.y * r1.y;
  r2.xyz = r2.xyz * float3(2,2,2) + -dc_f_cb0[6].xyz;
  r1.yzw = r1.yyy * r2.xyz + dc_f_cb0[6].xyz;
  r2.x = r1.x * r1.x;
  r2.x = r2.x * r2.x;
  r1.yzw = -dc_f_cb0[5].xyz + r1.yzw;
  r1.yzw = r2.xxx * r1.yzw + dc_f_cb0[5].xyz;
  r1.yzw = r1.yzw + -r0.xyz;
  o0.xyz = r1.xxx * r1.yzw + r0.xyz;
  o0.w = r0.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
