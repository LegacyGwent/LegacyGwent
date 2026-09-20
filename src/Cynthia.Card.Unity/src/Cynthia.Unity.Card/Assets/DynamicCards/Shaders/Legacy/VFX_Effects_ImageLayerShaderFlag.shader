Shader "DynamicCards/Legacy/VFX_Effects_ImageLayerShaderFlag" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_WaveSpeed("_WaveSpeed",Float)=50.0
_WaveStrength("_WaveStrength",Float)=1.0
_ShadowStrength("_ShadowStrength",Float)=0.25
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,0.5)
_Offset("_Offset",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
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
Texture2D<float4> _MainTex;
float _Offset;
float _ShadowStrength;
float _WaveSpeed;
float _WaveStrength;
float4 _MainTex_ST;
float4 _TintColor;
















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float p1 : TEXCOORD1)
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
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(_WaveSpeed,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ShadowStrength,_WaveStrength,_Offset,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = v0.x + v0.y;
  r0.x = v0.z + r0.x;
  r0.xyzw = float4(0.5,0.239999995,0.5,0.5) * r0.xxxx;
  r1.x = dc_v_cb1[0].x * dc_v_cb0[3].x + dc_v_cb0[5].z;
  r0.xyzw = r1.xxxx * float4(3.43000007,1.20000005,2.13000011,3.29999995) + r0.xyzw;
  r0.xz = sin(r0.xz);
  r0.yw = cos(r0.wy);
  r1.x = v2.x * v2.x;
  r0.x = dot(r0.xx, r1.xx);
  r0.z = r1.x * r0.z;
  r1.x = r1.x * r0.w;
  r0.yw = r0.yw * v2.xx + float2(0.75,1);
  r0.yw = r0.yw * float2(0.5,0.25) + float2(0.300000012,0.300000012);
  dc_pack_OSGN_1.z = r0.y * r0.w + dc_v_cb0[5].x;
  r0.x = r1.x * 4 + r0.x;
  r0.x = v1.x * r0.x;
  r0.x = dc_v_cb0[5].y * r0.x;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.xxxx;
  r1.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r1.xyzw;
  r0.x = v1.x * r0.z;
  r0.x = r0.x * dc_v_cb0[5].y + v0.z;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  dc_pack_OSGN_1.xy = v2.xy;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.z;
return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float w1 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[5];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1,0);

  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = dc_f_cb0[4].xyzw * r0.xyzw;
  r0.xyzw = r0.xyzw + r0.xyzw;
  o0.xyz = dc_pack_ISGN_1.zzz * r0.xyz;
  o0.w = r0.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
