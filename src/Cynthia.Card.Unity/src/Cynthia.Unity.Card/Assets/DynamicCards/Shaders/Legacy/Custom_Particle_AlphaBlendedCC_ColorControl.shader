Shader "DynamicCards/Legacy/Custom_Particle_AlphaBlendedCC_ColorControl" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_maxParticleBrightness("_maxParticleBrightness",Float)=1.0
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_HSV_Saturation("_HSV_Saturation",Float)=1.0
_HSV_Value("_HSV_Value",Float)=1.0
_RED("_RED",Float)=1.0
_GREEN("_GREEN",Float)=1.0
_BLUE("_BLUE",Float)=1.0
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
float _BLUE;
float _GREEN;
float _HSV_Saturation;
float _HSV_Value;
float _RED;
float _maxParticleBrightness;
float4 _MainTex_ST;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
  r0.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb1[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb1[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  uint v3 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[4]=float4(_maxParticleBrightness,_HSV_Saturation,_RED,_GREEN);
dc_f_cb0[5]=float4(_BLUE,_HSV_Value,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.zw = float2(-1,0.666666687);
  r1.zw = float2(1,-1);
  r2.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
  r0.x = dc_f_cb0[5].x * r2.z;
  r1.x = r2.y * dc_f_cb0[4].w + -r0.x;
  r3.xw = dc_f_cb0[4].wz * r2.yx;
  r2.x = cmp(r3.x >= r0.x);
  r2.x = r2.x ? 1.000000 : 0;
  r0.y = r3.x;
  r1.y = r2.z * dc_f_cb0[5].x + -r0.y;
  r0.xyzw = r2.xxxx * r1.xyzw + r0.xyzw;
  r1.x = v2.w * r2.w;
  o0.w = saturate(dc_f_cb0[4].x * r1.x);
  r1.x = cmp(r3.w >= r0.x);
  r1.x = r1.x ? 1.000000 : 0;
  r3.xyz = r0.xyw;
  r0.xyw = r3.wyx;
  r0.xyzw = r0.xyzw + -r3.xyzw;
  r0.xyzw = r1.xxxx * r0.xyzw + r3.xyzw;
  r1.x = min(r0.w, r0.y);
  r1.x = -r1.x + r0.x;
  r1.y = r1.x * 6 + 1.00000001e-10;
  r0.y = r0.w + -r0.y;
  r0.y = r0.y / r1.y;
  r0.y = r0.z + r0.y;
  r0.yzw = float3(0,-0.333333343,0.333333343) + abs(r0.yyy);
  r0.yzw = frac(r0.yzw);
  r0.yzw = -r0.yzw * float3(2,2,2) + float3(1,1,1);
  r0.yzw = saturate(abs(r0.yzw) * float3(3,3,3) + float3(-1,-1,-1));
  r0.yzw = float3(-1,-1,-1) + r0.yzw;
  r1.y = 1.00000001e-10 + r0.x;
  r0.x = dc_f_cb0[5].y * r0.x;
  r1.x = r1.x / r1.y;
  r1.x = dc_f_cb0[4].y * r1.x;
  r0.yzw = r1.xxx * r0.yzw + float3(1,1,1);
  r0.xyz = r0.yzw * r0.xxx;
  r0.xyz = dc_f_cb0[3].xyz * r0.xyz;
  o0.xyz = v2.xyz * r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
