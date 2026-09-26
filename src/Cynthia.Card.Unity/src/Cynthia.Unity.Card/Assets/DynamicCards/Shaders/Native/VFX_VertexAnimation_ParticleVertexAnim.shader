Shader "DynamicCards/Native/VFX_VertexAnimation_ParticleVertexAnim" {
Properties {
_MainTex("_MainTex",2D)="black" {}
_VertexAnimation("_VertexAnimation",2D)="black" {}
_MaxOffset("_MaxOffset",Float)=1.0
_TimeScale("_TimeScale",Float)=1.0
_AnimationLength("_AnimationLength",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"Campaign"="Tree" "IGNOREPROJECTOR"="true" "QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
Pass {
Cull Back
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
float _Cutoff;
float _MaxOffset;
float _TimeScale;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  float4 v3 : TEXCOORD1,
  float4 v4 : TEXCOORD2,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
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
float4 dc_v_cb0[5];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_MaxOffset,0,_TimeScale,_AnimationLength);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dot(v3.xyz, v3.xyz);
  r0.x = rsqrt(r0.x);
  r0.xyz = v3.xyz * r0.xxx;
  r1.xyz = dc_v_cb2[3].xyz + dc_v_cb2[2].xyz;
  r2.xyz = r1.yzx * r0.xyz;
  r2.xyz = r1.xyz * r0.yzx + -r2.xyz;
  r0.x = dot(r1.xyz, r0.xyz);
  r0.y = dot(r2.xyz, r2.xyz);
  r0.y = rsqrt(r0.y);
  r0.yzw = r2.xyz * r0.yyy;
  r1.x = abs(r0.x) * -0.0187292993 + 0.0742610022;
  r1.x = r1.x * abs(r0.x) + -0.212114394;
  r1.x = r1.x * abs(r0.x) + 1.57072878;
  r1.y = 1 + -abs(r0.x);
  r0.x = cmp(r0.x < -r0.x);
  r1.y = sqrt(r1.y);
  r1.z = r1.x * r1.y;
  r1.z = r1.z * -2 + 3.14159274;
  r0.x = r0.x ? r1.z : 0;
  r0.x = r1.x * r1.y + r0.x;
  r0.x = 0.5 * r0.x;
  sincos(r0.x, r0.x, r1.x);
  r0.xyz = r0.yzw * r0.xxx;
  r0.w = dc_v_cb1[0].y / dc_v_cb0[4].w;
  r1.y = 10 * v1.w;
  r2.x = r0.w * dc_v_cb0[4].z + r1.y;
  r2.y = v2.w;
  r2.xyzw = _VertexAnimation.SampleLevel(sampler_VertexAnimation, r2.xy, 0).xyzw;
  r1.yzw = dc_v_cb0[4].xxx * r2.xyz;
  r1.yzw = r1.yzw * float3(2,2,2) + -dc_v_cb0[4].xxx;
  r2.xyz = r1.wyz * r0.yzx;
  r2.xyz = r0.xyz * r1.yzw + -r2.xyz;
  r2.xyz = r1.xxx * r1.zwy + r2.xyz;
  r3.xyz = r2.xyz * r0.xyz;
  r0.xyz = r0.zxy * r2.yzx + -r3.xyz;
  r0.xyz = r0.xyz * float3(2,2,2) + r1.yzw;
  r0.xyz = r0.xyz * v4.www + v4.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v2.xy;
  o2.xyzw = v1.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_Cutoff,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r1.x = -dc_f_cb0[5].x + r0.w;
  r1.x = cmp(r1.x < 0);
  if (r1.x != 0) discard;
  o0.xyz = v2.xyz * r0.xyz;
  o0.w = r0.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
