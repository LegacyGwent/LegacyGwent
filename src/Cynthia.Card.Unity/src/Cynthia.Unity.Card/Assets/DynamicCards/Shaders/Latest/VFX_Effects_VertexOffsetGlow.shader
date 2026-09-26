Shader "DynamicCards/Latest/VFX_Effects_VertexOffsetGlow" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MoveDir("_MoveDir",Vector)=(1.0,1.0,1.0,0.0)
_MovementStrength("_MovementStrength",Float)=0.5
_WaveFrequency("_WaveFrequency",Float)=5.0
_VertexSpeed("_VertexSpeed",Float)=0.25
_GlowOffset("_GlowOffset",Float)=1.0
_RimCol("_RimCol",Color)=(0.0,0.0,0.0,1.0)
_Falloff("_Falloff",Float)=5.0
_Transparency("_Transparency",Float)=15.0
_FlowMap("_FlowMap",2D)="bump" {}
_OffsetAmount("_OffsetAmount",Float)=-0.6499999761581421
_Speed("_Speed",Float)=0.25
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_FlowMap;
SamplerState sampler_MainTex;
Texture2D<float4> _FlowMap;
Texture2D<float4> _MainTex;
float _Cutoff;
float _MovementStrength;
float _OffsetAmount;
float _Speed;
float _VertexSpeed;
float _WaveFrequency;
float4 _FlowMap_ST;
float4 _MainTex_ST;
float4 _MoveDir;
















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float2 v2 : TEXCOORD1,
  float v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1)
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
dc_v_cb0[2]=float4(_MoveDir.x,_MoveDir.y,_MoveDir.z,_MoveDir.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_MovementStrength,_WaveFrequency,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,_VertexSpeed,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 10 / v2.y;
  r0.x = dc_v_cb1[0].y * dc_v_cb0[6].z + r0.x;
  r0.x = sin(r0.x);
  r0.xyz = dc_v_cb0[2].xyz * r0.xxx;
  r0.xyz = r0.xyz / dc_v_cb0[4].yyy;
  r0.w = dc_v_cb0[4].x * v3.x;
  r0.xyz = r0.xyz * r0.www + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  dc_pack_OSGN_1.xy = v1.xy;
  dc_pack_OSGN_1.xy = v2.xy;
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
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_FlowMap_ST.x,_FlowMap_ST.y,_FlowMap_ST.z,_FlowMap_ST.w);
dc_f_cb0[6]=float4(_OffsetAmount,_Speed,0,_Cutoff);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _FlowMap.Sample(sampler_FlowMap, r0.xy).xyzw;
  r0.z = cmp(dc_f_cb0[6].w >= r0.w);
  if (r0.z != 0) discard;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = dc_f_cb0[6].xx * r0.xy;
  r0.z = dc_f_cb1[0].y * dc_f_cb0[6].y;
  r0.z = frac(r0.z);
  r1.xy = r0.xy * r0.zz + dc_pack_ISGN_1.xy;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r0.w = dc_f_cb1[0].y * dc_f_cb0[6].y + 0.5;
  r0.w = frac(r0.w);
  r0.xy = r0.xy * r0.ww + dc_pack_ISGN_1.xy;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.x = -0.5 + r0.z;
  r0.x = r0.x + r0.x;
  r2.xyzw = r2.xyzw + -r1.xyzw;
  o0.xyzw = abs(r0.xxxx) * r2.xyzw + r1.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
