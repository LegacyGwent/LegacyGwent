Shader "DynamicCards/Latest/UI_UIGlow" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_UVNoise("_UVNoise",2D)="white" {}
_Distortion("_Distortion",Float)=0.0
_DistortionSpeed("_DistortionSpeed",Float)=0.0
_SmokeGlow("_SmokeGlow",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,0.0,0.0,1.0)
_Alpha("_Alpha",Float)=1.0
_UIFade("_UIFade",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
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
SamplerState sampler_UVNoise;
Texture2D<float4> _MainTex;
Texture2D<float4> _UVNoise;
float _Alpha;
float _Distortion;
float _DistortionSpeed;
float _UIFade;
float4 _MainTex_ST;
float4 _TintColor;
float4 _UVNoise_ST;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1)
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
dc_v_cb2[9]=float4(unity_MatrixV[0][0],unity_MatrixV[1][0],unity_MatrixV[2][0],unity_MatrixV[3][0]);
dc_v_cb2[10]=float4(unity_MatrixV[0][1],unity_MatrixV[1][1],unity_MatrixV[2][1],unity_MatrixV[3][1]);
dc_v_cb2[11]=float4(unity_MatrixV[0][2],unity_MatrixV[1][2],unity_MatrixV[2][2],unity_MatrixV[3][2]);
dc_v_cb2[12]=float4(unity_MatrixV[0][3],unity_MatrixV[1][3],unity_MatrixV[2][3],unity_MatrixV[3][3]);
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
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);



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
  r1.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r1.xyzw;
  o1.xy = v1.xy;
  r0.y = dc_v_cb2[10].z * r0.y;
  r0.x = dc_v_cb2[9].z * r0.x + r0.y;
  r0.x = dc_v_cb2[11].z * r0.z + r0.x;
  r0.x = dc_v_cb2[12].z * r0.w + r0.x;
  o2.z = -r0.x;
  r0.x = dc_v_cb0[5].x * r1.y;
  r0.w = 0.5 * r0.x;
  r0.xz = float2(0.5,0.5) * r1.xw;
  o2.w = r1.w;
  o2.xy = r0.xw + r0.zz;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_UVNoise_ST.x,_UVNoise_ST.y,_UVNoise_ST.z,_UVNoise_ST.w);
dc_f_cb0[4]=float4(_Distortion,_DistortionSpeed,0,0);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_UIFade,_Alpha,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = cmp(dc_f_cb0[5].y >= dc_f_cb0[5].z);
  r0.x = r0.x ? 1.000000 : 0;
  r1.xy = dc_f_cb0[5].yz;
  r1.zw = float2(0,-0.333333343);
  r2.xy = dc_f_cb0[5].zy;
  r2.zw = float2(-1,0.666666687);
  r1.xyzw = -r2.xyzw + r1.xyzw;
  r0.xyzw = r0.xxxx * r1.xywz + r2.xywz;
  r1.x = cmp(dc_f_cb0[5].x >= r0.x);
  r1.x = r1.x ? 1.000000 : 0;
  r2.z = r0.w;
  r0.w = dc_f_cb0[5].x;
  r2.xyw = r0.wyx;
  r2.xyzw = r2.xyzw + -r0.xyzw;
  r0.xyzw = r1.xxxx * r2.xyzw + r0.xyzw;
  r1.x = min(r0.w, r0.y);
  r1.x = -r1.x + r0.x;
  r1.y = r1.x * 6 + 1.00000001e-10;
  r0.y = r0.w + -r0.y;
  r0.y = r0.y / r1.y;
  r0.y = r0.z + r0.y;
  r0.z = dc_f_cb0[5].w * 8 + -4;
  r0.w = cmp(0 < r0.z);
  r1.y = cmp(r0.z < 0);
  r0.z = r0.x * abs(r0.z);
  r0.w = (int)-r0.w + (int)r1.y;
  r0.w = (int)r0.w;
  r1.y = -r0.w * 0.0500000007 + abs(r0.y);
  r0.y = r0.w * 0.0500000007 + abs(r0.y);
  r2.xyz = float3(0,-0.333333343,0.333333343) + r0.yyy;
  r2.xyz = frac(r2.xyz);
  r2.xyz = -r2.xyz * float3(2,2,2) + float3(1,1,1);
  r2.xyz = saturate(abs(r2.xyz) * float3(3,3,3) + float3(-1,-1,-1));
  r2.xyz = float3(-1,-1,-1) + r2.xyz;
  r1.yzw = float3(0,-0.333333343,0.333333343) + r1.yyy;
  r1.yzw = frac(r1.yzw);
  r1.yzw = -r1.yzw * float3(2,2,2) + float3(1,1,1);
  r1.yzw = saturate(abs(r1.yzw) * float3(3,3,3) + float3(-1,-1,-1));
  r1.yzw = float3(-1,-1,-1) + r1.yzw;
  r0.y = 1.00000001e-10 + r0.x;
  r0.y = r1.x / r0.y;
  r1.xyz = r0.yyy * r1.yzw + float3(1,1,1);
  r0.y = saturate(255 * r0.y);
  r2.xyz = r0.yyy * r2.xyz + float3(1,1,1);
  r0.yzw = saturate(r2.xyz * r0.zzz);
  r1.xyz = r1.xyz * r0.xxx;
  r2.xy = v2.xy / v2.ww;
  r2.z = dc_f_cb1[0].x * dc_f_cb0[4].y + r2.y;
  r2.xy = r2.xz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _UVNoise.Sample(sampler_UVNoise, r2.xy).xyzw;
  r0.x = r2.x * 2 + -1;
  r2.xy = r0.xx * dc_f_cb0[4].xx + v1.xy;
  r2.xy = r2.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
  r1.xyz = r2.xxx * r1.xyz;
  r0.x = r2.y + r2.x;
  r0.x = dot(r0.xxx, float3(0.300000012,0.589999974,0.109999999));
  r0.x = r0.x + r0.x;
  r1.xyz = saturate(float3(4,4,4) * r1.xyz);
  r1.xyz = r1.xyz + -r0.yzw;
  o0.xyz = r0.xxx * r1.xyz + r0.yzw;
  r0.x = saturate(r0.x);
  r0.x = dc_f_cb0[6].y * r0.x;
  r0.y = 1 + -dc_f_cb0[6].x;
  o0.w = r0.x * r0.y;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
