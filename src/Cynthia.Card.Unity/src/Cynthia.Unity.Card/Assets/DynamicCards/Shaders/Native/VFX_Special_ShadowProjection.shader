Shader "DynamicCards/Native/VFX_Special_ShadowProjection" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_ANGLE("_ANGLE",Float)=1.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_AlphaCutOut("_AlphaCutOut",Float)=1.0
_Mask("_Mask",2D)="white" {}
_Multiply("_Multiply",Float)=1.0
_ShadowShape("_ShadowShape",2D)="white" {}
_MovementNoise("_MovementNoise",2D)="white" {}
_TimeSpeed("_TimeSpeed",Float)=1.0
_WiggleRange("_WiggleRange",Float)=0.0
_PostionCorrection("_PostionCorrection",Vector)=(0.0,0.0,0.0,0.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
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
SamplerState sampler_Mask;
SamplerState sampler_MovementNoise;
SamplerState sampler_ShadowShape;
Texture2D<float4> _MainTex;
Texture2D<float4> _Mask;
Texture2D<float4> _MovementNoise;
Texture2D<float4> _ShadowShape;
float _ANGLE;
float _AlphaCutOut;
float _Multiply;
float _TimeSpeed;
float _WiggleRange;
float4 _MainTex_ST;
float4 _Mask_ST;
float4 _MovementNoise_ST;
float4 _PostionCorrection;
float4 _ShadowShape_ST;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1)
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
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v1.xy;
  return;
}






























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb2[7];
dc_f_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_f_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_f_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_f_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_f_cb2[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_f_cb2[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_f_cb2[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[12];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_ANGLE,0,0,0);
dc_f_cb0[4]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[5]=float4(_AlphaCutOut,0,0,0);
dc_f_cb0[6]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[7]=float4(_Multiply,0,0,0);
dc_f_cb0[8]=float4(_ShadowShape_ST.x,_ShadowShape_ST.y,_ShadowShape_ST.z,_ShadowShape_ST.w);
dc_f_cb0[9]=float4(_MovementNoise_ST.x,_MovementNoise_ST.y,_MovementNoise_ST.z,_MovementNoise_ST.w);
dc_f_cb0[10]=float4(_TimeSpeed,_WiggleRange,0,0);
dc_f_cb0[11]=float4(_PostionCorrection.x,_PostionCorrection.y,_PostionCorrection.z,_PostionCorrection.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = r0.w * dc_f_cb0[5].x + -0.5;
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r1.y = dc_f_cb1[0].x * dc_f_cb0[10].x + v1.y;
  r1.x = v1.x;
  r1.xy = r1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r1.xyzw = _MovementNoise.Sample(sampler_MovementNoise, r1.xy).xyzw;
  r1.xy = r1.xy * float2(2,2) + float2(-1,-1);
  r1.xy = dc_f_cb0[10].yy * r1.xy;
  r2.xyz = dc_f_cb2[3].xyz + dc_f_cb0[11].xyz;
  r2.xyz = v2.xyz + -r2.xyz;
  r3.xyz = dc_f_cb2[5].xyz * r2.yyy;
  r2.xyw = dc_f_cb2[4].xyz * r2.xxx + r3.xyz;
  r2.xyz = dc_f_cb2[6].xyz * r2.zzz + r2.xyw;
  r3.yz = r2.xy + r2.yz;
  r0.w = r3.y * 0.5 + -r2.x;
  r3.x = dc_f_cb0[3].x * r0.w + r2.x;
  r1.xy = r3.xz * float2(0.100000001,0.0500000007) + r1.xy;
  r1.xy = r1.xy * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r1.xyzw = _ShadowShape.Sample(sampler_ShadowShape, r1.xy).xyzw;
  r1.xyz = dc_f_cb0[7].xxx * r1.xyz;
  r2.xy = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r2.xyzw = _Mask.Sample(sampler_Mask, r2.xy).xyzw;
  r1.xyz = r2.xyz * r1.xyz;
  r2.xyz = float3(1,1,1) + -dc_f_cb0[4].xyz;
  r0.w = cmp(dc_f_cb0[7].x >= 0);
  r0.w = r0.w ? 1.000000 : 0;
  r3.xyz = dc_f_cb0[4].xyz * r0.www;
  r1.w = cmp(0 >= dc_f_cb0[7].x);
  r1.w = r1.w ? 1.000000 : 0;
  r2.xyz = r1.www * r2.xyz + r3.xyz;
  r0.w = r1.w * r0.w;
  r3.xyz = dc_f_cb0[4].xyz + -r2.xyz;
  r2.xyz = r0.www * r3.xyz + r2.xyz;
  r1.xyz = r2.xyz * r1.xyz;
  r1.xyz = max(float3(-5,-5,-5), r1.xyz);
  r1.xyz = min(float3(1,1,1), r1.xyz);
  r1.xyz = float3(1,1,1) + -r1.xyz;
  o0.xyz = r0.xyz / r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
