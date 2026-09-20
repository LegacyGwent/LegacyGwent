Shader "DynamicCards/Latest/ShaderLibrary_Generic_GwentDissolve" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MainTex_Tint("_MainTex_Tint",Color)=(1.0,1.0,1.0,1.0)
_DissolveTex("_DissolveTex",2D)="grey" {}
_DisplacementTex("_DisplacementTex",2D)="grey" {}
_DisplacementTex_Tint("_DisplacementTex_Tint",Color)=(1.0,1.0,1.0,1.0)
_DisplacementMult("_DisplacementMult",Float)=1.0
_TimeMultiplier("_TimeMultiplier",Float)=1.0
_DisplacementAnimParams("_DisplacementAnimParams",Vector)=(0.0,0.0,0.0,0.0)
_RampTex("_RampTex",2D)="white" {}
_RampTex_Tint("_RampTex_Tint",Color)=(1.0,1.0,1.0,1.0)
_RampTex_Color("_RampTex_Color",Color)=(1.0,1.0,1.0,1.0)
_DissolveProgress("_DissolveProgress",Float)=0.0
_Brightness("_Brightness",Float)=1.0
_AlphaCutoff("_AlphaCutoff",Float)=1.0
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
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
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
SamplerState sampler_DisplacementTex;
SamplerState sampler_DissolveTex;
SamplerState sampler_MainTex;
SamplerState sampler_RampTex;
Texture2D<float4> _DisplacementTex;
Texture2D<float4> _DissolveTex;
Texture2D<float4> _MainTex;
Texture2D<float4> _RampTex;
float _AlphaCutoff;
float _Brightness;
float _DisplacementMult;
float _DissolveProgress;
float _TimeMultiplier;
float4 _DisplacementAnimParams;
float4 _DisplacementTex_ST;
float4 _DisplacementTex_Tint;
float4 _DissolveTex_ST;
float4 _MainTex_ST;
float4 _MainTex_Tint;
float4 _RampTex_ST;
float4 _RampTex_Tint;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  float2 v3 : TEXCOORD1,
  float4 v4 : NORMAL0,
  float4 v5 : TANGENT0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : COLOR0,
  out float4 o2 : TEXCOORD0)
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
float4 dc_v_cb0[13];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_DissolveTex_ST.x,_DissolveTex_ST.y,_DissolveTex_ST.z,_DissolveTex_ST.w);
dc_v_cb0[5]=float4(_TimeMultiplier,0,0,0);
dc_v_cb0[6]=float4(_DisplacementAnimParams.x,_DisplacementAnimParams.y,_DisplacementAnimParams.z,_DisplacementAnimParams.w);
dc_v_cb0[7]=float4(_DisplacementTex_ST.x,_DisplacementTex_ST.y,_DisplacementTex_ST.z,_DisplacementTex_ST.w);
dc_v_cb0[8]=float4(_DisplacementTex_Tint.x,_DisplacementTex_Tint.y,_DisplacementTex_Tint.z,_DisplacementTex_Tint.w);
dc_v_cb0[9]=float4(_DisplacementMult,0,0,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(0,0,0,0);
dc_v_cb0[12]=float4(0,_DissolveProgress,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyz = v5.yzx * v4.zxy;
  r0.xyz = v4.yzx * v5.zxy + -r0.xyz;
  r0.xyz = v5.www * r0.xyz;
  r0.w = dc_v_cb1[0].y * dc_v_cb0[5].x + v2.z;
  r0.w = v2.w + r0.w;
  r1.xy = v2.xy * dc_v_cb0[7].xy + dc_v_cb0[7].zw;
  r1.xy = dc_v_cb0[6].xy * r0.ww + r1.xy;
  r1.xyzw = _DisplacementTex.SampleLevel(sampler_DisplacementTex, r1.xy, 0).xyzw;
  r1.xyz = r1.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r1.xyz = dc_v_cb0[8].xyz * r1.xyz;
  r2.xyz = v5.xyz * r1.yyy;
  r0.xyz = r0.xyz * r1.xxx + r2.xyz;
  r0.xyz = v4.xyz * r1.zzz + r0.xyz;
  r0.xyz = dc_v_cb0[9].xxx * r0.xyz;
  r0.xyz = v1.www * r0.xyz;
  r0.xyz = dc_v_cb0[12].yyy * r0.xyz;
  r0.xyz = r0.xyz * dc_v_cb0[9].xxx + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xyzw = v1.xyzw;
  o2.xy = v2.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  o2.zw = v2.xy * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  return;
}






















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[13];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_MainTex_Tint.x,_MainTex_Tint.y,_MainTex_Tint.z,_MainTex_Tint.w);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(_RampTex_ST.x,_RampTex_ST.y,_RampTex_ST.z,_RampTex_ST.w);
dc_f_cb0[11]=float4(_RampTex_Tint.x,_RampTex_Tint.y,_RampTex_Tint.z,_RampTex_Tint.w);
dc_f_cb0[12]=float4(_Brightness,_DissolveProgress,_AlphaCutoff,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _DissolveTex.Sample(sampler_DissolveTex, v2.zw).xyzw;
  r0.x = -dc_f_cb0[12].y + r0.x;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v2.xy).xyzw;
  r0.y = -dc_f_cb0[12].z + r1.w;
  r0.y = min(r0.x, r0.y);
  r0.y = cmp(r0.y < 0);
  if (r0.y != 0) discard;
  r0.y = 0.00999999978 + dc_f_cb0[12].y;
  r0.y = 1 / r0.y;
  r0.x = saturate(r0.x * r0.y);
  r0.y = 0.5;
  r0.xy = r0.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r0.xyzw = _RampTex.Sample(sampler_RampTex, r0.xy).xyzw;
  r2.x = dc_f_cb0[11].w * r0.w;
  r2.y = cmp(0 < dc_f_cb0[12].y);
  r2.z = cmp(dc_f_cb0[12].y < 0);
  r2.y = (int)-r2.y + (int)r2.z;
  r2.y = max((int)r2.y, (int)-r2.y);
  r2.y = (int)r2.y;
  r2.x = r2.x * r2.y;
  r2.yzw = dc_f_cb0[3].xyz * v1.xyz;
  r1.xyz = r2.yzw * r1.xyz;
  r0.xyzw = r0.xyzw * dc_f_cb0[11].xyzw + -r1.xyzw;
  r0.xyzw = r2.xxxx * r0.xyzw + r1.xyzw;
  o0.xyz = saturate(dc_f_cb0[12].xxx * r0.xyz);
  o0.w = r0.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
