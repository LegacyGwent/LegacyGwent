Shader "DynamicCards/LatestVariant/ShaderLibrary_VFX_Particles_TextureAnimatedParticlesInstanced__d41d8cd98f" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MainTex_Tint("_MainTex_Tint",Color)=(1.0,1.0,1.0,1.0)
_FlowTex("_FlowTex",2D)="black" {}
_TimeMultiplier("_TimeMultiplier",Float)=1.0
_Mode("_Mode",Float)=0.0
_SrcBlend("_SrcBlend",Float)=1.0
_DstBlend("_DstBlend",Float)=0.0
_ZWrite("_ZWrite",Float)=1.0
_Cull("_Cull",Float)=2.0
_DoubleSided("_DoubleSided",Float)=2.0
_AlphaToMask("_AlphaToMask",Float)=0.0
_ZTest("_ZTest",Float)=4.0
_Offset("_Offset",Float)=0.0
_ColorMask("_ColorMask",Float)=15.0
_SrcAlphaBlend("_SrcAlphaBlend",Float)=1.0
_DstAlphaBlend("_DstAlphaBlend",Float)=10.0
_BlendOp("_BlendOp",Float)=0.0
_Overrides("_Overrides",Float)=0.0
}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull [_Cull]
ZWrite [_ZWrite]
ZTest [_ZTest]
Blend [_SrcBlend] [_DstBlend]
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_FlowTex;
SamplerState sampler_MainTex;
Texture2D<float4> _FlowTex;
Texture2D<float4> _MainTex;
float _Offset;
float _TimeMultiplier;
float4 _FlowTex_TexelSize;
float4 _MainTex_ST;
float4 _MainTex_Tint;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  float4 v3 : TEXCOORD1,
  float4 v4 : TEXCOORD2,
  uint v5 : SV_VertexID0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : COLOR0,
  out float2 o2 : TEXCOORD0)
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
float4 dc_v_cb1[8];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(0,0,0,0);
dc_v_cb1[6]=float4(0,0,0,0);
dc_v_cb1[7]=float4(_ZBufferParams.x,_ZBufferParams.y,_ZBufferParams.z,_ZBufferParams.w);
float4 dc_v_cb0[29];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(0,0,0,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(0,0,0,0);
dc_v_cb0[12]=float4(0,0,0,0);
dc_v_cb0[13]=float4(0,0,0,0);
dc_v_cb0[14]=float4(0,0,0,0);
dc_v_cb0[15]=float4(0,0,0,0);
dc_v_cb0[16]=float4(0,0,0,0);
dc_v_cb0[17]=float4(_FlowTex_TexelSize.x,_FlowTex_TexelSize.y,_FlowTex_TexelSize.z,_FlowTex_TexelSize.w);
dc_v_cb0[18]=float4(0,0,0,0);
dc_v_cb0[19]=float4(0,0,0,0);
dc_v_cb0[20]=float4(0,0,0,0);
dc_v_cb0[21]=float4(0,0,0,0);
dc_v_cb0[22]=float4(0,0,_TimeMultiplier,0);
dc_v_cb0[23]=float4(0,0,0,0);
dc_v_cb0[24]=float4(0,0,0,0);
dc_v_cb0[25]=float4(0,0,0,0);
dc_v_cb0[26]=float4(0,0,0,0);
dc_v_cb0[27]=float4(0,0,0,0);
dc_v_cb0[28]=float4(_Offset,0,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dot(v3.xyz, v3.xyz);
  r0.x = rsqrt(r0.x);
  r0.xyz = v3.xyz * r0.xxx;
  r1.xyz = float3(0,0,1) * r0.yzx;
  r1.xyz = r0.zxy * float3(1,0,0) + -r1.xyz;
  r0.w = dot(r1.xz, r1.xz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  r2.xyz = r1.yzx * r0.zxy;
  r2.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r0.w = dot(r2.xyz, r2.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = r2.xyz * r0.www;
  r0.w = 0.5 + v3.w;
  r3.x = dc_v_cb0[17].x * r0.w;
  r0.w = v2.z + v2.w;
  r3.y = dc_v_cb1[0].x * dc_v_cb0[22].z + r0.w;
  r3.xyzw = _FlowTex.SampleLevel(sampler_FlowTex, r3.xy, 0).xyzw;
  r3.xyz = r3.xyz * v4.www + v0.xyz;
  r3.xyz = -v4.xyz + r3.xyz;
  r2.xyz = r3.yyy * r2.xyz;
  r1.xyz = r3.xxx * r1.xyz + r2.xyz;
  r0.xyz = r3.zzz * r0.xyz + r1.xyz;
  r0.xyz = v4.xyz + r0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  r1.x = dc_v_cb0[28].x / dc_v_cb1[7].z;
  o0.z = r1.x + r0.z;
  o0.xyw = r0.xyw;
  o1.xyzw = v1.xyzw;
  o2.xy = v2.xy * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : COLOR0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[32];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(0,0,0,0);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(0,0,0,0);
dc_f_cb0[13]=float4(0,0,0,0);
dc_f_cb0[14]=float4(0,0,0,0);
dc_f_cb0[15]=float4(0,0,0,0);
dc_f_cb0[16]=float4(0,0,0,0);
dc_f_cb0[17]=float4(0,0,0,0);
dc_f_cb0[18]=float4(0,0,0,0);
dc_f_cb0[19]=float4(0,0,0,0);
dc_f_cb0[20]=float4(0,0,0,0);
dc_f_cb0[21]=float4(0,0,0,0);
dc_f_cb0[22]=float4(0,0,0,0);
dc_f_cb0[23]=float4(0,0,0,0);
dc_f_cb0[24]=float4(0,0,0,0);
dc_f_cb0[25]=float4(0,0,0,0);
dc_f_cb0[26]=float4(0,0,0,0);
dc_f_cb0[27]=float4(0,0,0,0);
dc_f_cb0[28]=float4(0,0,0,0);
dc_f_cb0[29]=float4(0,0,0,0);
dc_f_cb0[30]=float4(0,0,0,0);
dc_f_cb0[31]=float4(_MainTex_Tint.x,_MainTex_Tint.y,_MainTex_Tint.z,_MainTex_Tint.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 1 + -v1.w;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v2.xy).xyzw;
  r0.x = r1.w * dc_f_cb0[31].w + -r0.x;
  r1.xyzw = dc_f_cb0[31].xyzw * r1.xyzw;
  r0.x = cmp(r0.x < 0);
  if (r0.x != 0) discard;
  o0.xyz = v1.xyz * r1.xyz;
  o0.w = r1.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
