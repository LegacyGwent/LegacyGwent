Shader "DynamicCards/Legacy/VFX_VertexAnimation_ParticleVertAnimation" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_VertexAnimation("_VertexAnimation",2D)="white" {}
_AnimationLength("_AnimationLength",Float)=1.0
_Size("_Size",Float)=1.0
_Forward("_Forward",Vector)=(1.0,0.0,0.0,0.0)
_CutOff("_CutOff",Float)=0.5
}
SubShader {
Tags {}
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
SamplerState sampler_VertexAnimation;
Texture2D<float4> _MainTex;
Texture2D<float4> _VertexAnimation;
float _AnimationLength;
float _CutOff;
float _Size;
float4 _VertexAnimation_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  float4 v3 : TEXCOORD1,
  out float2 o0 : TEXCOORD0,
  out float4 o1 : SV_POSITION0,
  out float4 o2 : COLOR0)
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
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_VertexAnimation_ST.x,_VertexAnimation_ST.y,_VertexAnimation_ST.z,_VertexAnimation_ST.w);
dc_v_cb0[3]=float4(_AnimationLength,_Size,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v2.xy;
  r0.x = dc_v_cb1[0].y + dc_v_cb1[0].y;
  r0.x = r0.x / dc_v_cb0[3].x;
  r0.x = v1.w + r0.x;
  r0.y = v2.w;
  r0.xy = r0.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  r0.xyzw = _VertexAnimation.SampleLevel(sampler_VertexAnimation, r0.xy, 0).xyzw;
  r0.xyz = v3.www * r0.xyz;
  r0.xyz = r0.xyz * dc_v_cb0[3].yyy + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o1.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o2.xyzw = v1.xyzw;
  return;
}














void dc_f(
  float2 v0 : TEXCOORD0,
  float4 v1 : SV_POSITION0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_CutOff,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v0.xy).xyzw;
  r1.xyz = v2.xyz;
  r1.w = 1;
  r0.xyzw = r1.xyzw * r0.xyzw;
  r1.x = cmp(dc_f_cb0[5].x >= r0.w);
  if (r1.x != 0) discard;
  o0.xyzw = r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
