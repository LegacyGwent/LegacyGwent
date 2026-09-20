Shader "DynamicCards/Latest/UI_DissolvePaper" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_DissolveMask("_DissolveMask",2D)="white" {}
_EdgeMin("_EdgeMin",Float)=-15.0
_EdgeMax("_EdgeMax",Float)=15.0
_PaperMin("_PaperMin",Float)=0.0
_PaperMax("_PaperMax",Float)=1.0
_SpeedU("_SpeedU",Float)=0.0
_SpeedV("_SpeedV",Float)=0.0
_SecondMask("_SecondMask",2D)="white" {}
_SecondMaskMult("_SecondMaskMult",Float)=1.0
_DissolveColorRamp("_DissolveColorRamp",2D)="white" {}
_ColorGRadInfluence("_ColorGRadInfluence",Float)=1.0
_BlackBurnInfluence("_BlackBurnInfluence",Float)=0.0
_Progress("_Progress",Float)=0.0
_OffsetTex("_OffsetTex",2D)="white" {}
_SpeedHUE("_SpeedHUE",Float)=0.0
_OffsetMult("_OffsetMult",Float)=0.0
_BurnMaskInfl("_BurnMaskInfl",Float)=0.0
_BurnEdge("_BurnEdge",2D)="white" {}
_EdgeColor("_EdgeColor",Color)=(0.0,0.0,0.0,1.0)
_minRemap("_minRemap",Float)=0.0
_maxRemap("_maxRemap",Float)=1.0
_DissolveProgress("_DissolveProgress",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_BurnEdge;
SamplerState sampler_DissolveColorRamp;
SamplerState sampler_DissolveMask;
SamplerState sampler_MainTex;
SamplerState sampler_OffsetTex;
SamplerState sampler_SecondMask;
Texture2D<float4> _BurnEdge;
Texture2D<float4> _DissolveColorRamp;
Texture2D<float4> _DissolveMask;
Texture2D<float4> _MainTex;
Texture2D<float4> _OffsetTex;
Texture2D<float4> _SecondMask;
float _BlackBurnInfluence;
float _BurnMaskInfl;
float _ColorGRadInfluence;
float _DissolveProgress;
float _EdgeMax;
float _EdgeMin;
float _OffsetMult;
float _PaperMax;
float _PaperMin;
float _Progress;
float _SecondMaskMult;
float _SpeedHUE;
float _SpeedU;
float _SpeedV;
float _maxRemap;
float _minRemap;
float4 _BurnEdge_ST;
float4 _DissolveColorRamp_ST;
float4 _DissolveMask_ST;
float4 _EdgeColor;
float4 _MainTex_ST;
float4 _OffsetTex_ST;
float4 _SecondMask_ST;
float4 _TintColor;




















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : COLOR0)
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
dc_v_cb3[9]=float4(unity_MatrixV[0][0],unity_MatrixV[1][0],unity_MatrixV[2][0],unity_MatrixV[3][0]);
dc_v_cb3[10]=float4(unity_MatrixV[0][1],unity_MatrixV[1][1],unity_MatrixV[2][1],unity_MatrixV[3][1]);
dc_v_cb3[11]=float4(unity_MatrixV[0][2],unity_MatrixV[1][2],unity_MatrixV[2][2],unity_MatrixV[3][2]);
dc_v_cb3[12]=float4(unity_MatrixV[0][3],unity_MatrixV[1][3],unity_MatrixV[2][3],unity_MatrixV[3][3]);
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
float4 dc_v_cb1[6];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_v_cb0[12];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(0,0,0,0);
dc_v_cb0[10]=float4(_OffsetTex_ST.x,_OffsetTex_ST.y,_OffsetTex_ST.z,_OffsetTex_ST.w);
dc_v_cb0[11]=float4(_SpeedHUE,_OffsetMult,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.zw = float2(-1,0.666666687);
  r1.zw = float2(1,-1);
  r2.xy = v1.xy * dc_v_cb0[10].xy + dc_v_cb0[10].zw;
  r2.xyzw = _OffsetTex.SampleLevel(sampler_OffsetTex, r2.xy, 0).yzwx;
  r3.x = cmp(r2.x >= r2.y);
  r3.x = r3.x ? 1.000000 : 0;
  r0.xy = r2.yx;
  r1.xy = r2.xy + -r0.xy;
  r0.xyzw = r3.xxxx * r1.xyzw + r0.xyzw;
  r1.x = cmp(r2.w >= r0.x);
  r1.x = r1.x ? 1.000000 : 0;
  r2.xyz = r0.xyw;
  r0.xyw = r2.wyx;
  r0.xyzw = r0.xyzw + -r2.xyzw;
  r0.xyzw = r1.xxxx * r0.xyzw + r2.xyzw;
  r1.x = min(r0.w, r0.y);
  r1.x = -r1.x + r0.x;
  r1.y = r1.x * 6 + 1.00000001e-10;
  r0.y = r0.w + -r0.y;
  r0.y = r0.y / r1.y;
  r0.y = r0.z + r0.y;
  r0.y = dc_v_cb1[0].y * dc_v_cb0[11].x + abs(r0.y);
  r0.yzw = float3(0,-0.333333343,0.333333343) + r0.yyy;
  r0.yzw = frac(r0.yzw);
  r0.yzw = -r0.yzw * float3(2,2,2) + float3(1,1,1);
  r0.yzw = saturate(abs(r0.yzw) * float3(3,3,3) + float3(-1,-1,-1));
  r0.yzw = float3(-1,-1,-1) + r0.yzw;
  r1.y = 1.00000001e-10 + r0.x;
  r1.x = r1.x / r1.y;
  r0.yzw = r1.xxx * r0.yzw + float3(1,1,1);
  r0.xyz = r0.yzw * r0.xxx + float3(-0.5,-0.5,-0.5);
  r0.xyz = r0.xyz * dc_v_cb0[11].yyy + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r1.xyzw;
  o1.xy = v1.xy;
  r0.y = dc_v_cb3[10].z * r0.y;
  r0.x = dc_v_cb3[9].z * r0.x + r0.y;
  r0.x = dc_v_cb3[11].z * r0.z + r0.x;
  r0.x = dc_v_cb3[12].z * r0.w + r0.x;
  o2.z = -r0.x;
  r0.x = dc_v_cb1[5].x * r1.y;
  r0.w = 0.5 * r0.x;
  r0.xz = float2(0.5,0.5) * r1.xw;
  o2.w = r1.w;
  o2.xy = r0.xw + r0.zz;
  o3.xyzw = v2.xyzw;
  return;
}
































void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  uint v4 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[17];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_DissolveMask_ST.x,_DissolveMask_ST.y,_DissolveMask_ST.z,_DissolveMask_ST.w);
dc_f_cb0[4]=float4(_DissolveColorRamp_ST.x,_DissolveColorRamp_ST.y,_DissolveColorRamp_ST.z,_DissolveColorRamp_ST.w);
dc_f_cb0[5]=float4(_ColorGRadInfluence,_BlackBurnInfluence,_Progress,_EdgeMin);
dc_f_cb0[6]=float4(_EdgeMax,0,0,0);
dc_f_cb0[7]=float4(_SecondMask_ST.x,_SecondMask_ST.y,_SecondMask_ST.z,_SecondMask_ST.w);
dc_f_cb0[8]=float4(_SecondMaskMult,_SpeedU,_SpeedV,_PaperMax);
dc_f_cb0[9]=float4(_PaperMin,0,0,0);
dc_f_cb0[10]=float4(0,0,0,0);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[13]=float4(_BurnMaskInfl,0,0,0);
dc_f_cb0[14]=float4(_BurnEdge_ST.x,_BurnEdge_ST.y,_BurnEdge_ST.z,_BurnEdge_ST.w);
dc_f_cb0[15]=float4(_EdgeColor.x,_EdgeColor.y,_EdgeColor.z,_EdgeColor.w);
dc_f_cb0[16]=float4(_minRemap,_maxRemap,_DissolveProgress,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r0.xyzw = _SecondMask.Sample(sampler_SecondMask, r0.xy).xyzw;
  r0.x = -1 + r0.x;
  r0.x = dc_f_cb0[8].x * r0.x + 1;
  r0.yz = v2.xy / v2.ww;
  r0.yz = dc_f_cb1[0].yy * dc_f_cb0[8].yz + r0.yz;
  r0.yz = r0.yz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _DissolveMask.Sample(sampler_DissolveMask, r0.yz).xyzw;
  r0.y = dc_f_cb0[16].z + dc_f_cb0[5].z;
  r0.z = 1 + -r0.y;
  r0.z = r0.z * 2 + r1.x;
  r0.z = -1 + r0.z;
  r0.x = r0.x * r0.z;
  r0.zw = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.zw).xyzw;
  r0.z = r1.w * r0.x;
  r0.w = r0.z * dc_f_cb0[12].w + -0.5;
  r0.z = log2(r0.z);
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r0.w = dc_f_cb0[13].x * -r0.x + r1.x;
  r2.x = dc_f_cb0[14].x * r0.w;
  r2.yw = float2(0,0);
  r2.xy = dc_f_cb0[14].zw + r2.xy;
  r3.xyzw = _BurnEdge.Sample(sampler_BurnEdge, r2.xy).xyzw;
  r3.xyz = dc_f_cb0[15].xyz * r3.xyz;
  r0.w = dc_f_cb0[16].y + -dc_f_cb0[16].x;
  r3.xyz = r3.xyz * r0.www + dc_f_cb0[16].xxx;
  r3.xyz = -dc_f_cb0[12].xyz * r1.xyz + r3.xyz;
  r1.xyz = dc_f_cb0[12].xyz * r1.xyz;
  r1.xyz = r0.yyy * r3.xyz + r1.xyz;
  r1.xyz = float3(1,1,1) + -r1.xyz;
  r0.w = -dc_f_cb0[9].x + dc_f_cb0[8].w;
  r0.y = r0.y * r0.w + dc_f_cb0[9].x;
  r0.y = r0.y * r0.z;
  r0.y = exp2(r0.y);
  r0.y = min(1, r0.y);
  r0.yzw = r1.xyz / r0.yyy;
  r0.yzw = float3(1,1,1) + -r0.yzw;
  r1.x = dc_f_cb0[6].x + -dc_f_cb0[5].w;
  r0.x = r0.x * r1.x + dc_f_cb0[5].w;
  r1.x = saturate(r0.x);
  r0.x = saturate(-dc_f_cb0[5].y + r0.x);
  r1.x = 1 + -r1.x;
  r2.z = dc_f_cb0[4].x * r1.x;
  r1.xy = dc_f_cb0[4].zw + r2.zw;
  r1.xyzw = _DissolveColorRamp.Sample(sampler_DissolveColorRamp, r1.xy).xyzw;
  r1.xyz = dc_f_cb0[5].xxx * r1.xyz;
  o0.xyz = r0.yzw * r0.xxx + r1.xyz;
  o0.w = v3.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
